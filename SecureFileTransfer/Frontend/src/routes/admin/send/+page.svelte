<script lang="ts">
	import Toast from "$lib/components/Toast.svelte";
	import { getEmailHtml } from "$lib/templates/sendEmailTemplate";
	import { generatePasscode } from "$lib/passcode";
	import { onMount } from "svelte";

	let receiverId = '';
	let receiverEmail = "";
	let receiverFirstName = "";
	let receiverLastName = "";
	let subject = "";
	let body = "";
	let files: File[] = [];
	let expiresInDays = 7;
	let passcode = "";

	let isDragging = false;
	let showEmailPreview = false;
	let previewContainer: HTMLDivElement;
	let originalFirstName = "";
let originalLastName = "";

	let successMessage = "";
	let errorMessage = "";
	let uploadProgress = 0;
	let isUploading = false;
	let isFinalizing = false;
	let sessionId: string | null = null;
	let uploadStatus = "Starting...";
	let pollingInterval: ReturnType<typeof setInterval> | null = null;
	let downloadLink = "";
	let protectWithPasscode = true;
	let slug = "";
	let slugManuallyEdited = false;
	// 🔹 Handles file uploads and returns file paths

	let customerSuggestions: {id: string, email: string; name: string; lastName: string }[] = [];
	let showSuggestions = false;

	$: if (receiverEmail.length > 2) {
		fetchCustomerSuggestions()
	}

	async function fetchCustomerSuggestions() {
	const token = localStorage.getItem("token");

	try {
		const res = await fetch(
			`http://localhost:5105/api/customers/search?query=${encodeURIComponent(receiverEmail)}`,
			{
				headers: {
					"Authorization": `Bearer ${token}`,
					"Accept": "application/json"
				}
			}
		);

		if (!res.ok) {
			console.warn(`Customer search failed: ${res.status}`);
			customerSuggestions = [];
			return;
		}

		const data = await res.json();
		customerSuggestions = data;
	} catch (err) {
		console.error("Autocomplete error:", err);
		customerSuggestions = [];
	}
}


function selectCustomer(customer: {id: string; email: string; name: string; lastName: string}) {
	receiverId = customer.id;
	receiverEmail = customer.email;
	receiverFirstName = customer.name;
	receiverLastName = customer.lastName;
	originalFirstName = customer.name;
	originalLastName = customer.lastName;
	showSuggestions = false;
}

	$: {
	if (!slugManuallyEdited && (receiverFirstName || receiverLastName)) {
		const autoSlug = `${receiverFirstName}-${receiverLastName}-${receiverId}`
			.toLowerCase()
			.replace(/\s+/g, "-")
			.replace("--", "-")
			.replace(/[^a-z0-9\-]/g, "");

		if (autoSlug !== slug) slug = autoSlug;
	}
}

	async function uploadFiles(): Promise<string[] | null> {
		try {
			if (!files.length) {
				alert("Please select at least one file.");
				return null;
			}

			const formData = new FormData();
			for (const file of files) formData.append("files", file);

			const token = localStorage.getItem("token");
			isUploading = true;
			uploadProgress = 0;
			uploadStatus = "Uploading...";

			const xhr = new XMLHttpRequest();
			xhr.open("POST", "http://localhost:5105/api/files/upload", true);
			xhr.setRequestHeader("Authorization", `Bearer ${token}`);

			xhr.upload.onprogress = (e) => {
				if (e.lengthComputable)
					uploadProgress = Math.round((e.loaded / e.total) * 100);
			};

			xhr.send(formData);

			const result = await new Promise<{ sessionId: string }>(
				(resolve, reject) => {
					xhr.onload = () => {
						if (xhr.status >= 200 && xhr.status < 300) {
							resolve(JSON.parse(xhr.responseText));
						} else {
							reject("Upload failed");
						}
					};
					xhr.onerror = () => reject("Upload failed");
				},
			);

			sessionId = result.sessionId;
			startPolling(); // Just monitor upload status
			return await getUploadedFilePaths();
		} catch (err) {
			errorMessage = "Unexpected error during upload.";
			isUploading = false;
			return null;
		}
	}

	async function getUploadedFilePaths(): Promise<string[]> {
		const token = localStorage.getItem("token");
		const res = await fetch(
			`http://localhost:5105/api/files/by-session/${sessionId}`,
			{
				headers: { Authorization: `Bearer ${token}` },
			},
		);
		const data = await res.json();
		return data.map((f: any) => f.fullPath);
	}

	async function shareFiles(filePaths: string[]): Promise<string | null> {
	const token = localStorage.getItem("token");

	const res = await fetch("http://localhost:5105/api/files/share", {
		method: "POST",
		headers: {
			Authorization: `Bearer ${token}`,
			"Content-Type": "application/json",
		},
		body: JSON.stringify({
			FilePaths: filePaths,
			CustomerName: receiverFirstName,
			CustomerLastName: receiverLastName,
			CustomerEmail: receiverEmail,
			Passcode: protectWithPasscode ? passcode : null, // ✅ only include if protected
			ExpiresInDays: expiresInDays,
			Slug: slug.trim(),
		}),
	});

	if (!res.ok) {
		errorMessage = "Failed to share files.";
		return null;
	}

	const result = await res.json();
	downloadLink = result.link;
	successMessage = "Files shared successfully!";
	return downloadLink;
}


async function sendEmail() {
	const token = localStorage.getItem("token");
	const name = `${receiverFirstName} ${receiverLastName}`;
	const htmlBody = getEmailHtml(
		name,
		downloadLink,
		protectWithPasscode ? passcode : "", // ✅ no passcode in plain email if unprotected
		body
	);

	await fetch("http://localhost:5105/api/email/send", {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
			Authorization: `Bearer ${token}`,
		},
		body: JSON.stringify({
			ToEmail: receiverEmail,
			Subject: subject.trim() || "You've received secure files",
			HtmlBody: htmlBody,
		}),
	});
}


	// 🔹 Uploads and shares files, then sends the email
	async function uploadFilesAndSendEmail() {
	const filePaths = await uploadFiles();
	if (!filePaths) return;

	if (!slug.trim()) {
		slug = `${receiverFirstName}-${receiverLastName}`
			.toLowerCase()
			.replace(/\s+/g, "-");
	}

	// 💡 Only generate passcode if user checked the option
	if (protectWithPasscode && !passcode) {
		passcode = generatePasscode();
	}

	if (receiverId && (receiverFirstName !== originalFirstName || receiverLastName !== originalLastName)) {
		await updateCustomerName(receiverId, receiverFirstName, receiverLastName);
	}

	const link = await shareFiles(filePaths); // ✅ passcode is now correct when calling share
	if (!link) return;

	await sendEmail();
}


	async function updateCustomerName(id: string, name: string, lastName: string) {
	const token = localStorage.getItem("token");
	try {
		await fetch(`http://localhost:5105/api/customers/${id}`, {
			method: "PUT",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${token}`
			},
			body: JSON.stringify({ name, lastName })
		});
	} catch (err) {
		console.error("Failed to update customer:", err);
	}
}

	function startPolling() {
		if (!sessionId) return;

		pollingInterval = setInterval(async () => {
			const res = await fetch(
				`http://localhost:5105/api/files/upload-status/${sessionId}`,
			);
			if (!res.ok) return;

			const data = await res.json();
			uploadStatus = data.status;

			if (data.status === "Completed" || data.status === "Failed") {
				clearInterval(pollingInterval!);
				pollingInterval = null;
				isUploading = false;
				isFinalizing = false;
				if (data.status === "Failed")
					errorMessage = data.errorMessage || "Upload failed.";
			}
		}, 1000);
	}

	function openPreview() {
		showEmailPreview = true;
		setTimeout(() => {
			if (previewContainer)
				previewContainer.innerHTML = getEmailHtml(
					`${receiverFirstName} ${receiverLastName}`,
					downloadLink || "https://example.com",
					passcode,
					body,
				);
		}, 0);
	}

	function handleFileUpload(event: Event) {
		const input = event.target as HTMLInputElement;
		if (input?.files) files = Array.from(input.files);
	}

	function openFileDialog() {
		document.getElementById("fileInput")?.click();
	}

	function handleDrop(event: DragEvent) {
		event.preventDefault();
		isDragging = false;
		if (event.dataTransfer?.files)
			files = Array.from(event.dataTransfer.files);
	}

	function handleDragOver(event: DragEvent) {
		event.preventDefault();
		isDragging = true;
	}

	function handleDragLeave() {
		isDragging = false;
	}

	onMount(() => {
		if (showEmailPreview && previewContainer) {
			previewContainer.innerHTML = getEmailHtml(
				`${receiverFirstName} ${receiverLastName}`,
				downloadLink || "https://preview-link.com",
				passcode,
				body,
			);
		}
	});
</script>

<div class="min-h-screen bg-gray-950 px-4 py-12 text-white">
	<div
		class="max-w-3xl mx-auto bg-gray-900 p-8 rounded-2xl shadow-lg border border-gray-800"
	>
		<Toast
			message={successMessage}
			type="success"
			onClose={() => (successMessage = "")}
		/>

		<Toast
			message={errorMessage}
			type="error"
			onClose={() => (errorMessage = "")}
		/>
		<h1 class="text-2xl font-bold mb-6 text-center">
			Send Files to Receiver
		</h1>

		<div class="grid grid-cols-1 md:grid-cols-2 gap-6">
			<div class="relative">
				<input
					type="email"
					bind:value={receiverEmail}
					placeholder="Receiver Email"
					class="rounded-lg bg-gray-800 px-4 py-3 w-full placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
					on:input={() => showSuggestions = true}
				/>
			
				{#if showSuggestions}
					<ul class="absolute left-0 w-full bg-gray-900 border border-gray-700 rounded mt-1 z-10">
						{#each customerSuggestions as customer}
	<button
		type="button"
		class="w-full text-left px-4 py-2 hover:bg-gray-800 cursor-pointer"
		on:click={() => selectCustomer(customer)}
	>
		{customer.email}
	</button>
{/each}
					</ul>
				{/if}
			</div>

			<input
				type="text"
				bind:value={receiverFirstName}
				placeholder="Receiver First Name"
				class="rounded-lg bg-gray-800 px-4 py-3 w-full placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
			/>

			<input
				type="text"
				bind:value={receiverLastName}
				placeholder="Receiver Last Name"
				class="rounded-lg bg-gray-800 px-4 py-3 w-full placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
			/>
			<input
				type="number"
				bind:value={expiresInDays}
				min="1"
				placeholder="Expires in (days)"
				class="rounded-lg bg-gray-800 px-4 py-3 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
			/>
		</div>

		<div class="mt-6 space-y-4">
			<input
	type="text"
	bind:value={slug}
	on:input={() => (slugManuallyEdited = true)}
	placeholder="Custom Slug (e.g. john-doe-42)"
	class="rounded-lg bg-gray-800 px-4 py-3 w-full placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
/>
			<input
				type="text"
				bind:value={subject}
				placeholder="Email Subject"
				class="w-full rounded-lg bg-gray-800 px-4 py-3 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
			/>

			<textarea
				bind:value={body}
				placeholder="Email Body"
				rows="4"
				class="w-full rounded-lg bg-gray-800 px-4 py-3 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
			></textarea>
			<div class="mt-4">
				<button
					class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded"
					on:click={openPreview}
					type="button"
				>
					Preview Email
				</button>
			</div>

			<!-- File Drop Zone -->
			<div
				role="button"
				tabindex="0"
				aria-label="File drop zone"
				on:keydown={(e) => {
					if (e.key === "Enter" || e.key === " ") {
						openFileDialog();
					}
				}}
				on:click={openFileDialog}
				on:dragover={handleDragOver}
				on:dragleave={handleDragLeave}
				on:drop={handleDrop}
				class={`mt-2 w-full rounded-lg border-2 border-dashed ${
					isDragging
						? "border-blue-500 bg-gray-800"
						: "border-gray-700 bg-gray-900"
				} text-gray-400 text-center p-6 cursor-pointer transition`}
			>
				<p class="text-sm">
					{#if files.length > 0}
						{files.length} file(s) selected
					{:else}
						Drag & drop files here or <span
							class="text-blue-400 underline">browse files</span
						>
					{/if}
				</p>
				{#if files.length > 0}
					<ul
						class="mt-3 text-sm text-gray-300 list-disc list-inside"
					>
						{#each files as file}
							<li>{file.name}</li>
						{/each}
					</ul>
				{/if}
				<input
					id="fileInput"
					type="file"
					multiple
					class="hidden"
					on:change={handleFileUpload}
				/>
			</div>
			<label class="flex items-center gap-2 mt-2">
				<input type="checkbox" bind:checked={protectWithPasscode} />
				<span>Protect with passcode</span>
			</label>
		</div>
		<div class="flex gap-x-4 mt-6">
			<button
				on:click={uploadFiles}
				class="w-1/2 rounded-lg bg-green-600 py-3 text-white font-semibold hover:bg-green-700 transition duration-200"
				disabled={isUploading || isFinalizing}
			>
				Upload Files
			</button>
			<button
				on:click={uploadFilesAndSendEmail}
				class="w-1/2 rounded-lg bg-green-600 py-3 text-white font-semibold hover:bg-green-700 transition duration-200"
				disabled={isUploading || isFinalizing}
			>
				Upload & Send Files
			</button>
		</div>

		{#if isUploading || isFinalizing}
			<div class="mt-4 w-full bg-gray-700 rounded h-4 overflow-hidden">
				<div
					class={`h-full transition-all duration-300 ${
						isFinalizing
							? "bg-yellow-500 animate-pulse"
							: "bg-blue-600"
					}`}
					style="width: {uploadProgress}%"
				></div>
			</div>
			{#if isUploading}
				<p class="mt-2 text-sm text-center text-gray-300">
					Status: {uploadStatus}
				</p>
			{/if}
			<p class="mt-1 text-sm text-center text-gray-400">
				{#if isFinalizing}
					Finalizing upload on server...
				{:else}
					{uploadProgress}%
				{/if}
			</p>
		{/if}
		{#if showEmailPreview}
			<div
				class="fixed inset-0 bg-black bg-opacity-60 z-50 flex justify-center items-center"
			>
				<div
					class="bg-white text-black p-6 rounded-lg w-full max-w-2xl relative shadow-xl overflow-auto max-h-[80vh]"
				>
					<button
						on:click={() => (showEmailPreview = false)}
						class="absolute top-2 right-2 text-gray-600 hover:text-black text-2xl font-bold"
					>
						×
					</button>
					<h2 class="text-xl font-semibold mb-4">Email Preview</h2>
					<div
						class="prose max-w-full"
						bind:this={previewContainer}
					></div>
				</div>
			</div>
		{/if}
	</div>
</div>

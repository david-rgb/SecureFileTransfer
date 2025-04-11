<script lang="ts">
    let receiverEmail = "";
    let receiverFirstName = "";
    let receiverLastName = "";
    let receiverImage: File | null = null;

    let subject = "";
    let body = "";
    let files: File[] = [];
    let expiresInDays = 7;

    let isDragging = false;

    function handleFileUpload(event: Event) {
        const target = event.target as HTMLInputElement;
        if (target?.files) {
            files = Array.from(target.files);
        }
    }

    function openFileDialog() {
        document.getElementById("fileInput")?.click();
    }

    function handleDrop(event: DragEvent) {
        event.preventDefault();
        isDragging = false;
        if (event.dataTransfer?.files) {
            files = Array.from(event.dataTransfer.files);
        }
    }

    function handleDragOver(event: DragEvent) {
        event.preventDefault();
        isDragging = true;
    }

    function handleDragLeave() {
        isDragging = false;
    }

let downloadLink = '';
let passcode = ''; // Add this if passcode entry is supported
let successMessage = '';

async function handleSend() {
	try {
		console.log("Files before upload:", files);

		if (!files.length) {
			alert("Please select at least one file.");
			return;
		}

		// 1. Upload all files at once
		const formData = new FormData();
		for (const file of files) {
			formData.append('files', file);
		}

		const uploadRes = await fetch('http://localhost:5105/api/files/upload', {
			method: 'POST',
			body: formData
		});

		if (!uploadRes.ok) {
			const errorText = await uploadRes.text();
			console.error("Upload error response:", errorText);
			throw new Error("Upload failed");
		}

		const uploadData = await uploadRes.json();

		// 2. Extract file paths
		const filePaths: string[] = uploadData.files.map((f: any) => f.path.replace(/\\/g, '/'));

		console.log("Uploaded file paths:", filePaths);

		// 3. Share all file paths
		const shareRes = await fetch('http://localhost:5105/api/files/share', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({
				FilePaths: filePaths,
				CustomerName: `${receiverFirstName} ${receiverLastName}`,
				CustomerEmail: receiverEmail,
				Passcode: passcode,
				ExpiresInDays: expiresInDays
			})
		});

		if (!shareRes.ok) {
			const errText = await shareRes.text();
			throw new Error(`Failed to share: ${errText}`);
		}

		const shareData = await shareRes.json();
		downloadLink = shareData.link;
		successMessage = 'Files shared successfully!';
		console.log("Files shared successfully. Download link:", downloadLink);

	} catch (err) {
		console.error("Error uploading files:", err);
		alert("Something went wrong: " + err);
	}
}



</script>

<div class="min-h-screen bg-gray-950 px-4 py-12 text-white">
    <div
        class="max-w-3xl mx-auto bg-gray-900 p-8 rounded-2xl shadow-lg border border-gray-800"
    >
        <h1 class="text-2xl font-bold mb-6 text-center">
            Send Files to Receiver
        </h1>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <input
                type="email"
                bind:value={receiverEmail}
                placeholder="Receiver Email"
                class="rounded-lg bg-gray-800 px-4 py-3 w-full placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
            />

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
        </div>

        <button
            on:click={handleSend}
            class="mt-6 w-full rounded-lg bg-green-600 py-3 text-white font-semibold hover:bg-green-700 transition-colors duration-200"
        >
            Send Files
        </button>
    </div>
</div>

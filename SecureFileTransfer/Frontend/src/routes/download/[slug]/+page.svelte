<script lang="ts">
	import { page } from "$app/stores";
	import { onMount } from "svelte";

	let loading = true;
	let errorMessage = "";
	let passcode = "";
	let passcodeSubmitted = false;

	let customerName = "";
	let customerEmail = "";
	let expiresAt = "";
	let requiresPasscode = true;

	let slug = "";
	$: slug = $page.params.slug;

	// Files: array of objects { token, originalFileName, compressedFileName }
	let files: {
		originalFileName: string;
		compressedFileName: string;
		token: string;
		fileId: number;
	}[] = [];

	onMount(async () => {
		await fetchFileLink();
	});

	async function validatePasscode(): Promise<boolean> {
		try {
			const res = await fetch(
				"http://localhost:5105/api/auth/validate-passcode",
				{
					method: "POST",
					headers: { "Content-Type": "application/json" },
					body: JSON.stringify({ slug, passcode }),
				},
			);

			if (!res.ok) {
				const msg = await res.text();
				errorMessage = msg;
				return false;
			}

			const data = await res.json();
			return data.valid === true;
		} catch (err: any) {
			errorMessage = err.message || "Validation failed.";
			return false;
		}
	}

	async function fetchFileLink() {
		loading = true;
		errorMessage = "";

		try {
			let url = `http://localhost:5105/api/files/link/${slug}`;

			// Always include passcode if one is submitted (even if we don't yet know if it's required)
			if (passcodeSubmitted && passcode) {
				url += `?passcode=${encodeURIComponent(passcode)}`;
			}

			const res = await fetch(url);
			if (!res.ok) throw new Error(await res.text());

			const data = await res.json();

			customerName = data.customer.name;
			customerEmail = data.customer.email;
			expiresAt = data.expiresAt;
			requiresPasscode = data.requiresPasscode;

			files = data.files.map((f: any) => ({
				fileId: f.id,
				originalFileName: f.originalFileName,
				compressedFileName: f.compressedFileName,
				token: f.token,
			}));
		} catch (err: any) {
			errorMessage = err.message || "Could not retrieve file link.";
		} finally {
			loading = false;
		}
	}

	let downloadProgress = 0;
	let downloadingFile = "";

	async function downloadFile(fileId: number, originalFileName: string) {
		console.log("Trying to fetch file with id:", fileId);
		try {
			downloadingFile = originalFileName;
			downloadProgress = 0;

			const res = await fetch(
				"http://localhost:5105/api/files/download",
				{
					method: "POST",
					headers: { "Content-Type": "application/json" },
					body: JSON.stringify({ fileId }),
				},
			);

			if (!res.ok) throw new Error(await res.text());

			const contentLength = +res.headers.get("Content-Length")!;
			const reader = res.body!.getReader();
			let received = 0;
			const chunks: Uint8Array[] = [];

			while (true) {
				const { done, value } = await reader.read();
				if (done) break;
				if (value) {
					chunks.push(value);
					received += value.length;
					if (contentLength) {
						downloadProgress = Math.round(
							(received / contentLength) * 100,
						);
					}
				}
			}

			const blob = new Blob(chunks);
			const url = URL.createObjectURL(blob);
			const a = document.createElement("a");
			a.href = url;
			a.download = originalFileName;
			document.body.appendChild(a);
			a.click();
			document.body.removeChild(a);
			URL.revokeObjectURL(url);
		} catch (err: any) {
			alert(`Download failed: ${err.message}`);
		} finally {
			downloadingFile = "";
			downloadProgress = 0;
		}
	}
</script>

{#if loading}
	<p class="text-white text-center mt-12">Loading...</p>
{:else if errorMessage}
	<p class="text-red-500 text-center mt-12">{errorMessage}</p>
{:else}
	<div
		class="text-white max-w-4xl mx-auto mt-12 p-8 bg-gray-800 rounded-xl shadow space-y-6"
	>
		<h2 class="text-3xl font-bold text-center">Hello {customerName},</h2>

		<p class="text-center text-lg text-gray-300">
			Thank you for your cooperation. You’ve received some secure files
			from our system. Below you can see the list of files sent to you
			along with their details. The files will expire on <strong
				>{expiresAt}</strong
			>.
		</p>

		<p class="text-center text-sm text-gray-400">
			Customer Email: {customerEmail}
		</p>

		{#if requiresPasscode && !passcodeSubmitted}
			<div class="mt-4 text-center">
				<input
					type="password"
					bind:value={passcode}
					placeholder="Enter your secure passcode"
					class="px-4 py-2 w-full max-w-sm bg-gray-900 text-white rounded"
				/>
				<button
				class="mt-4 bg-blue-600 hover:bg-blue-700 px-6 py-2 rounded font-semibold"
				on:click={async () => {
					loading = true;
					errorMessage = "";
					const valid = await validatePasscode();
					passcodeSubmitted = valid;
					if (valid) {
						await fetchFileLink();
					}
					loading = false;
				}}
			>
				Unlock Files
			</button>
			
			</div>
		{:else}
			<div class="overflow-x-auto">
				<table
					class="w-full text-sm table-auto border border-gray-700 rounded"
				>
					<thead
						class="bg-gray-700 text-left text-gray-300 uppercase text-xs"
					>
						<tr>
							<th class="px-4 py-2">Original File</th>
							<th class="px-4 py-2">Compressed File</th>
							<th class="px-4 py-2 text-right">Action</th>
						</tr>
					</thead>
					<tbody>
						{#each files as file}
							<tr
								class="border-t border-gray-600 hover:bg-gray-700 transition"
							>
								<td class="px-4 py-2"
									>{file.originalFileName}</td
								>
								<td class="px-4 py-2"
									>{file.compressedFileName}</td
								>
								<td class="px-4 py-2 text-right">
									<button
										class="bg-green-600 hover:bg-green-700 text-white text-sm font-semibold px-4 py-1.5 rounded"
										on:click={() =>
											downloadFile(
												file.fileId,
												file.originalFileName,
											)}
									>
										Download
									</button>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
				{#if downloadingFile}
					<div class="mt-6">
						<p class="text-sm text-gray-300 mb-1">
							Downloading: {downloadingFile}
						</p>
						<div
							class="w-full h-4 bg-gray-700 rounded overflow-hidden"
						>
							<div
								class="h-full bg-blue-600 transition-all"
								style="width: {downloadProgress}%;"
							></div>
						</div>
						<p class="text-xs text-gray-400 mt-1">
							{downloadProgress}%
						</p>
					</div>
				{/if}
			</div>
		{/if}
	</div>
{/if}

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
		document.getElementById('fileInput')?.click();
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

	function handleSend() {
		console.log("Sending files to:", {
			receiverEmail,
			receiverFirstName,
			receiverLastName,
			subject,
			body,
			files,
			receiverImage,
			expiresInDays,
		});
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
                <input
                    id="fileInput"
                    type="file"
                    multiple
                    class="hidden"
                    on:change={handleFileUpload}
                />
            </div>

            <input
                type="number"
                bind:value={expiresInDays}
                min="1"
                placeholder="Expires in (days)"
                class="w-40 rounded-lg bg-gray-800 px-4 py-3 placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
            />
        </div>

        <button
            on:click={handleSend}
            class="mt-6 w-full rounded-lg bg-green-600 py-3 text-white font-semibold hover:bg-green-700 transition-colors duration-200"
        >
            Send Files
        </button>
    </div>
</div>

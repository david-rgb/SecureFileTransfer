<script lang="ts">
	import { onMount } from 'svelte';

	let passcodeInput = '';
	let correctPasscode = '1234'; // this would be injected via backend in real app
	let accessGranted = false;

	let receiverName = 'John Smith';
	let receiverImageUrl = '/placeholder.jpg'; // TODO: replace with real data
	let isLoading = false;

	function handleSubmit() {
		if (passcodeInput === correctPasscode) {
			accessGranted = true;
		} else {
			alert('Incorrect passcode. Please try again.');
		}
	}

	function handleDownload() {
		isLoading = true;
		// Simulate file download
		setTimeout(() => {
			alert('Your files are being downloaded...');
			isLoading = false;
		}, 2000);
	}
</script>

<svelte:head>
	<title>Secure File Download</title>
</svelte:head>

<div class="min-h-screen bg-gray-950 text-white flex flex-col items-center justify-center px-4 py-12">
	<h1 class="text-3xl font-bold mb-6 text-center">Secure File Access</h1>

	{#if !accessGranted}
		<div class="bg-gray-900 p-6 rounded-xl shadow-lg border border-gray-800 w-full max-w-sm">
			<p class="mb-4 text-sm text-gray-400">Enter your passcode to access the download.</p>

			<input
				type="text"
				bind:value={passcodeInput}
				placeholder="Passcode"
				class="w-full mb-4 px-4 py-3 rounded-lg bg-gray-800 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
			/>

			<button
				on:click={handleSubmit}
				class="w-full rounded-lg bg-blue-600 py-3 text-white font-semibold hover:bg-blue-700 transition-colors duration-200"
			>
				Unlock Files
			</button>
		</div>
	{:else}
		<!-- Greeting Section -->
		<div class="w-full max-w-2xl flex flex-col items-center space-y-4">
			<img src={receiverImageUrl} alt="Receiver" class="w-24 h-24 rounded-full border-4 border-blue-500 shadow-md" />
			<h2 class="text-2xl font-semibold">Hello, {receiverName} 👋</h2>
			<p class="text-gray-400 text-sm mb-4">Click the button below to download your files.</p>

			<button
				on:click={handleDownload}
				class="rounded-lg bg-green-600 px-6 py-3 text-white font-semibold hover:bg-green-700 transition"
				disabled={isLoading}
			>
				{isLoading ? 'Preparing...' : 'Download Your Files'}
			</button>
		</div>
	{/if}
</div>

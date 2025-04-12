<script lang="ts">
	import { onMount } from 'svelte';
	import FilesView from './views/FilesView.svelte';
	import ClientsTable from './views/ClientsTable.svelte';
	import CleanupView from './views/CleanupView.svelte';

	type SentFileRecord = {
		token: string;
		receiverName: string;
		receiverEmail: string;
		status: 'Pending' | 'Downloaded' | 'Expired';
		downloadCount: number;
		expiresIn: number;
		sentAt: string;
	};

	let sentLinks: SentFileRecord[] = [];
	let errorMessage = '';

    let customers: { name: string; email: string; downloadCount: number }[] = [];

async function fetchCustomers() {
	try {
		const res = await fetch('http://localhost:5105/api/dashboard/customers');
		if (!res.ok) throw new Error(await res.text());
		customers = await res.json();
	} catch (err: any) {
		errorMessage = err.message || 'Failed to fetch customer data.';
	}
}

	async function fetchSentLinks() {
		try {
			const res = await fetch('http://localhost:5105/api/dashboard/shared-links');
			if (!res.ok) throw new Error(await res.text());

			const data = await res.json();
			sentLinks = data;
		} catch (err: any) {
			errorMessage = err.message || 'Failed to fetch shared links.';
		}
	}

	function updateLinks(newLinks: SentFileRecord[]) {
		sentLinks = newLinks;
	}

	onMount(() => {
	fetchSentLinks();
	fetchCustomers();
});
</script>

<div class="min-h-screen bg-gray-950 text-white px-4 py-12">
	<div class="max-w-7xl mx-auto">
		<h1 class="text-4xl font-bold text-center mb-12">Admin Dashboard</h1>

		{#if errorMessage}
			<p class="text-red-500 text-center mb-4">{errorMessage}</p>
		{/if}

		<div class="grid grid-cols-1 lg:grid-cols-2 grid-rows-[minmax(0,300px)_auto] gap-6">
			<!-- Top Left: Clients -->
			<div class="bg-gray-900 p-4 rounded-xl border border-gray-800 overflow-auto">
				<h2 class="text-xl font-semibold mb-4">👥 Clients</h2>
				<ClientsTable {customers} />
			</div>

			<!-- Top Right: Cleanup -->
			<div class="bg-gray-900 p-4 rounded-xl border border-gray-800">
				<h2 class="text-xl font-semibold mb-4">🧹 Cleanup Tools</h2>
				<CleanupView {sentLinks} update={updateLinks} />
			</div>

			<!-- Bottom: All Files Table -->
			<div class="col-span-full bg-gray-900 p-4 rounded-xl border border-gray-800 overflow-auto">
				<h2 class="text-xl font-semibold mb-4">📄 All Files</h2>
				<FilesView {sentLinks} />
			</div>
		</div>
	</div>
</div>

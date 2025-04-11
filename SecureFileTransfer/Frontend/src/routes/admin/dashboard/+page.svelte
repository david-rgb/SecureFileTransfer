<script lang="ts">
	import FilesView from './views/FilesView.svelte';
	import ClientsTable from './views/ClientsTable.svelte';
	import CleanupView from './views/CleanupView.svelte';

	type SentFileRecord = {
		id: string;
		receiverName: string;
		receiverEmail: string;
		status: 'Pending' | 'Downloaded' | 'Expired';
		downloadCount: number;
		expiresIn: number;
		sentAt: string;
	};

	let sentLinks: SentFileRecord[] = [
		{
			id: '1',
			receiverName: 'Jane Doe',
			receiverEmail: 'jane@example.com',
			status: 'Pending',
			downloadCount: 0,
			expiresIn: 5,
			sentAt: '2025-04-10'
		},
		{
			id: '2',
			receiverName: 'John Smith',
			receiverEmail: 'john@example.com',
			status: 'Downloaded',
			downloadCount: 3,
			expiresIn: 0,
			sentAt: '2025-04-08'
		},
		{
			id: '3',
			receiverName: 'Jane Doe',
			receiverEmail: 'jane@example.com',
			status: 'Expired',
			downloadCount: 1,
			expiresIn: 0,
			sentAt: '2025-04-02'
		}
	];

	function updateLinks(newLinks: SentFileRecord[]) {
		sentLinks = newLinks;
	}
</script>

<div class="min-h-screen bg-gray-950 text-white px-4 py-12">
	<div class="max-w-7xl mx-auto">
		<h1 class="text-4xl font-bold text-center mb-12">Admin Dashboard</h1>

		<!-- Layout Grid -->
		<div class="grid grid-cols-1 lg:grid-cols-2 grid-rows-[minmax(0,300px)_auto] gap-6">

			<!-- Top Left: Clients -->
			<div class="bg-gray-900 p-4 rounded-xl border border-gray-800 overflow-auto">
				<h2 class="text-xl font-semibold mb-4">👥 Clients</h2>
				<ClientsTable {sentLinks} />
			</div>

			<!-- Top Right: Cleanup -->
			<div class="bg-gray-900 p-4 rounded-xl border border-gray-800">
				<h2 class="text-xl font-semibold mb-4">🧹 Cleanup Tools</h2>
				<CleanupView {sentLinks} update={updateLinks} />
			</div>

			<!-- Bottom: All Files Table (spans full width) -->
			<div class="col-span-full bg-gray-900 p-4 rounded-xl border border-gray-800 overflow-auto">
				<h2 class="text-xl font-semibold mb-4">📄 All Files</h2>
				<FilesView {sentLinks} />
			</div>
		</div>
	</div>
</div>

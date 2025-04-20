<script lang="ts">
	import { onMount } from 'svelte';
	import FilesView from './views/FilesView.svelte';
	import ClientsTable from './views/ClientsTable.svelte';
	import CleanupView from './views/CleanupView.svelte';
	import DownloadLinksTable from './views/DownloadLinksTable.svelte';

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

	let uploadedFiles: any[] = [];

async function fetchUploadedFiles() {
	const token = localStorage.getItem('token');
	const res = await fetch('http://localhost:5105/api/dashboard/files', {
		headers: { Authorization: `Bearer ${token}` }
	});
	if (!res.ok) return;
	uploadedFiles = await res.json();
}

async function fetchCustomers() {
	try {
        const token = localStorage.getItem('token');
		const res = await fetch('http://localhost:5105/api/dashboard/customers', {
	method: 'GET',
	headers: {
		Authorization: `Bearer ${token}`,
		'Content-Type': 'application/json'
	}
});
		if (!res.ok) throw new Error(await res.text());
		customers = await res.json();
        console.log(customers)
	} catch (err: any) {
		errorMessage = err.message || 'Failed to fetch customer data.';
	}
}

	async function fetchSentLinks() {
		try {
            const token = localStorage.getItem('token');
            const res = await fetch('http://localhost:5105/api/dashboard/shared-links', {
	method: 'GET',
	headers: {
		Authorization: `Bearer ${token}`,
		'Content-Type': 'application/json'
	}
});
			if (!res.ok) throw new Error(await res.text());

			const data = await res.json();
			sentLinks = data;
            console.log(sentLinks)
		} catch (err: any) {
			errorMessage = err.message || 'Failed to fetch shared links.';
		}
	}

	function updateLinks(newLinks: SentFileRecord[]) {
		sentLinks = newLinks;
	}

	onMount(() => {
		fetchUploadedFiles();
	fetchCustomers();
	fetchDownloadLinks()
});


let downloadLinks: any[] = [];

async function fetchDownloadLinks() {
	try {
		const token = localStorage.getItem('token');
		const res = await fetch('http://localhost:5105/api/dashboard/download-links', {
			headers: {
				Authorization: `Bearer ${token}`
			}
		});
		if (!res.ok) throw new Error(await res.text());
		downloadLinks = await res.json();
	} catch (err: any) {
		errorMessage = err.message || 'Failed to fetch download links.';
	}
}
</script>

<!-- Main content container -->
<!-- 2x2 Grid for Dashboard -->
<div class="grid grid-cols-1 lg:grid-cols-2 grid-rows-2 gap-4 h-full" style="padding-top: 30px;">
	<!-- Top Left: Cleanup -->
	<div class="bg-gray-900 p-2 rounded-lg border border-gray-800 overflow-hidden">
		<CleanupView {sentLinks} update={updateLinks} />
	</div>

	<!-- Top Right: Clients -->
	<div class="bg-gray-900 p-2 rounded-lg border border-gray-800 overflow-hidden">
		<ClientsTable {customers} />
	</div>

	<!-- Bottom Left: Download Links -->
	<div class="bg-gray-900 p-2 rounded-lg border border-gray-800 overflow-hidden">
		<!-- You will create a component like DownloadLinksTable.svelte -->
		<DownloadLinksTable {downloadLinks} />
	</div>

	<!-- Bottom Right: All Files -->
	<div class="bg-gray-900 p-2 rounded-lg border border-gray-800 overflow-hidden">
		<FilesView files={uploadedFiles} />
	</div>
</div>

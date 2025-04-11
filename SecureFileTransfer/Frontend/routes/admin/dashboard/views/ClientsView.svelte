<script lang="ts">
	export let sentLinks: any[];

	const clients = Array.from(
		new Map(sentLinks.map(link => [link.receiverEmail, link])).values()
	);
</script>

<div class="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
	{#each clients as client}
		<div class="bg-gray-900 border border-gray-800 p-6 rounded-xl shadow hover:shadow-lg transition">
			<h2 class="text-xl font-semibold mb-1">{client.receiverName}</h2>
			<p class="text-gray-400 mb-2">{client.receiverEmail}</p>
			<p class="text-sm text-gray-500">Status: {client.status}</p>
			<p class="text-sm text-gray-500">Downloads: {client.downloadCount}</p>
		</div>
	{/each}
</div>

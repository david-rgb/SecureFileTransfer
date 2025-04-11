<script lang="ts">
	export let sentLinks: any[];

	const clients = Array.from(
		new Map(sentLinks.map(link => [link.receiverEmail, link])).values()
	);
</script>

<table class="min-w-full text-sm table-auto">
	<thead class="bg-gray-800 text-gray-400 uppercase text-left">
		<tr>
			<th class="px-4 py-2">Name</th>
			<th class="px-4 py-2">Email</th>
			<th class="px-4 py-2">Status</th>
			<th class="px-4 py-2">Downloads</th>
		</tr>
	</thead>
	<tbody>
		{#each clients as client}
			<tr class="border-t border-gray-800 hover:bg-gray-800 transition">
				<td class="px-4 py-2">{client.receiverName}</td>
				<td class="px-4 py-2">{client.receiverEmail}</td>
				<td class="px-4 py-2">{client.status}</td>
				<td class="px-4 py-2">{client.downloadCount}</td>
			</tr>
		{/each}
	</tbody>
</table>

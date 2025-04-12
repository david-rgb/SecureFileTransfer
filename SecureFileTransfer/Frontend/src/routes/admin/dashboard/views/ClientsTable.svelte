<script lang="ts">
	export let customers: { name: string; email: string; downloadCount: number }[];
</script>

<table class="min-w-full text-sm table-auto">
	<thead class="bg-gray-800 text-gray-400 uppercase text-left">
		<tr>
			<th class="px-4 py-2">Name</th>
			<th class="px-4 py-2">Email</th>
			<th class="px-4 py-2">Downloads</th>
		</tr>
	</thead>
	<tbody>
		{#each customers as customer}
			<tr class="border-t border-gray-800 hover:bg-gray-800 transition">
				<td class="px-4 py-2">{customer.name}</td>
				<td class="px-4 py-2">{customer.email}</td>
				<td class="px-4 py-2">{customer.downloadCount}</td>
			</tr>
		{/each}
	</tbody>
</table>

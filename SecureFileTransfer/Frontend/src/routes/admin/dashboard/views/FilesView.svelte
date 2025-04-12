<script lang="ts">
	export let sentLinks: {
		token: string;
		receiverName: string;
		receiverEmail: string;
		status: 'Expired' | 'Pending' | 'Downloaded'; // extend as needed
		downloadCount: number;
		expiresIn: number;
		sentAt: string;
	}[];
</script>

<div class="overflow-auto rounded-xl border border-gray-800 bg-gray-900">
	<table class="min-w-full table-auto">
		<thead class="bg-gray-800 text-sm uppercase text-gray-400">
			<tr>
				<th class="px-6 py-4 text-left">Receiver</th>
				<th class="px-6 py-4 text-left">Email</th>
				<th class="px-6 py-4 text-left">Status</th>
				<th class="px-6 py-4 text-left">Downloads</th>
				<th class="px-6 py-4 text-left">Expires In</th>
				<th class="px-6 py-4 text-left">Sent At</th>
				<th class="px-6 py-4 text-left">Action</th>
			</tr>
		</thead>
		<tbody>
			{#each sentLinks as link}
				<tr class="border-t border-gray-800 hover:bg-gray-800 transition">
					<td class="px-6 py-4">{link.receiverName}</td>
					<td class="px-6 py-4">{link.receiverEmail}</td>
					<td class="px-6 py-4">
						<span class={
							link.status === 'Downloaded' ? 'text-green-400' :
							link.status === 'Expired' ? 'text-red-400' : 'text-yellow-400'
						}>
							{link.status}
						</span>
					</td>
					<td class="px-6 py-4">{link.downloadCount}</td>
					<td class="px-6 py-4">{link.expiresIn} day(s)</td>
					<td class="px-6 py-4">{new Date(link.sentAt).toLocaleString()}</td>
					<td class="px-6 py-4">
						<a
							class="text-blue-400 underline hover:text-blue-300"
							href={`/download/${link.token}`}
							target="_blank"
						>
							Open Link
						</a>
					</td>
				</tr>
			{/each}
		</tbody>
	</table>
</div>

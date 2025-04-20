<script lang="ts">
	export let sentLinks: any[];
	export let update: (newList: any[]) => void;

	function deleteExpired() {
		update(sentLinks.filter(link => link.status !== 'Expired'));
	}

	async function clearAll() {
        try {
            await fetch('http://localhost:5105/api/dashboard/clear-links', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' }
            })} catch (err: any) {
		        alert(err.message)
	}};
</script>

<div class="w-full  max-h-full bg-gray-900 p-6 rounded-xl border border-gray-800 text-center space-y-4">
	<h2 class="text-xl font-semibold mb-4">Cleanup Tools</h2>

	<button
		class="rounded-lg bg-red-600 py-2 text-white font-semibold hover:bg-red-700 transition"
        style="width: 60%;"
		on:click={deleteExpired}
	>
		Delete Expired Links
	</button>

	<button
		class="rounded-lg bg-yellow-600 py-2 text-white font-semibold hover:bg-yellow-700 transition"
        style="width: 60%;"
		on:click={clearAll}
	>
		Clear All Links
	</button>
</div>

<script lang="ts">
	export let sentLinks: any[];
	export let update: (newList: any[]) => void;

	function deleteExpired() {
		update(sentLinks.filter(link => link.status !== 'Expired'));
	}

	function clearAll() {
		update([]);
	}
</script>

<div class="max-w-md mx-auto bg-gray-900 p-6 rounded-xl border border-gray-800 text-center space-y-4">
	<h2 class="text-xl font-semibold mb-4">Cleanup Tools</h2>

	<button
		class="w-full rounded-lg bg-red-600 py-2 text-white font-semibold hover:bg-red-700 transition"
		on:click={deleteExpired}
	>
		Delete Expired Links
	</button>

	<button
		class="w-full rounded-lg bg-yellow-600 py-2 text-white font-semibold hover:bg-yellow-700 transition"
		on:click={clearAll}
	>
		Clear All Links
	</button>
</div>

<script lang="ts">
	import { fade } from 'svelte/transition';
	import { onMount, onDestroy } from 'svelte';

	export let message: string = '';
	export let type: 'success' | 'error' = 'success';
	export let onClose: () => void = () => {};
	export let duration: number = 5000;

	let timeout: ReturnType<typeof setTimeout>;

	onMount(() => {
		if (message) {
			timeout = setTimeout(() => {
				onClose();
			}, duration);
		}
	});

	onDestroy(() => {
		if (timeout) clearTimeout(timeout);
	});
</script>

{#if message}
	<div
		class={`fixed top-[78px] right-4 z-50 max-w-sm w-full px-4 py-2 rounded shadow-lg
			${type === 'success' ? 'bg-green-600 text-white' : 'bg-red-600 text-white'}`}
		transition:fade
	>
		<div class="flex items-start justify-between gap-4">
			<!-- Message -->
			<span class="break-words whitespace-normal flex-grow min-w-0">
				{message}
			</span>

			<!-- Close Button -->
			<button
				on:click={onClose}
				class="text-white text-lg font-bold leading-none hover:text-gray-200 focus:outline-none"
				aria-label="Close notification"
			>
				&times;
			</button>
		</div>
	</div>
{/if}

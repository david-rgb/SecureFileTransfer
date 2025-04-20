<script lang="ts">
	import Toast from "$lib/components/Toast.svelte";
	import { writable } from "svelte/store";

	export let title = '';
	export let onSubmit: () => Promise<void>;
	export let loading = false;

	const successMessage = writable('');
	const errorMessage = writable('');

	export function setSuccess(msg: string) {
		successMessage.set(msg);
	}

	export function setError(msg: string) {
		errorMessage.set(msg);
	}
</script>

<form
	class="bg-gray-900 p-6 rounded-lg shadow border border-gray-800 space-y-4"
	on:submit|preventDefault={() => !loading && onSubmit()}
>
	<h2 class="text-xl font-semibold text-white">{title}</h2>

	<!-- Show success toast -->
	{#if $successMessage}
		<Toast type="success" message={$successMessage} onClose={() => successMessage.set('')} />
	{/if}

	<!-- Show error toast -->
	{#if $errorMessage}
		<Toast type="error" message={$errorMessage} onClose={() => errorMessage.set('')} />
	{/if}

	<slot />

	<div class="pt-2">
		<slot name="actions">
			<button
				type="submit"
				class="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2 rounded transition"
				disabled={loading}
			>
				{loading ? 'Saving...' : 'Save'}
			</button>
		</slot>
	</div>
</form>

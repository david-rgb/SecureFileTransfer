<script lang="ts">
	let email = '';
	let password = '';
	let errorMessage = '';
	let successMessage = '';

	async function register() {
		errorMessage = '';
		successMessage = '';

		try {
			const res = await fetch('http://localhost:5105/api/auth/register', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ email, password })
			});

			if (!res.ok) throw new Error(await res.text());

			successMessage = 'Registration successful! You can now log in.';
		} catch (err: any) {
			errorMessage = err.message;
		}
	}
</script>

<div class="max-w-md mx-auto mt-16 bg-gray-900 text-white p-6 rounded-lg border border-gray-800 shadow-lg">
	<h2 class="text-2xl font-bold mb-4 text-center">Create Admin Account</h2>

	{#if errorMessage}
		<p class="text-red-500 mb-4 text-center">{errorMessage}</p>
	{/if}

	{#if successMessage}
		<p class="text-green-500 mb-4 text-center">{successMessage}</p>
	{/if}

	<input type="email" bind:value={email} placeholder="Email" class="input mb-3" required />
	<input type="password" bind:value={password} placeholder="Password" class="input mb-4" required />

	<button class="w-full bg-blue-600 hover:bg-blue-700 py-2 rounded font-semibold" on:click={register}>
		Register
	</button>

	<p class="mt-4 text-sm text-center">
		Already have an account? <a href="/admin/login" class="text-blue-400 hover:underline">Log in</a>
	</p>
</div>

<style>
	.input {
		@apply w-full px-4 py-2 bg-gray-800 text-white rounded outline-none focus:ring-2 focus:ring-blue-500;
	}
</style>

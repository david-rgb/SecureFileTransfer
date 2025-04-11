<script lang="ts">
	let email = '';
	let password = '';
	let error = '';

	async function handleLogin() {
		// Dummy check — replace with real API call later
		if (!email || !password) {
			error = 'Please fill in both fields.';
			return;
		}
		error = '';
		console.log('Logging in with:', email, password);
		// TODO: Call backend API and handle token
	}
</script>

<div class="flex min-h-screen items-center justify-center bg-gray-950 px-4 py-12">
	<div class="w-full max-w-md space-y-8 bg-gray-900 p-8 rounded-2xl shadow-lg border border-gray-800">
		<div class="text-center">
			<h1 class="text-3xl font-bold text-white">Secure Admin Login</h1>
			<p class="text-sm text-gray-400">Enter your credentials to continue</p>
		</div>

		<form on:submit|preventDefault={handleLogin} class="space-y-6">
			<div class="space-y-4">
				<input
					type="email"
					bind:value={email}
					placeholder="Email"
					class="w-full rounded-lg bg-gray-800 px-4 py-3 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
				/>

				<input
					type="password"
					bind:value={password}
					placeholder="Password"
					class="w-full rounded-lg bg-gray-800 px-4 py-3 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
				/>
			</div>

			{#if error}
				<p class="text-red-500 text-sm text-center">{error}</p>
			{/if}

			<button
				type="submit"
				class="w-full rounded-lg bg-blue-600 py-3 text-white font-semibold hover:bg-blue-700 transition-colors duration-200"
			>
				Log In
			</button>
		</form>
	</div>
</div>

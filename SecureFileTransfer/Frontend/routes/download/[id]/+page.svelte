<script lang="ts">
	import { goto } from '$app/navigation';

	let email = '';
	let password = '';
	let error = '';

	function isValidEmail(email: string): boolean {
		return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
	}

	async function handleLogin() {
		error = '';
		console.log('About to send login request with:', { email, password });

		if (!email || !password) {
			error = 'Both fields are required.';
			return;
		}

		if (!isValidEmail(email)) {
			error = 'Please enter a valid email address.';
			return;
		}

		try {
			const res = await fetch('http://localhost:5105/api/auth/login', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ email, password })
			});

			console.log('Response status:', res.status);

			if (!res.ok) {
				error = 'Invalid email or password.';
				console.warn('Login failed:', await res.text());
				return;
			}

			const data = await res.json();
			console.log('Login success, token:', data.token);

			localStorage.setItem('token', data.token);
			goto('/admin/dashboard');
		} catch (err) {
			console.error('Unexpected error:', err);
			error = 'An unexpected error occurred.';
		}
	}
</script>

<div class="min-h-screen bg-gray-950 flex flex-col items-center justify-center px-4 text-white">
	<div class="w-full max-w-sm p-8 rounded-2xl bg-gray-900 border border-gray-800 shadow-lg">
		<h1 class="text-2xl font-bold mb-6 text-center">Admin Login</h1>

		{#if error}
			<p class="text-red-500 mb-4 text-sm text-center">{error}</p>
		{/if}

		<input
			type="email"
			bind:value={email}
			placeholder="Email"
			class="mb-4 w-full px-4 py-3 rounded-lg bg-gray-800 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
		/>

		<input
			type="password"
			bind:value={password}
			placeholder="Password"
			class="mb-4 w-full px-4 py-3 rounded-lg bg-gray-800 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
		/>

		<button
			on:click={handleLogin}
			class="w-full rounded-lg bg-blue-600 py-3 font-semibold hover:bg-blue-700 transition-colors duration-200"
		>
			Log In
		</button>
	</div>
</div>

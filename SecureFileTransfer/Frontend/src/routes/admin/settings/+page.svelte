<script lang="ts">
	import Form from '$lib/components/Form.svelte';
	import { onMount } from 'svelte';

	let currentPassword = '';
	let newPassword = '';
	let confirmPassword = '';
	let passwordLoading = false;

	let smtpServer = '';
	let port = 587;
	let username = '';
	let senderEmail = '';
	let senderDisplayName = '';
	let useSSL = true;
	let emailPassword = '';
	let emailLoading = false;

	let passwordForm: any;
	let emailForm: any;

	import { tick } from 'svelte';

onMount(async () => {
	const token = localStorage.getItem('token');
	if (!token) return;

	try {
		const res = await fetch('http://localhost:5105/api/settings/email', {
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${token}`
			}
		});
		if (!res.ok) throw new Error(await res.text());
		const data = await res.json();
		smtpServer = data.smtpServer;
		port = data.port;
		username = data.username;
		senderEmail = data.senderEmail;
		senderDisplayName = data.senderDisplayName;
		useSSL = data.useSSL;
	} catch (err: any) {
		await tick(); // ensure `emailForm` is bound
		emailForm?.setError(err.message || 'Failed to load email settings.');
	}
});


	async function submitPassword() {
		passwordLoading = true;

		if (newPassword !== confirmPassword) {
			passwordForm?.setError('New passwords do not match.');
			passwordLoading = false;
			return;
		}

		try {
			const token = localStorage.getItem('token');
			const res = await fetch('http://localhost:5105/api/settings/password', {
				method: 'PUT',
				headers: {
					'Content-Type': 'application/json',
					Authorization: `Bearer ${token}`
				},
				body: JSON.stringify({
					currentPassword,
					newPassword,
					confirmPassword
				})
			});
			if (!res.ok) throw new Error(await res.text());

			passwordForm?.setSuccess('Password updated successfully!');
			currentPassword = newPassword = confirmPassword = '';
		} catch (err: any) {
			passwordForm?.setError(err.message || 'Something went wrong.');
		} finally {
			passwordLoading = false;
		}
	}

	async function submitEmailSettings() {
		emailLoading = true;

		try {
			const token = localStorage.getItem('token');
			const res = await fetch('http://localhost:5105/api/settings/email', {
				method: 'PUT',
				headers: {
					'Content-Type': 'application/json',
					Authorization: `Bearer ${token}`
				},
				body: JSON.stringify({
					smtpServer,
					port,
					username,
					password: emailPassword,
					senderEmail,
					senderDisplayName,
					useSSL
				})
			});

			if (!res.ok) throw new Error(await res.text());

			emailForm?.setSuccess('Email settings updated!');
			emailPassword = '';
		} catch (err: any) {
			emailForm?.setError(err.message || 'Something went wrong.');
		} finally {
			emailLoading = false;
		}
	}
</script>

<div class="flex justify-center mt-8">
	<div class="flex flex-col md:flex-row md:space-x-6 space-y-6 md:space-y-0 justify-center items-start w-full px-4">
		<!-- Change Password -->
		<div class="w-full md:w-1/2 max-w-md">
			<Form
				bind:this={passwordForm}
				title="Change Password"
				loading={passwordLoading}
				onSubmit={submitPassword}
			>
				<input type="password" bind:value={currentPassword} placeholder="Current Password" class="input" required />
				<input type="password" bind:value={newPassword} placeholder="New Password" class="input" required />
				<input type="password" bind:value={confirmPassword} placeholder="Confirm New Password" class="input" required />
			</Form>
		</div>

		<!-- Email Settings -->
		<div class="w-full md:w-1/2 max-w-md">
			<Form
				bind:this={emailForm}
				title="Email Settings"
				loading={emailLoading}
				onSubmit={submitEmailSettings}
			>
				<input type="text" bind:value={smtpServer} placeholder="SMTP Server" class="input" required />
				<input type="number" bind:value={port} placeholder="Port" class="input" required />
				<input type="text" bind:value={username} placeholder="SMTP Username" class="input" required />
				<div class="relative flex items-center">
					<input
						type="password"
						bind:value={emailPassword}
						placeholder="SMTP Password (leave blank to keep)"
						class="input pr-10"
					/>
				
					<a
						href="https://myaccount.google.com/apppasswords"
						target="_blank"
						rel="noopener noreferrer"
						class="absolute right-3 text-blue-400 hover:text-blue-600"
						title="How to generate a Gmail App Password"
					>
						<!-- 🔐 Icon from Heroicons or similar (or just use emoji if you prefer) -->
						<svg
							xmlns="http://www.w3.org/2000/svg"
							fill="none"
							viewBox="0 0 24 24"
							stroke-width="1.5"
							stroke="currentColor"
							class="w-5 h-5"
						>
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								d="M16.5 10.5V6.75a4.5 4.5 0 00-9 0v3.75m-1.5 0h12m-10.5 0v7.5a2.25 2.25 0 002.25 2.25h6a2.25 2.25 0 002.25-2.25v-7.5"
							/>
						</svg>
					</a>
				</div>
				<input type="email" bind:value={senderEmail} placeholder="Sender Email" class="input" required />
				<input type="text" bind:value={senderDisplayName} placeholder="Sender Name" class="input" />
				<label class="flex items-center space-x-2 mt-2">
					<input type="checkbox" bind:checked={useSSL} />
					<span>Use SSL</span>
				</label>
			</Form>
		</div>
	</div>
</div>

<style>
	.input {
		@apply w-full px-4 py-2 bg-gray-800 text-white rounded outline-none focus:ring-2 focus:ring-blue-600;
	}
</style>

<script lang="ts">
  import '../lib/styles/global.css';
  import { page } from '$app/stores';
  import { onMount } from 'svelte';
  import { isTokenExpired, logout } from '$lib/auth';
  import { goto } from '$app/navigation';

  $: currentRoute = $page.url.pathname;
  $: hideNavbar = !currentRoute.startsWith('/admin');

  onMount(() => {
    const token = localStorage.getItem('token');
    if (!token || isTokenExpired(token)) {
      localStorage.removeItem('token');
      if (currentRoute.startsWith('/admin')) {
        goto('/admin/login');
      }
    }
  });
</script>

<main class="min-h-screen bg-gray-950 text-white font-sans">
  {#if !hideNavbar}
    <nav class="bg-gray-900 text-white px-6 py-4 shadow-md flex justify-between items-center">
      <div class="text-xl font-bold">Secure Admin</div>
      <div class="flex items-center gap-6">
        <a href="/admin/send" class="hover:text-blue-400" class:text-blue-400={currentRoute === '/admin/send'}>Send Files</a>
        <a href="/admin/dashboard" class="hover:text-blue-400" class:text-blue-400={currentRoute === '/admin/dashboard'}>Analytics</a>
        <a href="/admin/settings" class="hover:text-blue-400" class:text-blue-400={currentRoute === '/admin/settings'}>Settings</a>
        <button
          on:click={logout}
          class="ml-4 bg-red-600 hover:bg-red-700 px-4 py-2 rounded font-semibold text-sm"
        >
          Logout
        </button>
      </div>
    </nav>
  {/if}

  <slot />
</main>

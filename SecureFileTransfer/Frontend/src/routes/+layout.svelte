<script>
  import '../lib/styles/global.css';
	import { page } from '$app/stores';
	$: currentRoute = $page.url.pathname;
</script>

<main class="min-h-screen bg-gray-950 text-white font-sans">
  <nav class="bg-gray-900 text-white px-6 py-4 shadow-md flex justify-between items-center">
    <div class="text-xl font-bold">Secure Admin</div>
    <div class="flex gap-4">
      <a href="/admin/send" class:text-blue-400={currentRoute === '/admin/send'}>Send Files</a>
      <a href="/admin/dashboard" class:text-blue-400={currentRoute === '/admin/dashboard'}>Analytics</a>
    </div>
  </nav>
  <slot />
</main>
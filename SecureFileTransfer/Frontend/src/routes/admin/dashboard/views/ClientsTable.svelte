<script lang="ts">
	import Table from '$lib/components/Table.svelte';

	export let customers: any[] = [];

	const baseUrl = 'http://localhost:5173/download';

	const columns = ['name', 'email', 'downloadCount', 'link'];

	$: rows = customers.map(customer => {
		const slug = `${customer.name.toLowerCase().replace(/\s+/g, '-')}-${customer.id}`;
		return {
			id: customer.id,
			name: customer.name,
			email: customer.email,
			downloadCount: customer.downloadCount,
		};
	});

	async function handleDelete(id: number) {
        const confirmed = confirm(
            "Are you sure you want to delete this download link?",
        );
        if (!confirmed) return;

        const token = localStorage.getItem("token");

        const res = await fetch(
            `http://localhost:5105/api/customers/${id}`,
            {
                method: "DELETE",
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            },
        );

        if (res.ok) {
            customers = customers.filter((customer) => customer.id !== id);
        } else {
            alert("Failed to delete the download link.");
        }
    }
</script>

<Table {columns} {rows} title="👥 Clients" onDelete={handleDelete} />

<script lang="ts">
    import Table from "$lib/components/Table.svelte";

    export let downloadLinks: any[] = [];

    const columns = [
        "slug",
        "customerName",
        "customerEmail",
        "fileCount",
        "expiresAt",
        "link",
    ];

    const baseUrl = "http://localhost:5173/download";

    $: rows = downloadLinks.map((link) => {
        const formattedDate = new Date(link.expiresAt).toLocaleDateString();
        return {
            id: link.id,
            slug: link.slug,
            customerName: link.customerName,
            customerEmail: link.customerEmail,
            fileCount: link.fileCount,
            expiresAt: formattedDate,
            link: `${baseUrl}/${link.slug}`,
        };
    });

    async function handleDelete(id: number) {
        const confirmed = confirm(
            "Are you sure you want to delete this download link?",
        );
        if (!confirmed) return;

        const token = localStorage.getItem("token");

        const res = await fetch(
            `http://localhost:5105/api/files/download-link/${id}`,
            {
                method: "DELETE",
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            },
        );

        if (res.ok) {
            downloadLinks = downloadLinks.filter((link) => link.id !== id);
        } else {
            alert("Failed to delete the download link.");
        }
    }
</script>

<Table {columns} {rows} title="🔗 Download Links" onDelete={handleDelete} />

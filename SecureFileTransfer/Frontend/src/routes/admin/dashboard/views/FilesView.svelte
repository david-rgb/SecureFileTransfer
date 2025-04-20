<script lang="ts">
	import Table from '$lib/components/Table.svelte';

	export let files: any[] = [];

	const columns = ['OriginalFileName', 'CompressedFileName', 'UploadedAt'];

	$: rows = files.map(file => ({
		id: file.id,
		OriginalFileName: file.originalFileName,
		CompressedFileName: file.compressedFileName,
		UploadedAt: new Date(file.uploadedAt).toLocaleString()
	}));


	async function handleDelete(id: number) {
	const confirmed = confirm("Are you sure you want to delete this download link?");
	if (!confirmed) return;

	const token = localStorage.getItem("token");

	const res = await fetch(`http://localhost:5105/api/files/${id}`, {
		method: "DELETE",
		headers: {
			Authorization: `Bearer ${token}`
		}
	});

	if (res.ok) {
		files = files.filter(file => file.id !== id);
	} else {
		alert("Failed to delete the download link.");
	}
}

</script>

<Table {columns} {rows} title="📁 All Uploaded Files" onDelete={handleDelete}/>
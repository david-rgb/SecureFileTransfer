<script lang="ts">
	import { onMount } from "svelte";
	import { Fa } from "svelte-fa";
	import { faExpand, faCompress } from "@fortawesome/free-solid-svg-icons";

	export let title: string = "Table";
	export let columns: string[] = [];
	export let rows: Record<string, any>[] = [];
	export let onDelete: ((id: number) => void) | null = null;

	let isFullscreen = false;

	let filters: { [key: string]: string } = {};
	let sortColumn: string | null = null;
	let sortAsc = true;

	onMount(() => {
		for (const col of columns) {
			filters[col] = "";
		}
	});

	$: filteredRows = rows
		.filter((row) =>
			columns.every((col) =>
				(row[col] ?? "")
					.toString()
					.toLowerCase()
					.includes(filters[col].toLowerCase()),
			),
		)
		.sort((a, b) => {
			if (!sortColumn) return 0;
			const valA = a[sortColumn];
			const valB = b[sortColumn];

			if (valA == null) return 1;
			if (valB == null) return -1;

			return sortAsc
				? valA.toString().localeCompare(valB.toString())
				: valB.toString().localeCompare(valA.toString());
		});
</script>

<div
	class={`transition-all duration-100 ${
		isFullscreen
			? "fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 z-50 bg-gray-900 border border-gray-800 w-[90vw] h-[90vh] flex flex-col rounded-xl shadow-2xl"
			: "relative bg-gray-900 border border-gray-800 flex flex-col h-full rounded-xl shadow-xl"
	}`}
	tabindex="0"
	role="dialog"
	aria-modal={isFullscreen}
	on:keydown={(e) => e.key === "Escape" && (isFullscreen = false)}
>
	<div class="flex items-center justify-between px-4 pt-2 pb-2">
		<h2 class="text-lg font-semibold text-white">{title}</h2>
		<button
			class="text-gray-400 hover:text-white bg-gray-800 hover:bg-gray-700 rounded-full p-2 shadow-md transition"
			on:click={() => {
				isFullscreen = !isFullscreen;
				document.body.classList.toggle(
					"fullscreen-active",
					isFullscreen,
				);
			}}
			aria-label="Toggle Fullscreen"
		>
			<Fa icon={isFullscreen ? faCompress : faExpand} class="w-4 h-4" />
		</button>
	</div>

	<!-- Table Headers -->
	<div class="flex-1 overflow-auto hide-scrollbar min-h-0">
		<table
			class="min-w-full text-sm table-auto border border-gray-700 rounded"
		>
			<thead
				class="bg-gray-800 text-gray-400 uppercase text-left sticky top-0 z-10"
			>
				<tr>
					{#each columns as col}
						<th
							class="px-4 py-2 cursor-pointer"
							on:click={() => {
								if (sortColumn === col) sortAsc = !sortAsc;
								else {
									sortColumn = col;
									sortAsc = true;
								}
							}}
						>
							{col}
							{#if sortColumn === col}
								<span class="ml-1">{sortAsc ? "▲" : "▼"}</span>
							{/if}
						</th>
					{/each}
					{#if onDelete}
						<th class="px-4 py-2 text-right">Actions</th>
					{/if}
				</tr>
				<tr>
					{#each columns as col}
						<th class="px-4 py-1">
							<input
								type="text"
								placeholder="Filter..."
								class="w-full px-2 py-1 text-sm bg-gray-700 text-white rounded"
								bind:value={filters[col]}
							/>
						</th>
					{/each}
					{#if onDelete}
						<th></th>
					{/if}
				</tr>
			</thead>
		</table>

		<!-- Table Body -->
		<div
			class={`${!isFullscreen ? "max-h-[18rem]" : ""} overflow-y-auto hide-scrollbar`}
		>
			<table
				class="min-w-full text-sm table-auto border-t border-gray-700"
			>
				<tbody>
					{#if filteredRows.length === 0}
						<tr>
							<td
								class="px-4 py-2 text-center text-gray-400"
								colspan={columns.length + (onDelete ? 1 : 0)}
							>
								No matching records found.
							</td>
						</tr>
					{:else}
						{#each filteredRows as row}
							<tr
								class="border-t border-gray-800 hover:bg-gray-800 transition"
							>
								{#each columns as col}
									<td class="px-4 py-2">
										{#if typeof row[col] === "string" && row[col].startsWith("http")}
											<a
												href={row[col]}
												target="_blank"
												class="text-blue-400 underline"
												>{row[col]}</a
											>
										{:else if row[col] === 0}
											<span class="text-gray-400"
												>None</span
											>
										{:else}
											{row[col]}
										{/if}
									</td>
								{/each}
								{#if onDelete}
									<td class="px-4 py-2 text-right">
										<button
											on:click={() => onDelete?.(row.id)}
											class="text-red-500 hover:text-red-700 ml-2"
											title="Delete"
										>
											🗑️
										</button>
									</td>
								{/if}
							</tr>
						{/each}
					{/if}
				</tbody>
			</table>
		</div>
	</div>
</div>

<style>
	:global(body.fullscreen-active) {
		overflow: hidden;
	}
	.hide-scrollbar {
		scrollbar-width: none;
		-ms-overflow-style: none;
	}
	.hide-scrollbar::-webkit-scrollbar {
		display: none;
	}
</style>

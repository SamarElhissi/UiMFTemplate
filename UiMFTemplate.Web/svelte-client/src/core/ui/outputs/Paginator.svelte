<script>
	import { OutputFieldValue } from "../../framework";
	import TableOutput from "./Table.svelte";
	import { onMount } from "svelte";
	export let field;
	export let app;

	export let form;
	export let parent;
	let container;
	let pages = [];
	let pageSize;

	onMount(() => {
		const paginatorInput = form.inputs.find(
			(t) =>
				t != null &&
				t.metadata.id ===
					field.metadata.customProperties.customizations.paginator
		);

		field.pageSize = paginatorInput.value.pageSize;
		pageSize = field.pageSize;

		const tableField = new OutputFieldValue();
		tableField.data = field.data.results;
		tableField.metadata = field.metadata;

		// eslint-disable-next-line no-new
		new TableOutput({
			target: container,
			props: {
				field: tableField,
				app: app,
				form,
				parent: parent,
			},
		});
	});

	function goToPage(page) {
		form.setInputFields(page.params);
		parent.submit(null, false);
	}

	function changePageSize(pageSize) {
		const paginatorInput = form.inputs.find(
			(t) =>
				t != null &&
				t.metadata.id ===
					field.metadata.customProperties.customizations.paginator
		);

		paginatorInput.value.pageSize = pageSize;
		paginatorInput.value.pageIndex = 1;

		const params = {};
		for (const i of form.inputs) {
			if (i != null) params[i.metadata.id] = i.value;
		}
		form.setInputFields(params);
		parent.submit(null, false);
	}

	$: {
		pages = [];
		const paginatorInput = form.inputs.find(
			(t) =>
				t != null &&
				t.metadata.id ===
					field.metadata.customProperties.customizations.paginator
		);

		const pageCount = Math.ceil(
			field.data.totalCount / paginatorInput.value.pageSize
		);

		const params = {};
		for (const i of form.inputs) {
			if (i != null) params[i.metadata.id] = i.value;
		}

		if (pageCount > 0) {
			if (pageCount < paginatorInput.value.pageIndex) {
				paginatorInput.value.pageIndex = 1;
				form.setInputFields(params);
				parent.submit(null, false);
			}

			for (let p = 1; p <= pageCount; ++p) {
				const pageParams = Object.assign({}, params);
				pageParams[paginatorInput.metadata.id] = Object.assign(
					{},
					pageParams[paginatorInput.metadata.id]
				);
				pageParams[paginatorInput.metadata.id].pageIndex = p;

				pages.push({
					text: p,
					params: pageParams,
					cssClass: paginatorInput.value.pageIndex === p ? "active" : "",
				});
			}

			const firstParams = Object.assign({}, params);
			firstParams[paginatorInput.metadata.id] = Object.assign(
				{},
				firstParams[paginatorInput.metadata.id]
			);
			firstParams[paginatorInput.metadata.id].pageIndex = 1;

			const first = {
				text: "First",
				params: firstParams,
				cssClass: paginatorInput.value.pageIndex === 1 ? " disabled" : "",
			};
			const prevParams = Object.assign({}, params);
			prevParams[paginatorInput.metadata.id] = Object.assign(
				{},
				prevParams[paginatorInput.metadata.id]
			);
			prevParams[paginatorInput.metadata.id].pageIndex =
				paginatorInput.value.pageIndex - 1;

			const previous = {
				text: "Prev",
				params: prevParams,
				cssClass: paginatorInput.value.pageIndex === 1 ? " disabled" : "",
			};

			const nextParams = Object.assign({}, params);
			nextParams[paginatorInput.metadata.id] = Object.assign(
				{},
				nextParams[paginatorInput.metadata.id]
			);
			nextParams[paginatorInput.metadata.id].pageIndex =
				paginatorInput.value.pageIndex + 1;

			const next = {
				text: "Next",
				params: nextParams,
				cssClass:
					paginatorInput.value.pageIndex === pageCount ? " disabled" : "",
			};

			const lastParams = Object.assign({}, params);
			lastParams[paginatorInput.metadata.id] = Object.assign(
				{},
				lastParams[paginatorInput.metadata.id]
			);
			lastParams[paginatorInput.metadata.id].pageIndex = pageCount;

			const last = {
				text: "Last",
				params: lastParams,
				cssClass:
					paginatorInput.value.pageIndex === pageCount ? " disabled" : "",
			};

			let from = paginatorInput.value.pageIndex;
			let to = paginatorInput.value.pageIndex;

			if (from < 5) {
				from = 0;
				to = 10;
			} else if (from > pageCount - 5 && pageCount > 10) {
				to = pageCount;
				from = pageCount - 10;
			} else {
				from -= 5;
				to += 5;
			}

			pages = pages.slice(from, to);
			pages.unshift(previous);
			pages.unshift(first);
			pages.push(next);
			pages.push(last);
		}
	}
</script>

{#if field.data != null && pages.length > 0}
	<div>
		<!-- svelte-ignore a11y-no-onchange -->
		<select
			class="pagination-size input-sm"
			bind:value={pageSize}
			on:change={() => changePageSize(pageSize)}>
			<option value="10">10</option>
			<option value="20">20</option>
			<option value="50">50</option>
			<option value="100">100</option>
			<option value="100">200</option>
		</select>

		<ul class="pagination pagination-sm">
			{#each pages as page}
				{#if parent.useUrl}
					<li class="page-item pagination-page {page.cssClass}">
						<a
							href={app.makeUrl(form.metadata.id, page.params)}
							class="page-link">{page.text}</a>
					</li>
				{:else}
					<li class="page-item pagination-page {page.cssClass}">
						<button
							on:click={goToPage(page)}
							class="page-link">{page.text}</button>
					</li>
				{/if}
			{/each}
		</ul>
	</div>
	<div class="paginator-summary pagination-page-count">
		Display
		{field.data.results.length}
		from
		{field.data.totalCount}
		items
	</div>
{/if}

<div bind:this={container} />

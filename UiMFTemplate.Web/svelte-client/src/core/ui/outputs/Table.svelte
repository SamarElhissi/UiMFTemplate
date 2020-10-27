<script>
	import FormComponent from "../Form.svelte";
	import FormOutput from "../Output.svelte";
	import Tooltip from "../help/Tooltip.svelte";
	import { ActionListEventArguments } from "./ActionListEventArguments";
	import { onMount } from "svelte";
	import { _ } from "../../../services/Translation";
	export let field;
	export let app;
	export let form;
	export let parent;
	let isBulkActionModalOpen = true;
	let bulkActionContainer;
	let currentBulkActionForm = null;

	let table;
	let disabled = false;
	let visible = true;
	let bulkActions = [];
	let rowCssClass = null;
	let map = null;

	function buildFilter(currentFormInstance, parameters) {
		let promise;

		const filter = {};
		if (parameters != null && parameters.length > 0) {
			promise = currentFormInstance.getSerializedInputValues().then((data) => {
				for (const p of parameters) {
					filter[p] = data[p];
				}

				return filter;
			});
		} else {
			promise = Promise.resolve(filter);
		}

		return promise;
	}

	const __this = {
		closeBulkActionModal: (response) => closeBulkActionModal(response),
	};
	const modals = [];

	// https://stackoverflow.com/a/3369743/111438
	// Close topmost modal when user presses escape key.
	document.addEventListener("keydown", (e) => {
		const evt = e || window.event;
		let isEscape = false;
		if ("key" in evt) {
			isEscape = evt.key === "Escape" || evt.key === "Esc";
		} else {
			isEscape = evt.keyCode === 27;
		}
		if (isEscape) {
			if (modals.length > 0) {
				// Close topmost modal.
				modals[modals.length - 1].closeBulkActionModal();
			}
		}
	});
	onMount(() => {
		if (field.data == null) {
			return;
		}
		const data = field.data;
		const metadata = field.metadata;
		rowCssClass = (metadata.customProperties || {}).rowCssClass || {};
		bulkActions = (metadata.customProperties || {}).bulkAction || [];

		// Create map, with key being the lowercase version of the property name
		// and value being the actual property name.
		map = {};
		if (data.length > 0) {
			const firstRow = data[0];

			for (const property of Object.keys(firstRow)) {
				map[property.toLowerCase()] = property;
			}
		}
	});
	function getField(row, column) {
		const value = row[map[column.id.toLowerCase()]];
		return {
			data: value,
			metadata: column,
		};
	}
	function getRowCssClass(row) {
		let cssClass = "";

		if (rowCssClass != null) {
			cssClass = rowCssClass.cssClass || "";

			if (rowCssClass.suffix != null) {
				cssClass += row[map[rowCssClass.suffix.toLowerCase()]];
			}
		}

		return cssClass;
	}
	let selectedItemsCount = 0;

	let columnsOrdered;

	$: {
		columnsOrdered = field.metadata.customProperties.columns
			.filter((b) => !b.hidden)
			.sort((a, b) => a.orderIndex - b.orderIndex);
	}
	function enableBulkButton() {
		let disabled = false;
	}

	async function runBulkAction(action) {
		disabled = true;
		// eslint-disable-next-line no-underscore-dangle
		const selectedItems = field.data.filter((t) => t.__selected === true);
		const selectedItemIds = selectedItems.map(
			(t) => t[map[action.itemIdentifierField.toLowerCase()]]
		);

		const formInstance = app.getFormInstance(action.formId, true);

		const filter = await buildFilter(form, action.parameters);
		filter.Items = { items: selectedItemIds };
		formInstance.setInputFields(filter);

		const isAllInputsHidden =
			formInstance.inputs.filter((t) => t.metadata.hidden === false).length > 0;

		if (!isAllInputsHidden) {
			try {
				const response = await formInstance.submit(app, false);
				onActionRun(formInstance.metadata.id, response, action);
				enableBulkButton();
			} catch (e) {
				enableBulkButton();
			}
		} else {
			isBulkActionModalOpen = true;

			const f = new FormComponent({
				target: bulkActionContainer,
				data: {
					metadata: formInstance.metadata,
					form: formInstance,
					app,
					useUrl: false,
				},
			});

			f.init();

			f.$on("form:responseHandled", (e) => {
				closeBulkActionModal(e.detail.response);
			});
			currentBulkActionForm = f;

			modals.push(__this);
		}
	}
	async function onActionRun(formId, response) {
		const parentForm = parent;
		const app = parentForm.app;
		if (
			response.metadata.handler !== "redirect" &&
			response.metadata.handler !== "reload"
		) {
			// If asked to redirect to another form, then we redirect
			// and do not reload parent form, as that would be a wasted effort.
			await parentForm.submit(null, true);
		}

		const eventArgs = new ActionListEventArguments(app, formId);
		parentForm.fireAndBubbleUp("action-list:run", eventArgs);
	}
	async function closeBulkActionModal(response) {
		enableBulkButton();
		isBulkActionModalOpen = false;
		currentBulkActionForm = null;

		currentBulkActionForm.$destroy();

		const parentFormComponent = parent;

		if (
			response != null &&
			response.metadata.handler !== "redirect" &&
			response.metadata.handler !== "reload"
		) {
			// If asked to redirect to another form, then we redirect
			// and do not reload parent form, as that would be a wasted effort.
			await parentFormComponent.submit(null, true);
		}

		modals.pop();
	}
	function isDisabled(row) {
		return (
			row.actions == null ||
			row.actions.actions == null ||
			row.actions.actions.filter((t) =>
				bulkActions.some((r) => r.formId === t.form)
			).length === 0
		);
	}

	function selectItem(checkboxElement, row) {
		// eslint-disable-next-line no-underscore-dangle, no-param-reassign
		row.__selected = checkboxElement.checked;

		// eslint-disable-next-line no-underscore-dangle
		const selectedItems = field.data.filter((t) => t.__selected === true);
		selectedItemsCount = selectedItems.length;
	}

	function selectAllItems(checkboxElement) {
		for (const row of field.data) {
			if (!isDisabled(row)) {
				// eslint-disable-next-line no-underscore-dangle, no-param-reassign
				row.__selected = checkboxElement.checked;
			}
		}

		const checkboxes = table.querySelectorAll("tbody>tr>td .checkbox");

		for (const checkbox of checkboxes) {
			if (!checkbox.disabled) {
				checkbox.checked = checkboxElement.checked;
			}
		}

		// eslint-disable-next-line no-underscore-dangle
		const selectedItems = field.data.filter((t) => t.__selected === true);
		selectedItemsCount = selectedItems.length;
	}
	function sortData(column, columns) {
		const paginatorInput = form.inputs.find(
			(t) =>
				t != null &&
				t.metadata.id ===
					field.metadata.customProperties.customizations.paginator
		);

		if (paginatorInput != null) {
			paginatorInput.value.orderBy = column.customProperties.sortableBy;
			for (const i of columns) {
				i.ascending = false;
			}

			paginatorInput.value.ascending = !paginatorInput.value.ascending;
			// eslint-disable-next-line no-param-reassign
			column.ascending = paginatorInput.value.ascending;

			const params = {};
			for (const i of form.inputs) {
				if (i != null) params[i.metadata.id] = i.value;
			}
			form.setInputFields(params);
			parent.submit(null, false);
		}
	}
</script>

<style>
	.btn-row {
		text-align: left;
	}

	.checkbox {
		clip: unset;
		clip-path: unset;
		position: unset;
		width: 15px;
		height: 15px;
	}

	.sortable-column {
		cursor: pointer;
	}

	.horizontal-scroll {
		overflow: hidden;
		overflow-x: auto;
		clear: both;
		width: 100%;
	}

	.table {
		min-width: rem-calc(640);
	}
</style>

{#if visible && field.data != null && field.data.length > 0 && map != null}
	<div class="horizontal-scroll">
		<table
			class="table table-condensed table-striped table-bordered table-hover jr-table"
			bind:this={table}>
			<thead>
				{#if bulkActions.length > 0}
					<tr>
						<td colspan={columnsOrdered.length + 1} class="btn-row">
							{#each bulkActions as action}
								{#if selectedItemsCount > 0}
									<button
										{disabled}
										style="direction: rtl;"
										class="btn btn-default"
										on:click={runBulkAction(action)}>
										{action.label}
										<small>({selectedItemsCount})</small>
									</button>
								{:else}
									<button
										class="btn btn-default"
										disabled>{action.label}</button>
								{/if}
							{/each}
						</td>
					</tr>
				{/if}
				<tr>
					{#if bulkActions.length > 0}
						<th class="jr-table-th">
							<input
								type="checkbox"
								class="checkbox"
								on:change={selectAllItems(this)} />
						</th>
					{/if}
					{#each columnsOrdered as column}
						{#if column.customProperties != null && column.customProperties['sortableBy'] != null}
							{#if column.ascending}
								<th
									class="sortable-column jr-table-th"
									on:click={sortData(column, columnsOrdered)}>
									{#if column.label != null && column.label != ""}
									{$_(`${form.metadata.id}.outputs.${column.id}`, {
										default: column.label,
									})}
									{/if}
									<i class="fa fa-sort-down" />
								</th>
							{:else}
								<th
									class="sortable-column jr-table-th"
									on:click={sortData(column, columnsOrdered)}>
									{#if column.label != null && column.label != ""}
									{$_(`${form.metadata.id}.outputs.${column.id}`, {
										default: column.label,
									})}
									{/if}
									<i class="fa fa-sort-up" />
								</th>
							{/if}
						{:else}
							<th class="jr-table-th">
								{#if column.customProperties != null && column.customProperties['documentation'] != null}
									<div class="help-tooltip">
										{#if column.label != null && column.label != ""}
										{$_(`${form.metadata.id}.outputs.${column.id}`, {
											default: column.label,
										})}
										{/if}
										<Tooltip data={column.customProperties.documentation[0]} />
									</div>
								{:else}
								{#if column.label != null && column.label != ""}
									{$_(`${form.metadata.id}.outputs.${column.id}`, {
										default: column.label,
									})}
									{/if}
								{/if}
							</th>
						{/if}
					{/each}
				</tr>
			</thead>
			<tbody>
				{#if map != null}
					{#each field.data as row}
						<tr class={getRowCssClass(row)}>
							{#if bulkActions.length > 0}
								<td>
									<div class="form-group form-check">
										<input
											disabled={row.actions == null || row.actions.actions == null || row.actions.actions.filter(
													(t) => bulkActions.some((r) => r.formId === t.form)
												).length === 0}
											type="checkbox"
											class="checkbox"
											on:change={selectItem(this, row)} />
									</div>
								</td>
							{/if}
							{#each columnsOrdered as column}
								<td>
									{#if !(getField(row, column).metadata.getCustomProperty('hideIfNull') === true && getField(row, column).data === null)}
										<FormOutput
											field={getField(row, column)}
											{app}
											{form}
											{parent}
											showLabel="false" />
									{/if}
								</td>
							{/each}
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	</div>

	{#if bulkActions.length > 0}
		<input
			type="checkbox"
			bind:checked={isBulkActionModalOpen}
			class="hidden" />
		<div class="modal">
			<div class="card">
				<span class="close modal-close" on:click={closeBulkActionModal(null)} />
				<div bind:this={bulkActionContainer} />
			</div>
		</div>
	{/if}
{:else}
	<div class="alert-nodata">No data found.</div>
{/if}

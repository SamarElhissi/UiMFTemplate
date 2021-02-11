<script>
	import * as umf from "../../framework";
	import * as uimfcore from "uimf-core";
	import FormOutput from "../Output.svelte";
	export let field;
	export let app;

	export let form;
	export let parent;

	let items = [];

	const columns = field.data.metadata;
	delete columns.customProperties;

	const formMetadata = new uimfcore.FormMetadata({
		customProperties: field.data.metadata.customProperties,
		outputFields: columns,
		inputFields: [],
	});
	let cssClass = (field.metadata.getCustomProperty("objectListConfig") || {}).itemsStyle || "";

	for (const item of field.data.items) {
		items.push(
			umf.FormInstance.getOutputFieldValues(formMetadata.outputFields, item)
		);
	}
</script>

<style>
	.object-list-item {
		margin: 10px 0;
		border-bottom: 1px solid #eee;
		padding: 10px 0;
	}
</style>

{#if field.data != null && items != null}
	{#each items as itemFields}
		<div class="object-list-item {cssClass}">
			{#each itemFields as itemField}
				{#if itemField.metadata.hidden == false && !(itemField.metadata.getCustomProperty('hideIfNull') === true && itemField.data === null)}
					<FormOutput field={itemField} {app} {form} {parent} />
				{/if}
			{/each}
		</div>
	{/each}
{/if}

<script context="module">
	let inputId = 1;
</script>

<script>
	import Tooltip from "./help/Tooltip.svelte";
	import { onMount } from "svelte";
	import { _ } from "../../services/Translation";

	export let field;
	export let app;
	export let form;
	export let formComponent;
	let alwaysHideLabel;
	export let tabindex;

	let id = `i${inputId++}`;
	let visible = false;
	let cssClass;
	let container;
	onMount(() => {
		const input = app.controlRegister.getInput(field.metadata.type);

		// Set correct css class based on the field type.
		const inputDisplayConfig = field.constants || input.constants || {};

		cssClass =
			field.metadata.customProperties != null
				? field.metadata.customProperties.cssClass
				: null;
		// Set correct css class based on the field type.
		if (inputDisplayConfig.block) {
			cssClass += " block";
		} else {
			cssClass += " inline";
		}
		alwaysHideLabel = inputDisplayConfig.alwaysHideLabel;

		const inputs = form.inputs;
		// If `inputs` is null, then it means our parent form has been closed
		// and "destroyed". In such cases we should just return.
		// TODO: find a better way to implement "parent form null check".
		if (inputs != null) {
			// Register input in the parent form.
			inputs.push(this);
		}
		const isVisible =
			!field.metadata.eventHandlers.length ||
			field.metadata.eventHandlers.find((t) => t.id === "depend-on") == null;

		visible = !!isVisible;

		container = app.controlRegister.getInput(field.metadata.type).component;
	});

	// if (!visible) {
	// 	field.value = null;
	// }
</script>

<style>
	.inline {
		display: inline-block;
	}

	.col-form-label {
		padding-left: 0;
	}
</style>

{#if visible}
	{#if !alwaysHideLabel && field.metadata.label !== ''}
		<div class="form-group {cssClass}">
			<label for={id} class="col-form-label">
				{#if field.metadata.customProperties != null && field.metadata.customProperties['documentation'] != null}
					<div class="help-tooltip">
						{$_(`${form.metadata.id}.inputs.${field.metadata.id}`, {
							default: field.metadata.label,
						})}:

						<Tooltip data={field.metadata.customProperties.documentation[0]} />
					</div>
				{:else}
					{$_(`${form.metadata.id}.inputs.${field.metadata.id}`, {
						default: field.metadata.label,
					})}
				{/if}
			</label>
			<div class="input-container">
			<svelte:component
				this={container}
				{field}
				{app}
				{tabindex}
				{id}
				{form}
				{visible}
				{formComponent} />
			</div>
		</div>
	{:else}
		<div class="form-group {cssClass}">
			<div class="input-container">
			<svelte:component
				this={container}
				{field}
				{app}
				{tabindex}
				{id}
				{form}
				{visible}
				{formComponent} />
			</div>
		</div>
	{/if}
{/if}

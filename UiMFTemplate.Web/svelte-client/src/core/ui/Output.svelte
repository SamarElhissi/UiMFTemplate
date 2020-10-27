<script>
	import Tooltip from "./help/Tooltip.svelte";
	import { onMount } from "svelte";
	import { _ } from "../../services/Translation";

	export let showLabel = true;
	export let field;
	export let app;
	export let form;
	export let parent;
	let container = null;
	let alwaysHideLabel = null;
	let cssClass = null;
	onMount(() => {
		const output = app.controlRegister.getOutput(field);
		const outputDisplayConfig = output.constants || {};
		alwaysHideLabel = outputDisplayConfig.alwaysHideLabel;

		cssClass =
			field.metadata.customProperties != null
				? field.metadata.customProperties.cssClass
				: null;
		if (outputDisplayConfig.block) {
			cssClass += " block";
		} else {
			cssClass += " inline";
		}

		container = output.constructor;
	});
</script>

{#if showLabel === true && !alwaysHideLabel && field.metadata.label !== ''}
	<div>
		{#if field.metadata.customProperties != null && field.metadata.customProperties['documentation'] != null}
			<span
				class="help-tooltip output-label">{$_(
					`${form.metadata.id}.outputs.${field.metadata.id}`,
					{
						default: field.metadata.label,
					}
				)}:

				<Tooltip data={field.metadata.customProperties.documentation[0]} />
			</span>
		{:else}
			<!-- svelte-ignore a11y-label-has-associated-control -->
			<label
				class="output-label">{$_(
					`${form.metadata.id}.outputs.${field.metadata.id}`,
					{
						default: field.metadata.label,
					}
				)}:</label>
		{/if}
		<div class="output-container {cssClass}" >
		<svelte:component
			this={container}
			{field}
			{app}
			{form}
			{parent} />
		</div>
	</div>
{:else}
<div class="output-container {cssClass}" >
	<svelte:component
		this={container}
		{field}
		{app}
		{form}
		{parent}/>
	</div>
{/if}

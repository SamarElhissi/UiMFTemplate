<script>
	import { onMount } from "svelte";
	export let id;
	export let field;
	export let tabindex;
	let config;
	let numberConfig;
	export let app;
	export let form;
	export let visible;
	export let formComponent;

	onMount(() => {
		config = field.metadata.customProperties || {
			disabled: false,
		};
		numberConfig = field.metadata.getCustomProperty("numberConfig") || {
			minValue: null,
			maxValue: null,
			step: 1,
		};
	});
</script>

{#if numberConfig != null}
	<input
		type="number"
		{id}
		bind:value={field.value}
		required={field.metadata.required}
		{tabindex}
		class="form-control"
		disabled={config.disabled}
		step={numberConfig.step}
		min={numberConfig.minValue}
		max={numberConfig.maxValue} />
{/if}

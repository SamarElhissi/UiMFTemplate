<script>
	import { onMount } from "svelte";
	export let field;
	export let tabindex;
	export let numberConfig;

	onMount(() => {
		numberConfig = (field.metadata.customProperties || {}).numberConfig || {
			minValue: null,
			maxValue: null,
			step: 1,
		};
	});
</script>

{#if numberConfig != null}
	<div class="input-group">
		<span class="input-group-addon">Min</span>
		<input
			type="number"
			bind:value={field.value.min}
			required={field.metadata.required}
			{tabindex}
			autocomplete="off"
			class="form-control"
			step={numberConfig.step}
			min={numberConfig.minValue}
			max={numberConfig.maxValue} />

		<span class="input-group-addon">Max</span>
		<input
			type="number"
			bind:value={field.value.max}
			required={field.metadata.required}
			{tabindex}
			autocomplete="off"
			class="form-control"
			step={numberConfig.step}
			min={numberConfig.minValue}
			max={numberConfig.maxValue} />
	</div>
{/if}

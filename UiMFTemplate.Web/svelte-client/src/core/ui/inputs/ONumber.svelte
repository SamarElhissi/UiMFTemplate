<script>
	import { onMount } from "svelte";
	export let field;
	export let tabindex;
	export let id;
	export let numberConfig;

	onMount(() => {
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
		bind:value={field.selected}
		required={field.metadata.required}
		{tabindex}
		class="form-control"
		step={numberConfig.step}
		min={numberConfig.minValue}
		max={numberConfig.maxValue} />
{/if}

<script>
	export let id;
	export let field;
	export let container;
	export let tabindex;
	import { onMount } from "svelte";

	onMount(() => {
		container.addEventListener("change", () => {
			field.init(field.selected);
		});
	});
</script>

<input
	type="Email"
	{id}
	bind:value={field.selected}
	required={field.metadata.required}
	{tabindex}
	class="form-control"
	bind:this={container} />

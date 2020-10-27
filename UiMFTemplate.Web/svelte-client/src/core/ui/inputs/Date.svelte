<script>
	import Pikaday from "pikaday";
	import { onMount } from "svelte";

	export let id;
	export let field;
	export let container;
	export let tabindex;

	onMount(() => {
		new Pikaday({
			field: container,
			format: "YYYY-MM-DD",
			onSelect: async function (date) {
				await field.init(date.toISOString());
			},
		});

		var formElement = container;

		formElement.addEventListener("change", function (e) {
			if (formElement.value == "") {
				field.init(null);
			}
		});
	});
</script>

<input
	type="text"
	{id}
	bind:value={field.valueAsText}
	required={field.metadata.required}
	{tabindex}
	autocomplete="off"
	class="form-control"
	bind:this={container} />

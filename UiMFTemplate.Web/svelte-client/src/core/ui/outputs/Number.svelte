<script>
	export let field;
	export let parent;
	export let app;
	export let form;
	let formatted;
	$: {
		if (field.data == null) {
			formatted = "";
		}
		const x = field.data;
		const parts = x.toString().split(".");
		parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");

		formatted = parts.join(".");
	}
</script>

{#if field.data != null}{formatted}{/if}

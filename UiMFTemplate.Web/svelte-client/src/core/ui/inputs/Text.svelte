<script>
	export let field;
	export let tabindex;
	export let id;
	export let app;
	export let form;
	export let visible;
	export let formComponent;
	let config = field.metadata.customProperties || { disabled: false };
</script>

<input
	type="text"
	{id}
	bind:value={field.value}
	required={field.metadata.required}
	disabled={config.disabled}
	{tabindex}
	class="form-control" />

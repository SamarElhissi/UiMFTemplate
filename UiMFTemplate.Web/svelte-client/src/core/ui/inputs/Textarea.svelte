<script>
	export let field;
	export let tabindex;
	export let id;
</script>

<style>
	textarea {
		width: 100%;
		height: 100px;
	}
</style>

<textarea
	{id}
	bind:value={field.selected}
	required={field.metadata.required}
	{tabindex}
	class="form-control" />

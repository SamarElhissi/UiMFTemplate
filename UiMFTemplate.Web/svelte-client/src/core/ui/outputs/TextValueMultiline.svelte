<script>
	export let field;
	export let parent;
	export let app;
	export let form;
</script>

<style>
	div {
		white-space: pre;
		margin: 0 0 5px 0;
		padding: 5px 15px;
	}
</style>

{#if (field.data || {}).value != null}
	<div>{field.data.value}</div>
{/if}

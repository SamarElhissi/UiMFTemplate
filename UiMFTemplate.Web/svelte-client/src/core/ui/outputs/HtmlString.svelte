<script>
	export let field;
	export let parent;
	export let app;
	export let form;
</script>

{#if field.data != null && field.data.value != null}
	{@html field.data.value}
{/if}

<script>
	export let field;
	export let app;
	export let parent;
	export let form;
</script>

{#if field.data != null}
	{#if field.data.form != null}
		{#if field.data.isNewUiMF == false}
		<a href="{app.makeOldFormsUrl(field.data.form, field.data.inputFieldValues)}">{@html field.data.label}</a>
		{:else}
		<a href="{app.makeUrl(field.data.form, field.data.inputFieldValues)}">{@html field.data.label}</a>
		{/if}
		{:else}
	<span>{field.data.label}</span>
	{/if}
{/if}

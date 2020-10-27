<script>
	export let field;
	export let parent;
	export let app;
	export let form;
</script>

{#if field.data != null}
	<a
		href={field.data.url}
		target={field.data.target}
		class={field.data.cssClass}>{@html field.data.anchor}</a>
{/if}

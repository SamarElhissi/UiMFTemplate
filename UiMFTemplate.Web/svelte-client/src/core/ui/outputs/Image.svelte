<script>
	export let field;
</script>

{#if field.data != null}<img src={field.data.url} width="100" alt="" />{/if}

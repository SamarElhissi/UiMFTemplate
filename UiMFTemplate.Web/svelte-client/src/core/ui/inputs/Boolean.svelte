<script>
	export let form;
	export let app;
	export let field;
	export let id;
	export let tabindex;
	export let formComponent;

	const self = {
		form,
		app,
	};
	function onChange() {
		formComponent.fireAndBubbleUp("input:changed", {
			app: app,
			form: formComponent,
			input: self,
		});
	}
</script>

{#if field.metadata.required}
	<input
		type="checkbox"
		{id}
		class="inlinecheckbox checkbox form-control"
		bind:checked={field.value}
		on:change={() => onChange()}
		{tabindex} />
{:else}
	<select
		bind:value={field.value}
		on:blur={() => onChange()}
		{id}
		class="form-control">
		<option />
		<option value={true}>Yes</option>
		<option value={false}>No</option>
	</select>
{/if}

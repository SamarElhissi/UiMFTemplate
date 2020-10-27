<script>
	import { onMount } from "svelte";
	export let id;
	export let field;
	export let tabindex;
	export let app;
	export let form;
	export let formComponent;
	let options = null;
	export let visible;

	onMount(() => {
		let source = field.metadata.customProperties.source;
		let items = field.metadata.customProperties.items;

		function mapToTypeaheadItems(items) {
			return items.map((t) => ({
				label: t.label,
				value: t.value.toString(),
			}));
		}

		function buildFilter(parentForm, parameters) {
			let promise;

			const filter = {};
			if (parameters != null && parameters.length > 0) {
				promise = parentForm
					.get()
					.form.getSerializedInputValues()
					.then((data) => {
						for (const p of parameters) {
							filter[p] = data[p];
						}

						return filter;
					});
			} else {
				promise = Promise.resolve(filter);
			}

			return promise;
		}

		if (items != null) {
			options = items;
			if (field.selected != null) {
				onChange();
			}
		} else if (typeof source === "string") {
			let parameters = field.metadata.customProperties;

			buildFilter(form, parameters).then((filter) => {
				app.server.postForm(source, filter).then((data) => {
					options = mapToTypeaheadItems(data.items);
					if (field.selected != null) {
						onChange();
					}
				});
			});
		}
	});

	function onChange() {
		field.initFromSelected();
		formComponent.fireAndBubbleUp("input:changed", {
			app: app,
			form: formComponent,
			input: this,
		});
	}
</script>

{#if options != null}
	<select
		{id}
		bind:value={field.selected}
		required={field.metadata.required}
		{tabindex}
		on:blur={() => onChange()}
		class="form-control">
		<option value="" />
		{#each options as option}
			<option value={option.value}>
				{@html option.label}
			</option>
		{/each}
	</select>
{/if}

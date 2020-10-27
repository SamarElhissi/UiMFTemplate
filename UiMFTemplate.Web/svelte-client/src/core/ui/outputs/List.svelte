<script>
	export let field;
	export let form;
	export let parent;
	export let app;
	let cssClass = "";

	$: {
		const dynamicCssClass = field.metadata.getCustomProperty("dynamicCssClass");
		if (dynamicCssClass != null) {
			cssClass = dynamicCssClass.cssClassPrefix;
			if (dynamicCssClass.outputFieldAsSuffix != null) {
				const suffixOutputField = form.outputs.find(
					(t) => t.metadata.id === dynamicCssClass.outputFieldAsSuffix
				);

				cssClass += suffixOutputField.data;
			}
		}
	}
</script>

{#if field.data != null}
	<div style="display: -webkit-inline-box;">
		<ul class={cssClass}>
			{#each field.data as item}
				<li>{item}</li>
			{/each}
		</ul>
	</div>
{/if}

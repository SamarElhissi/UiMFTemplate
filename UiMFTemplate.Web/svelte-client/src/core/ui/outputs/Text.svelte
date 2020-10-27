<script>
	export let field;
	export let form;
	export let parent;
	export let app;
	let cssClass = "";
	let BooleanChecker = "";

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
	$: {
		if (typeof field.data === "boolean") {
			BooleanChecker = field.data ? "Yes" : "No";
		}

		BooleanChecker = field.data;
	}
</script>

{#if field.data != null}<span class={cssClass}>{BooleanChecker}</span>{/if}

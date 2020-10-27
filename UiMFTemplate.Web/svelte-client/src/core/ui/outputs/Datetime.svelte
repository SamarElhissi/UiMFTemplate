<script>
	import { momentLocale } from "../../../services/Translation";
	export let field;
	export let app;
	export let parent;
	export let form;

	$: relativeFormat = (field) => {
		if (
			field.data != null &&
			field.metadata.getCustomProperty("dateTimeStyle") === null
		) {
			return $momentLocale.utc(field.data).local().fromNow();
		}
		return null;
	}
	$: format = (field) => {
		if (field.data != null) {
			const dateTimeStyle = field.metadata.getCustomProperty("dateTimeStyle");
			let format = "MMMM Do YYYY, h:mm:ss a";
			if (dateTimeStyle === "dateTime") {
				format = "D-MM-YYYY hh:mm A";
			} else if (dateTimeStyle === "time") {
				format = "HH:mm";
			} else if (dateTimeStyle === "relativeTime") {
				return $momentLocale.utc(field.data).local().fromNow();
			} else if (dateTimeStyle === "date") {
				format = "D-M-YYYY";
				return $momentLocale.utc(field.data).local().format(format);
			}
			return $momentLocale.utc(field.data).local().format(format);
		}
		return "";
	}
</script>

<div style="text-align: center">
	{#if relativeFormat(field) != null}
		<div>{relativeFormat(field)}</div>
	{/if}

	{#if relativeFormat(field) != null}
		<div><small>{format(field)}</small></div>
	{:else}
		<div>{format(field)}</div>
	{/if}
</div>

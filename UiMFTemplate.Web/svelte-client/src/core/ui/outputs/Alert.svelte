<script>
	import { momentLocale } from "../../../services/Translation";
	export let field;
	export let parent;
	export let app;
	export let form;

	$: format = (date) => {
		return $momentLocale.utc(date).local().format("MMMM Do YYYY, h:mm:ss a");
	}
	$: mainData = (date) => {
		return $momentLocale.utc(date).local().fromNow();
	}
</script>

{#if field != null && field.data != null}
	<div class="alert {field.data.style} col-md-12" style="position: relative;">
		{#if field.data.heading != null || field.data.icon != null}
			<div class="heading">
				{#if field.data.icon != null}
					<span>{@html field.data.icon}</span>
				{/if}
				<strong style="font-size:16px">{field.data.heading}</strong>
			</div>
		{/if}
		{#if field.data.message != null}
			<div class="body" style="font-size:16px" >
				{@html field.data.message}
			</div>
		{/if}

		{#if field.data.actionDate != null}
			<div class="dateview">
				<div>{mainData(field.data.actionDate)}</div>
				<div>
					<strong><small>{format(field.data.actionDate)}</small></strong>
				</div>
			</div>
		{/if}
	</div>
{/if}

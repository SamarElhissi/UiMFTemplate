<script>
	export let field;
	export let app;
	export let parent;
	export let form;
	function getCssClass(tab, tabstrip) {
		return tab.form === tabstrip.currentTab ? "active" : "";
	}
</script>

<div class="tabstrip">
	{#each field.data.tabs as tab}
		<div>
			<a
				href={app.makeUrl(tab.form, tab.inputFieldValues)}
				class={getCssClass(tab, field.data)}>{tab.label}</a>
		</div>
	{/each}
</div>

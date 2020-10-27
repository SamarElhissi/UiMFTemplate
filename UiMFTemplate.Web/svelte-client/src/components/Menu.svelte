<script>
	export let app;
	export let menu;
	import MenuItem from "./MenuItem.svelte";
</script>

{#if menu != null}
<ul class="menu">
	{#each menu.children as item}
	<li>
		<MenuItem item={item} menu={menu} app={app} />
	</li>
	{/each}
</ul>
{/if}

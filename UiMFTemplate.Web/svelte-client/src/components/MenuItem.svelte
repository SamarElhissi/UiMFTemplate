<script>
	import { onMount } from "svelte";
	import FormLink from "../core/ui/outputs/FormLink.svelte";
	let currentInstanceId = 0;
	let nested = {data: null};
	let field;
	export let app;
	export let item;
	export let menu;
	export let id;

	$:{
		if (item != null && item.children.length > 0) {
            nested = {
                    data: item.children[0]
                };
		}
		field = {
			data: item
		}
	}
	onMount(() => {
		currentInstanceId += 1;
	});

</script>


{#if item.children.length === 0}
    {#if item.form != null}
    <FormLink field={field} app={app}/>
    {:else}
    <span>{item.label}</span>
    {/if}
{:else}
    <div class="menu top-menu-div">
        {#if item.children.length > 1}
        <label for="menu{id}" class="toggle-sub" onclick="">{@html item.label}</label>
        {:else}
        <FormLink field={nested} app={app}/>
        {/if}
    </div>

    <input type="checkbox" id="menu{id}" class="sub-nav-check" />
    {#if item.children.length > 1}
    <ul class="sub-nav">
        {#each item.children as child}
        <li>
            <svelte:self item={child} menu={menu} app={app} />
        </li>
        {/each}
    </ul>
    {/if}
{/if}
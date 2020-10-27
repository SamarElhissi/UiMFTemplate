<script>
	import { onMount } from "svelte";
	export let field;
	export let tabindex;
	export let id;
	export let passwordConfig;

	onMount(() => {
		let config = (field.metadata.customProperties || {}).passwordInputConfig;

		passwordConfig = config || {
			regex: null,
			requireConfirmation: null,
		};
	});

	function onChange(confirmPassword) {
		if (field.selected !== confirmPassword.value) {
			confirmPassword.setCustomValidity(
				"Passwords do not match. Please make sure they are exactly the same."
			);
		} else {
			confirmPassword.setCustomValidity("");
		}
	}
</script>

<style>
	span {
		color: #9a9a9a;
		font-size: 13px;
		margin-bottom: 5px;
		display: block;
	}

	.confirmation-password {
		padding-top: 10px;
	}
</style>

{#if passwordConfig != null}
	{#if passwordConfig.regex}
		<span>{passwordConfig.regexDescription}</span>
		<input
			type="password"
			{id}
			bind:value={field.selected}
			pattern={passwordConfig.regex}
			required={field.metadata.required}
			{tabindex}
			class="form-control" />
	{:else}
		<input
			type="password"
			{id}
			bind:value={field.selected}
			required={field.metadata.required}
			{tabindex}
			class="form-control" />
	{/if}
	{#if passwordConfig.requireConfirmation}
		<div class="confirmation-password">
			<input
				placeholder="Confirm password"
				type="password"
				on:change={() => onChange(this)}
				required={field.metadata.required}
				tabindex={tabindex + 1}
				class="form-control" />
		</div>
	{/if}
{/if}

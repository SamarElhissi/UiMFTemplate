<script>
	import Form from "../Form.svelte";
	import { onDestroy, onMount } from "svelte";

	export let field;
	export let app;
	export let parent;
	export let current;
	export let container;
	export let form;

	onMount(() => {
		if(field.data == null || field.data.form == null){
			return null;
		}
		const formInstance = app.getFormInstance(field.data.form, true);

		formInstance.initializeInputFields(field.data.inputFieldValues).then(() => {
			const f = new Form({
				target: container,
				props: {
					metadata: formInstance.metadata,
					form: formInstance,
					app,
					useUrl: false,
					parent: parent,
				},
			});

			f.init();
			current = f;
		});
	});
	//this.get().parent.on("destroy", () => this.destroy());

	onDestroy(() => {
		if (current != null) {
			current.$destroy();
		}
	});
</script>

<style>
	.inline-form {
		border-width: 1px 1px 1px;
		border-style: solid;
		border-color: #899294;
		margin: 30px 0;
		border-radius: 5px;
	}

	.inline-form .form-header {
		border-bottom: none;
		padding-top: 0;
		padding-bottom: 0;
		background: #eee;
	}

	.inline-form .response {
		margin-top: 0;
		padding: 10px 15px;
	}

	.inline-form h2 {
		margin: 0;
		font-size: 15px;
		padding: 10px 15px 15px;
	}

	.inline-form .response .form-header {
		padding-top: 10px;
		border-bottom: 1px solid rgb(231, 154, 147);
		background-color: #fff;
	}

	.inline-form .response h2 {
		font-size: 2rem;
	}
</style>

<div bind:this={container} class="inline-form" />

<script context="module">
	let globalModalId = 1;
</script>

<script>
	import Form from "../Form.svelte";
	import * as alertifyLib from "alertifyjs";
	import { ActionListEventArguments } from "./ActionListEventArguments";
	import { _ } from "../../../services/Translation";

	export let field;
	export let open = false;
	let current = null;
	let currentId = null;
	export let app;
	let modalId = globalModalId++;
	let container;
	let modals = [];
	let disabled = false;
	let modalCssClass = "";
	export let parent;
	export let form;

	// eslint-disable-next-line no-debugger
	const alertify = alertifyLib.default;

	// https://stackoverflow.com/a/3369743/111438
	// Close topmost modal when user presses escape key.
	document.addEventListener("keydown", (e) => {
		const evt = e || window.event;
		let isEscape = false;
		if ("key" in evt) {
			isEscape = evt.key === "Escape" || evt.key === "Esc";
		} else {
			isEscape = evt.keyCode === 27;
		}
		if (isEscape) {
			if (modals.length > 0) {
				// Close topmost modal.
				modals[modals.length - 1].close();
			}
		}
	});
	const __this = {
		close: () => close(),
	};
	function enableActionButton() {
		disabled = false;
	}

	async function run(action) {
		let formInstance = app.getFormInstance(action.form, true);

		// TODO: find a way to initialize from action.inputFieldValues directly.
		let serializedInputValues = formInstance.getSerializedInputValuesFromObject(
			action.inputFieldValues
		);
		await formInstance.initializeInputFields(serializedInputValues);

		if (action.action === "run") {
			const allRequiredInputsHaveData = await formInstance.allRequiredInputsHaveData(
				false
			);

			if (allRequiredInputsHaveData) {
				disabled = true;
				if (action.confirmationMessage) {
					alertify.confirm(
						"Caution",
						action.confirmationMessage,
						async () => {
							try {
								const response = await formInstance.submit(app, false);
								onActionRun(formInstance.metadata.id, response, action);
							} catch (e) {
								enableActionButton();
							}
						},
						() => {
							enableActionButton();
						}
					);
				} else {
					try {
						const response = await formInstance.submit(app, false);
						onActionRun(formInstance.metadata.id, response, action);
					} catch (e) {
						enableActionButton();
					}
				}
			} else {
				alertify.error("Please fill required inputs");
			}
		} else {
			open = true;
			modalCssClass = action.modalCssClass || "";

			let f = new Form({
				target: container,
				props: {
					metadata: formInstance.metadata,
					form: formInstance,
					app,
					useUrl: false,
				},
			});
			f.init();

			f.$on("form:responseHandled", (event) => {
				handleResponse(event);
			});

			current = f;
			currentId = formInstance.metadata.id;
			modals.push(__this);
		}
	}
	export function handleResponse(e) {
		if (e.detail.invokedByUser && e.detail.form.metadata.closeOnPostIfModal) {
			close(e.detail.response);
		}
	}
	function close(response) {
		open = false;
		enableActionButton();

		// Destroy underlying form instance.

		if (response != null) {
			const formId = currentId;
			onActionRun(formId, response);
		}

		current.$destroy();
		modals.pop();
	}

	async function onActionRun(formId, response) {
		const parentForm = parent;

		if (
			response.metadata.handler !== "redirect" &&
			response.metadata.handler !== "reload"
		) {
			// If asked to redirect to another form, then we redirect
			// and do not reload parent form, as that would be a wasted effort.

			await parentForm.submit(null, true);
		}

		const eventArgs = new ActionListEventArguments(app, formId);
		parentForm.fireAndBubbleUp("action-list:run", eventArgs);
	}
</script>

{#if field.data != null && field.data.actions != null && field.data.actions.length > 0}
	<ul class="actionlist">
		{#each field.data.actions as action}
			<li>
				{#if action.action !== 'redirect'}
					<button
						on:click={() => run(action, app)}
						{disabled}
						class="btn btn-default {action.cssClass}">
						{@html $_(`${form.metadata.id}.actions.${action.form}`, {
							default: action.label,
						})}</button>
				{:else if action.link !== null}
					<a
						href={action.link}
						class="btn btn-default {action.cssClass}">{@html $_(
							`${form.metadata.id}.actions.${action.form}`,
							{
								default: action.label,
							}
						)}</a>
				{:else}
					<a
						href={app.makeUrl(action.form, action.inputFieldValues)}
						class="btn btn-default {action.cssClass}">{@html $_(
							`${form.metadata.id}.actions.${action.form}`,
							{
								default: action.label,
							}
						)}</a>
				{/if}
			</li>
		{/each}
	</ul>

	<input
		id="modal-{modalId}"
		type="checkbox"
		bind:checked={open}
		class="hidden" />
	<div class="modal">
		<div class="card {modalCssClass}">
			<div
				class="fa fa-times text-danger icon-arrow-right close-btn close"
				on:click={() => close()} />
			<div bind:this={container} on:responseHandled={handleResponse} />
		</div>
	</div>
{/if}

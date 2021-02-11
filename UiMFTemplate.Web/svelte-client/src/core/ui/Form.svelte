<script context="module">
	let globalTabindex = 1;
</script>

<script>
	import FormInput from "./Input.svelte";
	import FormOutput from "./Output.svelte";
	import Help from "./help/Help.svelte";
	import { createEventDispatcher, onDestroy } from "svelte";
	import { _ } from "../../services/Translation";

	export let form;
	export let app;
	export let disabled = false;
	export let tabindex = globalTabindex++;
	export let useUrl = true;
	export let parent = null; // Can be set if this is a nested form within another form (i.e. - InlineForm.html)
	export let initialized = false;
	export let metadata;
	let responseMetadata = {};
	let inputs = [];
	let dispatch = createEventDispatcher();
	let openForms = [];
	let outputFieldValues = null;
	let visibleInputFields = [];
	let submitButtonLabel = "";
	let cssClass = "";
	let initiallyHideForm;
	let documentation;
	let hideForm;
	let title = metadata.label;
	const handlers = [];

	const __this = {
		renderResponse: (response) => renderResponse(response),
		reloadTopForm: () => reloadTopForm(),
		reloadAllForms: () => reloadAllForms(),
		fireAndBubbleUp: (eventName, eventArgs) =>
			fireAndBubbleUp(eventName, eventArgs),
		submit: (event, redirect) => submit(event, redirect),
		init: () => init(),
		destoryOutputs: () => destoryOutputs(),
		useUrl: useUrl,
	};

	export function destoryOutputs() {
		outputFieldValues = null;
	}

	export function renderResponse(response) {
		outputFieldValues = null;
		outputFieldValues = form.outputs;
		responseMetadata = null;
		responseMetadata = form.metadata;
		if (parent == null && response.metadata != null && response.metadata.title != null) {
			document.title = response.metadata.title;
			title = response.metadata.title;
		}
	}

	export function reloadTopForm() {
		if (parent != null) {
			parent.reloadTopForm();
		} else {
			submit(null, true);
		}
	}
	export function reloadAllForms() {
		for (const f of openForms) {
			f.reloadTopForm();
		}
	}

	export function fireAndBubbleUp(eventName, eventArgs) {
		dispatch(eventName, eventArgs);
		const parentFormComponent = parent;

		if (parentFormComponent != null) {
			parentFormComponent.fireAndBubbleUp(eventName, eventArgs);
		}
	}

	export async function submit(event, redirect) {
		// Disable double-posts.
		disabled = true;
		if (event != null) {
			event.preventDefault();
		}

		// If not all required inputs are filled.
		await form
			.allRequiredInputsHaveData(redirect == null)
			.then((allRequiredInputsHaveValues) => {
				if (!allRequiredInputsHaveValues) {
					enableForm();
					return;
				}
			});

		// If postOnLoad == true, then the input field values should appear in the url.
		// Reason is that postOnLoad == true is used by "report" pages, which need
		// their filters to be saved in the url. This does not apply to forms
		// with postOnLoad == false, because those forms are usually for creating new data
		// and hence should not be tracked in browser's history based on parameters.

		const isRedirectForm =
			form.metadata.inputFields.filter((t) =>
				t.eventHandlers.some((r) => r.id === "bind-to-output")
			).length > 0;

		if (form.metadata.postOnLoad && redirect && useUrl && !isRedirectForm) {
			var urlParams = await form.getSerializedInputValues();

			app.go(form.metadata.id, urlParams);
			return;
		}

		try {
			const response = await form.submit(app, redirect == null, {
				formComponent: __this,
			});

			// Signal event to child controls.

			dispatch("form:responseHandled", {
				form: form,
				invokedByUser: event != null,
				response,
			});
			enableForm();
		} catch (e) {
			enableForm();
		}
	}

	export function init() {
		if (!initialized) {
			visibleInputFields = form.inputs.filter(
				(t) => t != null && t.metadata.hidden === false
			);
			initialized = true;
			submitButtonLabel =
				form.metadata.getCustomProperty("submitButtonLabel") || "Submit";
			cssClass = form.metadata.getCustomProperty("cssClass") || "";

			initiallyHideForm =
				form.metadata.getCustomProperty("initiallyHideForm") || null;
			documentation = form.metadata.getCustomProperty("documentation") || null;
			hideForm =
				form.metadata.getCustomProperty("initiallyHideForm") != null
					? "hidden"
					: null;

			metadata.inputFields.forEach((t) =>
				t.eventHandlers.forEach((h) => handlers.push(h))
			);

			metadata.outputFields.forEach((t) =>
				t.eventHandlers.forEach((h) => handlers.push(h))
			);

			metadata.eventHandlers.forEach((h) => handlers.push(h));
			bindEventHandlersToCustomEvents(handlers);

			form.fire("form:loaded", { app });

			// Auto-submit form if necessary.
			if (form.metadata.postOnLoad) {
				submit().then(function () {});
			}

			openForms.push(form);

			if (parent == null) {
				if ((responseMetadata.metadata == null || responseMetadata.metadata.title == null) && form.metadata.label != null) {
					document.title = form.metadata.label;
				}
			}
		}
	}

	export function toggleForm() {
		// eslint-disable-next-line no-console
		if (hideForm.hideForm == null) {
			hideForm = "hidden";
		} else {
			hideForm = null;
		}
	}

	onDestroy(() => {
		openForms = openForms.filter((f) => f.id !== form.id);
	});

	export function bindEventHandlersToCustomEvents(eventHandlers) {
		for (const eventHandler of eventHandlers) {
			// Don't bind default event handlers, because they are already auto-bound inside FormInstance.
			if (eventHandler.runAt.indexOf("form:") === 0) {
				continue;
			}

			dispatch(eventHandler.runAt, (e) => {
				// Augment event args with form which is firing the event. This is needed,
				// so that event handler can know from which particular form this event is coming.
				e.form = form;
				form.handleEvent(eventHandler.runAt, eventHandler, e);
			});
		}
	}

	export function enableForm() {
		disabled = false;
		visibleInputFields = [];
		visibleInputFields = form.inputs.filter(
			(t) => t != null && t.metadata.hidden === false
		);
	}

	export function getInputComponent(inputId) {
		return inputs != null
			? inputs.find((t) => t.field != null && t.field.metadata.id === inputId)
			: null;
	}
</script>

{#if initialized}
	<div class="inputs-horizontal-one-column {cssClass} jf panel panel-default">
		{#if (title != null && title != '')}
			<div class="form-header">
				<h3 class="text-center">
					 {title}
				</h3>
				{#if responseMetadata.metadata != null && responseMetadata.metadata.status}
					<div class="alert {responseMetadata.metadata.status.style}">
						{#if responseMetadata.metadata.status.heading != null}
							<div class="heading">
								{#if responseMetadata.metadata.status.icon != null}
									<span>{@html responseMetadata.metadata.status.icon}</span>
								{/if}
								{responseMetadata.metadata.status.heading}
							</div>
						{/if}
						{#if responseMetadata.metadata.status.message != null}
							<div class="body">
								{@html responseMetadata.metadata.status.message}
							</div>
						{/if}
					</div>
				{/if}
				{#if documentation}
					{#each documentation as document}
						<Help data={document} />
					{/each}
				{/if}
				<div class="border-bottom col-md-12" />
			</div>
		{/if}

		{#if initialized && visibleInputFields.length > 0}
			<div class="form-body">
				{#if initiallyHideForm}
					<div class="hideForm-header" on:click={toggleForm()}>
						<span>{@html initiallyHideForm}</span>
					</div>
				{/if}
				<form on:submit={(e) => submit(e, true)} class={hideForm}>
					{#each visibleInputFields as inputField}
						{#key inputField}
							<FormInput
								field={inputField}
								{app}
								tabindex={tabindex * 100 + inputField.metadata.orderIndex}
								{form}
								formComponent={__this} />
						{/key}
					{/each}
					<div class="full-width" style="margin-bottom: 10px;">
						<button
							type="submit"
							{disabled}
							tabindex="-1"
							class="btn btn-default">{@html submitButtonLabel}</button>
					</div>
				</form>
			</div>
		{/if}

		{#if outputFieldValues != null}
			<div class="response">
				{#each outputFieldValues as outputField}
					{#if outputField.metadata.hidden == false && !(outputField.metadata.getCustomProperty('hideIfNull') === true && outputField.data === null)}
						<FormOutput field={outputField} {app} {form} parent={__this} />
					{/if}
				{/each}
			</div>
		{/if}
	</div>
{/if}

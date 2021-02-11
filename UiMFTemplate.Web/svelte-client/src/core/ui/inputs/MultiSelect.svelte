<script>
	import Choices from "choices.js";
	import * as alertifyLib from "alertifyjs";
	const alertify = alertifyLib.default;
	import { onMount, afterUpdate } from "svelte";

	let input;
	export let id;
	export let field;
	export let tabindex;
	export let form;
	export let formComponent;
	export let app;
	export let visible;

	const { source } = field.metadata.customProperties;
	let inlineChoices;

	onMount(() => {
		const a = new Choices(input, {
			duplicateItems: true,
			addItems: true,
			searchResultLimit: 10,
			removeItemButton: true,
			maxItemCount: field.maxItemCount,
			noChoicesText: "Start type",
			noResultsText: "No result found",
			itemSelectText: "",
			loadingText: "downloading ...",
			maxItemText: (maxItemCount) =>
				`Cannot add more than ${maxItemCount} item`,
		});

		const formElement = input.closest("form");

		if (typeof field.value.value === "undefined" && field.metadata.required) {
			input.closest("div").classList.add("divError");
		}

		formElement.addEventListener("submit", (e) => {
			if (field.metadata.required && field.value.value == null) {
				e.preventDefault();
				alertify.error("The field " + field.metadata.label + " is required");
			}
		});

		let timer = null;
		const parent = input.closest("div");
		const parentInputElements = parent.getElementsByTagName("input");

		if (
			typeof source === "string" &&
			parentInputElements != null &&
			parentInputElements.length > 0
		) {
			parentInputElements[0].addEventListener("keydown", (e) => {
				if (e.keyCode === 8 && e.target.value.length === 1) {
					if (timer != null) {
						clearTimeout(timer);
					}
					timer = setTimeout(() => {
						populateChoicesWithAjax(a, "");
					}, 300);
				}
			});
			parentInputElements[0].addEventListener("keyup", (e) => {
				if (
					e.keyCode === 8 &&
					(e.target.value === "" || e.target.value == null)
				) {
					if (timer != null) {
						clearTimeout(timer);
					}
					timer = setTimeout(() => {
						populateChoicesWithAjax(a, "");
					}, 300);
				}
			});
		}

		a.passedElement.addEventListener("removeItem", (event) => {
			if (field.metadata.required && a.getValue().length === 0) {
				input.closest("div").classList.add("divError");
			}
		});

		a.passedElement.addEventListener("addItem", (event) => {
			if (field.metadata.required) {
				input.closest("div").classList.remove("divError");
			}
		});

		if (typeof source === "string") {
			const addedItems = {};
			let query = "";
			a.passedElement.addEventListener("search", (value) => {
				query = value.detail.value;

				if (timer != null) {
					// Cancel previous timer, thus extending the delay until user has stopped typing.
					clearTimeout(timer);
				}
				// Search when user types something, but introduce a short delay
				// to avoid excessive http requests.
				timer = setTimeout(() => {
					populateChoicesWithAjax(a, query);
				}, 300);
			});
			const currentValue = getIdsQuery(field);
			populateChoicesWithAjax(a, "");
			// If the field has a value, we need to load it.
			if (currentValue.length > 0) {
				populateChoicesWithAjax(a, query, currentValue).then(() => {
					setInputValue(a, field);
					onChange();
				});
			}
		} else {
			a.setChoices(mapToTypeaheadItems(source), "value", "label", true);
			inlineChoices = a;
		}
		a.passedElement.addEventListener("change", () => {
			field.value = calculateFieldValue(field, a.getValue());
		});
	});

	afterUpdate(() => {
		if (inlineChoices != null) {
			setInputValue(inlineChoices, field);
		}
	});

	function mapToTypeaheadItems(items) {
		return items.map((t) => ({
			label: t.label,
			value: t.value.toString(),
		}));
	}
	function calculateFieldValue(field, value) {
		if (field.maxItemCount === 1) {
			let result = field.value;
			result = {
				value: value[0] != null ? value[0].value : null,
			};
			// We need to convert the value to string, otherwise it doesn't work.
			// This is due to the way UmfApp deals with url parameters (or something
			// along those lines).
			if (result.value != null) {
				result.value = result.value.toString();
			}
			return result;
		}
		return {
			items: value
				.filter((t) => !t.label.includes("<small>New</small>"))
				.map((t) => t.value),
			newItems: value
				.filter((t) => t.label.includes("<small>New</small>"))
				.map((t) => t.value),
		};
	}
	function setInputValue(a, field) {
		if (field.maxItemCount === 1) {
			const v = (field.value || {}).value || null;
			if (v != null) {
				a.setValueByChoice(v.toString());
			}
		} else {
			const v = ((field.value || {}).items || []).map((t) => t.toString());
			a.setValueByChoice(v);
		}
	}
	function getIdsQuery(field) {
		let currentValue =
			field.maxItemCount === 1
				? [(field.value || {}).value || ""]
				: (field.value || {}).items || [];
		// Put values into an array.
		if (currentValue[0] === "") {
			currentValue = [];
		}
		return currentValue;
	}
	function buildFilter(parentForm, parameters, query) {
		let promise;
		const filter = { query };
		if (parameters != null && parameters.length > 0) {
			promise = parentForm.getSerializedInputValues().then((data) => {
				for (const p of parameters) {
					filter[p] = data[p];
				}
				return filter;
			});
		} else {
			promise = Promise.resolve(filter);
		}
		return promise;
	}

	function populateChoicesWithAjax(choicesComponent, query, selectedItemIds) {
		const {
			parameters,
			source,
			typeaheadAllowNew,
		} = field.metadata.customProperties;

		// eslint-disable-next-line no-param-reassign
		return new Promise((resolve) => {
			choicesComponent.ajax((callback) =>
				buildFilter(form, parameters, query).then((filter) => {
					if (selectedItemIds != null) {
						// eslint-disable-next-line no-param-reassign
						filter.ids = { items: selectedItemIds };
					}
					return app.server.postForm(source, filter).then((data) => {
						choicesComponent.setChoices(
							mapToTypeaheadItems([]),
							"value",
							"label",
							true
						);
						// Mark items as added as "choices".
						if (typeaheadAllowNew) {
							if (data.items.length === 0) {
								if (selectedItemIds != null) {
									const newItems = [];
									selectedItemIds.forEach((e) => {
										newItems.push({
											label: `${e} <small>New</small>`,
											value: e,
										});
									});
									callback(
										mapToTypeaheadItems(newItems),
										"value",
										"label",
										true
									);
									resolve();
								} else {
									const newLabel = `${query} <small>New</small>`;
									const newItem = { label: newLabel, value: query };
									callback(
										mapToTypeaheadItems([newItem]),
										"value",
										"label",
										true
									);
									resolve();
								}
							} else {
								callback(
									mapToTypeaheadItems(data.items),
									"value",
									"label",
									true
								);
								resolve();
							}
						} else {
							callback(mapToTypeaheadItems(data.items), "value", "label", true);
							resolve();
						}
					});
				})
			);
		});
	}

	function onChange() {
		formComponent.fireAndBubbleUp("input:changed", {
			app: app,
			form: formComponent,
			input: this,
		});
	}
</script>

<select
	on:blur={() => onChange()}
	class="multi-select form-control"
	{id}
	{tabindex}
	bind:this={input}
	multiple />

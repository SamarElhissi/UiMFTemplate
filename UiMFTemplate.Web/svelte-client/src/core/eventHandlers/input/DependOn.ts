import * as umf from "uimf-core";
import {
	InputController,
	InputEventArguments,
	InputFieldEventHandler
} from "../../framework/index";

export class DependOn extends InputFieldEventHandler {
	public run(input: InputController<any>, eventHandlerMetadata: umf.EventHandlerMetadata, args: InputEventArguments): Promise<any> {
		const subscribedToField = eventHandlerMetadata.customProperties.field;
		const subscribedToValue = eventHandlerMetadata.customProperties.value;
		const field = args.input.get("field").metadata != null
		? args.input.get("field")
		: args.input.get("field").field;
		const fieldChanged = field.metadata.id;
		if (subscribedToField === fieldChanged) {
			return input.serialize().then((t) => {
				const parentInputController = field;
				const childWrapper = args.form.getInputComponent(input.metadata.id);
				if (childWrapper != null) {
				let childShouldBeVisible = false;
				if (["typeahead", "dropdown"].indexOf(parentInputController.metadata.type) !== -1) {
					if (parentInputController.value != null && parentInputController.value.value != null) {
						if (subscribedToValue === "isNew") {
							childShouldBeVisible = isNaN(parentInputController.value.value);
						} else if (subscribedToValue === "") {
							childShouldBeVisible = true;
						} else {
							childShouldBeVisible = parentInputController.value.value.toString() === subscribedToValue ||
							parentInputController.value.value.includes(subscribedToValue);
						}
					}
				} else {
					if (parentInputController.value != null) {
						if (subscribedToValue === "") {
							childShouldBeVisible = true;
						} else {
							childShouldBeVisible = parentInputController.value.toString() === subscribedToValue ||
						parentInputController.value.includes(subscribedToValue);
						}
						childShouldBeVisible = parentInputController.value != null
						&& (parentInputController.value.toString() === subscribedToValue ||
						parentInputController.value.includes(subscribedToValue));
					}
				}
				childWrapper.show(childShouldBeVisible);
			}
			});
		}

		return Promise.resolve();
	}
}

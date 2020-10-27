import * as umf from "../../framework";
import { ITypeaheadConfig } from "./TypeaheadInputController";

export class MultiSelectInputController
	extends umf.InputController<MultiSelectValue>
	implements ITypeaheadConfig {
	public maxItemCount: number = -1;

	public serializeValue(value: MultiSelectValue | string): string {
		if (typeof (value) === "string") {
			return value;
		}

		return value != null ? (value.items || []).join(",") : null;
	}

	public init(value: string): Promise<MultiSelectInputController> {
		return new Promise((resolve, reject) => {
			this.value = this.parse(value);
			resolve(this);
		});
	}

	public getValue(): Promise<MultiSelectValue> {
		const valueToSubmit =
			this.value == null ||
			((this.value.items == null || this.value.items.length === 0) &&
				(this.value.newItems == null || this.value.newItems.length === 0))
				? null
				: this.value;

		return Promise.resolve(valueToSubmit);
	}

	private parse(value: string): MultiSelectValue {
		return value == null || value === ""
			? new MultiSelectValue()
			: new MultiSelectValue(value.split(","));
	}
}

// tslint:disable-next-line:max-classes-per-file
class MultiSelectValue {
	constructor(items?: any[], newItems?: any[]) {
		this.items = items;
		this.newItems = newItems;
	}

	public items: any[] = [];
	public newItems: any[] = [];
}

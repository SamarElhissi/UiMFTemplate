import * as umf from "../../framework";

export class HtmlEditorInputController extends umf.InputController<HtmEditor> {
	public selected: string;

	public serializeValue(value: HtmEditor | string): string {
		if (typeof (value) === "string") {
			return value;
		}

		return value != null ? value.value : null;
	}

	public init(value: string): Promise<HtmlEditorInputController> {
		return new Promise((resolve, reject) => {
			this.selected = value;
			this.value = this.parse(value);
			resolve(this);
		});
	}

	public getValue(): Promise<HtmEditor> {
		return Promise.resolve(this.parse(this.selected));
	}

	private parse(value: string): HtmEditor {
		return value == null || value === "" ? null : { value };
	}
}

// tslint:disable-next-line:max-classes-per-file
class HtmEditor {
	public value: string;
}

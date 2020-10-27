import * as umf from "../../framework";

export class NumberObjectInputController extends umf.InputController<NumberObject> {
	public selected: string;

	public serializeValue(value: NumberObject): string {
		if (typeof (value) === "number") {
			return value;
		}

		return value != null ? value.value : null;
	}

	public init(value: string): Promise<NumberObjectInputController> {
		return new Promise((resolve, reject) => {
			this.selected = value;
			this.value = this.parse(value);
			resolve(this);
		});
	}

	public getValue(): Promise<NumberObject> {
		return Promise.resolve(this.parse(this.selected));
	}

	private parse(value: string): NumberObject {
		return value == null ? null : NumberObject.parse(value);
	}
}

// tslint:disable-next-line:max-classes-per-file
class NumberObject {
	constructor(value: string = null) {
		this.value = value;
	}
	public static parse(value: string): NumberObject {
		return new NumberObject(value);
	}
	public value: string;
}

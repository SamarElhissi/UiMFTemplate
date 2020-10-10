import * as umf from "core-framework";
import { NumberInputController } from "core-ui/inputs/NumberInputController";

export class ReceiptListInputController extends umf.InputController<ReceiptList> {

	public entityId: number = null;
	public price: number = null;
	public quantity: number = null;

	public init(value: string): Promise<ReceiptListInputController> {
		return new Promise((resolve, reject) => {
			this.value = this.parse(value);
			resolve(this);
		});
	}

	public getValue(): Promise<ReceiptList> {
		return Promise.resolve(this.value);
	}

	public serializeValue(list: ReceiptList): any {
		const parsed = this.parse(list);
		return parsed != null ? parsed.serialize() : "";
	}

	private parse(value: ReceiptList | string): ReceiptList {
		if (value == null) {
			return new ReceiptList();
		}
		return typeof (value) === "string"
		? ReceiptList.parse(value)
		: value;
	}
}

// tslint:disable-next-line:max-classes-per-file
class ReceiptList {
	constructor(entityId: number = null, quantity: number = null, price: number = null) {
		this.entityId = entityId;
		this.quantity = quantity;
		this.price = price;
	}

	public entityId: number;
	public price: number;
	public quantity: number;

	public static parse(list: string): ReceiptList {
		const split = list.split("|");
		const entityId = parseFloat(split[0]);
		const price = parseFloat(split[1]);
		const quantity = parseFloat(split[2]);

		return new ReceiptList(entityId, quantity, price);
	}

	public serialize(): string {
		return `${NumberInputController.serialize(this.entityId)}|${NumberInputController.serialize(this.price)}|${NumberInputController.serialize(this.quantity)}`;
	}
}

import { IFunctionRunner } from "core-framework";
import * as umf from "uimf-core";

export class Redirect implements IFunctionRunner {
	public run(metadata: umf.ClientFunctionMetadata): Promise<void> {
		(window as any).app.go(metadata.customProperties.form, metadata.customProperties.inputFieldValues);
		return Promise.resolve();
	}
}

import { IFunctionRunner } from "../framework";
import * as umf from "uimf-core";

export class Redirect implements IFunctionRunner {
	public run(metadata: umf.ClientFunctionMetadata): Promise<void> {
		if(!metadata.customProperties.isNewUiMF){
			window.location.href = this.makeOldFormsUrl(metadata.customProperties.form, metadata.customProperties.inputFieldValues);
			return Promise.reject();
		}
		else {
			(window as any).uimfapp.go(metadata.customProperties.form, metadata.customProperties.inputFieldValues);
		}
		return Promise.resolve();
	}

	public makeOldFormsUrl(form: string, values: any): string {
				var href = `/#/form/${form}?`;
				var props = Object.getOwnPropertyNames(values);
	
				var q = "";
				props.forEach((prop, index) =>{
					q += prop +"=" + values[prop];
					if(index + 1 != props.length)
						q += "&";
				})
				return href + q;
			}
}

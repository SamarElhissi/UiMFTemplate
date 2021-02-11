import { FormMetadata, FormResponse, FormResponseMetadata } from "uimf-core";

export class UmfServer {
	private readonly getMetadataUrl: string;
	private readonly getMenuUrl: string;
	private readonly postFormUrl: string;
	private eventHandlers: { [id: string]: IEventHandler[] } = {};

	/**
	 * Creates a new instance of UmfApp.
	 */
	constructor(getMetadataUrl: string, postFormUrl: string, getMenuUrl: string) {
		this.getMetadataUrl = getMetadataUrl;
		this.postFormUrl = postFormUrl;
		this.getMenuUrl = getMenuUrl;
	}

	public on(event: string, handler: IEventHandler): void {
		this.eventHandlers[event] = this.eventHandlers[event] || [];
		this.eventHandlers[event].push(handler);
	}

	private fire(event: string, params?: any): void {
		const handlersForEvent = this.eventHandlers[event];
		if (handlersForEvent != null && handlersForEvent.length > 0) {
			for (const handler of handlersForEvent) {
				handler(params);
			}
		}
	}

	public getMetadata(formId: string): Promise<FormMetadata> {
		this.fire("request:started");
		return fetch(`${this.getMetadataUrl}/${formId}`, {
			method: "GET",
			headers: {
				"Content-Type": "application/json"
			}
		})
			.then((response: Response) => {
				return response.json();
			})
			.then((data: FormMetadata) => {
				this.fire("request:completed");
				return data;
			})
			.catch((e) => {
				// tslint:disable-next-line:no-console
				console.warn(`Did not find form "${formId}".`);
				this.fire("request:completed");
				return null;
			});
	}

	public getAllMetadata(): Promise<{ forms: FormMetadata[]; menu: any }> {
		this.fire("request:started");
		return fetch(this.getMetadataUrl, {
			method: "GET",
			headers: {
				"Content-Type": "application/json"
			}
		})
			.then((response: Response) => {
				this.fire("request:completed");
				return response.json();
			})
			.catch((e) => {
				// tslint:disable-next-line:no-console
				this.fire("request:completed");
				return null;
			});
	}

	public getAllMenu(): Promise<{ forms: FormMetadata[]; menu: any }> {
		return fetch(this.getMenuUrl, {
			method: "GET"
		}).then((response: Response) => {
			return response.json();
		});
	}

	public postForm(form: string, data: any): Promise<FormResponse> {
		this.fire("request:started");
		return fetch(this.postFormUrl, {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify([
				{
					Form: form,
					RequestId: 1,
					InputFieldValues: data
				}
			])
		})
			.then((response: Response) => {
				return response.json();
			})
			.then((invokeFormResponses: any) => {
				if (invokeFormResponses.error != null) {
					this.fire("request:completed", invokeFormResponses.error);
					return null;
				}
				invokeFormResponses[0].data.metadata =
					invokeFormResponses[0].data.metadata || new FormResponseMetadata();
				this.fire("request:completed");
				return invokeFormResponses[0].data;
			})
			.catch((error) => {
				this.fire("request:completed", error.Message);
				return null;
			});
	}
}

interface InvokeFormResponse {
	data: FormResponse;
	requestId: string;
}

type IEventHandler = (params?: any) => any;

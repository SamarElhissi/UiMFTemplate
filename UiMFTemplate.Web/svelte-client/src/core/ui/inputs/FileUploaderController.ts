import * as umf from "../../framework";

export class FileUploaderController extends umf.InputController<
	FileUploaderValue
	> {
	public selected: any[];
	public filesIds: any[] = [];
	public serializeValue(value: FileUploaderValue | string): string {
		return value != null ? JSON.stringify(value) : null;
	}

	public init(): Promise<FileUploaderController> {
		return new Promise((resolve, reject) => {
			// Don't do anything. File uploader doesn't allow initialization
			// from pre-existing value.
			resolve(this);
		});
	}

	public getValue(): Promise<FileUploaderValue> {
		const self = this;

		if (self.selected == null || self.selected.length === 0) {
			return Promise.resolve(new FileUploaderValue());
		}
		const promises = [];
		const result = new FileUploaderValue();
		const files = self.selected;
		if (self.filesIds.length > 0) {
			for (const fileId of self.filesIds) {
				result.files.push(fileId);
			}
			self.filesIds = [];
			self.selected = null;
		} else {
			const p = new Promise((resolve, reject) => {
				const formData = new FormData();
				for (const f of files) {
					formData.append("file", f);
				}

				// Make http request to upload the files.
				fetch("/document/UploadDocument", {
					method: "POST",
					body: formData
				})
					.then((res) => res.json())
					.then((response) => {
						if (response.Exception != null) {
							alert(response.Message);
							reject(response.Exception);
						}
						if (response != null && response.length > 0) {
							for (const file of response) {
								result.files.push(file.HashId);
								self.filesIds.push(file.HashId);
							}
						}
						resolve();
					})
					.catch((error) => {
						alert(error.response.data.Message);
						reject(error);
					});
			});

			promises.push(p);
		}

		return Promise.all(promises).then((t) => {
			return result;
		});
	}
}

// tslint:disable-next-line:max-classes-per-file
class FileUploaderValue {
	public files: number[] = [];
}

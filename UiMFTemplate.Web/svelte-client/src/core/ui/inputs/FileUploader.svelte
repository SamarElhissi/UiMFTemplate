<script>
	import { onMount, afterUpdate } from "svelte";
	export let id;
	export let field;
	export let tabindex;
	export let app;
	export let selectedFiles;
	export let uploaderConfig;
	export let dropzone;
	export let files;

	afterUpdate(() => {
		if (files != null) addFiles();
	});

	onMount(() => {
		uploaderConfig = field.metadata.getCustomProperty("fileUploaderConfig") || {
			allowMultipleFiles: false,
			allowedFileExtensions: "*",
		};

		dropzone.addEventListener("dragenter", () => {
			dropzone.classList.add("hover");
		});

		["dragleave", "drop"].forEach((t) => {
			dropzone.addEventListener(t, () => {
				dropzone.classList.remove("hover");
			});
		});

		dropzone.ondragover = (e) => {
			e.preventDefault();
		};

		dropzone.ondrop = (e) => {
			e.preventDefault();
			addFiles(e.dataTransfer.files);
			return false;
		};
	});

	function objectToArray(obj) {
		if (obj) {
			return Array.from(Object.keys(obj), (k) => obj[k]);
		}
		return [];
	}

	function getExtension(filename) {
		return `.${filename.split(".").pop()}`;
	}

	function getAllowedFileExtensions() {
		let allowedExtension = "*";
		if (uploaderConfig && uploaderConfig.allowedFileExtensions) {
			allowedExtension = uploaderConfig.allowedFileExtensions
				.split(",")
				.map((item) => item.trim());
		}
		return allowedExtension;
	}
	function canFileBeAdded(filelist, file, allowedFileExtensions, app) {
		const isFileExtensionAllowed =
			allowedFileExtensions.indexOf("*") > -1 ||
			allowedFileExtensions.indexOf(getExtension(file.name)) > -1;

		const fileWasAlreadyAdded = filelist.find((f) => f.name === file.name);

		if (isFileExtensionAllowed && !fileWasAlreadyAdded) {
			return true;
		}

		if (!isFileExtensionAllowed) {
			app.showError(
				`File ${file.name} extension not allowed. Only ` +
					`these file extensions are allowed: ${allowedFileExtensions}.`
			);
		}
		return false;
	}

	export function addFiles() {
		const filesToAdd = objectToArray(files);
		let listedFiles = [];

		if (selectedFiles) {
			listedFiles = selectedFiles;
		}

		const allowedFileExtensions = getAllowedFileExtensions();
		const filesThatCanBeAdded = filesToAdd.filter((file) =>
			canFileBeAdded(listedFiles, file, allowedFileExtensions, app)
		);

		if (filesThatCanBeAdded.length > 0) {
			if (!uploaderConfig.allowMultipleFiles) {
				selectedFiles = filesThatCanBeAdded.slice(0, 1);
			} else {
				selectedFiles = filesThatCanBeAdded.concat(listedFiles);
			}
		}
		field.selected = selectedFiles;
	}

	function removeFile(index) {
		selectedFiles.splice(index, 1).splice(index, 1);
		field.selected = selectedFiles;
		if (uploaderConfig != null && !uploaderConfig.allowMultipleFiles) {
			files = null;
		}
	}
</script>

{#if selectedFiles != null && selectedFiles.length > 0}
	<ul class="files-list">
		{#each selectedFiles as file, index}
			<li>
				{file.name}
				<i
					class="fa fa-times second-color"
					on:click={() => removeFile(index)} />
			</li>
		{/each}
	</ul>
{/if}
<div class="file-drop-area" bind:this={dropzone}>
	{#if uploaderConfig != null && uploaderConfig.allowMultipleFiles}
		<input
			type="file"
			{id}
			{tabindex}
			multiple="multiple"
			bind:files
			accept={uploaderConfig ? uploaderConfig.allowedFileExtensions : '*'} />
	{:else}
		<input
			type="file"
			{id}
			{tabindex}
			bind:files
			accept={uploaderConfig ? uploaderConfig.allowedFileExtensions : '*'} />
	{/if}
	<label for={id}>drag or
		<strong class="second-color">upload</strong>
		files</label>
</div>

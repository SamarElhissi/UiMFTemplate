<script context="module">
	let globalModalId = 1;
</script>

<script>
	import Help from "./Help.svelte";

	function humanize(e) {
		return e
			.replace(/\.[^/.]+$/, "")
			.split(/(?=[A-Z])/)
			.join(" ");
	}

	export let data;
	let files = data.files.map((i) => ({ file: i, name: humanize(i) }));
	let modalId = globalModalId++;
	let open = false;

	function onOpen() {
		open = true;
		currentlyOpenHelpModal = new Help();
	}

	function onClose() {
		open = false;
	}

	function handleKeydown(evt) {
		const theEvent = evt || window.event;
		let isEscape = false;
		if ("key" in theEvent) {
			isEscape = theEvent.key === "Escape" || theEvent.key === "Esc";
		} else {
			isEscape = theEvent.keyCode === 27;
		}
		if (isEscape) {
			onClose();
		}
	}
</script>

<svelte:window on:keydown={handleKeydown} />
{#if data != null}
	{#if data.placement == 'Hint'}
		<div class="help-content modal-help">
			<i
				on:click={onOpen}
				class="far fa-question-circle"
				title="Click to open help for this form" />
			<input
				id="modal-{modalId}"
				type="checkbox"
				bind:checked={open}
				class="hidden" />
			<div class="modal">
				<div class="card">
					<span class="close" on:click={onClose} />
					{@html data.content}

					{#if files && files.length > 0}
						<div class="help-files">
							<span>For more information:</span>
							<ul>
								{#each files as file}
									<li>
										<a href="/#/form/help?FileId={file.file}">{file.name}</a>
									</li>
								{/each}
							</ul>
						</div>
					{/if}
				</div>
			</div>
		</div>
	{:else if data.placement == 'Inline'}
		<div class="help-content inline-help">
			{@html data.content}
			{#if files && files.length > 0}
				<div class="help-files">
					<span>For more information</span>
					<ul>
						{#each files as file}
							<li><a href="/#/form/help?FileId={file.file}">{file.name}</a></li>
						{/each}
					</ul>
				</div>
			{/if}
		</div>
	{/if}
{/if}

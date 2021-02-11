<script>
	import { onMount } from "svelte";

	export let field;

	let content;
	let output;

	let closeLevel = 1;
	let result = "<ul>";
	let counter = 0;

	onMount(() => {
		content.querySelectorAll("h1, h2, h3").forEach((e) => {
			e.setAttribute("id", counter);
			const openLevel = e.tagName.match(/\d/g)[0];
			if (openLevel > closeLevel) {
				result += `${"<ul><li><a data-target='"}${counter}'>${e.innerText}</a>`;
			} else if (openLevel < closeLevel) {
				const closingTags = Array(closeLevel - openLevel)
					.fill()
					.map(() => "</li></ul>")
					.join("");

				result += `${closingTags}</li><li><a data-target='${counter}'>${e.innerText}</a>`;
			} else {
				if (closeLevel !== 1) {
					result += "</li>";
				}
				result += `<li><a data-target='${counter}'>${e.innerText}</a>`;
			}
			closeLevel = openLevel;
			counter += 1;
		});

		result += Array(closeLevel)
			.fill()
			.map(() => "</li></ul>")
			.join("");

		output.innerHTML = result;

		document.querySelectorAll(".table-of-contents a").forEach((a) => {
			a.addEventListener("click", (event) => {
				const target = event.currentTarget.getAttribute("data-target");
				document.getElementById(target).scrollIntoView({ behavior: "smooth" });

				// clear active
				document
					.querySelectorAll(".table-of-contents li.active")
					.forEach((li) => {
						li.classList.remove("active");
					});

				// set active
				let parent = a.parentElement;
				while (parent) {
					if (parent.tagName === "LI") {
						parent.classList.add("active");
					}
					parent = parent.parentElement;
				}

				event.stopPropagation();
			});
		});
	});
</script>

<div class="documentation">
	<div class="wrapper">
		<div class="content" bind:this={content}>
			{#if field.data != null && field.data.value != null}
				{@html field.data.value}
			{/if}
		</div>

		<div class="table-of-contents" bind:this={output} />
	</div>
</div>

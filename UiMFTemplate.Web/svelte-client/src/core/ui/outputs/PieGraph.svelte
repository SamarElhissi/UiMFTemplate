<script>
	import * as d3 from "d3";
	import Alert from "./Alert.svelte";
	export let field;
	let pieElement;
	let message = null;
	export let parent;
	export let app;
	export let form;

	function shuffle(a) {
		for (let i = a.length - 1; i > 0; i--) {
			const j = Math.floor(a.length - 1 * (i + 1));
			// eslint-disable-next-line no-param-reassign
			[a[i], a[j]] = [a[j], a[i]];
		}
		return a;
	}

	// eslint-disable-next-line prefer-destructuring
	let data = field.data;
	message = {
		data: field.data.chartHeader,
	};

	data = data.data.map((d) => ({ presses: d.data, letter: d.label }));
	const width = 600,
		height = 400,
		radius = Math.min(width, height) / 2;

	// legend dimensions
	const legendRectSize = 25; // defines the size of the colored squares in legend
	const legendSpacing = 8; // defines spacing between squares
	// eslint-disable-next-line prefer-const
	let colors = d3.schemeCategory10;
	shuffle(colors);
	// define color scale
	const color = d3.scaleOrdinal(colors);
	// more color scales: https://bl.ocks.org/pstuffa/3393ff2711a53975040077b7453781a9

	const svg = d3
		.select(pieElement) // select element in the DOM with id 'chart'
		.append("svg") // append an svg element to the element we've selected
		.attr("viewBox", `25 0 ${width} ${height}`)
		.attr("preserveAspectRatio", "xMinYMin")
		.append("g") // append 'g' element to the svg element
		.attr("transform", `translate(${width / 2},${height / 2})`)
		.style("padding-right", "60px");

	const arc = d3
		.arc()
		.outerRadius(radius - 10)
		.innerRadius(0);

	const pie = d3
		.pie()
		.value((d) => d.count)
		.sort(null);

	// define tooltip
	const tooltip = d3
		.select(pieElement) // select element in the DOM with id 'chart'
		.append("div") // append a div element to the element we've selected
		.attr("class", "tooltips"); // add class 'tooltip' on the divs we just selected

	tooltip
		.append("div") // add divs to the tooltip defined above
		.attr("class", "label"); // add class 'label' on the selection

	tooltip
		.append("div") // add divs to the tooltip defined above
		.attr("class", "count"); // add class 'count' on the selection

	tooltip
		.append("div") // add divs to the tooltip defined above
		.attr("class", "percent"); // add class 'percent' on the selection

	data.forEach((d) => {
		// eslint-disable-next-line no-param-reassign
		d.count = +d.presses; // calculate count as we iterate through the data
		// eslint-disable-next-line no-param-reassign
		d.enabled = true; // add enabled property to track which entries are checked
		// eslint-disable-next-line no-param-reassign
		d.label = d.letter;
	});

	// creating the chart
	let path = svg.selectAll(".path");
	path = path
		.data(pie(data))
		.enter()
		.append("path")
		.attr("d", arc)
		.attr("fill", (d) => color(d.data.label))
		// eslint-disable-next-line no-underscore-dangle
		.each((d) => path._current - d);

	let pathText = svg
		.selectAll(".path")
		.data(pie(data))
		.enter()
		.append("text")
		.text((d) => {
			const total = d3.sum(data.map((m) => (m.enabled ? m.count : 0)));
			const percent = Math.round((1000 * d.data.count) / total) / 10; // calculate percent
			return `${d.data.count} - ${percent}%`;
		})
		// eslint-disable-next-line prefer-template
		.attr("transform", (d) => "translate(" + arc.centroid(d) + ")")
		.style("text-anchor", "middle")
		.style("font-size", 14)
		.style("direction", "ltr");

	path.on("mouseover", (d) => {
		// when mouse enters div
		const total = d3.sum(data.map((m) => (m.enabled ? m.count : 0)));
		const percent = Math.round((1000 * d.data.count) / total) / 10; // calculate percent
		tooltip.select(".label").html(d.data.label); // set current label
		tooltip.select(".count").html(`${d.data.count}`); // set current count
		tooltip.select(".percent").html(`${percent}%`); // set percent calculated above
		tooltip.style("display", "block"); // set display
	});

	path.on("mouseout", () => {
		// when mouse leaves div
		tooltip.style("display", "none"); // hide tooltip for that element
	});

	path.on("mousemove", () => {
		// when mouse moves
		tooltip
			.style("top", `${d3.event.layerY + 10}px`) // always 10px below the cursor
			.style("left", `${d3.event.layerX + 10}px`); // always 10px to the right of the mouse
	});

	path
		.transition() // transition of redrawn pie
		.duration(750) //
		.attrTween("d", (d) => {
			// eslint-disable-next-line no-underscore-dangle
			const interpolate = d3.interpolate(path._current, d);
			// eslint-disable-next-line no-underscore-dangle
			path._current = interpolate(0);
			return (t) => arc(interpolate(t));
		});

	// define legend
	const legend = svg
		.selectAll(".legend")
		.data(color.domain())
		.enter()
		.append("g")
		.attr("class", "legend")
		.attr("transform", (d, i) => {
			const legendHeight = legendRectSize + legendSpacing;
			const offset = (legendHeight * color.domain().length) / 2;
			const horz = 8 * legendRectSize;
			const vert = i * legendHeight - offset;
			return `translate(${horz},${vert})`;
		});

	let rectElement = legend.append("rect");

	// adding colored squares to legend
	rectElement = rectElement
		.attr("width", legendRectSize)
		.attr("height", legendRectSize)
		.style("fill", color)
		.style("direction", "ltr")
		.style("stroke", color);

	rectElement.on("click", (label) => {
		// eslint-disable-next-line no-param-reassign
		const { event } = window;
		const target = event.target || event.srcElement;
		const rect = d3.select(target);
		let enabled = true;
		const totalEnabled = d3.sum(data.map((d) => (d.enabled ? 1 : 0)));

		if (rect.attr("class") === "disabled") {
			// if class is disabled
			rect.attr("class", ""); // remove class disabled
		} else {
			// else
			if (totalEnabled < 2) return; // if less than two labels are flagged, exit
			rect.attr("class", "disabled"); // otherwise flag the square disabled
			enabled = false; // set enabled to false
		}

		pie.value((d) => {
			// eslint-disable-next-line no-param-reassign
			if (d.label === label) d.enabled = enabled;
			return d.enabled ? d.count : 0;
		});

		path = path.data(pie(data)); // update pie with new data
		// pathText = pathText.data(pie(data));

		pathText = pathText
			.data(pie(data))
			.text((d) => {
				if (!d.data.enabled) {
					return "";
				}
				const total = d3.sum(data.map((m) => (m.enabled ? m.count : 0)));
				const percent = Math.round((1000 * d.data.count) / total) / 10; // calculate percent
				return `${d.data.count} - ${percent}%`;
			})
			.attr("transform", (d) => `translate(${arc.centroid(d)})`)
			.style("text-anchor", "middle")
			.style("font-size", 14)
			.style("direction", "ltr");

		path
			.transition() // transition of redrawn pie
			.duration(750) //
			.attrTween("d", (d) => {
				// eslint-disable-next-line no-underscore-dangle
				const interpolate = d3.interpolate(path._current, d);
				// eslint-disable-next-line no-underscore-dangle
				path._current = interpolate(0);
				return (t) => arc(interpolate(t));
			});
	});

	// adding text to legend
	legend
		.append("text")
		.attr("x", legendRectSize + legendSpacing)
		.attr("y", legendRectSize - legendSpacing)
		.style("direction", "ltr")
		.text((d) => d); // return label
</script>

<style>
	.legend {
		font-size: 14px;
	}

	rect {
		cursor: pointer;
		stroke-width: 2;
	}

	rect.disabled {
		fill: transparent !important;
	}

	div {
		max-width: 600px;
		margin: 0 auto;
		position: relative;
		display: block;
	}

	/* tooltip */
	.tooltips {
		background: #eee;
		box-shadow: 0 0 5px #ebe5e5;
		color: #333;
		display: none;
		font-size: 18px;
		right: 130px;
		padding: 10px;
		position: absolute;
		text-align: center;
		top: 95px;
		width: 80px;
		z-index: 10;
	}
	.path-label {
		display: block;
		color: #000;
		z-index: 20;
	}
</style>

{#if field.data != null}
	{#if message != null}
		<Alert field={message} />
	{/if}
	<div class="pie-chart" bind:this={pieElement} />
{/if}

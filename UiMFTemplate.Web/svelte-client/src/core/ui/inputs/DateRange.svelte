<script>
	import Pikaday from "pikaday";
	import { onMount } from "svelte";
	import { DateInputController } from "./DateInputController";

	export let id;
	export let field;
	let minElement;
	let maxElement;
	export let tabindex;
	export let form;
	export let visible;
	export let formComponent;
	export let app;
	let minElementId = id + "min";
	let maxElementId = id + "max";

	onMount(() => {
		let includeTime = field.metadata.getCustomProperty("includeTime") || false;

		new Pikaday({
			field: minElement,
			format: includeTime ? "YYYY-MM-DD hh:mm A" : "YYYY-MM-DD",
			showTime: includeTime,
			showMinutes: true,
			showSeconds: false,
			use24hour: false,
			incrementHourBy: 1,
			incrementMinuteBy: 5,
			incrementSecondBy: 1,
			i18n: {
				midnight: "0 AM", // Label for 0 AM
				noon: "12 AM", // Label for 12 AM
				previousMonth: "Previous Month",
				nextMonth: "Next Month",
				months: [
					"January",
					"February",
					"March",
					"April",
					"May",
					"June",
					"July",
					"August",
					"September",
					"October",
					"November",
					"December",
				],
				weekdays: [
					"Sunday",
					"Monday",
					"Tuesday",
					"Wednesday",
					"Thursday",
					"Friday",
					"Saturday",
				],
				weekdaysShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
			},
			timeLabel: "Time", // optional string added to left of time select
			onSelect(date) {
				field.value.min = date;
				field.minValueAsText =
					date != null
						? `${date.getFullYear()}-${DateInputController.format2DecimalPlaces(
								date.getMonth() + 1
						  )}-${DateInputController.format2DecimalPlaces(date.getDate())}`
						: null;
			},
		});

		// eslint-disable-next-line no-new
		new Pikaday({
			field: maxElement,
			format: includeTime ? "YYYY-MM-DD hh:mm A" : "YYYY-MM-DD",
			showTime: includeTime,
			showMinutes: true,
			showSeconds: false,
			use24hour: false,
			incrementHourBy: 1,
			incrementMinuteBy: 5,
			incrementSecondBy: 1,
			i18n: {
				midnight: "0 AM", // Label for 0 AM
				noon: "12 AM", // Label for 12 AM
				previousMonth: "Previous Month",
				nextMonth: "Next Month",
				months: [
					"January",
					"February",
					"March",
					"April",
					"May",
					"June",
					"July",
					"August",
					"September",
					"October",
					"November",
					"December",
				],
				weekdays: [
					"Sunday",
					"Monday",
					"Tuesday",
					"Wednesday",
					"Thursday",
					"Friday",
					"Saturday",
				],
				weekdaysShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
			},
			onSelect(date) {
				field.value.max = date;
				field.maxValueAsText =
					date != null
						? `${date.getFullYear()}-${DateInputController.format2DecimalPlaces(
								date.getMonth() + 1
						  )}-${DateInputController.format2DecimalPlaces(date.getDate())}`
						: null;
			},
		});

		minElement.addEventListener("change", () => {
			if (minElement.value === "") {
				field.value.min = null;
			}
		});

		maxElement.addEventListener("change", () => {
			if (maxElement.value === "") {
				field.value.max = null;
			}
		});
	});
</script>

<div class="input-group">
	<span class="input-group-addon">Min</span>
	<input
		type="text"
		{minElementId}
		bind:value={field.minValueAsText}
		required={field.metadata.required}
		{tabindex}
		autocomplete="off"
		class="form-control"
		bind:this={minElement} />

	<span class="input-group-addon">Max</span>

	<input
		type="text"
		{maxElementId}
		bind:value={field.maxValueAsText}
		required={field.metadata.required}
		{tabindex}
		autocomplete="off"
		class="form-control"
		bind:this={maxElement} />
</div>

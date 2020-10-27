import * as umf from "./core/framework";

import { BooleanInputController } from "./core/ui/inputs/BooleanInputController";
import { DateInputController } from "./core/ui/inputs/DateInputController";
import { DateRangeInputController } from "./core/ui/inputs/DateRangeInputController";
import { DropdownInputController } from "./core/ui/inputs/DropdownInputController";
import { EmailInputController } from "./core/ui/inputs/EmailInputController";
import { FileUploaderController } from "./core/ui/inputs/FileUploaderController";
//import { HtmlEditorInputController } from "./core/ui/inputs/HtmlEditorInputController";
import { MultiSelectInputController } from "./core/ui/inputs/MultiSelectInputController";
import { NumberInputController } from "./core/ui/inputs/NumberInputController";
import { NumberObjectInputController } from "./core/ui/inputs/NumberObjectInputController";
import { NumberRangeInputController } from "./core/ui/inputs/NumberRangeInputController";
import { PaginatorInputController } from "./core/ui/inputs/PaginatorInputController";
import { PasswordInputController } from "./core/ui/inputs/PasswordInputController";
import { TextareaInputController } from "./core/ui/inputs/TextareaInputController";
import { TypeaheadInputController } from "./core/ui/inputs/TypeaheadInputController";

import BooleanInput from "./core/ui/inputs/Boolean.svelte";
import DateInput from "./core/ui/inputs/Date.svelte";
import DateRangeInput from "./core/ui/inputs/DateRange.svelte";
import DropdownInput from "./core/ui/inputs/Dropdown.svelte";
import EmailInput from "./core/ui/inputs/Email.svelte";
import FileUploader from "./core/ui/inputs/FileUploader.svelte";
//import HtmlEditor from "./core/ui/inputs/HtmlEditor";
import MultiSelectInput from "./core/ui/inputs/MultiSelect.svelte";
import NumberInput from "./core/ui/inputs/Number.svelte";
import NumberRangeInput from "./core/ui/inputs/NumberRange.svelte";
import ONumberInput from "./core/ui/inputs/ONumber.svelte";
import Password from "./core/ui/inputs/Password.svelte";
import TextInput from "./core/ui/inputs/Text.svelte";
import Textarea from "./core/ui/inputs/Textarea.svelte";
import PieGraph from "./core/ui/outputs/PieGraph.svelte";
import Checklist from "./core/ui/inputs/Checklist.svelte";

import ActionList from "./core/ui/outputs/ActionList.svelte";
import Alert from "./core/ui/outputs/Alert.svelte";
import DateTimeOutput from "./core/ui/outputs/Datetime.svelte";
import Documentation from "./core/ui/outputs/Documentation.svelte";
import DownloadableFile from "./core/ui/outputs/DownloadableFile.svelte";
import FileSize from "./core/ui/outputs/FileSize.svelte";
import FormLink from "./core/ui/outputs/FormLink.svelte";
import HtmlString from "./core/ui/outputs/HtmlString.svelte";
import Image from "./core/ui/outputs/Image.svelte";
import InlineForm from "./core/ui/outputs/InlineForm.svelte";
import Link from "./core/ui/outputs/Link.svelte";
import NumberOutput from "./core/ui/outputs/Number.svelte";
import ObjectList from "./core/ui/outputs/ObjectList.svelte";
import Paginator from "./core/ui/outputs/Paginator.svelte";
import TableOutput from "./core/ui/outputs/Table.svelte";
import Tabstrip from "./core/ui/outputs/Tabstrip.svelte";
import TextOutput from "./core/ui/outputs/Text.svelte";
import TextValue from "./core/ui/outputs/TextValue.svelte";
import TextValueMultiline from "./core/ui/outputs/TextValueMultiline.svelte";
import ListOutput from "./core/ui/outputs/List.svelte";

import {
	BindToOutput,
	DependOn,
	FormLogToConsole,
	InputLogToConsole,
	OutputLogToConsole,
	ReloadFormAfterAction
} from "./core/eventHandlers";

import { Growl, Redirect } from "./core/functions";


const controlRegister = new umf.ControlRegister();
controlRegister.registerInputFieldControl("text", TextInput, umf.StringInputController);
controlRegister.registerInputFieldControl("email", EmailInput, EmailInputController);
controlRegister.registerInputFieldControl("datetime", DateInput, DateInputController);
controlRegister.registerInputFieldControl("date-range", DateRangeInput, DateRangeInputController);
controlRegister.registerInputFieldControl("number", NumberInput, NumberInputController);
controlRegister.registerInputFieldControl("onumber", ONumberInput, NumberObjectInputController);
controlRegister.registerInputFieldControl("dropdown", DropdownInput, DropdownInputController);
controlRegister.registerInputFieldControl("boolean", BooleanInput, BooleanInputController);
controlRegister.registerInputFieldControl("paginator", null, PaginatorInputController);
controlRegister.registerInputFieldControl("typeahead", MultiSelectInput, TypeaheadInputController);
controlRegister.registerInputFieldControl("my-typeahead", MultiSelectInput, TypeaheadInputController);
controlRegister.registerInputFieldControl("multiselect", MultiSelectInput, MultiSelectInputController);

controlRegister.registerInputFieldControl("password", Password, PasswordInputController);
controlRegister.registerInputFieldControl("textarea", Textarea, TextareaInputController, new umf.OutputControlConfiguration(false, true));
controlRegister.registerInputFieldControl("file-uploader", FileUploader, FileUploaderController, new umf.OutputControlConfiguration(false, true));
controlRegister.registerInputFieldControl("number-range", NumberRangeInput, NumberRangeInputController);
//controlRegister.registerInputFieldControl("html-editor", HtmlEditor, HtmlEditorInputController, new umf.OutputControlConfiguration(false, true));

controlRegister.registerOutputFieldControl("text", TextOutput);
controlRegister.registerOutputFieldControl("number", NumberOutput);
controlRegister.registerOutputFieldControl("datetime", DateTimeOutput);
controlRegister.registerOutputFieldControl("table", TableOutput, new umf.OutputControlConfiguration(false, true));
controlRegister.registerOutputFieldControl("formlink", FormLink);
controlRegister.registerOutputFieldControl("tabstrip", Tabstrip, new umf.OutputControlConfiguration(true, true));
controlRegister.registerOutputFieldControl("paginated-data", Paginator, new umf.OutputControlConfiguration(false, true));
controlRegister.registerOutputFieldControl("action-list", ActionList, new umf.OutputControlConfiguration(true, true));
controlRegister.registerOutputFieldControl("inline-form", InlineForm, new umf.OutputControlConfiguration(true, true));
controlRegister.registerOutputFieldControl("text-value", TextValue);
controlRegister.registerOutputFieldControl("text-value-multiline", TextValueMultiline, new umf.OutputControlConfiguration(false, true));
controlRegister.registerOutputFieldControl("downloadable-file", DownloadableFile);
controlRegister.registerOutputFieldControl("alert", Alert, new umf.OutputControlConfiguration(true, true));
controlRegister.registerOutputFieldControl("file-size", FileSize);
controlRegister.registerOutputFieldControl("image", Image, new umf.OutputControlConfiguration(false, true));
controlRegister.registerOutputFieldControl("link", Link);
controlRegister.registerOutputFieldControl("object-list", ObjectList, new umf.OutputControlConfiguration(false, true));
controlRegister.registerOutputFieldControl("html-string", HtmlString);
controlRegister.registerOutputFieldControl("documentation", Documentation, new umf.OutputControlConfiguration(true, true));
controlRegister.registerOutputFieldControl("pie-graph", PieGraph, new umf.OutputControlConfiguration(false, true));
controlRegister.registerOutputFieldControl("list", ListOutput);


// Form event handlers.
controlRegister.registerFormEventHandler("log-to-console", new FormLogToConsole());
controlRegister.registerFormEventHandler("reload-form-after-action", new ReloadFormAfterAction());

// Input event handlers.
controlRegister.registerInputFieldEventHandler("bind-to-output", new BindToOutput());
controlRegister.registerInputFieldEventHandler("log-to-console", new InputLogToConsole());
controlRegister.registerInputFieldEventHandler("depend-on", new DependOn());

// Output event handlers.
controlRegister.registerOutputFieldEventHandler("log-to-console", new OutputLogToConsole());

// Functions.
controlRegister.registerFunction("growl", new Growl());
controlRegister.registerFunction("redirect", new Redirect());

export default controlRegister;

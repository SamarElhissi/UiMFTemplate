namespace UiMFTemplate.Web.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Cors;
	using Microsoft.AspNetCore.Mvc;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Excel;
	using UiMFTemplate.Excel.Renderer;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;

	[Route("api/form")]
	[EnableCors(Startup.CorsAllowAllPolicy)]
	public class FormController : Controller
	{
		private const string ContentType = "application/json";
		private readonly FormRegister formRegister;
		private readonly IMediator mediator;

		public FormController(IMediator mediator, FormRegister formRegister)
		{
			this.mediator = mediator;
			this.formRegister = formRegister;
		}

		[HttpGet("{form}/{field}/exportToExcel")]
		public async Task<FileResult> ExportToExcel(string form, string field)
		{
			var urlDecode = this.Request.Query.ToJObject();

			var request = new InvokeForm.Request
			{
				Form = form,
				InputFieldValues = urlDecode
			};
			var data = await this.mediator.Send(request);

			var metadata = this.Metadata(form);

			var filename = $"{form}-{DateTime.Today:dd.MM.yyyy}";

			var excelFile = new
					ExcelEngine(new AppExcelColumnRenderer())
				.BuildExcelFile(data.Data, metadata, field);

			return this.File(excelFile.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename + ".xlsx");
		}

		[HttpGet("menu")]
		public MyMenu.Response Menu()
		{
			this.Response.ContentType = ContentType;
			this.Response.Headers["Access-Control-Allow-Origin"] = "*";

			return this.mediator.Send(new MyMenu.Request
			{
				IncludeWithContextBasedSecurity = true
			}).Result;
		}

		[HttpGet("metadata/{id}")]
		public FormMetadata Metadata(string id)
		{
			this.Response.ContentType = ContentType;
			return this.formRegister.GetFormInfo(id)?.Metadata;
		}

		[HttpGet("metadata")]
		public MyForms.Response Metadata()
		{
			this.Response.ContentType = ContentType;
			this.Response.Headers["Access-Control-Allow-Origin"] = "*";

			return this.mediator.Send(new MyForms.Request
			{
				IncludeWithContextBasedSecurity = true
			}).Result;
		}

		[HttpPost("run")]
		public async Task<List<InvokeForm.Response>> Run([FromBody] InvokeForm.Request[] requests)
		{
			var results = new List<InvokeForm.Response>();
			foreach (var request in requests)
			{
				var response = await this.mediator.Send(request);
				results.Add(new InvokeForm.Response
				{
					RequestId = request.RequestId,
					Data = response.Data
				});
			}

			this.Response.ContentType = ContentType;
			this.Response.Headers["Access-Control-Allow-Origin"] = "*";

			return results;
		}
	}
}
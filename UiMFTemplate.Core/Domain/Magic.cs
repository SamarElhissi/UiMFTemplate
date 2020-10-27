// ReSharper disable UnusedAutoPropertyAccessor.Local

// ReSharper disable UnusedMember.Local

namespace UiMFTemplate.Core.Domain
{
	using System;
	using UiMetadataFramework.Basic.Output;
	using UiMFTemplate.Core.Commands;
	using UiMFTemplate.Core.Domain.Enum;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Security;

	public class Magic : IDeletable
	{
		private Magic()
		{
		}

		public Magic(string title, string details, int createdByUserId)
		{
			this.Title = title;
			this.Details = details;
			this.CreatedByUserId = createdByUserId;
			this.CreatedOn = DateTime.Now;
			this.IsDeleted = false;
			this.Status = MagicStatus.Draft;
		}

		public bool CanClose => this.Status == MagicStatus.Submitted;

		public bool CanEdit => this.Status == MagicStatus.Draft;

		public DateTime? ClosedOn { get; private set; }
		public virtual RegisteredUser CreatedByUser { get; private set; }
		public int CreatedByUserId { get; private set; }
		public DateTime CreatedOn { get; private set; }
		public string Details { get; private set; }
		public int Id { get; private set; }
		public MagicStatus Status { get; private set; }
		public DateTime? SubmittedOn { get; private set; }
		public string Title { get; private set; }
		public bool IsDeleted { get; set; }

		public void Close()
		{
			this.Status = MagicStatus.Closed;
			this.ClosedOn = DateTime.Now;
		}

		public void Delete()
		{
			this.IsDeleted = true;
		}

		public void Edit(string title, string details)
		{
			this.Title = title;
			this.Details = details;
		}

		public ActionList GetActions(UserSecurityContext permissionManager)
		{
			var result = new ActionList();

			if (this.CanEdit)
			{
				if (permissionManager.CanAccess<EditMagic>())
				{
					result.Actions.Add(EditMagic.Button(this.Id, UiFormConstants.EditLabel)
						.WithCustomUi(LinkStyle.DefaultSmall));
				}

				if (permissionManager.CanAccess<DeleteMagic>())
				{
					result.Actions.Add(DeleteMagic.Button(this.Id, UiFormConstants.DeleteLabel)
						.WithCustomUi(LinkStyle.DangerSmall));
				}

				if (permissionManager.CanAccess<SubmitMagic>())
				{
					result.Actions.Add(SubmitMagic.Button(this.Id)
						.WithCustomUi(LinkStyle.SuccessSmall));
				}
			}

			if (this.CanClose)
			{
				if (permissionManager.CanAccess<CloseMagic>())
				{
					result.Actions.Add(CloseMagic.Button(this.Id)
						.WithCustomUi(LinkStyle.SuccessSmall));
				}
			}

			return result;
		}

		public Alert GetStatus()
		{
			var alert = new Alert(this.Status.ToString());
			switch (this.Status)
			{
				case MagicStatus.Draft:
					alert.Message = this.CreatedOn.ToString("D");
					break;
				case MagicStatus.Submitted:
					alert.Style = AlertStyle.Info;
					alert.Message = this.SubmittedOn?.ToString("D");
					break;
				case MagicStatus.Closed:
					alert.Style = AlertStyle.Success;
					alert.Message = this.ClosedOn?.ToString("D");
					break;
			}

			return alert;
		}

		public void Submit()
		{
			this.Status = MagicStatus.Submitted;
			this.SubmittedOn = DateTime.Now;
		}
	}
}
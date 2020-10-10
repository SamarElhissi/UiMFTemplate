using System;
using System.Collections.Generic;
using System.Text;

namespace UiMFTemplate.Core.Domain
{
  public interface IDeletable
  {
    bool IsDeleted {get; set; }
  }
}

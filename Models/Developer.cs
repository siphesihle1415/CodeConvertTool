using System;
using System.Collections.Generic;

namespace CodeConverterTool.Models;

public partial class Developer
{
    public int DevId { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<Script> Scripts { get; set; } = new List<Script>();
}

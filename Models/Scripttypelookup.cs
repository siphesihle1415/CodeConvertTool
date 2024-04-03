using System;
using System.Collections.Generic;

namespace CodeConverterTool.Models;

public partial class Scripttypelookup
{
    public int TypeId { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Script> Scripts { get; set; } = new List<Script>();
}

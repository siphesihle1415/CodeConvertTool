using System;
using System.Collections.Generic;

namespace CodeConverterTool.Models;

public partial class Script
{
    public int ScriptId { get; set; }

    public int DevId { get; set; }

    public string ScriptName { get; set; } = null!;

    public string ScriptS3Uri { get; set; } = null!;

    public int ScriptType { get; set; }

    public int ScriptVersion { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Developer Dev { get; set; } = null!;

    public virtual Scripttypelookup ScriptTypeNavigation { get; set; } = null!;
}

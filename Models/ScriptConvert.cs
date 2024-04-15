using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CodeConverterTool.Models;

public class ScriptConvert
{
  public string? Model { get; set; }
  public string? TargetScript { get; set; }
  public string? SourceScript { get; set; }
  public string? Content { get; set; }
  public int MaxTokens { get; set; }
}

public class ScriptConvertGemini
{
    [JsonPropertyName("target")]
    public string? Target { get; set; }
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class Part
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part>? Parts { get; set; }
}
public class GenerateContentRequest
{
    [JsonPropertyName("contents")]
    public List<Content>? Contents { get; set; }
}

public class GenerateContentResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate>? Candidates { get; set; }
}

public class Candidate
{
    [JsonPropertyName("content")]
    public Content? Content { get; set; }
}
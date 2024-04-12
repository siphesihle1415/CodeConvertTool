namespace CodeConverterTool.Models;

public class ScriptConvert
{
  public string Model { get; set; }
  public string TargetScript { get; set; }
  public string SourceScript { get; set; }
  public string Content { get; set; }
  public  int MaxTokens { get; set; }
}
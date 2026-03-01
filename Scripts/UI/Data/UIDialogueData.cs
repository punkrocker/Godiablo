namespace Diablo.UI.Data;

/// <summary>
/// UI对话数据
/// </summary>
public class UIDialogueData
{
    public string SpeakerName { get; set; } = "";
    public string Text { get; set; } = "";
    public string[] Choices { get; set; } = System.Array.Empty<string>();
    public bool HasChoices => Choices != null && Choices.Length > 0;
}


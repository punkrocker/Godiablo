using Godot;
using Diablo.NPC.Dialogue;

namespace Diablo.UI.Controllers;

/// <summary>
/// 对话UI控制器
/// 显示对话框、说话人名、对话文本和选项按钮
/// </summary>
public partial class DialogueUIController : Control
{
    [Export] public Label SpeakerLabel { get; set; }
    [Export] public RichTextLabel DialogueText { get; set; }
    [Export] public VBoxContainer ChoicesContainer { get; set; }
    [Export] public PackedScene ChoiceButtonScene { get; set; }
    [Export] public Button ContinueButton { get; set; }

    public override void _Ready()
    {
        Visible = false;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.DialogueNodeDisplayed += OnDialogueNodeDisplayed;
            DialogueManager.Instance.DialogueEnded += OnDialogueEnded;
        }

        if (ContinueButton != null)
        {
            ContinueButton.Pressed += OnContinuePressed;
        }
    }

    private void OnDialogueNodeDisplayed(string speaker, string text, string[] choices)
    {
        Visible = true;

        if (SpeakerLabel != null)
            SpeakerLabel.Text = speaker;

        if (DialogueText != null)
            DialogueText.Text = text;

        // 清除旧选项
        if (ChoicesContainer != null)
        {
            foreach (var child in ChoicesContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (choices.Length > 0)
        {
            if (ContinueButton != null) ContinueButton.Visible = false;

            for (int i = 0; i < choices.Length; i++)
            {
                CreateChoiceButton(i, choices[i]);
            }
        }
        else
        {
            if (ContinueButton != null) ContinueButton.Visible = true;
        }
    }

    private void CreateChoiceButton(int index, string text)
    {
        if (ChoicesContainer == null) return;

        Button button;
        if (ChoiceButtonScene != null)
        {
            button = ChoiceButtonScene.Instantiate<Button>();
        }
        else
        {
            button = new Button();
        }

        button.Text = $"{index + 1}. {text}";
        int capturedIndex = index;
        button.Pressed += () => OnChoiceSelected(capturedIndex);
        ChoicesContainer.AddChild(button);
    }

    private void OnChoiceSelected(int index)
    {
        DialogueManager.Instance?.SelectChoice(index);
    }

    private void OnContinuePressed()
    {
        DialogueManager.Instance?.AdvanceDialogue();
    }

    private void OnDialogueEnded()
    {
        Visible = false;
    }
}


using Godot;
using Diablo.UI.Data;

namespace Diablo.UI.Controllers;

/// <summary>
/// 工具提示UI控制器
/// 鼠标悬停时显示物品/技能详细信息
/// </summary>
public partial class TooltipUIController : Control
{
    [Export] public Panel TooltipPanel { get; set; }
    [Export] public Label TitleLabel { get; set; }
    [Export] public Label TypeLabel { get; set; }
    [Export] public RichTextLabel DescriptionLabel { get; set; }
    [Export] public VBoxContainer StatsContainer { get; set; }
    [Export] public Label ValueLabel { get; set; }
    [Export] public Label WeightLabel { get; set; }

    public override void _Ready()
    {
        Hide();
    }

    public override void _Process(double delta)
    {
        if (Visible && TooltipPanel != null)
        {
            // 跟随鼠标
            var mousePos = GetViewport().GetMousePosition();
            TooltipPanel.GlobalPosition = mousePos + new Vector2(15, 15);

            // 确保不超出屏幕
            var screenSize = GetViewportRect().Size;
            var panelSize = TooltipPanel.Size;
            if (mousePos.X + panelSize.X + 15 > screenSize.X)
            {
                TooltipPanel.GlobalPosition = new Vector2(
                    mousePos.X - panelSize.X - 15,
                    TooltipPanel.GlobalPosition.Y);
            }
        }
    }

    /// <summary>
    /// 显示工具提示
    /// </summary>
    public void ShowTooltip(UITooltipData data)
    {
        if (data == null) return;

        if (TitleLabel != null)
        {
            TitleLabel.Text = data.Title;
            TitleLabel.AddThemeColorOverride("font_color", new UIItemSlotData { Rarity = data.Rarity }.GetRarityColor());
        }

        if (TypeLabel != null)
            TypeLabel.Text = data.TypeLabel;

        if (DescriptionLabel != null)
            DescriptionLabel.Text = string.Join("\n", data.DescriptionLines);

        // 显示属性加成
        if (StatsContainer != null)
        {
            foreach (var child in StatsContainer.GetChildren())
                child.QueueFree();

            foreach (var stat in data.StatModifiers)
            {
                var label = new Label();
                label.Text = stat;
                label.AddThemeColorOverride("font_color", Colors.Green);
                StatsContainer.AddChild(label);
            }
        }

        if (ValueLabel != null)
            ValueLabel.Text = $"价值: {data.Value}";

        if (WeightLabel != null)
            WeightLabel.Text = $"重量: {data.Weight:F1}";

        Show();
    }

    /// <summary>
    /// 隐藏工具提示
    /// </summary>
    public void HideTooltip()
    {
        Hide();
    }
}


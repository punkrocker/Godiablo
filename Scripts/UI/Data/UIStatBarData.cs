namespace Diablo.UI.Data;

/// <summary>
/// UI属性条数据 - 用于显示HP/MP/耐力等
/// </summary>
public class UIStatBarData
{
    public string Label { get; set; } = "";
    public float CurrentValue { get; set; } = 0f;
    public float MaxValue { get; set; } = 100f;
    public float Percentage => MaxValue > 0 ? CurrentValue / MaxValue : 0f;
}


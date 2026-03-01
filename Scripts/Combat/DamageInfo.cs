using Diablo.Core.Enums;

namespace Diablo.Combat;

/// <summary>
/// 伤害信息数据结构
/// 用于在攻击者与受击者之间传递伤害数据
/// </summary>
public class DamageInfo
{
    /// <summary>攻击来源实体ID</summary>
    public string SourceId { get; set; } = "";

    /// <summary>目标实体ID</summary>
    public string TargetId { get; set; } = "";

    /// <summary>伤害数值</summary>
    public float Amount { get; set; }

    /// <summary>伤害类型</summary>
    public DamageType Type { get; set; } = DamageType.Physical;

    /// <summary>是否暴击</summary>
    public bool IsCritical { get; set; } = false;

    /// <summary>击退力度</summary>
    public float KnockbackForce { get; set; } = 0f;

    public DamageInfo() { }

    public DamageInfo(string sourceId, float amount, DamageType type, bool isCritical = false, float knockback = 0f)
    {
        SourceId = sourceId;
        Amount = amount;
        Type = type;
        IsCritical = isCritical;
        KnockbackForce = knockback;
    }
}


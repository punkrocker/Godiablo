using Godot;
using Diablo.Core.Enums;
using Diablo.Core.Helpers;
using Diablo.Character;

namespace Diablo.Combat;

/// <summary>
/// 伤害计算器 - 静态工具类
/// 根据攻击者属性、武器数据、防御者护甲和抗性计算最终伤害
/// </summary>
public static class DamageCalculator
{
    /// <summary>
    /// 计算完整的攻击伤害
    /// </summary>
    public static DamageInfo CalculateAttackDamage(
        CharacterStats attackerStats,
        float weaponBaseDamage,
        DamageType damageType,
        string sourceId)
    {
        // 基础伤害 = 武器伤害 + 力量/智力加成
        float baseDamage = weaponBaseDamage;

        if (damageType == DamageType.Physical)
        {
            baseDamage += attackerStats.Strength * 0.5f;
        }
        else
        {
            baseDamage += attackerStats.Intelligence * 0.5f;
        }

        // 伤害浮动 (±10%)
        baseDamage *= MathUtils.RandRange(0.9f, 1.1f);

        // 暴击判定
        bool isCrit = MathUtils.RollChance(attackerStats.CriticalChance);
        if (isCrit)
        {
            baseDamage *= attackerStats.CriticalMultiplier;
        }

        // 击退力度
        float knockback = attackerStats.Strength * 0.1f;

        return new DamageInfo(sourceId, baseDamage, damageType, isCrit, knockback);
    }

    /// <summary>
    /// 计算防御减伤
    /// </summary>
    public static float CalculateDefense(float rawDamage, float armor, float resistance)
    {
        // 护甲减伤公式: damage * (100 / (100 + armor))
        float armorReduction = 100f / (100f + armor);
        float afterArmor = rawDamage * armorReduction;

        // 元素抗性减伤
        float afterResistance = afterArmor * (1f - Mathf.Clamp(resistance, 0f, 0.8f));

        return Mathf.Max(afterResistance, 1f); // 最低1点伤害
    }

    /// <summary>
    /// 计算远程伤害（根据距离衰减）
    /// </summary>
    public static float ApplyDistanceFalloff(float damage, float distance, float maxRange)
    {
        if (distance >= maxRange) return 0f;
        float falloff = 1f - (distance / maxRange) * 0.5f;
        return damage * Mathf.Clamp(falloff, 0.5f, 1f);
    }
}


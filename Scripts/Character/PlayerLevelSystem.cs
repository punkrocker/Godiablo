using Godot;
using Diablo.Core.Events;

namespace Diablo.Character;

/// <summary>
/// 玩家等级系统
/// 管理经验值获取、升级判定、属性点分配
/// </summary>
public partial class PlayerLevelSystem : Node
{
    [Export] public CharacterStats Stats { get; set; }

    /// <summary>
    /// 每级获得的可分配属性点数
    /// </summary>
    [Export] public int AttributePointsPerLevel { get; set; } = 5;

    /// <summary>
    /// 经验值曲线系数（XP需求 = BaseXP * Level ^ Exponent）
    /// </summary>
    [Export] public float XPCurveExponent { get; set; } = 1.5f;
    [Export] public int BaseXPRequired { get; set; } = 100;

    public int AvailableAttributePoints { get; private set; } = 0;

    [Signal] public delegate void LeveledUpEventHandler(int newLevel, int attributePoints);

    public override void _Ready()
    {
        GameEvents.OnEntityDeath += OnEntityKilled;
    }

    public override void _ExitTree()
    {
        GameEvents.OnEntityDeath -= OnEntityKilled;
    }

    /// <summary>
    /// 增加经验值
    /// </summary>
    public void AddXP(int amount)
    {
        if (Stats == null) return;

        Stats.CurrentXP += amount;

        while (Stats.CurrentXP >= Stats.XPToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Stats.CurrentXP -= Stats.XPToNextLevel;
        Stats.Level++;
        Stats.XPToNextLevel = CalculateXPForLevel(Stats.Level);

        AvailableAttributePoints += AttributePointsPerLevel;

        // 升级回满生命与法力
        Stats.CurrentHealth = Stats.MaxHealth;
        Stats.CurrentMana = Stats.MaxMana;
        Stats.CurrentStamina = Stats.MaxStamina;

        EmitSignal(SignalName.LeveledUp, Stats.Level, AvailableAttributePoints);
        GameEvents.EmitPlayerLevelUp(Stats.Level);
    }

    /// <summary>
    /// 分配属性点
    /// </summary>
    public bool AllocateAttribute(string attribute)
    {
        if (AvailableAttributePoints <= 0 || Stats == null) return false;

        switch (attribute.ToLower())
        {
            case "strength":
                Stats.Strength++;
                Stats.MaxHealth += 5;
                break;
            case "agility":
                Stats.Agility++;
                Stats.MaxStamina += 3;
                Stats.CriticalChance += 0.005f;
                break;
            case "intelligence":
                Stats.Intelligence++;
                Stats.MaxMana += 5;
                break;
            case "endurance":
                Stats.Endurance++;
                Stats.MaxHealth += 3;
                Stats.BaseArmor += 1;
                break;
            case "luck":
                Stats.Luck++;
                Stats.CriticalChance += 0.01f;
                break;
            default:
                return false;
        }

        AvailableAttributePoints--;
        return true;
    }

    private int CalculateXPForLevel(int level)
    {
        return (int)(BaseXPRequired * Mathf.Pow(level, XPCurveExponent));
    }

    private void OnEntityKilled(string entityId, string killerId)
    {
        // 可以在这里根据击杀的怪物给予经验值
        // 具体经验值由怪物的 XPReward 决定
    }
}


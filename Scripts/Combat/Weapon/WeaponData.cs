using Godot;
using Diablo.Core.Enums;

namespace Diablo.Combat.Weapon;

/// <summary>
/// 武器数据资源
/// 定义武器的基础属性，可在编辑器中创建和配置
/// </summary>
[GlobalClass]
public partial class WeaponData : Resource
{
    [Export] public string WeaponId { get; set; } = "";
    [Export] public string WeaponName { get; set; } = "未命名武器";
    [Export] public float BaseDamage { get; set; } = 10f;
    [Export] public float AttackSpeed { get; set; } = 1.0f;
    [Export] public float AttackRange { get; set; } = 2.0f;
    [Export] public WeaponType Type { get; set; } = WeaponType.Sword;
    [Export] public DamageType PrimaryDamageType { get; set; } = DamageType.Physical;
    [Export] public float StaminaCost { get; set; } = 10f;
    [Export] public float ManaCost { get; set; } = 0f;
    [Export] public float KnockbackForce { get; set; } = 2f;
    [Export] public string ModelPath { get; set; } = "";
}


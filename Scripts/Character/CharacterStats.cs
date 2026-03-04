using Godot;
using Diablo.Core.Enums;
using System.Collections.Generic;

namespace Diablo.Character;

/// <summary>
/// 角色属性资源 - 存储角色的所有数值属性
/// 可作为 Resource 在编辑器中配置，也可在运行时动态修改
/// </summary>
[GlobalClass]
public partial class CharacterStats : Resource
{
	// ===== 等级与经验 =====
	[Export] public int Level { get; set; } = 1;
	[Export] public int CurrentXP { get; set; } = 0;
	[Export] public int XPToNextLevel { get; set; } = 100;

	// ===== 三维属性（生命/法力/耐力） =====
	[Export] public float MaxHealth { get; set; } = 100f;
	[Export] public float CurrentHealth { get; set; } = 100f;

	[Export] public float MaxMana { get; set; } = 50f;
	[Export] public float CurrentMana { get; set; } = 50f;

	[Export] public float MaxStamina { get; set; } = 80f;
	[Export] public float CurrentStamina { get; set; } = 80f;

	// ===== 基础属性 =====
	[Export] public int Strength { get; set; } = 10;
	[Export] public int Agility { get; set; } = 10;
	[Export] public int Intelligence { get; set; } = 10;
	[Export] public int Endurance { get; set; } = 10;
	[Export] public int Luck { get; set; } = 5;

	// ===== 再生速率 =====
	[Export] public float HealthRegenRate { get; set; } = 1f;
	[Export] public float ManaRegenRate { get; set; } = 2f;
	[Export] public float StaminaRegenRate { get; set; } = 5f;

	// ===== 战斗相关 =====
	[Export] public float BaseArmor { get; set; } = 0f;
	[Export] public float CriticalChance { get; set; } = 0.05f;
	[Export] public float CriticalMultiplier { get; set; } = 1.5f;

	/// <summary>
	/// 元素抗性字典 (DamageType => 抗性百分比 0~1)
	/// </summary>
	public Dictionary<DamageType, float> Resistances { get; set; } = new()
	{
		{ DamageType.Fire, 0f },
		{ DamageType.Frost, 0f },
		{ DamageType.Shock, 0f },
		{ DamageType.Poison, 0f },
		{ DamageType.Magic, 0f },
	};

	public bool IsAlive => CurrentHealth > 0;

	/// <summary>
	/// 修改当前生命值，自动限制在[0, MaxHealth]
	/// </summary>
	public void ModifyHealth(float amount)
	{
		CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
	}

	/// <summary>
	/// 修改当前法力值
	/// </summary>
	public void ModifyMana(float amount)
	{
		CurrentMana = Mathf.Clamp(CurrentMana + amount, 0, MaxMana);
	}

	/// <summary>
	/// 修改当前耐力值
	/// </summary>
	public void ModifyStamina(float amount)
	{
		CurrentStamina = Mathf.Clamp(CurrentStamina + amount, 0, MaxStamina);
	}

	/// <summary>
	/// 处理自然回复
	/// </summary>
	public void Regenerate(float delta)
	{
		ModifyHealth(HealthRegenRate * delta);
		ModifyMana(ManaRegenRate * delta);
		ModifyStamina(StaminaRegenRate * delta);
	}

	/// <summary>
	/// 创建一份属性副本
	/// </summary>
	public CharacterStats Duplicate()
	{
		return (CharacterStats)this.MemberwiseClone();
	}
}

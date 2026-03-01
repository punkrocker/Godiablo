using Godot;
using Diablo.Character;

namespace Diablo.Combat.Weapon;

/// <summary>
/// 武器基类
/// 处理武器的装备/卸下、攻击动画触发、HitBox管理
/// </summary>
public partial class WeaponBase : Node3D
{
    [Export] public WeaponData Data { get; set; }

    protected HitBox WeaponHitBox;
    protected CharacterBase Wielder;
    protected AnimationPlayer WeaponAnimPlayer;
    protected bool IsAttacking = false;
    protected float AttackCooldown = 0f;

    public override void _Ready()
    {
        WeaponHitBox = GetNodeOrNull<HitBox>("HitBox");
        WeaponAnimPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        Wielder = GetParentOrNull<CharacterBase>();

        WeaponHitBox?.Deactivate();
    }

    public override void _Process(double delta)
    {
        if (AttackCooldown > 0)
        {
            AttackCooldown -= (float)delta;
        }
    }

    /// <summary>
    /// 装备武器
    /// </summary>
    public virtual void Equip(CharacterBase wielder)
    {
        Wielder = wielder;
        Visible = true;
    }

    /// <summary>
    /// 卸下武器
    /// </summary>
    public virtual void Unequip()
    {
        Wielder = null;
        Visible = false;
        WeaponHitBox?.Deactivate();
    }

    /// <summary>
    /// 执行攻击
    /// </summary>
    public virtual bool Attack()
    {
        if (IsAttacking || AttackCooldown > 0 || Wielder == null || Data == null) return false;

        // 检查耐力消耗
        if (Wielder.Stats.CurrentStamina < Data.StaminaCost) return false;

        IsAttacking = true;
        Wielder.Stats.ModifyStamina(-Data.StaminaCost);

        // 创建伤害信息
        var damageInfo = DamageCalculator.CalculateAttackDamage(
            Wielder.Stats, Data.BaseDamage, Data.PrimaryDamageType, Wielder.EntityId);
        damageInfo.KnockbackForce = Data.KnockbackForce;

        WeaponHitBox?.Activate(damageInfo);
        WeaponAnimPlayer?.Play("attack");

        AttackCooldown = 1f / Data.AttackSpeed;

        // 延迟关闭HitBox
        GetTree().CreateTimer(0.3).Timeout += OnAttackEnd;

        return true;
    }

    protected virtual void OnAttackEnd()
    {
        IsAttacking = false;
        WeaponHitBox?.Deactivate();
    }
}


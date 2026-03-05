using Diablo.Character;
using Godot;
using Diablo.Core.Enums;
using Diablo.Core.Events;
using Diablo.Combat;
using Diablo.Scripts.Character;

namespace Diablo.Character;

/// <summary>
/// 角色基类 - 所有可战斗实体（玩家、敌人、NPC）的抽象基类
/// 继承自 CharacterBody3D，提供移动、受伤、死亡等基础功能
/// </summary>
public abstract partial class CharacterBase : CharacterBody3D
{
    [Export] public CharacterStats Stats { get; set; }
    [Export] public float MoveSpeed { get; set; } = 5.0f;
    [Export] public float Gravity { get; set; } = 9.8f;
    [Export] public string EntityId { get; set; } = "";

    [Signal] public delegate void DamageTakenEventHandler(float amount, int damageType);
    [Signal] public delegate void HealedEventHandler(float amount);
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void StatsChangedEventHandler();

    protected AnimationPlayer AnimPlayer;
    protected bool IsDead;

    public override void _Ready()
    {
        if (Stats == null)
        {
            Stats = new CharacterStats();
        }

        AnimPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

        if (string.IsNullOrEmpty(EntityId))
        {
            EntityId = Name;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsDead) return;

        // 应用重力
        if (!IsOnFloor())
        {
            var vel = Velocity;
            vel.Y -= Gravity * (float)delta;
            Velocity = vel;
        }

        // 自然回复
        Stats.Regenerate((float)delta);
        EmitSignal(SignalName.StatsChanged);
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead) return;

        float resistance = 0f;
        if (Stats.Resistances != null)
        {
            Stats.Resistances.TryGetValue(damageInfo.Type, out resistance);
        }
        float finalDamage = DamageCalculator.CalculateDefense(damageInfo.Amount, Stats.BaseArmor, resistance);

        Stats.ModifyHealth(-finalDamage);
        // 如果是玩家，立即广播血量变化以便 HUD 立刻更新
        if (this is PlayerCharacter)
        {
            GameEvents.EmitPlayerStatsChanged(StatType.Health, Stats.CurrentHealth, Stats.MaxHealth);
        }
        EmitSignal(SignalName.DamageTaken, finalDamage, (int)damageInfo.Type);
        EmitSignal(SignalName.StatsChanged);

        GameEvents.EmitCombatHit(damageInfo.SourceId, EntityId, finalDamage, damageInfo.Type);

        if (!Stats.IsAlive)
        {
            Die(damageInfo.SourceId);
        }
    }

    /// <summary>
    /// 治疗
    /// </summary>
    public virtual void Heal(float amount)
    {
        if (IsDead) return;

        Stats.ModifyHealth(amount);
        EmitSignal(SignalName.Healed, amount);
        EmitSignal(SignalName.StatsChanged);
    }

    /// <summary>
    /// 死亡处理
    /// </summary>
    protected virtual void Die(string killerId)
    {
        IsDead = true;
        EmitSignal(SignalName.Died);
        GameEvents.EmitEntityDeath(EntityId, killerId);

        AnimPlayer?.Play("death");
    }

    /// <summary>
    /// 应用移动向量
    /// </summary>
    protected void ApplyMovement(Vector3 direction, float speed, float delta)
    {
        if (direction != Vector3.Zero)
        {
            var vel = Velocity;
            vel.X = direction.X * speed;
            vel.Z = direction.Z * speed;
            Velocity = vel;
        }
        else
        {
            var vel = Velocity;
            vel.X = Mathf.MoveToward(vel.X, 0, speed * delta);
            vel.Z = Mathf.MoveToward(vel.Z, 0, speed * delta);
            Velocity = vel;
        }

        MoveAndSlide();
    }
}

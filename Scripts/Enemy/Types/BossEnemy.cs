using Godot;
using Diablo.Core.Enums;

namespace Diablo.Enemy.Types;

/// <summary>
/// Boss敌人 - 具有多阶段战斗、特殊技能和增强掉落
/// </summary>
public partial class BossEnemy : EnemyBase
{
    [Export] public int TotalPhases { get; set; } = 3;
    [Export] public string BossName { get; set; } = "无名Boss";

    private int _currentPhase = 1;

    /// <summary>每个阶段对应的生命值百分比阈值</summary>
    private float[] _phaseThresholds;

    [Signal] public delegate void PhaseChangedEventHandler(int newPhase);
    [Signal] public delegate void BossDefeatedEventHandler(string bossName);

    public int CurrentPhase => _currentPhase;

    public override void _Ready()
    {
        base._Ready();

        // 计算阶段阈值（如3阶段: 66%, 33%, 0%）
        _phaseThresholds = new float[TotalPhases];
        for (int i = 0; i < TotalPhases; i++)
        {
            _phaseThresholds[i] = 1f - (float)(i + 1) / TotalPhases;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsDead) return;

        CheckPhaseTransition();
    }

    private void CheckPhaseTransition()
    {
        float healthPercent = Stats.CurrentHealth / Stats.MaxHealth;

        for (int i = _currentPhase - 1; i < _phaseThresholds.Length; i++)
        {
            if (healthPercent <= _phaseThresholds[i] && _currentPhase <= i + 1)
            {
                _currentPhase = i + 2;
                if (_currentPhase <= TotalPhases)
                {
                    OnPhaseChange(_currentPhase);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 阶段切换时的处理
    /// </summary>
    protected virtual void OnPhaseChange(int newPhase)
    {
        EmitSignal(SignalName.PhaseChanged, newPhase);
        GD.Print($"[Boss] {BossName} 进入第 {newPhase} 阶段！");

        // 根据阶段增强属性
        switch (newPhase)
        {
            case 2:
                MoveSpeed *= 1.2f;
                Stats.Strength += 5;
                AnimPlayer?.Play("phase2_transition");
                break;
            case 3:
                MoveSpeed *= 1.3f;
                Stats.Strength += 10;
                AttackRange *= 1.5f;
                AnimPlayer?.Play("phase3_transition");
                break;
        }
    }

    /// <summary>
    /// 使用特殊技能
    /// </summary>
    public virtual void UseSpecialAbility()
    {
        if (GetCurrentTarget() == null) return;

        switch (_currentPhase)
        {
            case 1:
                // 阶段1: 重击
                var damageInfo = Combat.DamageCalculator.CalculateAttackDamage(
                    Stats, Stats.Strength * 3f, DamageType.Physical, EntityId);
                damageInfo.KnockbackForce = 8f;
                GetCurrentTarget().TakeDamage(damageInfo);
                break;
            case 2:
                // 阶段2: 范围攻击
                AreaAttack();
                break;
            case 3:
                // 阶段3: 全力一击
                var ultimateInfo = Combat.DamageCalculator.CalculateAttackDamage(
                    Stats, Stats.Strength * 5f, DamageType.Physical, EntityId);
                ultimateInfo.KnockbackForce = 15f;
                GetCurrentTarget().TakeDamage(ultimateInfo);
                break;
        }
    }

    private void AreaAttack()
    {
        var bodies = GetTree().GetNodesInGroup("Player");
        foreach (var body in bodies)
        {
            if (body is Character.CharacterBase target)
            {
                float dist = GlobalPosition.DistanceTo(target.GlobalPosition);
                if (dist <= AttackRange * 2f)
                {
                    var damageInfo = Combat.DamageCalculator.CalculateAttackDamage(
                        Stats, Stats.Strength * 2f, DamageType.Physical, EntityId);
                    target.TakeDamage(damageInfo);
                }
            }
        }
    }

    protected override void Die(string killerId)
    {
        EmitSignal(SignalName.BossDefeated, BossName);
        base.Die(killerId);
    }
}


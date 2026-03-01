using Diablo.Core.Enums;

namespace Diablo.Enemy.AI;

/// <summary>
/// 攻击状态 - 敌人对目标发动攻击，攻击后有冷却时间
/// </summary>
public class AttackState : IEnemyState
{
    private float _attackCooldown = 0f;
    private const float AttackInterval = 1.5f;

    public void Enter(EnemyBase enemy)
    {
        _attackCooldown = 0f;
    }

    public void Execute(EnemyBase enemy, float delta)
    {
        var target = enemy.GetCurrentTarget();

        if (target == null || !target.Stats.IsAlive)
        {
            var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
            sm?.ChangeState(new IdleState());
            return;
        }

        float dist = enemy.GlobalPosition.DistanceTo(target.GlobalPosition);

        // 超出攻击范围，回到追击
        if (dist > enemy.AttackRange * 1.2f)
        {
            var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
            sm?.ChangeState(new ChaseState());
            return;
        }

        // 面向目标
        var lookTarget = target.GlobalPosition;
        lookTarget.Y = enemy.GlobalPosition.Y;
        enemy.LookAt(lookTarget);

        // 攻击冷却
        _attackCooldown -= delta;
        if (_attackCooldown <= 0f)
        {
            PerformAttack(enemy, target);
            _attackCooldown = AttackInterval;
        }
    }

    private void PerformAttack(EnemyBase enemy, Character.CharacterBase target)
    {
        var damageInfo = Combat.DamageCalculator.CalculateAttackDamage(
            enemy.Stats, enemy.Stats.Strength * 1.5f, DamageType.Physical, enemy.EntityId);
        target.TakeDamage(damageInfo);
    }

    public void Exit(EnemyBase enemy) { }
}


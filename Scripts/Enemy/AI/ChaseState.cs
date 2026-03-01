namespace Diablo.Enemy.AI;

/// <summary>
/// 追击状态 - 敌人追踪目标，进入攻击范围后切换到攻击状态
/// </summary>
public class ChaseState : IEnemyState
{
    private float _lostTargetTimer = 0f;
    private const float LostTargetTimeout = 5f;

    public void Enter(EnemyBase enemy)
    {
        _lostTargetTimer = 0f;
    }

    public void Execute(EnemyBase enemy, float delta)
    {
        var target = enemy.GetCurrentTarget();

        if (target == null || !target.Stats.IsAlive)
        {
            _lostTargetTimer += delta;
            if (_lostTargetTimer >= LostTargetTimeout)
            {
                enemy.LoseTarget();
                var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
                sm?.ChangeState(new IdleState());
            }
            return;
        }

        _lostTargetTimer = 0f;
        float dist = enemy.GlobalPosition.DistanceTo(target.GlobalPosition);

        // 进入攻击范围
        if (dist <= enemy.AttackRange)
        {
            var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
            sm?.ChangeState(new AttackState());
            return;
        }

        // 导航追踪
        enemy.NavigateToTarget(delta);
    }

    public void Exit(EnemyBase enemy) { }
}


using Godot;

namespace Diablo.Enemy.AI;

/// <summary>
/// 空闲状态 - 敌人原地待命，播放闲置动画，检测玩家
/// </summary>
public class IdleState : IEnemyState
{
    private float _idleTimer = 0f;
    private float _idleDuration = 3f;

    public void Enter(EnemyBase enemy)
    {
        _idleTimer = 0f;
        _idleDuration = GD.Randf() * 3f + 2f; // 2~5秒随机闲置
    }

    public void Execute(EnemyBase enemy, float delta)
    {
        _idleTimer += delta;

        // 如果发现目标，切换到追击
        if (enemy.GetCurrentTarget() != null)
        {
            var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
            sm?.ChangeState(new ChaseState());
            return;
        }

        // 闲置时间到，切换到巡逻
        if (_idleTimer >= _idleDuration)
        {
            var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
            sm?.ChangeState(new PatrolState());
        }
    }

    public void Exit(EnemyBase enemy) { }
}


using Godot;
using System.Collections.Generic;

namespace Diablo.Enemy.AI;

/// <summary>
/// 巡逻状态 - 敌人在预设路径点之间巡逻
/// </summary>
public class PatrolState : IEnemyState
{
    private List<Vector3> _patrolPoints = new();
    private int _currentPointIndex = 0;
    private float _waitTimer = 0f;
    private bool _isWaiting = false;
    private const float ArrivalThreshold = 1.5f;
    private const float WaitTimeAtPoint = 2f;

    public void Enter(EnemyBase enemy)
    {
        // 尝试获取巡逻路径点
        var pathNode = enemy.GetNodeOrNull<Path3D>("PatrolPath");
        if (pathNode != null)
        {
            var curve = pathNode.Curve;
            for (int i = 0; i < curve.PointCount; i++)
            {
                _patrolPoints.Add(pathNode.ToGlobal(curve.GetPointPosition(i)));
            }
        }

        // 如果没有路径点，就在原地附近随机巡逻
        if (_patrolPoints.Count == 0)
        {
            var origin = enemy.GlobalPosition;
            for (int i = 0; i < 3; i++)
            {
                _patrolPoints.Add(origin + new Vector3(
                    GD.Randf() * 10f - 5f, 0, GD.Randf() * 10f - 5f));
            }
        }
    }

    public void Execute(EnemyBase enemy, float delta)
    {
        // 发现目标时切换到追击
        if (enemy.GetCurrentTarget() != null)
        {
            var sm = enemy.GetNodeOrNull<EnemyStateMachine>("StateMachine");
            sm?.ChangeState(new ChaseState());
            return;
        }

        if (_isWaiting)
        {
            _waitTimer += delta;
            if (_waitTimer >= WaitTimeAtPoint)
            {
                _isWaiting = false;
                _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Count;
            }
            return;
        }

        if (_patrolPoints.Count == 0) return;

        var target = _patrolPoints[_currentPointIndex];
        var dist = enemy.GlobalPosition.DistanceTo(target);

        if (dist <= ArrivalThreshold)
        {
            _isWaiting = true;
            _waitTimer = 0f;
        }
        else
        {
            var direction = (target - enemy.GlobalPosition).Normalized();
            direction.Y = 0;

            var vel = enemy.Velocity;
            vel.X = direction.X * enemy.MoveSpeed * 0.5f;
            vel.Z = direction.Z * enemy.MoveSpeed * 0.5f;
            enemy.Velocity = vel;
            enemy.MoveAndSlide();

            if (direction != Vector3.Zero)
            {
                enemy.LookAt(enemy.GlobalPosition + direction);
            }
        }
    }

    public void Exit(EnemyBase enemy) { }
}


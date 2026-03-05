using Godot;
using Diablo.Character;
using Diablo.Loot;
using Diablo.Scripts.Character;

namespace Diablo.Enemy;

/// <summary>
/// 敌人基类
/// 扩展 CharacterBase，添加仇恨系统、导航、经验奖励和战利品表
/// </summary>
public partial class EnemyBase : CharacterBase
{
    [Export] public float AggroRange { get; set; } = 15f;
    [Export] public float AttackRange { get; set; } = 2f;
    [Export] public float DeaggroRange { get; set; } = 25f;
    [Export] public int XPReward { get; set; } = 50;
    [Export] public LootTable LootTableData { get; set; }

    protected NavigationAgent3D NavAgent;
    protected CharacterBase CurrentTarget;
    protected AI.EnemyStateMachine StateMachine;

    [Signal] public delegate void AggroChangedEventHandler(bool hasTarget);

    public override void _Ready()
    {
        base._Ready();
        NavAgent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
        StateMachine = GetNodeOrNull<AI.EnemyStateMachine>("StateMachine");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsDead) return;

        DetectTarget();
    }

    /// <summary>
    /// 检测玩家目标
    /// </summary>
    protected virtual void DetectTarget()
    {
        if (CurrentTarget != null)
        {
            float dist = GlobalPosition.DistanceTo(CurrentTarget.GlobalPosition);
            if (dist > DeaggroRange || !CurrentTarget.Stats.IsAlive)
            {
                LoseTarget();
            }
            return;
        }

        // 搜索范围内的玩家
        var bodies = GetTree().GetNodesInGroup("Player");
        foreach (var body in bodies)
        {
            if (body is CharacterBase target && target.Stats.IsAlive)
            {
                float dist = GlobalPosition.DistanceTo(target.GlobalPosition);
                if (dist <= AggroRange)
                {
                    AcquireTarget(target);
                    break;
                }
            }
        }
    }

    public void AcquireTarget(CharacterBase target)
    {
        CurrentTarget = target;
        EmitSignal(SignalName.AggroChanged, true);
    }

    public void LoseTarget()
    {
        CurrentTarget = null;
        EmitSignal(SignalName.AggroChanged, false);
    }

    public CharacterBase GetCurrentTarget() => CurrentTarget;

    /// <summary>
    /// 向目标导航移动
    /// </summary>
    public void NavigateToTarget(float delta)
    {
        if (NavAgent == null || CurrentTarget == null) return;

        NavAgent.TargetPosition = CurrentTarget.GlobalPosition;
        var nextPos = NavAgent.GetNextPathPosition();
        var direction = (nextPos - GlobalPosition).Normalized();
        direction.Y = 0;

        ApplyMovement(direction, MoveSpeed, delta);

        // 面向移动方向
        if (direction != Vector3.Zero)
        {
            LookAt(GlobalPosition + direction);
        }
    }

    protected override void Die(string killerId)
    {
        base.Die(killerId);

        // 掉落战利品
        LootTableData?.DropLoot(GlobalPosition, GetTree());

        // 延迟移除
        GetTree().CreateTimer(3f).Timeout += () => QueueFree();
    }
}


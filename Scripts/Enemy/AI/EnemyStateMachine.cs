using Godot;

namespace Diablo.Enemy.AI;

/// <summary>
/// 敌人AI状态机
/// 管理状态切换和当前状态的执行
/// </summary>
public partial class EnemyStateMachine : Node
{
    private IEnemyState _currentState;
    private EnemyBase _owner;

    [Signal] public delegate void StateChangedEventHandler(string newStateName);

    public override void _Ready()
    {
        _owner = GetParentOrNull<EnemyBase>();
        ChangeState(new IdleState());
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState?.Execute(_owner, (float)delta);
    }

    /// <summary>
    /// 切换AI状态
    /// </summary>
    public void ChangeState(IEnemyState newState)
    {
        _currentState?.Exit(_owner);
        _currentState = newState;
        _currentState?.Enter(_owner);
        EmitSignal(SignalName.StateChanged, newState.GetType().Name);
    }

    public IEnemyState GetCurrentState() => _currentState;
    public string GetCurrentStateName() => _currentState?.GetType().Name ?? "None";
}


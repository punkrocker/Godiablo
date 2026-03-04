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
		// 尝试在父节点链中查找 EnemyBase（允许将 StateMachine 放在子节点层级中）
		_owner = GetParentOrNull<EnemyBase>();
		if (_owner == null)
		{
			Node p = GetParent();
			while (p != null && _owner == null)
			{
				_owner = p as EnemyBase;
				p = p?.GetParent();
			}
		}

		if (_owner == null)
		{
			GD.PrintErr($"{GetPath()}: EnemyStateMachine: could not find EnemyBase in parent chain. State machine will be inactive until an owner is set.");
			return;
		}

		ChangeState(new IdleState());
	}

	public override void _PhysicsProcess(double delta)
	{
		// 如果运行时丢失 owner，尝试再次查找
		if (_owner == null)
		{
			node_try_find_owner:
			_owner = GetParentOrNull<EnemyBase>();
			if (_owner == null)
			{
				Node p = GetParent();
				while (p != null && _owner == null)
				{
					_owner = p as EnemyBase;
					p = p?.GetParent();
				}
			}
			if (_owner == null) return; // 仍然没有 owner，跳过执行
		}

		_currentState?.Execute(_owner, (float)delta);
	}

	/// <summary>
	/// 切换AI状态
	/// </summary>
	public void ChangeState(IEnemyState newState)
	{
		// 只有当 owner 存在时才调用状态的 Exit/Enter，以避免向 null 传递 EnemyBase
		if (_currentState != null && _owner != null)
		{
			_currentState.Exit(_owner);
		}

		_currentState = newState;

		if (_currentState != null && _owner != null)
		{
			_currentState.Enter(_owner);
		}
		else if (_currentState != null)
		{
			GD.PrintErr($"{GetPath()}: ChangeState called but owner is null; Enter not called for state {_currentState.GetType().Name}");
		}

		EmitSignal(SignalName.StateChanged, newState.GetType().Name);
	}

	public IEnemyState GetCurrentState() => _currentState;
	public string GetCurrentStateName() => _currentState?.GetType().Name ?? "None";
}

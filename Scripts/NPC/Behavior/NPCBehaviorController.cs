using Godot;

namespace Diablo.NPC.Behavior;

/// <summary>
/// NPC行为控制器
/// 根据日程表驱动NPC在不同时间段执行不同行为
/// </summary>
public partial class NPCBehaviorController : Node
{
    [Export] public NPCSchedule Schedule { get; set; }
    [Export] public NPCBase NPC { get; set; }
    [Export] public NavigationAgent3D NavAgent { get; set; }

    private ScheduleEntry _currentEntry;
    private bool _hasArrived = false;
    private float _gameTimeOfDay = 8f; // 默认早8点

    /// <summary>设置游戏内时间（0~24）</summary>
    public float GameTimeOfDay
    {
        get => _gameTimeOfDay;
        set
        {
            _gameTimeOfDay = value;
            UpdateSchedule();
        }
    }

    public override void _Ready()
    {
        if (NPC == null) NPC = GetParentOrNull<NPCBase>();
        NavAgent = NPC?.GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
        UpdateSchedule();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (NPC == null || Schedule == null || _currentEntry == null) return;

        if (!_hasArrived)
        {
            NavigateToTarget((float)delta);
        }
    }

    private void UpdateSchedule()
    {
        if (Schedule == null) return;

        var entry = Schedule.GetCurrentEntry(_gameTimeOfDay);
        if (entry != _currentEntry)
        {
            _currentEntry = entry;
            _hasArrived = false;
        }
    }

    private void NavigateToTarget(float delta)
    {
        if (NavAgent == null || _currentEntry == null) return;

        NavAgent.TargetPosition = _currentEntry.TargetPosition;

        if (NavAgent.IsNavigationFinished())
        {
            _hasArrived = true;
            OnArrived();
            return;
        }

        var nextPos = NavAgent.GetNextPathPosition();
        var direction = (nextPos - NPC.GlobalPosition).Normalized();
        direction.Y = 0;

        var vel = NPC.Velocity;
        vel.X = direction.X * 2f; // NPC移动速度较慢
        vel.Z = direction.Z * 2f;
        NPC.Velocity = vel;
        NPC.MoveAndSlide();

        if (direction != Vector3.Zero)
        {
            NPC.LookAt(NPC.GlobalPosition + direction);
        }
    }

    private void OnArrived()
    {
        if (_currentEntry == null) return;

        var animPlayer = NPC.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        if (animPlayer != null && !string.IsNullOrEmpty(_currentEntry.AnimationName))
        {
            animPlayer.Play(_currentEntry.AnimationName);
        }
    }
}


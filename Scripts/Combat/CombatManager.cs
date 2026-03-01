using Godot;
using Diablo.Core.Events;
using Diablo.Core.Enums;

namespace Diablo.Combat;

/// <summary>
/// 战斗管理器（单例/Autoload）
/// 协调战斗流程，处理伤害事件和战斗日志
/// </summary>
public partial class CombatManager : Node
{
    public static CombatManager Instance { get; private set; }

    [Signal] public delegate void CombatLogEntryEventHandler(string message);

    public override void _Ready()
    {
        Instance = this;
        GameEvents.OnCombatHit += OnCombatHit;
        GameEvents.OnEntityDeath += OnEntityDeath;
    }

    public override void _ExitTree()
    {
        GameEvents.OnCombatHit -= OnCombatHit;
        GameEvents.OnEntityDeath -= OnEntityDeath;
    }

    private void OnCombatHit(string attackerId, string targetId, float damage, DamageType damageType)
    {
        string log = $"{attackerId} 对 {targetId} 造成了 {damage:F1} 点 {damageType} 伤害";
        EmitSignal(SignalName.CombatLogEntry, log);
        GD.Print($"[战斗] {log}");
    }

    private void OnEntityDeath(string entityId, string killerId)
    {
        string log = $"{entityId} 被 {killerId} 击杀";
        EmitSignal(SignalName.CombatLogEntry, log);
        GD.Print($"[战斗] {log}");
    }
}


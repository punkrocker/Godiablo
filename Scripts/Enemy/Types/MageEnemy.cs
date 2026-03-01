using Godot;
using Diablo.Combat;
using Diablo.Combat.Projectile;
using Diablo.Core.Enums;

namespace Diablo.Enemy.Types;

/// <summary>
/// 法师敌人 - 如术士、巫妖等
/// 施放元素法术，消耗法力值
/// </summary>
public partial class MageEnemy : EnemyBase
{
    [Export] public PackedScene SpellScene { get; set; }
    [Export] public DamageType SpellElement { get; set; } = DamageType.Fire;
    [Export] public float SpellSpeed { get; set; } = 15f;
    [Export] public float ManaCostPerSpell { get; set; } = 15f;
    [Export] public float CastCooldown { get; set; } = 3f;
    [Export] public float PreferredDistance { get; set; } = 12f;
    [Export] public Node3D SpellSpawnPoint { get; set; }

    private float _castTimer = 0f;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsDead) return;

        _castTimer -= (float)delta;

        var target = GetCurrentTarget();
        if (target == null) return;

        float dist = GlobalPosition.DistanceTo(target.GlobalPosition);

        // 保持距离
        if (dist < PreferredDistance * 0.5f)
        {
            var direction = (GlobalPosition - target.GlobalPosition).Normalized();
            direction.Y = 0;
            ApplyMovement(direction, MoveSpeed, (float)delta);
        }

        // 面向目标
        var lookTarget = target.GlobalPosition;
        lookTarget.Y = GlobalPosition.Y;
        LookAt(lookTarget);

        // 施法
        if (_castTimer <= 0 && Stats.CurrentMana >= ManaCostPerSpell)
        {
            CastSpell();
            _castTimer = CastCooldown;
        }
    }

    private void CastSpell()
    {
        if (SpellScene == null || GetCurrentTarget() == null) return;

        Stats.ModifyMana(-ManaCostPerSpell);

        var spell = SpellScene.Instantiate<SpellProjectile>();
        var spawnPos = SpellSpawnPoint?.GlobalPosition ?? GlobalPosition + Vector3.Up;
        var direction = (GetCurrentTarget().GlobalPosition - spawnPos).Normalized();

        var damageInfo = DamageCalculator.CalculateAttackDamage(
            Stats, Stats.Intelligence * 1.5f, SpellElement, EntityId);

        GetTree().Root.AddChild(spell);
        spell.GlobalPosition = spawnPos;
        spell.Launch(direction, SpellSpeed, damageInfo);

        AnimPlayer?.Play("cast");
    }
}


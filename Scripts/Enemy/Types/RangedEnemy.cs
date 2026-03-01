using Godot;
using Diablo.Combat;
using Diablo.Combat.Projectile;
using Diablo.Core.Enums;

namespace Diablo.Enemy.Types;

/// <summary>
/// 远程敌人 - 如骷髅弓箭手、盗贼等
/// 保持距离攻击，发射投射物
/// </summary>
public partial class RangedEnemy : EnemyBase
{
    [Export] public PackedScene ProjectileScene { get; set; }
    [Export] public float PreferredDistance { get; set; } = 10f;
    [Export] public float ProjectileSpeed { get; set; } = 20f;
    [Export] public float AttackCooldownTime { get; set; } = 2f;
    [Export] public Node3D ProjectileSpawnPoint { get; set; }

    private float _attackTimer = 0f;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsDead) return;

        _attackTimer -= (float)delta;

        var target = GetCurrentTarget();
        if (target == null) return;

        float dist = GlobalPosition.DistanceTo(target.GlobalPosition);

        // 保持距离
        if (dist < PreferredDistance * 0.6f)
        {
            MoveAwayFromTarget((float)delta);
        }

        // 攻击
        if (_attackTimer <= 0 && dist <= AggroRange)
        {
            FireProjectile();
            _attackTimer = AttackCooldownTime;
        }
    }

    private void MoveAwayFromTarget(float delta)
    {
        var target = GetCurrentTarget();
        if (target == null) return;

        var direction = (GlobalPosition - target.GlobalPosition).Normalized();
        direction.Y = 0;
        ApplyMovement(direction, MoveSpeed, delta);
    }

    private void FireProjectile()
    {
        if (ProjectileScene == null || GetCurrentTarget() == null) return;

        var projectile = ProjectileScene.Instantiate<ProjectileBase>();
        var spawnPos = ProjectileSpawnPoint?.GlobalPosition ?? GlobalPosition + Vector3.Up;
        var direction = (GetCurrentTarget().GlobalPosition - spawnPos).Normalized();

        var damageInfo = DamageCalculator.CalculateAttackDamage(
            Stats, Stats.Agility * 1.2f, DamageType.Physical, EntityId);

        GetTree().Root.AddChild(projectile);
        projectile.GlobalPosition = spawnPos;
        projectile.Launch(direction, ProjectileSpeed, damageInfo);

        AnimPlayer?.Play("shoot");
    }
}


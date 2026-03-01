using Godot;

namespace Diablo.Enemy.Types;

/// <summary>
/// 近战敌人 - 如骷髅战士、强盗等
/// 具有冲锋行为，近距离攻击
/// </summary>
public partial class MeleeEnemy : EnemyBase
{
    [Export] public float ChargeSpeed { get; set; } = 8f;
    [Export] public float ChargeDistance { get; set; } = 8f;
    [Export] public float ChargeCooldown { get; set; } = 10f;

    private float _chargeTimer = 0f;
    private bool _isCharging = false;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (IsDead) return;

        _chargeTimer -= (float)delta;

        if (_isCharging)
        {
            ProcessCharge((float)delta);
        }
    }

    /// <summary>
    /// 触发冲锋攻击
    /// </summary>
    public void TriggerCharge()
    {
        if (_chargeTimer > 0 || _isCharging || GetCurrentTarget() == null) return;

        float dist = GlobalPosition.DistanceTo(GetCurrentTarget().GlobalPosition);
        if (dist <= ChargeDistance && dist > AttackRange)
        {
            _isCharging = true;
            AnimPlayer?.Play("charge");
        }
    }

    private void ProcessCharge(float delta)
    {
        if (GetCurrentTarget() == null)
        {
            _isCharging = false;
            return;
        }

        var direction = (GetCurrentTarget().GlobalPosition - GlobalPosition).Normalized();
        direction.Y = 0;

        var vel = Velocity;
        vel.X = direction.X * ChargeSpeed;
        vel.Z = direction.Z * ChargeSpeed;
        Velocity = vel;
        MoveAndSlide();

        float dist = GlobalPosition.DistanceTo(GetCurrentTarget().GlobalPosition);
        if (dist <= AttackRange)
        {
            _isCharging = false;
            _chargeTimer = ChargeCooldown;

            // 冲锋额外伤害
            var damageInfo = Combat.DamageCalculator.CalculateAttackDamage(
                Stats, Stats.Strength * 2f, Core.Enums.DamageType.Physical, EntityId);
            damageInfo.KnockbackForce = 5f;
            GetCurrentTarget().TakeDamage(damageInfo);
        }
    }
}


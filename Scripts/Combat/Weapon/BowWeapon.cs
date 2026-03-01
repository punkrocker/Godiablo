using Godot;
using Diablo.Combat.Projectile;

namespace Diablo.Combat.Weapon;

/// <summary>
/// 弓箭武器
/// 支持蓄力射击机制，蓄力越久伤害越高
/// </summary>
public partial class BowWeapon : WeaponBase
{
    [Export] public PackedScene ArrowScene { get; set; }
    [Export] public float MaxChargeTime { get; set; } = 2.0f;
    [Export] public float MinChargeMultiplier { get; set; } = 0.3f;
    [Export] public float ArrowSpeed { get; set; } = 30f;
    [Export] public Node3D ArrowSpawnPoint { get; set; }

    private bool _isCharging = false;
    private float _chargeTime = 0f;

    public float ChargePercent => Mathf.Clamp(_chargeTime / MaxChargeTime, 0f, 1f);

    /// <summary>
    /// 开始蓄力
    /// </summary>
    public void StartCharge()
    {
        if (Wielder == null || Data == null || IsAttacking) return;
        _isCharging = true;
        _chargeTime = 0f;
        WeaponAnimPlayer?.Play("draw");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_isCharging)
        {
            _chargeTime += (float)delta;
            _chargeTime = Mathf.Min(_chargeTime, MaxChargeTime);
        }
    }

    /// <summary>
    /// 释放射击
    /// </summary>
    public void Release()
    {
        if (!_isCharging || Wielder == null || Data == null) return;

        float chargeMultiplier = Mathf.Lerp(MinChargeMultiplier, 1f, ChargePercent);
        Wielder.Stats.ModifyStamina(-Data.StaminaCost * chargeMultiplier);

        var damageInfo = DamageCalculator.CalculateAttackDamage(
            Wielder.Stats, Data.BaseDamage * chargeMultiplier, Data.PrimaryDamageType, Wielder.EntityId);

        SpawnArrow(damageInfo);

        _isCharging = false;
        _chargeTime = 0f;
        AttackCooldown = 1f / Data.AttackSpeed;
        WeaponAnimPlayer?.Play("release");
    }

    private void SpawnArrow(DamageInfo damageInfo)
    {
        if (ArrowScene == null) return;

        var arrow = ArrowScene.Instantiate<ProjectileBase>();
        var spawnPos = ArrowSpawnPoint?.GlobalPosition ?? GlobalPosition;
        var direction = -Wielder.GlobalTransform.Basis.Z;

        GetTree().Root.AddChild(arrow);
        arrow.GlobalPosition = spawnPos;
        arrow.Launch(direction, ArrowSpeed, damageInfo);
    }

    public override bool Attack()
    {
        // 弓箭使用 StartCharge/Release 流程
        if (!_isCharging)
        {
            StartCharge();
        }
        else
        {
            Release();
        }
        return true;
    }
}


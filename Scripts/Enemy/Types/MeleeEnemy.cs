using Diablo.Character;
using Diablo.Enemy.AI;
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

	public override void _Ready()
	{
		base._Ready();
		if (AnimPlayer == null)
		{
			GD.PrintErr(
				$"AnimationPlayer not found on '{Name}'. Animations like 'attack' or 'cast_spell' will be unavailable.");
		}

		this.MoveSpeed = 3.0f;
		this.Gravity = 9.8f;
		this.EntityId = "melee_enemy_01";
		this.AggroRange = 12.0f;
		this.AttackRange = 1.8f;
		this.DeaggroRange = 22.0f;
		this.XPReward = 50;
		// Stats 通过资源加载res://Data/melee_enemy_stats.tres
		this.Stats = ResourceLoader.Load<CharacterStats>("res://Data/melee_enemy_stats.tres");
	}

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

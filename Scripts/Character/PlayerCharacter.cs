using Diablo.Character;
using Godot;
using Diablo.Core.Events;
using Diablo.Core.Enums;

namespace Diablo.Scripts.Character;

/// <summary>
/// 玩家角色控制器
/// 处理玩家输入、移动、冲刺、潜行、交互等
/// </summary>
public partial class PlayerCharacter : CharacterBase
{
	[Export] public float SprintMultiplier { get; set; } = 1.6f;
	[Export] public float SneakMultiplier { get; set; } = 0.5f;
	[Export] public float StaminaDrainRate { get; set; } = 10f;
	[Export] public float InteractionRange { get; set; } = 3.0f;
	[Export] public float JumpVelocity { get; set; } = 5.0f;
	[Export] public float RotationSmoothSpeed { get; set; } = 3.0f; // radians/sec smoothing


	private Camera3D _camera;
	private RayCast3D _interactionRay;
	private bool _isSprinting;
	private bool _isSneaking;

	public bool IsSprinting => _isSprinting;
	public bool IsSneaking => _isSneaking;

	public override void _Ready()
	{
		base._Ready();
		_camera = GetNodeOrNull<Camera3D>("Camera3D");
		_interactionRay = GetNodeOrNull<RayCast3D>("InteractionRay");

		if (_interactionRay != null)
		{
			_interactionRay.TargetPosition = new Vector3(0, 0, -InteractionRange);
		}

		if (AnimPlayer == null)
		{
			GD.PrintErr(
				$"AnimationPlayer not found on '{Name}'. Animations like 'attack' or 'cast_spell' will be unavailable.");
		}

		this.MoveSpeed = 5.0f;
		this.Gravity = 9.8f;
		this.EntityId = "player";
		// Stats 通过资源加载res://Data/player_stats.tres
		this.Stats = ResourceLoader.Load<CharacterStats>("res://Data/player_stats.tres");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (IsDead) return;

		HandleMovement((float)delta);
		HandleInteraction();
		UpdatePlayerStats();
	}

	public override void _Input(InputEvent @event)
	{
		// 跳跃
		if (InputMap.HasAction(PlayerInputActions.Jump) && @event.IsActionPressed(PlayerInputActions.Jump) &&
			IsOnFloor())
		{
			var vel = Velocity;
			vel.Y = JumpVelocity;
			Velocity = vel;
		}

		// 交互键
		if (InputMap.HasAction(PlayerInputActions.Interact) && @event.IsActionPressed(PlayerInputActions.Interact))
		{
			TryInteract();
		}

		// 攻击
		if (InputMap.HasAction(PlayerInputActions.Attack) && @event.IsActionPressed(PlayerInputActions.Attack))
		{
			PerformAttack();
		}

		// 使用技能/魔法
		if (InputMap.HasAction(PlayerInputActions.CastSpell) && @event.IsActionPressed(PlayerInputActions.CastSpell))
		{
			CastSpell();
		}
	}

	private void HandleMovement(float delta)
	{
		var inputDir = Input.GetVector(
			PlayerInputActions.MoveLeft, PlayerInputActions.MoveRight,
			PlayerInputActions.MoveForward, PlayerInputActions.MoveBackward
		);

		var direction = Vector3.Zero;
		if (_camera != null)
		{
			var camTransform = _camera.GlobalTransform;
			direction = (camTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			direction.Y = 0;
		}
		else
		{
			direction = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
		}

		// 冲刺与潜行
		_isSprinting = InputMap.HasAction(PlayerInputActions.Sprint) &&
					Input.IsActionPressed(PlayerInputActions.Sprint) && Stats.CurrentStamina > 0;
		_isSneaking = InputMap.HasAction(PlayerInputActions.Sneak) && Input.IsActionPressed(PlayerInputActions.Sneak);

		float speed = MoveSpeed;
		if (_isSprinting)
		{
			speed *= SprintMultiplier;
			Stats.ModifyStamina(-StaminaDrainRate * delta);
		}
		else if (_isSneaking)
		{
			speed *= SneakMultiplier;
		}

		// WoW-style facing rules:
		// - When the player is holding the camera rotate button (e.g. RMB), movement strafes relative to camera and the character does NOT auto-rotate.
		// - When not holding camera rotate and there is movement input, the character smoothly rotates to face movement direction.

		bool holdingCameraRotate = Input.IsActionPressed("camera_rotate");

		if (direction != Vector3.Zero)
		{
			if (!holdingCameraRotate)
			{
				// Smoothly rotate towards movement direction (only yaw)
				var flatDir = direction;
				flatDir.Y = 0;
				if (flatDir.Length() > 0.001f)
				{
					// Calculate target yaw based on movement direction
					float targetYaw = Mathf.Atan2(flatDir.X, flatDir.Z);
					float currentYaw = Rotation.Y;
					float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, Mathf.Clamp(delta * RotationSmoothSpeed, 0f, 1f));
					Rotation = new Vector3(0f, newYaw, 0f);
				}
			}
			else
			{
				// holding camera rotate -> do not rotate character; optionally we can keep player facing current rotation
			}
		}

		ApplyMovement(direction, speed, delta);
	}

	private void HandleInteraction()
	{
		if (_interactionRay == null) return;

		if (_interactionRay.IsColliding())
		{
			var collider = _interactionRay.GetCollider();
			if (collider is Node node)
			{
				GameEvents.EmitInteractionAvailable(node.Name, $"按 [E] 交互: {node.Name}");
			}
		}
		else
		{
			GameEvents.EmitInteractionCleared();
		}
	}

	private void TryInteract()
	{
		if (_interactionRay == null || !_interactionRay.IsColliding()) return;

		var collider = _interactionRay.GetCollider();
		// 交互逻辑由具体的可交互对象处理
		if (collider is Node node && node.HasMethod("OnInteract"))
		{
			node.Call("OnInteract", this);
		}
	}

	private void PerformAttack()
	{
		// 由武器系统处理具体攻击逻辑
		if (AnimPlayer != null)
		{
			if (AnimPlayer.HasAnimation("attack"))
			{
				AnimPlayer.Play("attack");
				return;
			}
			else
			{
				GD.PrintErr(
					$"Animation 'attack' not found on '{Name}' AnimationPlayer. Falling back to weapon Attack().");
			}
		}

		// Fallback: try to call Attack on the equipped weapon instance (WeaponSlot/MeleeWeapon)
		var weaponNode = GetNodeOrNull("WeaponSlot/MeleeWeapon");
		if (weaponNode != null && weaponNode.HasMethod("Attack"))
		{
			var res = weaponNode.Call("Attack");
			GD.Print($"Called weapon Attack(), result={res}");
			return;
		}

		// As last fallback, try to call Attack on any child Weapon node
		foreach (var child in GetChildren())
		{
			if (child is Node n && n.HasMethod("Attack"))
			{
				var r = n.Call("Attack");
				GD.Print($"Called child {n.Name}.Attack(), result={r}");
				return;
			}
		}

		GD.PrintErr($"No attack animation or weapon Attack method found for '{Name}'.");
	}

	private void CastSpell()
	{
		// 由法杖/法术系统处理具体施法逻辑
		if (AnimPlayer != null)
		{
			if (AnimPlayer.HasAnimation("cast_spell"))
			{
				AnimPlayer.Play("cast_spell");
			}
			else
			{
				GD.PrintErr($"Animation 'cast_spell' not found on '{Name}' AnimationPlayer. Skipping play.");
			}
		}
	}

	private void UpdatePlayerStats()
	{
		GameEvents.EmitPlayerStatsChanged(StatType.Health, Stats.CurrentHealth, Stats.MaxHealth);
		GameEvents.EmitPlayerStatsChanged(StatType.Mana, Stats.CurrentMana, Stats.MaxMana);
		GameEvents.EmitPlayerStatsChanged(StatType.Stamina, Stats.CurrentStamina, Stats.MaxStamina);
	}
}

using Godot;

namespace Diablo.Combat.Projectile;

/// <summary>
/// 投射物基类
/// 处理飞行、碰撞检测、生命周期管理
/// </summary>
public partial class ProjectileBase : CharacterBody3D
{
    [Export] public float Speed { get; set; } = 20f;
    [Export] public float Lifetime { get; set; } = 5f;
    [Export] public float GravityEffect { get; set; } = 0f;

    protected DamageInfo Damage;
    protected Vector3 Direction;
    protected float AliveTime = 0f;
    protected bool HasHit = false;

    [Signal] public delegate void ProjectileHitEventHandler(Node3D target);

    public override void _Ready()
    {
        // 设置碰撞检测
        var hitBox = GetNodeOrNull<HitBox>("HitBox");
        if (hitBox != null && Damage != null)
        {
            hitBox.Activate(Damage);
            hitBox.HitLanded += OnHitLanded;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (HasHit) return;

        AliveTime += (float)delta;
        if (AliveTime >= Lifetime)
        {
            Destroy();
            return;
        }

        // 应用重力
        Direction.Y -= GravityEffect * (float)delta;

        Velocity = Direction * Speed;
        var collision = MoveAndCollide(Velocity * (float)delta);

        if (collision != null)
        {
            OnCollision(collision);
        }
    }

    /// <summary>
    /// 发射投射物
    /// </summary>
    public virtual void Launch(Vector3 direction, float speed, DamageInfo damageInfo)
    {
        Direction = direction.Normalized();
        Speed = speed;
        Damage = damageInfo;

        // 面向飞行方向
        if (direction != Vector3.Zero)
        {
            LookAt(GlobalPosition + direction);
        }

        var hitBox = GetNodeOrNull<HitBox>("HitBox");
        hitBox?.Activate(damageInfo);
    }

    protected virtual void OnCollision(KinematicCollision3D collision)
    {
        HasHit = true;
        Destroy();
    }

    protected virtual void OnHitLanded(Node3D target)
    {
        HasHit = true;
        EmitSignal(SignalName.ProjectileHit, target);
        Destroy();
    }

    protected virtual void Destroy()
    {
        QueueFree();
    }
}


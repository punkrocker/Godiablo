using Godot;

namespace Diablo.Combat.Projectile;

/// <summary>
/// 箭矢投射物
/// 具有重力弧线，命中后可以插在目标上
/// </summary>
public partial class ArrowProjectile : ProjectileBase
{
    [Export] public bool StickOnImpact { get; set; } = true;
    [Export] public float StickDuration { get; set; } = 5f;

    public override void _Ready()
    {
        base._Ready();
        GravityEffect = 3f; // 箭矢受重力影响
    }

    protected override void OnCollision(KinematicCollision3D collision)
    {
        HasHit = true;

        if (StickOnImpact && collision.GetCollider() is Node3D target)
        {
            // 插在碰撞点
            var hitPos = collision.GetPosition();
            GlobalPosition = hitPos;

            // 如果是角色，将箭矢挂到角色身上
            if (target is CharacterBody3D body)
            {
                Reparent(body);
            }

            // 停止物理模拟
            SetPhysicsProcess(false);

            // 延迟销毁
            GetTree().CreateTimer(StickDuration).Timeout += Destroy;
        }
        else
        {
            Destroy();
        }
    }
}


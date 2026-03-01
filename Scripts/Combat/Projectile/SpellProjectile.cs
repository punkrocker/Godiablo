using Godot;
using Diablo.Character;

namespace Diablo.Combat.Projectile;

/// <summary>
/// 法术投射物
/// 支持元素视觉效果和范围伤害（AoE）
/// </summary>
public partial class SpellProjectile : ProjectileBase
{
    [Export] public bool IsAoE { get; set; } = false;
    [Export] public float AoERadius { get; set; } = 3f;
    [Export] public PackedScene ImpactEffectScene { get; set; }

    public override void _Ready()
    {
        base._Ready();
        GravityEffect = 0f; // 魔法弹不受重力
    }

    protected override void OnCollision(KinematicCollision3D collision)
    {
        HasHit = true;

        if (IsAoE)
        {
            ApplyAoEDamage();
        }

        SpawnImpactEffect();
        Destroy();
    }

    protected override void OnHitLanded(Node3D target)
    {
        if (IsAoE)
        {
            ApplyAoEDamage();
        }

        SpawnImpactEffect();
        HasHit = true;
        EmitSignal(SignalName.ProjectileHit, target);
        Destroy();
    }

    private void ApplyAoEDamage()
    {
        if (Damage == null) return;

        // 获取范围内所有物体
        var spaceState = GetWorld3D().DirectSpaceState;
        var shape = new SphereShape3D { Radius = AoERadius };
        var query = new PhysicsShapeQueryParameters3D
        {
            Shape = shape,
            Transform = GlobalTransform
        };

        var results = spaceState.IntersectShape(query);
        foreach (var result in results)
        {
            if (result["collider"].Obj is CharacterBase target && target.EntityId != Damage.SourceId)
            {
                // AoE伤害随距离衰减
                float distance = GlobalPosition.DistanceTo(target.GlobalPosition);
                float falloff = 1f - (distance / AoERadius);
                var aoeDamage = new DamageInfo(Damage.SourceId, Damage.Amount * falloff, Damage.Type);
                target.TakeDamage(aoeDamage);
            }
        }
    }

    private void SpawnImpactEffect()
    {
        if (ImpactEffectScene == null) return;

        var effect = ImpactEffectScene.Instantiate<Node3D>();
        GetTree().Root.AddChild(effect);
        effect.GlobalPosition = GlobalPosition;
    }
}


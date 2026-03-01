using Godot;
using Diablo.Core.Enums;
using Diablo.Combat.Projectile;

namespace Diablo.Combat.Weapon;

/// <summary>
/// 法杖武器
/// 支持不同法术选择，消耗法力值发射魔法弹
/// </summary>
public partial class StaffWeapon : WeaponBase
{
    [Export] public PackedScene SpellProjectileScene { get; set; }
    [Export] public DamageType SpellElement { get; set; } = DamageType.Fire;
    [Export] public float SpellSpeed { get; set; } = 20f;
    [Export] public Node3D SpellSpawnPoint { get; set; }
    [Export] public bool IsAoE { get; set; } = false;
    [Export] public float AoERadius { get; set; } = 3f;

    public override bool Attack()
    {
        if (IsAttacking || AttackCooldown > 0 || Wielder == null || Data == null) return false;

        // 检查法力消耗
        if (Wielder.Stats.CurrentMana < Data.ManaCost) return false;

        IsAttacking = true;
        Wielder.Stats.ModifyMana(-Data.ManaCost);

        var damageInfo = DamageCalculator.CalculateAttackDamage(
            Wielder.Stats, Data.BaseDamage, SpellElement, Wielder.EntityId);

        SpawnSpellProjectile(damageInfo);

        AttackCooldown = 1f / Data.AttackSpeed;
        WeaponAnimPlayer?.Play("cast");

        GetTree().CreateTimer(0.5).Timeout += OnAttackEnd;
        return true;
    }

    private void SpawnSpellProjectile(DamageInfo damageInfo)
    {
        if (SpellProjectileScene == null) return;

        var spell = SpellProjectileScene.Instantiate<SpellProjectile>();
        var spawnPos = SpellSpawnPoint?.GlobalPosition ?? GlobalPosition;
        var direction = -Wielder.GlobalTransform.Basis.Z;

        GetTree().Root.AddChild(spell);
        spell.GlobalPosition = spawnPos;
        spell.IsAoE = IsAoE;
        spell.AoERadius = AoERadius;
        spell.Launch(direction, SpellSpeed, damageInfo);
    }
}


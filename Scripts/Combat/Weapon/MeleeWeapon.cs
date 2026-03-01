using Godot;

namespace Diablo.Combat.Weapon;

/// <summary>
/// 近战武器
/// 支持连击系统和耐力消耗
/// </summary>
public partial class MeleeWeapon : WeaponBase
{
    [Export] public int MaxComboCount { get; set; } = 3;
    [Export] public float ComboWindowTime { get; set; } = 0.8f;

    private int _currentCombo = 0;
    private float _comboTimer = 0f;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_comboTimer > 0)
        {
            _comboTimer -= (float)delta;
            if (_comboTimer <= 0)
            {
                _currentCombo = 0;
            }
        }
    }

    public override bool Attack()
    {
        if (IsAttacking || AttackCooldown > 0 || Wielder == null || Data == null) return false;
        if (Wielder.Stats.CurrentStamina < Data.StaminaCost) return false;

        IsAttacking = true;
        _currentCombo = (_currentCombo + 1) % (MaxComboCount + 1);
        if (_currentCombo == 0) _currentCombo = 1;

        _comboTimer = ComboWindowTime;

        // 连击伤害递增
        float comboMultiplier = 1f + (_currentCombo - 1) * 0.15f;
        Wielder.Stats.ModifyStamina(-Data.StaminaCost);

        var damageInfo = DamageCalculator.CalculateAttackDamage(
            Wielder.Stats, Data.BaseDamage * comboMultiplier, Data.PrimaryDamageType, Wielder.EntityId);
        damageInfo.KnockbackForce = Data.KnockbackForce * (1f + _currentCombo * 0.1f);

        WeaponHitBox?.Activate(damageInfo);
        WeaponAnimPlayer?.Play($"attack_{_currentCombo}");

        AttackCooldown = 1f / Data.AttackSpeed;
        GetTree().CreateTimer(0.3).Timeout += OnAttackEnd;

        return true;
    }
}


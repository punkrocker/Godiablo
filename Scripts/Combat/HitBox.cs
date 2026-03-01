using Godot;
using Diablo.Character;

namespace Diablo.Combat;

/// <summary>
/// 伤害判定区域（攻击方）
/// 挂载在武器或法术上，当进入 HurtBox 时触发伤害
/// </summary>
public partial class HitBox : Area3D
{
    [Export] public bool IsActive { get; set; } = false;

    /// <summary>当前关联的伤害信息</summary>
    public DamageInfo CurrentDamageInfo { get; set; }

    [Signal] public delegate void HitLandedEventHandler(Node3D target);

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
        Monitoring = false;
    }

    /// <summary>
    /// 启用攻击判定
    /// </summary>
    public void Activate(DamageInfo damageInfo)
    {
        CurrentDamageInfo = damageInfo;
        IsActive = true;
        Monitoring = true;
    }

    /// <summary>
    /// 关闭攻击判定
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        Monitoring = false;
        CurrentDamageInfo = null;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!IsActive || CurrentDamageInfo == null) return;

        if (body is CharacterBase target && target.EntityId != CurrentDamageInfo.SourceId)
        {
            target.TakeDamage(CurrentDamageInfo);
            EmitSignal(SignalName.HitLanded, target);
        }
    }

    private void OnAreaEntered(Area3D area)
    {
        if (!IsActive || CurrentDamageInfo == null) return;

        if (area is HurtBox hurtBox)
        {
            hurtBox.ReceiveHit(CurrentDamageInfo);
        }
    }
}


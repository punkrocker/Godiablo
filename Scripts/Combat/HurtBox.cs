using Godot;
using Diablo.Character;

namespace Diablo.Combat;

/// <summary>
/// 受伤判定区域（防御方）
/// 挂载在角色身上，接收 HitBox 的碰撞并转发伤害
/// </summary>
public partial class HurtBox : Area3D
{
    /// <summary>所属角色引用</summary>
    [Export] public CharacterBase Owner { get; set; }

    [Signal] public delegate void HurtEventHandler(float damage);

    public override void _Ready()
    {
        if (Owner == null)
        {
            Owner = GetParentOrNull<CharacterBase>();
        }
    }

    /// <summary>
    /// 接收伤害
    /// </summary>
    public void ReceiveHit(DamageInfo damageInfo)
    {
        if (Owner == null) return;

        Owner.TakeDamage(damageInfo);
        EmitSignal(SignalName.Hurt, damageInfo.Amount);
    }
}


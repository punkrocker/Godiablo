using Godot;

namespace Diablo.Loot;

/// <summary>
/// 战利品掉落器
/// 挂载到敌人或宝箱上，在触发时根据战利品表生成掉落物
/// </summary>
public partial class LootDropper : Node
{
    [Export] public LootTable Table { get; set; }
    [Export] public PackedScene LootPickupScene { get; set; }

    /// <summary>当前是否已掉落过</summary>
    private bool _hasDropped = false;

    [Signal] public delegate void LootDroppedEventHandler();

    /// <summary>
    /// 触发掉落
    /// </summary>
    public void Drop()
    {
        if (_hasDropped || Table == null) return;

        _hasDropped = true;

        var parentPos = Vector3.Zero;
        if (GetParent() is Node3D parent3D)
        {
            parentPos = parent3D.GlobalPosition;
        }

        var results = Table.Roll();

        foreach (var result in results)
        {
            if (result.IsGold)
            {
                GD.Print($"[掉落] 金币 x{result.GoldAmount}");
                SpawnLootPickup(parentPos + RandomOffset(), null, result.GoldAmount);
            }
            else if (result.Item != null)
            {
                GD.Print($"[掉落] {result.Item.ItemName} x{result.Quantity}");
                SpawnLootPickup(parentPos + RandomOffset(), result, 0);
            }
        }

        EmitSignal(SignalName.LootDropped);
    }

    private void SpawnLootPickup(Vector3 position, LootResult result, int gold)
    {
        if (LootPickupScene == null) return;

        var pickup = LootPickupScene.Instantiate<Node3D>();
        GetTree().Root.AddChild(pickup);
        pickup.GlobalPosition = position;

        // 设置掉落物数据（具体实现取决于 LootPickup 场景脚本）
        if (pickup.HasMethod("Setup"))
        {
            pickup.Call("Setup", result?.Item?.ItemId ?? "", result?.Quantity ?? 0, gold);
        }
    }

    private Vector3 RandomOffset()
    {
        return new Vector3(
            (float)GD.RandRange(-1.5, 1.5),
            0.5f,
            (float)GD.RandRange(-1.5, 1.5)
        );
    }
}


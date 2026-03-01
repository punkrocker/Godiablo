using Godot;
using System.Collections.Generic;
using Diablo.Inventory.Items;
using Diablo.Core.Helpers;

namespace Diablo.Inventory;

/// <summary>
/// 物品数据库（单例）
/// 加载和管理所有物品资源，提供按ID查找功能
/// </summary>
public partial class ItemDatabase : Node
{
    public static ItemDatabase Instance { get; private set; }

    private Dictionary<string, ItemData> _items = new();

    public override void _Ready()
    {
        Instance = this;
        LoadAllItems();
    }

    /// <summary>
    /// 加载所有物品资源
    /// </summary>
    private void LoadAllItems()
    {
        LoadItemsFromDirectory(ResourcePaths.ItemDataPath);
        LoadItemsFromDirectory(ResourcePaths.WeaponDataPath);
        LoadItemsFromDirectory(ResourcePaths.ArmorDataPath);

        GD.Print($"[物品数据库] 已加载 {_items.Count} 个物品");
    }

    private void LoadItemsFromDirectory(string path)
    {
        if (!DirAccess.DirExistsAbsolute(path)) return;

        using var dir = DirAccess.Open(path);
        if (dir == null) return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();
        while (!string.IsNullOrEmpty(fileName))
        {
            if (fileName.EndsWith(".tres") || fileName.EndsWith(".res"))
            {
                var item = GD.Load<ItemData>(path + fileName);
                if (item != null && !string.IsNullOrEmpty(item.ItemId))
                {
                    _items[item.ItemId] = item;
                }
            }
            fileName = dir.GetNext();
        }
    }

    /// <summary>
    /// 根据ID获取物品数据
    /// </summary>
    public ItemData GetItem(string itemId)
    {
        return _items.GetValueOrDefault(itemId, null);
    }

    /// <summary>
    /// 获取所有物品
    /// </summary>
    public IReadOnlyDictionary<string, ItemData> GetAllItems()
    {
        return _items;
    }

    /// <summary>
    /// 注册新物品
    /// </summary>
    public void RegisterItem(ItemData item)
    {
        if (item != null && !string.IsNullOrEmpty(item.ItemId))
        {
            _items[item.ItemId] = item;
        }
    }
}


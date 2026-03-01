using Godot;
using Diablo.NPC;
using Diablo.Inventory.Items;

namespace Diablo.UI.Controllers;

/// <summary>
/// 商店UI控制器
/// 显示商人买卖界面
/// </summary>
public partial class ShopUIController : Control
{
    [Export] public GridContainer ShopItemsGrid { get; set; }
    [Export] public GridContainer PlayerItemsGrid { get; set; }
    [Export] public Label PlayerGoldLabel { get; set; }
    [Export] public Label MerchantGoldLabel { get; set; }
    [Export] public Button CloseButton { get; set; }

    private MerchantNPC _currentMerchant;
    private Inventory.Inventory _playerInventory;
    private int _playerGold = 0;

    [Signal] public delegate void PurchaseMadeEventHandler(string itemId, int price);
    [Signal] public delegate void SaleMadeEventHandler(string itemId, int price);

    public override void _Ready()
    {
        Visible = false;

        if (CloseButton != null)
        {
            CloseButton.Pressed += CloseShop;
        }
    }

    /// <summary>
    /// 打开商店
    /// </summary>
    public void OpenShop(MerchantNPC merchant, Inventory.Inventory playerInventory, int playerGold)
    {
        _currentMerchant = merchant;
        _playerInventory = playerInventory;
        _playerGold = playerGold;

        Visible = true;
        RefreshDisplay();
    }

    /// <summary>
    /// 关闭商店
    /// </summary>
    public void CloseShop()
    {
        _currentMerchant?.CloseShop();
        _currentMerchant = null;
        Visible = false;
    }

    private void RefreshDisplay()
    {
        if (PlayerGoldLabel != null)
            PlayerGoldLabel.Text = $"金币: {_playerGold}";

        if (MerchantGoldLabel != null && _currentMerchant != null)
            MerchantGoldLabel.Text = $"商人金币: {_currentMerchant.Gold}";
    }

    /// <summary>
    /// 购买物品
    /// </summary>
    public void BuyItem(int shopSlotIndex)
    {
        if (_currentMerchant == null || _playerInventory == null) return;

        if (_currentMerchant.BuyItem(shopSlotIndex, _playerInventory, ref _playerGold))
        {
            RefreshDisplay();
        }
    }

    /// <summary>
    /// 出售物品
    /// </summary>
    public void SellItem(ItemData item)
    {
        if (_currentMerchant == null || _playerInventory == null || item == null) return;

        if (_currentMerchant.SellItem(item, _playerInventory, ref _playerGold))
        {
            RefreshDisplay();
        }
    }
}


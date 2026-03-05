using Diablo.Core.Enums;
using Godot;

namespace Diablo.Core.Events;

/// <summary>
/// 全局事件总线 - 用于各系统间解耦通信
/// 使用 C# 静态事件实现，各系统订阅感兴趣的事件即可
/// </summary>
public static class GameEvents
{
    // ===== 战斗事件 =====
    public delegate void CombatHitHandler(string attackerId, string targetId, float damage, DamageType damageType);
    public static event CombatHitHandler OnCombatHit;
    public static void EmitCombatHit(string attackerId, string targetId, float damage, DamageType damageType)
        => OnCombatHit?.Invoke(attackerId, targetId, damage, damageType);

    public delegate void EntityDeathHandler(string entityId, string killerId);
    public static event EntityDeathHandler OnEntityDeath;
    public static void EmitEntityDeath(string entityId, string killerId)
        => OnEntityDeath?.Invoke(entityId, killerId);

    // ===== 物品事件 =====
    public delegate void ItemPickedUpHandler(string itemId, int quantity);
    public static event ItemPickedUpHandler OnItemPickedUp;
    public static void EmitItemPickedUp(string itemId, int quantity)
        => OnItemPickedUp?.Invoke(itemId, quantity);

    public delegate void ItemDroppedHandler(string itemId, int quantity);
    public static event ItemDroppedHandler OnItemDropped;
    public static void EmitItemDropped(string itemId, int quantity)
        => OnItemDropped?.Invoke(itemId, quantity);

    public delegate void ItemUsedHandler(string itemId);
    public static event ItemUsedHandler OnItemUsed;
    public static void EmitItemUsed(string itemId)
        => OnItemUsed?.Invoke(itemId);

    public delegate void ItemEquippedHandler(string itemId, EquipmentSlot slot);
    public static event ItemEquippedHandler OnItemEquipped;
    public static void EmitItemEquipped(string itemId, EquipmentSlot slot)
        => OnItemEquipped?.Invoke(itemId, slot);

    public delegate void ItemUnequippedHandler(string itemId, EquipmentSlot slot);
    public static event ItemUnequippedHandler OnItemUnequipped;
    public static void EmitItemUnequipped(string itemId, EquipmentSlot slot)
        => OnItemUnequipped?.Invoke(itemId, slot);

    // ===== 任务事件 =====
    public delegate void QuestAcceptedHandler(string questId);
    public static event QuestAcceptedHandler OnQuestAccepted;
    public static void EmitQuestAccepted(string questId)
        => OnQuestAccepted?.Invoke(questId);

    public delegate void QuestCompletedHandler(string questId);
    public static event QuestCompletedHandler OnQuestCompleted;
    public static void EmitQuestCompleted(string questId)
        => OnQuestCompleted?.Invoke(questId);

    public delegate void QuestObjectiveUpdatedHandler(string questId, int objectiveIndex, int currentCount);
    public static event QuestObjectiveUpdatedHandler OnQuestObjectiveUpdated;
    public static void EmitQuestObjectiveUpdated(string questId, int objectiveIndex, int currentCount)
        => OnQuestObjectiveUpdated?.Invoke(questId, objectiveIndex, currentCount);

    public delegate void QuestFailedHandler(string questId);
    public static event QuestFailedHandler OnQuestFailed;
    public static void EmitQuestFailed(string questId)
        => OnQuestFailed?.Invoke(questId);

    // ===== 对话事件 =====
    public delegate void DialogueStartedHandler(string npcId);
    public static event DialogueStartedHandler OnDialogueStarted;
    public static void EmitDialogueStarted(string npcId)
        => OnDialogueStarted?.Invoke(npcId);

    public delegate void DialogueEndedHandler(string npcId);
    public static event DialogueEndedHandler OnDialogueEnded;
    public static void EmitDialogueEnded(string npcId)
        => OnDialogueEnded?.Invoke(npcId);

    // ===== 角色事件 =====
    public delegate void PlayerLevelUpHandler(int newLevel);
    public static event PlayerLevelUpHandler OnPlayerLevelUp;
    public static void EmitPlayerLevelUp(int newLevel)
        => OnPlayerLevelUp?.Invoke(newLevel);

    public delegate void PlayerStatsChangedHandler(StatType statType, float newValue, float maxValue);
    public static event PlayerStatsChangedHandler OnPlayerStatsChanged;
    public static void EmitPlayerStatsChanged(StatType statType, float newValue, float maxValue)
    {
        GD.Print($"[GameEvents] EmitPlayerStatsChanged: {statType} {newValue}/{maxValue}");
        OnPlayerStatsChanged?.Invoke(statType, newValue, maxValue);
    }

    // ===== 交互事件 =====
    public delegate void InteractionAvailableHandler(string interactableId, string promptText);
    public static event InteractionAvailableHandler OnInteractionAvailable;
    public static void EmitInteractionAvailable(string interactableId, string promptText)
        => OnInteractionAvailable?.Invoke(interactableId, promptText);

    public delegate void InteractionClearedHandler();
    public static event InteractionClearedHandler OnInteractionCleared;
    public static void EmitInteractionCleared()
        => OnInteractionCleared?.Invoke();
}

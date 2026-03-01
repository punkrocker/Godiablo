namespace Diablo.Character;

/// <summary>
/// 玩家输入动作名称常量
/// 统一管理所有输入映射的字符串，避免拼写错误
/// 需要在 Godot 项目设置 > Input Map 中配置对应的键位
/// </summary>
public static class PlayerInputActions
{
    // 移动
    public const string MoveForward = "move_forward";
    public const string MoveBackward = "move_backward";
    public const string MoveLeft = "move_left";
    public const string MoveRight = "move_right";

    // 动作
    public const string Jump = "jump";
    public const string Sprint = "sprint";
    public const string Sneak = "sneak";
    public const string Interact = "interact";

    // 战斗
    public const string Attack = "attack";
    public const string Block = "block";
    public const string CastSpell = "cast_spell";

    // UI
    public const string OpenInventory = "open_inventory";
    public const string OpenQuestJournal = "open_quest_journal";
    public const string OpenMap = "open_map";
    public const string Pause = "pause";
}


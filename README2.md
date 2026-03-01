Elder-Scrolls 风格 RPG 脚本框架（Godot 4.6 / C#）

简介
- 这是为 Godot 4.6 + C# (.NET 8.0) 项目准备的一套通用 RPG 框架脚本（放在 `Scripts/` 目录下）。
- 主要系统：角色（玩家/敌人/NPC）、战斗（近战/远程/法术）、物品与背包、装备、任务、对话、战利品、UI 数据与控制器。

总览（按文件夹）
- `Scripts/Core/`：公共枚举、事件总线 `GameEvents`、工具类 `MathUtils`、资源路径常量 `ResourcePaths`。
- `Scripts/Character/`：`CharacterStats`、`CharacterBase`（抽象）、`PlayerCharacter`、`PlayerLevelSystem`、`PlayerInputActions`。
- `Scripts/Enemy/`：`EnemyBase`、AI 状态机（`AI/`）以及常见类型（`Types/`：`MeleeEnemy`、`RangedEnemy`、`MageEnemy`、`BossEnemy`）。
- `Scripts/NPC/`：`NPCBase`、`MerchantNPC`、对话系统（`Dialogue/`）与日程/行为（`Behavior/`）。
- `Scripts/Combat/`：`CombatManager`、`DamageInfo`、`DamageCalculator`、`HitBox`、`HurtBox`、武器体系（`Weapon/`）、投射物（`Projectile/`）。
- `Scripts/Inventory/`：物品数据（`Items/`：`ItemData`、`WeaponItemData`、`ArmorItemData`、`ConsumableItemData` 等）、`Inventory`、`EquipmentManager`、`ItemDatabase`。
- `Scripts/Quest/`：`QuestData`、`QuestObjective`、`QuestInstance`、`QuestJournal`、`QuestManager`。
- `Scripts/Loot/`：`LootTable`、`LootEntry`、`LootDropper`。
- `Scripts/UI/`：UI 数据结构（`UI/Data/`）和 UI 控制器（`UI/Controllers/`：HUD、背包、装备、对话、任务日志、商店、战利品弹窗、工具提示等）。

快速准备（必须步骤）
1. 在 Godot 项目设置 > Input Map 中添加以下动作（名称必须与 `PlayerInputActions` 一致）：
   - `move_forward`, `move_backward`, `move_left`, `move_right`
   - `jump`, `sprint`, `sneak`, `interact`
   - `attack`, `block`, `cast_spell`
   - `open_inventory`, `open_quest_journal`, `open_map`, `pause`

2. 注册 Autoload（单例）
   - 建议将以下脚本以 Node（或脚本）形式添加为 Autoload：
     - `Scripts/Combat/CombatManager.cs` -> 名称: `CombatManager`
     - `Scripts/NPC/Dialogue/DialogueManager.cs` -> 名称: `DialogueManager`
     - `Scripts/Quest/QuestManager.cs` -> 名称: `QuestManager`
     - `Scripts/Inventory/ItemDatabase.cs` -> 名称: `ItemDatabase`
   - 在 Godot 中：Project > Project Settings > Autoload，选择对应脚本或创建一个空 Node 并 Attach 对应 C# 脚本，然后添加为单例。

3. 物品/武器/护甲/任务/对话/战利品资源
   - 在 `res://Data/` 下建立子目录（与 `ResourcePaths` 常量一致），使用 Godot 的 "New Resource" 创建 `.tres` 或 `.res` 资源：
     - WeaponData（放在 `res://Data/Weapons/`）
     - ItemData / WeaponItemData / ArmorItemData / ConsumableItemData（放在 `res://Data/Items/` 或 `res://Data/Armors/`）
     - QuestData（放在 `res://Data/Quests/`）
     - DialogueData（放在 `res://Data/Dialogues/`）
     - LootTable（放在 `res://Data/LootTables/`）

如何把脚本组件到场景（关键示例）

1) 创建玩家（Player）
- Scene: Node3D (或 CharacterBody3D)
- 添加组件（作为子节点或给该节点 Attach）
  - Attach 脚本 `Scripts/Character/PlayerCharacter.cs` 到根节点（或从继承自 `CharacterBase` 的自定义节点）。
  - 添加 `Camera3D`（命名 `Camera3D`），以便 `PlayerCharacter` 能读取视角。
  - 添加 `RayCast3D`（命名 `InteractionRay`），用于交互检测（在 Inspector 中设置 TargetPosition 的 Z 轴为 -InteractionRange 或用脚本设置）。
  - 添加 `AnimationPlayer`，包含 `attack`, `cast_spell`, `death` 等动画名。
  - 添加 `HurtBox`（Area3D）作为子节点，并设置 `Owner` 指向玩家节点（或让 `HurtBox` 自动在 `_Ready` 找到父节点）。
  - 添加 `Inventory` 节点并 Attach `Scripts/Inventory/Inventory.cs`，为玩家准备背包。
  - 添加 `EquipmentManager` 节点并 Attach `Scripts/Inventory/EquipmentManager.cs`，设置 `OwnerStats` 指向玩家的 `CharacterStats` 资源实例。

2) 敌人（Enemy）
- Scene: `EnemyBase` 派生节点（例如使用 `MeleeEnemy`、`RangedEnemy`）
- 配置：
  - 给节点 Attach 对应脚本（`Scripts/Enemy/Types/MeleeEnemy.cs` 等）。
  - 添加 `NavigationAgent3D`、`AnimationPlayer`、`HurtBox`（指向父角色）、`LootDropper`（并指定 `LootTable` 资源）。
  - （可选）给敌人一个 `Path3D` 命名为 `PatrolPath`，用于巡逻状态。

3) 武器与攻击判定
- 武器节点应继承自 `WeaponBase`（或直接使用 `MeleeWeapon` / `BowWeapon` / `StaffWeapon`），
  - 包含子节点 `HitBox`（Area3D，Attach `Scripts/Combat/HitBox.cs`）并带碰撞形状（CollisionShape3D）。
  - 在武器发起攻击时，`HitBox.Activate(damageInfo)` 会在命中时调用目标的 `TakeDamage` 或 HurtBox 的 `ReceiveHit`。
- 远程武器：为 `BowWeapon` 配置 `ArrowScene`（一个包含 `ProjectileBase` 的场景），并设置 `ArrowSpawnPoint`。
- 法杖武器：为 `StaffWeapon` 配置 `SpellProjectileScene`，设定 `SpellSpawnPoint`。

4) 投射物（Projectile）
- `ProjectileBase` / `ArrowProjectile` / `SpellProjectile` 提供 `Launch(direction, speed, damageInfo)` 方法。
- 在命中时会调用 `HitBox`/`HurtBox` 的逻辑并自动销毁或触发 AoE。

5) 对话系统（NPC）
- 给 NPC 使用 `Scripts/NPC/NPCBase.cs` (或 `MerchantNPC.cs`)。
- 在 NPC 上设置 `DialogueResource` 指向 `DialogueData` 资源。
- 将 `DialogueManager` 注册为 Autoload（单例），UI 使用 `DialogueManager.Instance` 接收事件并显示对话。

6) 任务系统
- `QuestData` 作为资源创建并放在 `res://Data/Quests/`。
- `QuestManager` 应作为 Autoload，并持有或引用 `QuestJournal`（玩家的任务日志节点）。
- `QuestManager` 会监听 `GameEvents`（如击杀、拾取）并调用 `QuestJournal.AdvanceObjective` 来推进任务。

7) 物品数据库与背包
- 将 `ItemDatabase` 加为 Autoload；它会在 `_Ready()` 中尝试读取 `ResourcePaths` 定义目录下的资源（`res://Data/Items/`, `res://Data/Weapons/`, `res://Data/Armors/`）。
- 使用 `Inventory.AddItem(itemData, qty)` / `RemoveItem(itemId, qty)` 等 API 操作背包。

8) 战利品表与掉落
- 配置 `LootTable` 资源，给敌人挂 `LootDropper` 并在死亡时调用 `Drop()`（`EnemyBase.Die` 已在示例中调用）。

9) UI
- UI 控制器脚本位于 `Scripts/UI/Controllers/`，它们期望有特定的子节点（例如 `HUDController` 里会查找 `HealthBar`, `ManaBar` 等，或通过导出在 Inspector 里绑定）。
- 把 UI 场景置于主场景中并在 Inspector 中绑定对应控件。

资源/工具创建示例（在编辑器中操作）
- 新增武器资源：右键 -> New Resource -> 选择 `WeaponData` -> 设置 `WeaponId`/`WeaponName`/`BaseDamage` 等 -> 保存为 `res://Data/Weapons/sword_basic.tres`
- 新增物品资源：New Resource -> `ItemData` 或 `ConsumableItemData` -> 保存到 `res://Data/Items/`。
- 新增任务资源：New Resource -> `QuestData` -> 配置 Objectives 数组并保存。

事件总线（`GameEvents`）
- 通过 `GameEvents` 可以订阅全局事件（例如 `GameEvents.OnCombatHit`, `GameEvents.OnItemPickedUp`, `GameEvents.OnQuestAccepted` 等），各系统之间通过它来通信，减少耦合。

常见用法/代码片段
- 发起一次攻击（在代码中）
  - 构造 `DamageInfo`：
    - 通过 `DamageCalculator.CalculateAttackDamage(attackerStats, weaponBaseDamage, damageType, attackerId)` 获取 `DamageInfo`，
    - 将其传给 `HitBox.Activate(damageInfo)` 或直接对目标调用 `target.TakeDamage(damageInfo)`。

- 发放任务奖励（在 `QuestJournal.CompleteQuest` 中处理后，可调用）：
  - 给玩家 `Inventory.AddItem(item, qty)`，增加金币/经验等。

调试与诊断
- 核心单例（如 `CombatManager`, `DialogueManager`, `QuestManager`, `ItemDatabase`）建议启动时在 Autoload 中注册以便任何脚本通过单例访问。
- 常见问题：若 `ItemDatabase` 没加载资源，请确认 `ResourcePaths` 指定目录存在且资源文件是 `.tres`/`.res`。

开发建议与扩展点（可选）
- 保存/加载系统：当前框架未包含完整序列化模块，建议将 `Inventory`, `QuestJournal`, `CharacterStats` 序列化到 JSON 或 Godot Resource 存档。
- 网络/多人：将事件总线适配为网络 RPC 或添加中间层以支持多人同步。
- 平衡：把伤害和经验曲线分离成可编辑资源以便美术/策划调整。

常见文件清单与使用指南（速览）
- `Scripts/Character/CharacterBase.cs`：所有可战斗实体基类；在自定义角色场景上 Attach 并确保包含 `AnimationPlayer`/`HurtBox`。
- `Scripts/Character/PlayerCharacter.cs`：玩家输入、移动、交互逻辑；场景中需有 `Camera3D` 与 `RayCast3D (InteractionRay)`。
- `Scripts/Combat/HitBox.cs` 与 `Scripts/Combat/HurtBox.cs`：分别放在武器（攻击）与角色（受击）节点上，配置 `CollisionShape3D` 后即可生效。
- `Scripts/Combat/Weapon/WeaponData.cs`：武器资源模版，供 `WeaponBase` 与其子类使用。
- `Scripts/NPC/Dialogue/DialogueManager.cs`：对话流程控制器，建议 Autoload。
- `Scripts/Inventory/Inventory.cs`：玩家背包，使用 `AddItem` / `RemoveItem` 管理物品。
- `Scripts/Quest/QuestManager.cs`：任务推进管理器，监听 `GameEvents` 来推进任务目标。

如果你希望，我可以：
- 为你把常用 Autoload 单例直接在 `project.godot` 中添加示例（我可以生成一个简单的 `autoload.cfg` 片段或给出步骤）。
- 为 Player、Enemy 提供一个最小可运行示例场景（带必要节点与导出属性设置）。
- 将上面的要点写成更详细的快速入门（含逐步截图/节点树示例）。

---
生成器说明：本 README 是由项目脚本自动扫描并汇总生成的使用说明。如果要我把 README 翻译为英文或补充示例场景/资源，我可以继续生成。

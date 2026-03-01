# Diablo-Style RPG Framework — 快速入门指南
## Godot 4.6 / C# (.NET 8.0)

---

## 目录

1. [项目结构总览](#1-项目结构总览)
2. [第一步：配置 Input Map（必须）](#2-第一步配置-input-map必须)
3. [第二步：注册 Autoload 单例](#3-第二步注册-autoload-单例)
4. [第三步：创建数据资源（.tres）](#4-第三步创建数据资源tres)
5. [第四步：搭建 Player 场景](#5-第四步搭建-player-场景)
6. [第五步：搭建 Enemy 场景](#6-第五步搭建-enemy-场景)
7. [第六步：搭建武器（HitBox/HurtBox）](#7-第六步搭建武器hitboxhurtbox)
8. [第七步：搭建主场景（Main Scene）](#8-第七步搭建主场景main-scene)
9. [第八步：搭建 HUD](#9-第八步搭建-hud)
10. [第九步：NavigationMesh 烘焙（敌人寻路）](#10-第九步navigationmesh-烘焙敌人寻路)
11. [碰撞层速查表](#11-碰撞层速查表)
12. [常用代码片段](#12-常用代码片段)
13. [常见问题排查](#13-常见问题排查)
14. [扩展方向](#14-扩展方向)

---

## 1. 项目结构总览

```
diablo/
├── Scripts/
│   ├── Character/          CharacterBase, PlayerCharacter, CharacterStats …
│   ├── Enemy/              EnemyBase, MeleeEnemy, RangedEnemy, AI/ …
│   ├── Combat/             HitBox, HurtBox, DamageInfo, Weapon/ …
│   ├── Inventory/          Inventory, EquipmentManager, Items/ …
│   ├── Loot/               LootTable, LootDropper …
│   ├── NPC/                NPCBase, MerchantNPC, Dialogue/ …
│   ├── Quest/              QuestData, QuestManager …
│   ├── UI/                 Controllers/ (HUD, Inventory, Shop …)
│   └── Core/               Enums/, Events/GameEvents, Helpers/ …
├── scenes/
│   ├── player/player.tscn          ← 玩家预制场景（已生成）
│   ├── enemy/melee_enemy.tscn      ← 近战敌人预制场景（已生成）
│   ├── enemy/ranged_enemy.tscn     ← 远程敌人预制场景（已生成）
│   ├── weapon/melee_weapon.tscn    ← 武器预制场景（已生成）
│   └── main/
│       ├── main_scene.tscn         ← 主场景（已生成，可直接运行）
│       └── hud.tscn                ← HUD（已生成）
└── Data/
    ├── player_stats.tres           ← 玩家属性资源（已生成）
    ├── melee_enemy_stats.tres      ← 敌人属性资源（已生成）
    └── Weapons/sword_basic.tres    ← 基础武器数据（已生成）
```

> **所有 `scenes/` 和 `Data/` 文件均已预生成，可直接在 Godot 4.6 中打开。**

---

## 2. 第一步：配置 Input Map（必须）

`PlayerCharacter.cs` 通过 `PlayerInputActions` 常量读取输入，必须在项目设置中注册这些动作名称。

**操作步骤：**

```
Godot 编辑器顶部菜单
  └── Project（项目）
      └── Project Settings（项目设置）
          └── Input Map（输入映射）标签页
```

在 "Add New Action"（添加新动作）输入框中逐一添加以下动作，并绑定按键：

| 动作名称           | 推荐按键        | 说明         |
|--------------------|----------------|--------------|
| `move_forward`     | W              | 向前移动     |
| `move_backward`    | S              | 向后移动     |
| `move_left`        | A              | 向左移动     |
| `move_right`       | D              | 向右移动     |
| `jump`             | Space          | 跳跃         |
| `sprint`           | Shift（左）    | 冲刺（按住） |
| `sneak`            | Ctrl（左）     | 潜行（按住） |
| `interact`         | E              | 交互         |
| `attack`           | Mouse Button 1 | 普通攻击     |
| `block`            | Mouse Button 2 | 格挡         |
| `cast_spell`       | Q              | 施法         |
| `open_inventory`   | I 或 Tab       | 打开背包     |
| `open_quest_journal` | J            | 任务日志     |
| `open_map`         | M              | 地图         |
| `pause`            | Escape         | 暂停         |

---

## 3. 第二步：注册 Autoload 单例

**操作步骤：**

```
Project（项目）→ Project Settings（项目设置）→ Autoload 标签页
```

点击文件夹图标选择脚本，填写名称后点击 Add：

| 路径                                          | 单例名称         |
|-----------------------------------------------|-----------------|
| `res://Scripts/Combat/CombatManager.cs`       | `CombatManager`  |
| `res://Scripts/NPC/Dialogue/DialogueManager.cs` | `DialogueManager` |
| `res://Scripts/Quest/QuestManager.cs`         | `QuestManager`   |
| `res://Scripts/Inventory/ItemDatabase.cs`     | `ItemDatabase`   |

> ⚠️ 顺序建议：`ItemDatabase` 放最前，其他系统在 `_Ready()` 中可能调用它。

**节点树示意（Autoload 标签）：**
```
Autoloads
  ├── ItemDatabase      (res://Scripts/Inventory/ItemDatabase.cs)
  ├── CombatManager     (res://Scripts/Combat/CombatManager.cs)
  ├── DialogueManager   (res://Scripts/NPC/Dialogue/DialogueManager.cs)
  └── QuestManager      (res://Scripts/Quest/QuestManager.cs)
```

---

## 4. 第三步：创建数据资源（.tres）

所有游戏数据通过 Godot 资源（`.tres`）配置，以下是创建示例：

### 4.1 创建 WeaponData 资源

```
FileSystem 面板 → 右键 res://Data/Weapons/ → New Resource
  → 搜索 "WeaponData" → 选中 → 保存为 sword_basic.tres
```

**Inspector 属性设置（`sword_basic.tres`）：**
```
WeaponId          = "sword_basic"
WeaponName        = "铁剑"
BaseDamage        = 12.0
AttackSpeed       = 1.2
AttackRange       = 2.0
Type              = Sword (0)
PrimaryDamageType = Physical (0)
StaminaCost       = 8.0
KnockbackForce    = 2.5
```

### 4.2 创建 CharacterStats 资源（玩家）

```
New Resource → 搜索 "CharacterStats" → 保存为 res://Data/player_stats.tres
```

**关键属性：**
```
MaxHealth     = 120    CurrentHealth  = 120
MaxMana       = 60     CurrentMana    = 60
MaxStamina    = 100    CurrentStamina = 100
Strength      = 12     Agility        = 10
BaseArmor     = 5
```

### 4.3 创建 LootTable 资源（可选）

```
New Resource → 搜索 "LootTable" → 保存为 res://Data/LootTables/basic_enemy.tres
```
在 `LootEntries` 数组中添加 `LootEntry`，设置掉落物品和概率。

---

## 5. 第四步：搭建 Player 场景

> **已预生成文件：`res://scenes/player/player.tscn`**，可跳到第 5.3 节直接检查 Inspector。

### 5.1 节点树结构

```
Player (CharacterBody3D)          ← [脚本] PlayerCharacter.cs
├── CollisionShape3D              ← CapsuleShape3D(r=0.4, h=1.8)
├── MeshInstance3D                ← CapsuleMesh（占位，可换模型）
├── Camera3D                      ← 名称必须是 "Camera3D"
│     transform: pos(0,1.8,3.5), 轻微俯角
│     current = true
├── InteractionRay (RayCast3D)    ← 名称必须是 "InteractionRay"
│     target_position = (0,0,-3)
│     enabled = true
├── HurtBox (Area3D)              ← [脚本] HurtBox.cs
│   └── CollisionShape3D         ← 同胶囊形状
│     collision_layer = 2
│     collision_mask  = 4
├── AnimationPlayer               ← 含 "attack"/"cast_spell"/"death" 动画
├── Inventory (Node)              ← [脚本] Inventory.cs
│     MaxSlots = 40
├── EquipmentManager (Node)       ← [脚本] EquipmentManager.cs
└── WeaponSlot (Node3D)           ← 位置: (0.5, 0.9, -0.3)
    └── MeleeWeapon [实例]        ← scenes/weapon/melee_weapon.tscn
```

### 5.2 手动搭建步骤（若需从头创建）

1. **新建场景** → 根节点选 `CharacterBody3D` → 重命名为 `Player`
2. **Attach 脚本** → `Scripts/Character/PlayerCharacter.cs`
3. **设置碰撞层**：`collision_layer = 2`，`collision_mask = 1`
4. **添加子节点** `CollisionShape3D` → 形状选 `CapsuleShape3D`（radius=0.4, height=1.8）
5. **添加** `MeshInstance3D` → mesh 选 `CapsuleMesh`
6. **添加** `Camera3D`，**名称必须为 `Camera3D`**，设为 current
7. **添加** `RayCast3D`，**重命名为 `InteractionRay`**，`target_position = (0,0,-3)`，勾选 enabled
8. **添加** `Area3D` → 重命名为 `HurtBox` → Attach `HurtBox.cs` → 设碰撞层 2/掩码 4 → 添加子节点 `CollisionShape3D`（同胶囊）
9. **添加** `AnimationPlayer`（留空，后续补充动画）
10. **添加** `Node` → 重命名 `Inventory` → Attach `Inventory.cs` → MaxSlots=40
11. **添加** `Node` → 重命名 `EquipmentManager` → Attach `EquipmentManager.cs`
12. **添加** `Node3D` → 重命名 `WeaponSlot` → 实例化 `melee_weapon.tscn` 为子节点
13. **保存**为 `res://scenes/player/player.tscn`

### 5.3 Inspector 导出属性检查

选中 **Player** 根节点，在 **Inspector** 中确认：

| 属性               | 值                          |
|--------------------|-----------------------------|
| `Stats`            | `res://Data/player_stats.tres` |
| `MoveSpeed`        | 5.0                         |
| `Gravity`          | 9.8                         |
| `EntityId`         | `"player"`                  |
| `SprintMultiplier` | 1.6                         |
| `SneakMultiplier`  | 0.5                         |
| `StaminaDrainRate` | 10.0                        |
| `InteractionRange` | 3.0                         |
| `JumpVelocity`     | 5.0                         |

> ⚠️ **重要**：`Stats` 字段若留空，代码会在 `_Ready()` 中自动 `new CharacterStats()`，所有属性为默认值（HP=100）。建议填入 `.tres` 资源以便编辑器调整。

### 5.4 添加玩家到 "Player" 组

选中 Player 根节点 → **Node 面板 → Groups（组）** → 输入 `Player` → 点击 **Add**。

（`EnemyBase.DetectTarget()` 通过 `GetTree().GetNodesInGroup("Player")` 搜索玩家。）

---

## 6. 第五步：搭建 Enemy 场景

> **已预生成文件：`res://scenes/enemy/melee_enemy.tscn`** 和 `ranged_enemy.tscn`

### 6.1 近战敌人（MeleeEnemy）节点树

```
MeleeEnemy (CharacterBody3D)      ← [脚本] MeleeEnemy.cs，组: "Enemy"
├── CollisionShape3D              ← CapsuleShape3D(r=0.4, h=1.8)
├── MeshInstance3D                ← 占位网格（可换骷髅模型）
├── NavigationAgent3D             ← 名称必须是 "NavigationAgent3D"
│     path_desired_distance  = 0.5
│     target_desired_distance = 1.0
├── StateMachine (Node)           ← 名称必须是 "StateMachine"
│     [脚本] EnemyStateMachine.cs
│     → 自动以 IdleState 启动
│     → 检测到玩家 → ChaseState → AttackState
├── HurtBox (Area3D)              ← [脚本] HurtBox.cs
│   └── CollisionShape3D
│     collision_layer = 1
│     collision_mask  = 4
├── AnimationPlayer
└── LootDropper (Node)            ← [脚本] LootDropper.cs
      LootTableData = (可选 LootTable 资源)
```

### 6.2 Inspector 导出属性（MeleeEnemy）

| 属性              | 值                                |
|-------------------|----------------------------------|
| `Stats`           | `res://Data/melee_enemy_stats.tres` |
| `MoveSpeed`       | 3.5                              |
| `EntityId`        | `"melee_enemy_01"`               |
| `AggroRange`      | 12.0                             |
| `AttackRange`     | 1.8                              |
| `DeaggroRange`    | 22.0                             |
| `XPReward`        | 50                               |
| `ChargeSpeed`     | 8.0                              |
| `ChargeDistance`  | 8.0                              |
| `ChargeCooldown`  | 10.0                             |

### 6.3 AI 状态机流转图

```
  [IdleState]  ──发现玩家──▶  [ChaseState]  ──进入攻击范围──▶  [AttackState]
      ▲                             │                                  │
      │                       超出 DeaggroRange                        │
      └──────────── 失去目标 ◀──────┘         攻击冷却 ────────────────┘
      │
   闲置时间到
      │
  [PatrolState] ──巡逻完成──▶ [IdleState]
```

### 6.4 添加敌人到 "Enemy" 组

选中敌人根节点 → **Node → Groups** → 添加 `Enemy`。

---

## 7. 第六步：搭建武器（HitBox/HurtBox）

> **已预生成文件：`res://scenes/weapon/melee_weapon.tscn`**

### 7.1 武器场景节点树

```
MeleeWeapon (Node3D)              ← [脚本] MeleeWeapon.cs
├── MeshInstance3D                ← 剑身网格（BoxMesh 0.1×0.8×0.1）
└── HitBox (Area3D)               ← 名称必须是 "HitBox"
    │   [脚本] HitBox.cs
    │   collision_layer = 4
    │   collision_mask  = 3 (=1+2，同时检测敌人和玩家)
    │   monitoring = false (默认关闭，Attack() 时才激活)
    └── CollisionShape3D          ← BoxShape3D(0.25×0.9×0.25)
```

### 7.2 攻击流程时序

```
PlayerCharacter._Input(attack)
  └── PerformAttack()
      └── MeleeWeapon.Attack()
          ├── DamageCalculator.CalculateAttackDamage(Stats, BaseDamage, Type, EntityId)
          ├── HitBox.Activate(damageInfo)     ← monitoring = true
          │     └── OnBodyEntered(body)
          │           └── target.TakeDamage(damageInfo)  ← 敌人受伤
          └── Timer(0.3s) → HitBox.Deactivate()          ← monitoring = false
```

### 7.3 碰撞层配对规则

```
角色 HurtBox  collision_layer = 2 (玩家) / 1 (敌人)
武器 HitBox   collision_layer = 4
              collision_mask  = 3 (1|2，能检测玩家和敌人)

当 HitBox 进入 HurtBox 的 Area → OnAreaEntered → HurtBox.ReceiveHit(damageInfo)
当 HitBox 进入 CharacterBody3D → OnBodyEntered → target.TakeDamage(damageInfo)
```

---

## 8. 第七步：搭建主场景（Main Scene）

> **已预生成文件：`res://scenes/main/main_scene.tscn`**

### 8.1 主场景节点树

```
MainScene (Node3D)
├── WorldEnvironment              ← 天空/环境设置
├── DirectionalLight3D            ← 方向光（已设阴影）
├── Ground (StaticBody3D)         ← 地面
│   ├── MeshInstance3D            ← BoxMesh(30×0.2×30)
│   └── CollisionShape3D          ← BoxShape3D(30×0.2×30)
│         collision_layer = 1
├── NavigationRegion3D            ← ⚠️ 需要在编辑器中烘焙 NavigationMesh
├── Player [实例]                 ← scenes/player/player.tscn，位置(0,1,0)
├── MeleeEnemy [实例]             ← scenes/enemy/melee_enemy.tscn，位置(5,1,-5)
└── CanvasLayer (layer=10)
    └── HUD [实例]                ← scenes/main/hud.tscn
```

### 8.2 地面碰撞层设置

**Ground → Inspector：**
```
collision_layer = 1    ← 敌人和玩家都站在上面
collision_mask  = 0    ← 地面不检测任何东西
```

---

## 9. 第八步：搭建 HUD

> **已预生成文件：`res://scenes/main/hud.tscn`**

### 9.1 HUD 场景节点树

```
HUD (Control)                     ← [脚本] HUDController.cs
│   anchors_preset = Full Rect
├── StatsPanel (VBoxContainer)    ← 左下角
│   ├── HealthBar  (ProgressBar)
│   ├── ManaBar    (ProgressBar)
│   ├── StaminaBar (ProgressBar)
│   └── LabelsRow  (HBoxContainer)
│       ├── HealthLabel   (Label)   "HP 120/120"
│       ├── ManaLabel     (Label)   "MP 60/60"
│       ├── StaminaLabel  (Label)   "SP 100/100"
│       └── LevelLabel    (Label)   "Lv.1"
├── CrosshairPanel (Panel)        ← 屏幕正中，4×4 像素
└── InteractionPromptLabel (Label) ← "[E] 交互"，默认 visible=false
```

### 9.2 HUDController Inspector 属性绑定

选中 **HUD** 根节点，在 Inspector 中将每个导出字段拖入对应节点：

| 导出属性                  | 绑定节点路径                          |
|--------------------------|--------------------------------------|
| `HealthBar`              | `StatsPanel/HealthBar`               |
| `ManaBar`                | `StatsPanel/ManaBar`                 |
| `StaminaBar`             | `StatsPanel/StaminaBar`              |
| `HealthLabel`            | `StatsPanel/LabelsRow/HealthLabel`   |
| `ManaLabel`              | `StatsPanel/LabelsRow/ManaLabel`     |
| `StaminaLabel`           | `StatsPanel/LabelsRow/StaminaLabel`  |
| `LevelLabel`             | `StatsPanel/LabelsRow/LevelLabel`    |
| `CrosshairPanel`         | `CrosshairPanel`                     |
| `InteractionPromptLabel` | `InteractionPromptLabel`             |

> **操作方法**：Inspector 中每个导出属性旁有拖放区，直接从场景树将对应节点拖入即可；或点击右侧 `[空]` 按钮选择节点路径。

### 9.3 HUD 与玩家属性联动

`HUDController` 监听 `GameEvents.OnPlayerStatsChanged`，当玩家属性变化时自动更新。
确保 `PlayerCharacter.UpdatePlayerStats()` 中会调用：
```csharp
GameEvents.EmitPlayerStatsChanged(StatType.Health, Stats.CurrentHealth, Stats.MaxHealth);
```

---

## 10. 第九步：NavigationMesh 烘焙（敌人寻路）

敌人 AI 使用 `NavigationAgent3D` 寻路，必须先烘焙地图。

**步骤：**

1. 在主场景中选中 **NavigationRegion3D** 节点
2. 在 Inspector 中点击 **NavigationMesh** 字段旁的 `[空]` → 新建 `NavigationMesh`
3. 设置 `Agent Radius = 0.5`，`Agent Height = 2.0`
4. 在 3D 视口上方工具栏中选中 NavigationRegion3D 后，点击菜单 **Bake NavigationMesh**（或 Inspector 右上角的烘焙按钮）
5. 烘焙完成后，地面上会出现蓝色网格 → 保存场景

```
NavigationRegion3D
  └── NavigationMesh (Resource)
        CellSize     = 0.25
        AgentRadius  = 0.5
        AgentHeight  = 2.0
        AgentMaxSlope = 45°
```

---

## 11. 碰撞层速查表

| 层编号 | 用途               | 使用的节点                    |
|-------|--------------------|-------------------------------|
| 1     | 世界/敌人物理层    | Ground、MeleeEnemy、StaticBody |
| 2     | 玩家物理层         | Player (CharacterBody3D)       |
| 3     | NPC 层（可选）     | NPCBase                        |
| 4     | 武器/攻击判定层    | HitBox (Area3D)                |

**掩码配对逻辑：**
```
Player 移动碰撞: layer=2, mask=1      (与地面/敌人碰撞)
Enemy  移动碰撞: layer=1, mask=1      (与地面碰撞)
Player HurtBox:  layer=2, mask=4      (被武器/HitBox层检测)
Enemy  HurtBox:  layer=1, mask=4      (被武器/HitBox层检测)
武器   HitBox:   layer=4, mask=3(1+2) (检测玩家和敌人)
```

---

## 12. 常用代码片段

### 12.1 让玩家受到伤害

```csharp
// 从任意地方获取玩家节点
var player = GetTree().GetFirstNodeInGroup("Player") as CharacterBase;

// 创建伤害信息
var dmg = DamageCalculator.CalculateAttackDamage(
    attackerStats,      // CharacterStats
    15f,                // 基础伤害
    DamageType.Physical,
    "enemy_boss_01"     // 攻击者 ID
);
dmg.KnockbackForce = 3f;

// 施加伤害
player?.TakeDamage(dmg);
```

### 12.2 触发敌人掉落战利品

```csharp
// 在 EnemyBase.Die() 中（已有框架，确认调用）
var dropper = GetNodeOrNull<LootDropper>("LootDropper");
dropper?.Drop();
```

### 12.3 给玩家添加背包物品

```csharp
// 获取玩家的 Inventory 节点
var inventory = player.GetNode<Inventory>("Inventory");

// 从 ItemDatabase 获取物品数据
var itemDb = GetNode<ItemDatabase>("/root/ItemDatabase");
var sword = itemDb.GetItem("sword_basic");

inventory.AddItem(sword, 1);
```

### 12.4 订阅全局事件

```csharp
// 在任意节点的 _Ready() 中订阅
GameEvents.OnCombatHit += (attackerId, targetId, damage, type) => {
    GD.Print($"{attackerId} → {targetId}: {damage:F1} ({type})");
};

// 在 _ExitTree() 中取消订阅（防止内存泄漏）
GameEvents.OnCombatHit -= OnCombatHit;
```

### 12.5 触发任务目标更新

```csharp
// 击杀敌人后推进任务（通常在 EnemyBase.Die() 中触发）
GameEvents.EmitEntityDeath(EntityId, killerId);
// QuestManager 监听此事件并自动推进关联的 Kill 类型目标
```

---

## 13. 常见问题排查

| 问题                                | 原因                                      | 解决方案                                               |
|------------------------------------|-------------------------------------------|--------------------------------------------------------|
| 玩家无法移动                        | Input Map 未配置                           | 完成第 2 步，添加所有 `move_*` 动作                    |
| 敌人不追玩家                        | 玩家未加入 `"Player"` 组                  | 选中 Player → Node → Groups → 添加 `Player`            |
| 敌人不寻路（原地转圈）               | NavigationMesh 未烘焙                      | 完成第 10 步，烘焙 NavigationRegion3D                  |
| 攻击没有伤害                        | HitBox/HurtBox 碰撞层不匹配               | 对照第 11 节检查碰撞层和掩码设置                        |
| HUD 属性条不更新                    | HUDController 导出属性未绑定               | 完成第 9.2 节，将节点拖入 Inspector 导出字段           |
| `ItemDatabase` 资源加载失败         | `res://Data/` 目录不存在或文件名不匹配     | 确认资源保存路径与 `ResourcePaths` 常量一致             |
| 运行时 `Stats = null` 错误          | Stats 资源未在 Inspector 中赋值            | 将 `.tres` 资源拖入 Inspector 的 `Stats` 字段          |
| 武器不显示/不攻击                   | `WeaponBase.Data` 为 null                 | 将 `WeaponData.tres` 拖入武器节点 Inspector 的 `Data` 字段 |
| 敌人 AI 状态机报错 `_owner = null`  | `StateMachine` 节点不是 EnemyBase 的直接子节点 | 确保 StateMachine 节点是敌人根节点的**直接**子节点     |

---

## 14. 扩展方向

### 14.1 替换占位网格为真实模型

```
1. 导入 .glb / .gltf 模型到 res://Assets/Models/
2. 在 Player/Enemy 场景中删除 MeshInstance3D
3. 拖入模型文件作为子节点（或使用 AnimatedSprite3D）
4. 将 AnimationPlayer 替换为模型内嵌的 AnimationTree
```

### 14.2 添加新的敌人类型

```csharp
// 继承 EnemyBase 或已有类型
public partial class BossEnemy : EnemyBase
{
    [Export] public float EnrageHealthThreshold { get; set; } = 0.3f;
    
    // 覆盖死亡方法，触发特殊事件
    protected override void Die(string killerId)
    {
        GameEvents.EmitEntityDeath(EntityId, killerId);
        // 额外逻辑：播放死亡动画、开门等
        base.Die(killerId);
    }
}
```

### 14.3 保存/加载系统（建议方案）

```csharp
// 将背包和属性序列化到 JSON
var saveData = new Godot.Collections.Dictionary {
    ["health"] = Stats.CurrentHealth,
    ["inventory"] = /* Inventory.Slots 序列化 */
};
var json = Json.Stringify(saveData);
FileAccess.Open("user://save.json", FileAccess.ModeFlags.Write).StoreString(json);
```

### 14.4 多类型敌人场景（复用预制体）

在主场景中实例化多个敌人时，**每个实例的 `EntityId` 必须唯一**（或在代码中自动生成），否则 `GameEvents` 的死亡/伤害事件会混乱：

```csharp
// EnemyBase._Ready() 中自动生成唯一 ID
if (string.IsNullOrEmpty(EntityId))
    EntityId = $"{GetType().Name}_{GetInstanceId()}";
```

---

*本快速入门基于 Diablo-style RPG Framework 自动生成，涵盖所有预生成场景文件的完整搭建流程。*

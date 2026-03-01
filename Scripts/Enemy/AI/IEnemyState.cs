namespace Diablo.Enemy.AI;

/// <summary>
/// 敌人AI状态接口
/// 所有AI状态必须实现此接口
/// </summary>
public interface IEnemyState
{
    /// <summary>进入状态时调用</summary>
    void Enter(EnemyBase enemy);

    /// <summary>每帧执行</summary>
    void Execute(EnemyBase enemy, float delta);

    /// <summary>退出状态时调用</summary>
    void Exit(EnemyBase enemy);
}


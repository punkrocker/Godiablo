using Godot;

namespace Diablo.Core.Helpers;

/// <summary>
/// 通用数学工具类
/// </summary>
public static class MathUtils
{
    private static readonly RandomNumberGenerator _rng = new();

    /// <summary>
    /// 将值限制在[min, max]范围内
    /// </summary>
    public static float Clamp(float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }

    /// <summary>
    /// 根据权重随机选择索引
    /// </summary>
    public static int WeightedRandomIndex(float[] weights)
    {
        float total = 0f;
        foreach (float w in weights) total += w;

        float roll = _rng.RandfRange(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative) return i;
        }
        return weights.Length - 1;
    }

    /// <summary>
    /// 判断是否命中（概率 0~1）
    /// </summary>
    public static bool RollChance(float chance)
    {
        return _rng.Randf() <= chance;
    }

    /// <summary>
    /// 在范围内生成随机整数
    /// </summary>
    public static int RandRange(int min, int max)
    {
        return _rng.RandiRange(min, max);
    }

    /// <summary>
    /// 在范围内生成随机浮点数
    /// </summary>
    public static float RandRange(float min, float max)
    {
        return _rng.RandfRange(min, max);
    }
}


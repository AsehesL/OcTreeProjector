using UnityEngine;
using System.Collections;

/// <summary>
/// Bounds扩展类
/// </summary>
public static class BoundsEx
{
    /// <summary>
    /// 绘制Bounds
    /// </summary>
    /// <param name="bounds">包围盒</param>
    /// <param name="H">H</param>
    /// <param name="S">S</param>
    /// <param name="V">V</param>
    public static void DrawBounds(this Bounds bounds, float H, float S, float V)
    {
        if (H > 1)
            H = 0;
        Color col = Color.HSVToRGB(H, S, V);
        DrawBounds(bounds, col);
    }

    /// <summary>
    /// 绘制Bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    public static void DrawBounds(this Bounds bounds, Color color)
    {
        Gizmos.color = color;

        Vector3 p1 = new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2, bounds.center.z - bounds.size.z / 2);
        Vector3 p2 = new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2, bounds.center.z - bounds.size.z / 2);
        Vector3 p3 = new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2, bounds.center.z + bounds.size.z / 2);
        Vector3 p4 = new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2, bounds.center.z + bounds.size.z / 2);

        Vector3 p5 = new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2, bounds.center.z - bounds.size.z / 2);
        Vector3 p6 = new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2, bounds.center.z - bounds.size.z / 2);
        Vector3 p7 = new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2, bounds.center.z + bounds.size.z / 2);
        Vector3 p8 = new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2, bounds.center.z + bounds.size.z / 2);

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p6, p7);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p8, p5);

        Gizmos.DrawLine(p1, p5);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p3, p7);
        Gizmos.DrawLine(p4, p8);
    }

    /// <summary>
    /// 是否包含另一个Bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="compareTo"></param>
    /// <returns></returns>
    public static bool IsBoundsContainsAnotherBounds(this Bounds bounds, Bounds compareTo)
    {
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, -compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, -compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, -compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, -compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        return true;
    }
}

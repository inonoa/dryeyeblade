using static UnityEngine.Mathf;
using System;
using UniRx;
using UnityEngine;

public enum Dir8
{
    None, R, RU, U, LU, L, LD, D, RD
}

public static class Dir8Extension
{
    static readonly float invRoot2 = 0.707f;
    
    public static Vector2 ToVec2(this Dir8 dir)
    {
        switch (dir)
        {
            case Dir8.None: return Vector2.zero;
            
            case Dir8.R:  return new Vector2(  1,           0);
            case Dir8.RU: return new Vector2(  invRoot2,    invRoot2);
            case Dir8.U:  return new Vector2(  0,           1);
            case Dir8.LU: return new Vector2(- invRoot2,    invRoot2);
            case Dir8.L:  return new Vector2(- 1,           0);
            case Dir8.LD: return new Vector2(- invRoot2,  - invRoot2);
            case Dir8.D:  return new Vector2(  0,         - 1);
            case Dir8.RD: return new Vector2(  invRoot2,  - invRoot2);
        }
        throw new ArgumentException();
    }

    public static Dir8 ToDir8(this Vector2 vec)
    {
        if (vec == Vector2.zero) return Dir8.None;
            
        float angle = Atan2(vec.y, vec.x);
        
        if (angle < -7 * PI / 8) return Dir8.L;
        if (angle < -5 * PI / 8) return Dir8.LD;
        if (angle < -3 * PI / 8) return Dir8.D;
        if (angle < -1 * PI / 8) return Dir8.RD;
        if (angle <      PI / 8) return Dir8.R;
        if (angle < 3  * PI / 8) return Dir8.RU;
        if (angle < 5  * PI / 8) return Dir8.U;
        if (angle < 7  * PI / 8) return Dir8.LU;
        return Dir8.L;
    }

    public static Dir8 Random()
    {
        return (Dir8) UnityEngine.Random.Range(1, 9);
    }

    public static bool IsR(this Dir8 dir)
    {
        return dir == Dir8.R || dir == Dir8.RD || dir == Dir8.RU;
    }
    public static bool IsL(this Dir8 dir)
    {
        return dir == Dir8.L || dir == Dir8.LD || dir == Dir8.LU;
    }
}
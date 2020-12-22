
using System;
using UniRx;
using UnityEngine;

public enum Dir8
{
    R, RU, U, LU, L, LD, D, RD
}

public static class Dir8Extension
{
    static readonly float invRoot2 = 0.707f;
    
    public static Vector2 ToVec2(this Dir8 dir)
    {
        switch (dir)
        {
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
}
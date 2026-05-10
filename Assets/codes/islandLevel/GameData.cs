using UnityEngine;

public static class GameData
{
    public const int MAX_BULLETS = 10;

    public static float[] bulletPosX     = new float[MAX_BULLETS];
    public static float[] bulletPosY     = new float[MAX_BULLETS];
    public static float[] bulletSpeed    = new float[MAX_BULLETS];
    public static float[] bulletLifeTime = new float[MAX_BULLETS];
    public static bool[]  bulletActive   = new bool[MAX_BULLETS];
    public static int bulletCount = 0;

    public static float cannonPosX;
    public static float cannonPosY;
    public static float cannonFireRate;
    public static float cannonNextFireTime;
}
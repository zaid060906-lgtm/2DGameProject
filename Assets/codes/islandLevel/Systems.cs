public static class BulletMoveSystem
{
    public static void Update(float deltaTime)
    {
        for (int i = 0; i < GameData.bulletCount; i++)
        {
            if (!GameData.bulletActive[i]) continue;

            // لفوق بدل اليمين
            GameData.bulletPosY[i] += GameData.bulletSpeed[i] * deltaTime;

            GameData.bulletLifeTime[i] -= deltaTime;

            if (GameData.bulletLifeTime[i] <= 0f)
                GameData.bulletActive[i] = false;
        }
    }
}

public static class CannonFireSystem
{
    public static void Update(float currentTime)
    {
        if (currentTime < GameData.cannonNextFireTime) return;

        SpawnBullet(GameData.cannonPosX, GameData.cannonPosY);
        GameData.cannonNextFireTime = currentTime + GameData.cannonFireRate;
    }

    static void SpawnBullet(float x, float y)
    {
        for (int i = 0; i < GameData.MAX_BULLETS; i++)
        {
            if (!GameData.bulletActive[i])
            {
                GameData.bulletPosX[i]     = x;
                GameData.bulletPosY[i]     = y;
                GameData.bulletSpeed[i]    = 8f;
                GameData.bulletLifeTime[i] = 2f;
                GameData.bulletActive[i]   = true;

                if (i >= GameData.bulletCount)
                    GameData.bulletCount = i + 1;

                return;
            }
        }
    }
}
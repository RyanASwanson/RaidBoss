using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TagStringData
{
    private const string _heroHitboxTagName = "HeroHitbox";
    private const string _bossHitboxTagName = "BossHitbox";

    public static string GetHeroHitboxTagName() => _heroHitboxTagName;
    public static string GetBossHitboxTagName() => _bossHitboxTagName;
}

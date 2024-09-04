using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the constant strings for important tags
/// </summary>
public static class TagStringData
{
    private const string HERO_HITBOX_TAG_NAME = "HeroHitbox";
    private const string BOSS_HITBOX_TAG_NAME = "BossHitbox";

    public static string GetHeroHitboxTagName() => HERO_HITBOX_TAG_NAME;
    public static string GetBossHitboxTagName() => BOSS_HITBOX_TAG_NAME;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the constant strings for important tags
/// </summary>
public static class TagStringData
{
    private const string BOSS_HITBOX_TAG_NAME = "BossHitbox";
    private const string BOSS_MINION_HITBOX_TAG_NAME = "BossMinionHitbox";
    private const string HERO_HITBOX_TAG_NAME = "HeroHitbox";
    
    public static string GetBossHitboxTagName() => BOSS_HITBOX_TAG_NAME;
    public static string GetBossMinionHitboxTagName() => BOSS_MINION_HITBOX_TAG_NAME;
    public static string GetHeroHitboxTagName() => HERO_HITBOX_TAG_NAME;

    public static bool DoesColliderBelongToBoss(Collider collision)
    {
        return collision.gameObject.CompareTag(BOSS_HITBOX_TAG_NAME);
    }

    public static bool DoesColliderBelongToBossMinion(Collider collision)
    {
        return collision.gameObject.CompareTag(BOSS_MINION_HITBOX_TAG_NAME);
    }
    
    public static bool DoesColliderBelongToHero(Collider collision)
    {
        return collision.gameObject.CompareTag(HERO_HITBOX_TAG_NAME);
    }
}

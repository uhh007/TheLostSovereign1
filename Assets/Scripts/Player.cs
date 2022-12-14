using UnityEngine;

public class Player
{
    public static float FULL_HP = 100f;
    public static float FULL_STAMINA = 80f;
    public static int TimeWhenTakedDamage;
    public static int TimeWhenRun = 0;
    public static int TimeAfterRun = 0;

    public static GameObject gameObject;
    public static float Damage = 10f;
    public static float CriticalDamage = 49f;
    public static float CriticalDamageChance = 0.02f;
    public static float Stamina = FULL_STAMINA;
    public static float StaminaRegeneration = 9f;
    public static float Health = FULL_HP;
    public static float HealthRegeneration = 8f;
    public static float HealthRegenerationTimeout = 6f;
    public static int PlayerLevel = 1;
    public static int SkillPoints = 10;
    public static float PlayerExperience = 0f;
    public static float PlayerKills = 0f;
    public static float Money = 1000f;

    public static bool isDied = false;
}
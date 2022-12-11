using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static GameObject playerGameObject;
    public static float playerXPosition;
    public static float playerYPosition;
    public static float playerZPosition;

    public static float playerXRotation;
    public static float playerYRotation;
    public static float playerZRotation;
    public static float playerWRotation;

    public static float playerDamage;
    public static float playerCriticalDamage;
    public static float playerCriticalDamageChance;
    public static float playerStamina;
    public static float playerStaminaRegeneration;
    public static float playerHealth;
    public static float playerHealthRegeneration;
    public static float playerHealthRegenerationTimeout;
    public static int playerLevel;
    public static int playerSkillPoints;
    public static float playerExperience;
    
    public static void SaveGame()
    {
        ResetData();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Checkpoint.dat");
        SaveData data = new SaveData();

        data.savedPlayerXPosition = playerXPosition;
        data.savedPlayerYPosition = playerYPosition;
        data.savedPlayerZPosition = playerZPosition;
 
        data.savedPlayerXRotation = playerXRotation;
        data.savedPlayerYRotation = playerYRotation;
        data.savedPlayerZRotation = playerZRotation;
        data.savedPlayerWRotation = playerWRotation;

        data.savedPlayerDamage = playerDamage;
        data.savedPlayerCriticalDamage = playerCriticalDamage;
        data.savedPlayerCriticalDamageChance = playerCriticalDamageChance;
        data.savedPlayerStamina = playerStamina;
        data.savedPlayerStaminaRegeneration = playerStaminaRegeneration;
        data.savedPlayerHealth = playerHealth;
        data.savedPlayerHealthRegeneration = playerHealthRegeneration;
        data.savedPlayerHealthRegenerationTimeout = playerHealthRegenerationTimeout;
        data.savedPlayerLevel = playerLevel;
        data.savedPlayerSkillPoints = playerSkillPoints;
        data.savedPlayerExperience = playerExperience;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/Checkpoint.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Checkpoint.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            playerXPosition = data.savedPlayerXPosition;
            playerYPosition = data.savedPlayerYPosition;
            playerZPosition = data.savedPlayerZPosition;

            playerXRotation = data.savedPlayerXRotation;
            playerYRotation = data.savedPlayerYRotation;
            playerZRotation = data.savedPlayerZRotation;
            playerWRotation = data.savedPlayerWRotation;

            playerDamage = data.savedPlayerDamage;
            playerCriticalDamage = data.savedPlayerCriticalDamage;
            playerCriticalDamageChance = data.savedPlayerCriticalDamageChance;
            playerStamina = data.savedPlayerStamina;
            playerStaminaRegeneration = data.savedPlayerStaminaRegeneration;
            playerHealth = data.savedPlayerHealth;
            playerHealthRegeneration = data.savedPlayerHealthRegeneration;
            playerHealthRegenerationTimeout = data.savedPlayerHealthRegenerationTimeout;
            playerLevel = data.savedPlayerLevel;
            playerSkillPoints = data.savedPlayerSkillPoints;
            playerExperience = data.savedPlayerExperience;

            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }

    public static void ResetData()
    {
        if (File.Exists(Application.persistentDataPath + "/Checkpoint.dat"))
        {
            File.Delete(Application.persistentDataPath + "/Checkpoint.dat");

            CheckpointInitialize();
            //playerGameObject = null;
            //playerDamage = 0f;
            //playerCriticalDamage = 0f;
            //playerCriticalDamageChance = 0f;
            //playerStamina = 0f;
            //playerStaminaRegeneration = 0f;
            //playerHealth = 0f;
            //playerHealthRegeneration = 0f;
            //playerHealthRegenerationTimeout = 0f;
            //playerLevel = 0;
            //playerSkillPoints = 0;
            //playerExperience = 0f;
            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");
    }

    public static void CheckpointInitialize()
    {
        playerXPosition = Player.gameObject.transform.position.x;
        playerYPosition = Player.gameObject.transform.position.y;
        playerZPosition = Player.gameObject.transform.position.z;

        playerXRotation = Player.gameObject.transform.rotation.x;
        playerYRotation = Player.gameObject.transform.rotation.y;
        playerZRotation = Player.gameObject.transform.rotation.z;
        playerWRotation = Player.gameObject.transform.rotation.w;

        playerDamage = Player.Damage;
        playerCriticalDamage = Player.CriticalDamage;
        playerCriticalDamageChance = Player.CriticalDamageChance;
        playerStamina = Player.Stamina;
        playerStaminaRegeneration = Player.StaminaRegeneration;
        playerHealth = Player.Health;
        playerHealthRegeneration = Player.HealthRegeneration;
        playerHealthRegenerationTimeout = Player.HealthRegenerationTimeout;
        playerLevel = Player.PlayerLevel;
        playerSkillPoints = Player.SkillPoints;
        playerExperience = Player.PlayerExperience;
    }

    public static void LoadPlayer()
    {
        Player.gameObject.transform.position = new Vector3(playerXPosition, playerYPosition, playerZPosition);
        Player.gameObject.transform.rotation.Set(playerXRotation, playerYRotation, playerZRotation, playerWRotation);

        Player.Damage = playerDamage;
        Player.CriticalDamage = playerCriticalDamage;
        Player.CriticalDamageChance = playerCriticalDamageChance;
        Player.Stamina = playerStamina;
        Player.StaminaRegeneration = playerStaminaRegeneration;
        Player.Health = playerHealth;
        Player.HealthRegeneration = playerHealthRegeneration;
        Player.HealthRegenerationTimeout = playerHealthRegenerationTimeout;
        Player.PlayerLevel = playerLevel;
        Player.SkillPoints = playerSkillPoints;
        Player.PlayerExperience = playerExperience;
    }
}

[Serializable]

class SaveData
{
    public float savedPlayerXPosition;
    public float savedPlayerYPosition;
    public float savedPlayerZPosition;

    public float savedPlayerXRotation;
    public float savedPlayerYRotation;
    public float savedPlayerZRotation;
    public float savedPlayerWRotation;

    public float savedPlayerDamage;
    public float savedPlayerCriticalDamage;
    public float savedPlayerCriticalDamageChance;
    public float savedPlayerStamina;
    public float savedPlayerStaminaRegeneration;
    public float savedPlayerHealth;
    public float savedPlayerHealthRegeneration;
    public float savedPlayerHealthRegenerationTimeout;
    public int savedPlayerLevel;
    public int savedPlayerSkillPoints;
    public float savedPlayerExperience;
}

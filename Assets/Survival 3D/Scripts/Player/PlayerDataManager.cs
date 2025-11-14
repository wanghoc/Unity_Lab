using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public float health;
    public float hunger;
    public float thirst;
    public float sleep;
    public float posX, posY, posZ;
    public float timeOfDay;
    
    public PlayerData(PlayerNeeds needs, Transform playerTransform, float currentTime)
    {
        // Save needs
        health = needs.health.currentValue;
        hunger = needs.hunger.currentValue;
        thirst = needs.thirst.currentValue;
        sleep = needs.sleep.currentValue;
        
        // Save position
        posX = playerTransform.position.x;
        posY = playerTransform.position.y;
        posZ = playerTransform.position.z;
        
        // Save time of day
        timeOfDay = currentTime;
    }
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;
    
    private string saveFilePath;
    
    [Header("Auto Save")]
    public bool enableAutoSave = true;
    public float autoSaveInterval = 60f; // Tự động save mỗi 60 giây
    private float lastSaveTime;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        saveFilePath = Application.persistentDataPath + "/playerdata.json";
        Debug.Log($"💾 Save file path: {saveFilePath}");
    }
    
    void Update()
    {
        // Auto save
        if (enableAutoSave && Time.time - lastSaveTime > autoSaveInterval)
        {
            SaveGame();
            lastSaveTime = Time.time;
        }
    }
    
    public void SaveGame()
    {
        if (PlayerNeeds.instance == null || PlayerController.instance == null)
        {
            Debug.LogWarning("⚠️ Cannot save: Player not found!");
            return;
        }
        
        float currentTime = DayNight.instance != null ? DayNight.instance.time : 0.5f;
        
        PlayerData data = new PlayerData(
            PlayerNeeds.instance, 
            PlayerController.instance.transform,
            currentTime
        );
        
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        
        Debug.Log("💾 Game saved successfully!");
    }
    
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("📂 No save file found. Starting new game.");
            return;
        }
        
        string json = File.ReadAllText(saveFilePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        
        // Load needs
        if (PlayerNeeds.instance != null)
        {
            PlayerNeeds.instance.health.currentValue = data.health;
            PlayerNeeds.instance.hunger.currentValue = data.hunger;
            PlayerNeeds.instance.thirst.currentValue = data.thirst;
            PlayerNeeds.instance.sleep.currentValue = data.sleep;
        }
        
        // Load position
        if (PlayerController.instance != null)
        {
            PlayerController.instance.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        }
        
        // Load time
        if (DayNight.instance != null)
        {
            DayNight.instance.time = data.timeOfDay;
        }
        
        Debug.Log("📂 Game loaded successfully!");
    }
    
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }
    
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("🗑️ Save file deleted!");
        }
    }
    
    // Gọi khi player chết hoặc quit game
    void OnApplicationQuit()
    {
        if (enableAutoSave)
        {
            SaveGame();
        }
    }
}

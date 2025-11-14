using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public GameObject zombiePrefab; // Enemy Zombie prefab
        public Transform spawnPosition; // Vị trí spawn
    }

    [Header("Wave Settings")]
    public Wave[] waves; // Danh sách các waves
    private int currentWaveIndex = 0;
    private GameObject currentZombie;
    private bool waveActive = false;

    [Header("Star System")]
    public int currentStars = 0;
    public GameObject starPrefab; // Prefab ngôi sao UI
    public Transform starContainer; // Container chứa stars trong UI
    private List<GameObject> spawnedStars = new List<GameObject>();

    [Header("Events")]
    public UnityEvent<int> onWaveComplete; // Event khi hoàn thành wave (số sao)
    public UnityEvent onAllWavesComplete; // Event khi hoàn thành tất cả

    [Header("UI")]
    public TMPro.TextMeshProUGUI waveText; // Hiển thị "Wave X/Y"
    
    [Header("Audio & Effects")]
    public AudioClip waveStartSound; // Âm thanh bắt đầu wave
    public AudioClip waveCompleteSound; // Âm thanh hoàn thành wave
    public AudioClip allWavesCompleteSound; // Âm thanh hoàn thành tất cả
    public GameObject waveCompleteEffect; // Particle effect khi hoàn thành wave
    private AudioSource audioSource;
    
    public static WaveManager instance;

    private void Awake()
    {
        instance = this;
        
        // Tạo AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        UpdateWaveUI();
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("🎉 All waves completed!");
            PlaySound(allWavesCompleteSound);
            onAllWavesComplete?.Invoke();
            return;
        }

        waveActive = true;
        Wave currentWave = waves[currentWaveIndex];
        
        Debug.Log($"🌊 Starting {currentWave.waveName}");
        
        // Play wave start sound
        PlaySound(waveStartSound);
        
        // Spawn zombie
        if (currentWave.zombiePrefab != null && currentWave.spawnPosition != null)
        {
            currentZombie = Instantiate(
                currentWave.zombiePrefab, 
                currentWave.spawnPosition.position, 
                currentWave.spawnPosition.rotation
            );
            
            // Subscribe to zombie death
            NPC zombieNPC = currentZombie.GetComponent<NPC>();
            if (zombieNPC != null)
            {
                // Add listener for death - UnityEvent dùng AddListener thay vì +=
                zombieNPC.onDeath.AddListener(OnZombieDeath);
            }
        }
        else
        {
            Debug.LogError($"⚠️ Wave {currentWaveIndex + 1}: Missing prefab or spawn position!");
        }

        UpdateWaveUI();
    }

    private void OnZombieDeath()
    {
        if (!waveActive) return;

        Debug.Log($"✅ Zombie defeated! Wave {currentWaveIndex + 1} complete!");
        
        waveActive = false;
        
        // Play wave complete sound
        PlaySound(waveCompleteSound);
        
        // Spawn particle effect
        if (waveCompleteEffect != null && currentZombie != null)
        {
            Instantiate(waveCompleteEffect, currentZombie.transform.position, Quaternion.identity);
        }
        
        // Tăng số sao
        currentStars++;
        AddStar();
        
        // Trigger event
        onWaveComplete?.Invoke(currentStars);
        
        // Chuyển sang wave tiếp theo
        currentWaveIndex++;
        
        // Delay trước khi spawn wave mới
        Invoke("StartNextWave", 2f);
    }

    private void AddStar()
    {
        if (starPrefab != null && starContainer != null)
        {
            GameObject star = Instantiate(starPrefab, starContainer);
            spawnedStars.Add(star);
            
            Debug.Log($"⭐ Stars: {currentStars}/{waves.Length}");
        }
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            if (currentWaveIndex < waves.Length)
            {
                waveText.text = $"Wave {currentWaveIndex + 1}/{waves.Length}";
            }
            else
            {
                waveText.text = $"Complete! ⭐{currentStars}";
            }
        }
    }

    public Transform GetCurrentZombiePosition()
    {
        if (currentZombie != null)
        {
            return currentZombie.transform;
        }
        return null;
    }

    public bool IsWaveActive()
    {
        return waveActive && currentZombie != null;
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

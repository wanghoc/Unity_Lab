using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegeneration : MonoBehaviour
{
    [Header("Regeneration Settings")]
    public bool enableRegen = true;
    public float regenRate = 1f; // Máu hồi mỗi giây
    public float regenDelay = 5f; // Delay sau khi bị damage
    
    [Header("Conditions")]
    public bool requireFullHunger = false; // Cần no đủ mới hồi máu?
    public float minHungerPercent = 0.5f; // Tối thiểu 50% hunger
    public bool requireFullThirst = false;
    public float minThirstPercent = 0.5f;
    
    [Header("Resting Bonus")]
    public bool bonusWhileResting = true; // Hồi nhanh hơn khi ngủ
    public float restingMultiplier = 2f; // Hồi gấp đôi khi nghỉ ngơi
    
    private float timeSinceLastDamage;
    private PlayerNeeds playerNeeds;
    
    void Start()
    {
        playerNeeds = GetComponent<PlayerNeeds>();
        
        if (playerNeeds == null)
        {
            Debug.LogError("⚠️ HealthRegeneration: PlayerNeeds component not found!");
            enabled = false;
            return;
        }
        
        // Subscribe to damage event
        playerNeeds.onTakeDamage.AddListener(OnPlayerDamaged);
        
        timeSinceLastDamage = regenDelay; // Bắt đầu với delay đủ để hồi máu
    }
    
    void Update()
    {
        if (!enableRegen || playerNeeds == null)
            return;
        
        timeSinceLastDamage += Time.deltaTime;
        
        // Kiểm tra điều kiện để hồi máu
        if (CanRegenerateHealth())
        {
            float regenAmount = regenRate * Time.deltaTime;
            
            // Bonus khi nghỉ ngơi (sleeping)
            if (bonusWhileResting && IsResting())
            {
                regenAmount *= restingMultiplier;
            }
            
            playerNeeds.Heal(regenAmount);
        }
    }
    
    bool CanRegenerateHealth()
    {
        // Phải đợi đủ delay sau khi bị damage
        if (timeSinceLastDamage < regenDelay)
            return false;
        
        // Máu đã đầy
        if (playerNeeds.health.currentValue >= playerNeeds.health.maxValue)
            return false;
        
        // Kiểm tra hunger
        if (requireFullHunger)
        {
            float hungerPercent = playerNeeds.hunger.GetPercentage();
            if (hungerPercent < minHungerPercent)
                return false;
        }
        
        // Kiểm tra thirst
        if (requireFullThirst)
        {
            float thirstPercent = playerNeeds.thirst.GetPercentage();
            if (thirstPercent < minThirstPercent)
                return false;
        }
        
        return true;
    }
    
    bool IsResting()
    {
        // Coi như đang nghỉ nếu không di chuyển
        if (PlayerController.instance != null)
        {
            Rigidbody rig = PlayerController.instance.GetComponent<Rigidbody>();
            if (rig != null)
            {
                // Nếu vận tốc thấp = đang đứng yên
                return rig.linearVelocity.magnitude < 0.1f;
            }
        }
        return false;
    }
    
    void OnPlayerDamaged()
    {
        // Reset timer khi bị damage
        timeSinceLastDamage = 0f;
    }
    
    void OnDestroy()
    {
        if (playerNeeds != null)
        {
            playerNeeds.onTakeDamage.RemoveListener(OnPlayerDamaged);
        }
    }
}

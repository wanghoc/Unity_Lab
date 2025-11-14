using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickItemHotbar : MonoBehaviour
{
    [System.Serializable]
    public class HotbarSlot
    {
        public KeyCode keyCode; // Phím để dùng item (1,2,3,4...)
        public ItemDatabase assignedItem; // Item được gán vào slot
        public Image iconImage; // Icon UI
        public TMPro.TextMeshProUGUI countText; // Số lượng
        [HideInInspector] public int count = 0;
    }
    
    [Header("Hotbar Slots")]
    public HotbarSlot[] hotbarSlots = new HotbarSlot[5]; // 5 slots cho phím 1-5
    
    [Header("Settings")]
    public Color emptySlotColor = new Color(1f, 1f, 1f, 0.3f);
    public Color filledSlotColor = new Color(1f, 1f, 1f, 1f);
    
    [Header("Audio")]
    public AudioClip useItemSound;
    private AudioSource audioSource;
    
    public static QuickItemHotbar instance;
    
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        UpdateAllSlotsUI();
    }
    
    void Update()
    {
        // Kiểm tra input cho mỗi hotbar slot
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (Input.GetKeyDown(hotbarSlots[i].keyCode))
            {
                UseHotbarItem(i);
            }
        }
    }
    
    public void AssignItemToSlot(int slotIndex, ItemDatabase item)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length)
            return;
        
        hotbarSlots[slotIndex].assignedItem = item;
        hotbarSlots[slotIndex].count = GetItemCountInInventory(item);
        UpdateSlotUI(slotIndex);
        
        Debug.Log($"📦 Assigned {item.displayName} to hotbar slot {slotIndex + 1}");
    }
    
    public void UseHotbarItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length)
            return;
        
        HotbarSlot slot = hotbarSlots[slotIndex];
        
        if (slot.assignedItem == null)
        {
            Debug.Log($"⚠️ Hotbar slot {slotIndex + 1} is empty!");
            return;
        }
        
        // Kiểm tra xem có item trong inventory không
        if (Inventory.instance.HasItem(slot.assignedItem, 1))
        {
            // Sử dụng item
            ConsumeItem(slot.assignedItem);
            
            // Remove 1 item từ inventory
            Inventory.instance.RemoveItem(slot.assignedItem);
            
            // Update count
            slot.count = GetItemCountInInventory(slot.assignedItem);
            UpdateSlotUI(slotIndex);
            
            // Play sound
            if (useItemSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(useItemSound);
            }
            
            Debug.Log($"✅ Used {slot.assignedItem.displayName} from hotbar slot {slotIndex + 1}");
        }
        else
        {
            Debug.Log($"❌ No {slot.assignedItem.displayName} in inventory!");
        }
    }
    
    void ConsumeItem(ItemDatabase item)
    {
        if (PlayerNeeds.instance == null)
            return;
        
        // Áp dụng hiệu ứng của item từ consumables array
        if (item.type == ItemType.Consumable && item.consumables != null)
        {
            for (int i = 0; i < item.consumables.Length; i++)
            {
                switch (item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        PlayerNeeds.instance.Heal(item.consumables[i].value);
                        break;
                    case ConsumableType.Hunger:
                        PlayerNeeds.instance.Eat(item.consumables[i].value);
                        break;
                    case ConsumableType.Thirst:
                        PlayerNeeds.instance.Drink(item.consumables[i].value);
                        break;
                    case ConsumableType.Sleep:
                        PlayerNeeds.instance.Sleep(item.consumables[i].value);
                        break;
                }
            }
        }
        else if (item.type == ItemType.Resource)
        {
            Debug.Log($"❌ Cannot consume resource: {item.displayName}");
        }
        else if (item.type == ItemType.Equipable)
        {
            Debug.Log($"❌ Cannot quick-use equipable: {item.displayName}");
        }
    }
    
    int GetItemCountInInventory(ItemDatabase item)
    {
        if (Inventory.instance == null || item == null)
            return 0;
        
        return Inventory.instance.GetItemCount(item);
    }
    
    void UpdateSlotUI(int slotIndex)
    {
        HotbarSlot slot = hotbarSlots[slotIndex];
        
        if (slot.iconImage != null)
        {
            if (slot.assignedItem != null && slot.assignedItem.icon != null)
            {
                slot.iconImage.sprite = slot.assignedItem.icon;
                slot.iconImage.color = filledSlotColor;
            }
            else
            {
                slot.iconImage.sprite = null;
                slot.iconImage.color = emptySlotColor;
            }
        }
        
        if (slot.countText != null)
        {
            if (slot.count > 0)
            {
                slot.countText.text = slot.count.ToString();
                slot.countText.enabled = true;
            }
            else
            {
                slot.countText.enabled = false;
            }
        }
    }
    
    void UpdateAllSlotsUI()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            UpdateSlotUI(i);
        }
    }
    
    public void RefreshHotbar()
    {
        // Cập nhật số lượng tất cả các items
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].assignedItem != null)
            {
                hotbarSlots[i].count = GetItemCountInInventory(hotbarSlots[i].assignedItem);
                UpdateSlotUI(i);
            }
        }
    }
}

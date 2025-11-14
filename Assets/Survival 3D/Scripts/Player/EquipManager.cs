using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class EquipManager : MonoBehaviour
{
    public Equip currentEquip;
    public Transform equipParent;
    private PlayerController controller;
    
    
    //s
    public static EquipManager instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentEquip != null && controller.canLook == true)
        {
            currentEquip.OnAttackInput();
        }
    }
    
    public void OnAltAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && currentEquip != null && controller.canLook == true)
        {
            currentEquip.OnAltAttackInput();
        }
    }

    public void EquipNewItem(ItemDatabase item)
    {
        UnEquipItem();
        
        // Debug: Check if prefab and parent exist
        if (item.equipPrefab == null)
        {
            Debug.LogError($"⚠️ EQUIP PREFAB IS NULL for item: {item.displayName}!");
            Debug.LogError($"Fix: Select item '{item.displayName}' in Project → Inspector → Equip Items → Assign Equip Prefab");
            return;
        }
        
        if (equipParent == null)
        {
            Debug.LogError("⚠️ EQUIP PARENT IS NULL!");
            Debug.LogError("Fix: Player → Inspector → EquipManager → Assign Equip Parent transform");
            return;
        }
        
        // Spawn the equipment
        GameObject equippedObj = Instantiate(item.equipPrefab, equipParent);
        currentEquip = equippedObj.GetComponent<Equip>();
        
        if (currentEquip == null)
        {
            Debug.LogError($"⚠️ Prefab '{item.equipPrefab.name}' is missing Equip component!");
            Destroy(equippedObj);
            return;
        }
        
        // Reset local position/rotation to be relative to equipParent
        equippedObj.transform.localPosition = Vector3.zero;
        equippedObj.transform.localRotation = Quaternion.identity;
        
        // Force set layer to Default (ensure camera can see it)
        SetLayerRecursively(equippedObj, LayerMask.NameToLayer("Default"));
        
        // Check renderers
        var renderers = equippedObj.GetComponentsInChildren<Renderer>();
        Debug.Log($"✅ Successfully equipped: {item.displayName}");
        Debug.Log($"   World Position: {equippedObj.transform.position}");
        Debug.Log($"   Local Position: {equippedObj.transform.localPosition}");
        Debug.Log($"   Local Scale: {equippedObj.transform.localScale}");
        Debug.Log($"   Parent: {equipParent.name}");
        Debug.Log($"   Renderers found: {renderers.Length}");
        
        if (renderers.Length == 0)
        {
            Debug.LogWarning("⚠️ No Renderer found on equipped object! Object won't be visible!");
        }
        else
        {
            foreach (var r in renderers)
            {
                Debug.Log($"   - Renderer: {r.name}, Enabled: {r.enabled}, Layer: {LayerMask.LayerToName(r.gameObject.layer)}");
            }
        }
    }
    
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public void UnEquipItem()
    {
        if (currentEquip != null)
        {
            Destroy(currentEquip.gameObject);
            currentEquip = null;
        }
    }
    
    
}

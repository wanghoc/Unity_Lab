using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Cactus : MonoBehaviour
{

    public int damage;
    public float damageRate;
    
    [Header("Visual Feedback")]
    public Color damageColor = new Color(1f, 0.3f, 0.3f, 0.5f); // Màu đỏ nhạt
    public float flashDuration = 0.2f;
    private Renderer cactusRenderer;
    private Color originalColor;

    private List<IDamagable> thingsToDamage = new List<IDamagable>();
    
    private void Awake()
    {
        cactusRenderer = GetComponent<Renderer>();
        if (cactusRenderer != null)
        {
            originalColor = cactusRenderer.material.color;
        }
    }
    
    private void Start()
    {
        StartCoroutine(DealDamage());
    }
    //hasar arası gecikme (delay) gibi düşünülebilir.
    IEnumerator DealDamage()
    {
        while(true)
        {
            for (int i = 0; i < thingsToDamage.Count; i++)
            {
                thingsToDamage[i].TakePhysicDamage(damage);
                
                // Flash effect khi gây damage
                StartCoroutine(FlashEffect());
            }
            yield return new WaitForSeconds(damageRate);
        }
    }
    
    IEnumerator FlashEffect()
    {
        if (cactusRenderer != null)
        {
            cactusRenderer.material.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            cactusRenderer.material.color = originalColor;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Add(collision.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsToDamage.Remove(collision.gameObject.GetComponent<IDamagable>());
        }
    }
}

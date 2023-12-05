using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.stunnerOn = true;
            UIPowerupBar.instance.SetValue(1f);
            controller.PlaySound(collectedClip);
            Destroy(gameObject);
        }
    }
}

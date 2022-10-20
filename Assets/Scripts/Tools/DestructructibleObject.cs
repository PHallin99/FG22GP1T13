using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructructibleObject : MonoBehaviour
{
   public int currentHealth;
    
    void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(transform.parent.gameObject);

        }    
    }
}

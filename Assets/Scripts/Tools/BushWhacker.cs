using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushWhacker : MonoBehaviour
{
    [SerializeField] private DestructructibleObject DestructructibleObject;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Destroyable")
        {
            DestructructibleObject =  other.gameObject.GetComponent<DestructructibleObject>();
            DestructructibleObject.currentHealth++;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{

    
    void Awake()
    {
    
        if(transform.parent.tag == "Invisible Trash")
        { 
            transform.parent.Find("Trash").position = transform.position;
            transform.parent.Find("Trash").transform.parent = null;
            Destroy(transform.parent.gameObject);
        }
    }
}

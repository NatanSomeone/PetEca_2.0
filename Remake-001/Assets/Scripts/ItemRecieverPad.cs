using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRecieverPad : MonoBehaviour
{

    private void Awake()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="pickItem")
        {
            if (other.transform.parent.name=="ItemCollection")
            {
                Debug.Log("Item Entregue");
                Destroy(other.gameObject);
            }
        }
    }
}

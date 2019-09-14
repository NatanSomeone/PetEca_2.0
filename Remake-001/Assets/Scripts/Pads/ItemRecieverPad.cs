using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRecieverPad : MonoBehaviour
{

    private void Awake()
    {
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "pickItem")
        {
            Debug.Log($"Item Parent:{other.transform.parent.name}");
            if (other.transform.parent.name == "ItemCollection")
            {
                PersistentScript.currentScore += Random.Range(1, 3);
                Debug.Log("Item Entregue");
                other.GetComponent<Item>().PadParent.SpawnItem();
                Destroy(other.gameObject);
                //Spawn destructed version that has a rb with collider in a different mask ocluded by the itemHandler 
            }
        }

    }
}

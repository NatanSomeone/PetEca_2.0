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
            //Debug.Log($"Item Parent:{other.transform.parent.name}");
            if (other.transform.parent.name == "ItemCollection")
            {
                PersistentScript.currentScore += (PersistentScript.playType == 0) ? Random.Range(1, 3) : (PersistentScript.currentScore > 0) ? -1 : 0;
                other.GetComponent<Item>().PadParent.SpawnItem();
                Destroy(other.gameObject);
                //Spawn destructed version that has a rb with collider in a different mask ocluded by the itemHandler 


                if (PersistentScript.playType == 1 && PersistentScript.currentScore == 0)
                {
                    MenuManager_InGame.ShowGOverMessage((int)(1000f / PersistentScript.timeT0));
                        /*MenuManager_InGame.ShowInfoMessage($"Congrats ,  you got all the itens in {PersistentScript.timeT0}",true);*/ }
            }
        }

    }
}

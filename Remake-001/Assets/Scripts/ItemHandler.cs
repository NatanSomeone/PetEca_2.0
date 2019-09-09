using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{


    MovementDriver driver;

    static Transform ItemCollection;
    Transform cageTransform;
    Transform ItemHeld;
    Transform triggObj;

    bool graping;
    



    private void Awake()
    {
        if (ItemCollection == null) ItemCollection = GameObject.Find("ItemCollection").transform;
        driver = GetComponent<MovementDriver>();

        cageTransform= transform.Find("cage");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "pickItem")
        {
            triggObj = other.transform;
            if (ItemHeld == null)
            {
                if (driver.clawState)//gaiola levantada
                {
                    graping = true;
                }
                else if(graping) //Item é Pego
                {
                    triggObj.SetParent(cageTransform);
                    triggObj.localPosition = Vector3.zero;
                    ItemHeld = triggObj;
                    graping = false;
                }
            }

        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "pickItem")
        {
            if (ItemHeld != null)
            {
                if (driver.clawState)//Gaiola levantada
                {
                    triggObj.SetParent(cageTransform);
                }
                else // tá cheio
                {
                    Debug.Log("tá cheio");
                }
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        triggObj = null;
    }

    private void OnCollisionStay(Collision c)
    {
        //Vector3 dir = c.GetContact(0).point - transform.position;
        string cTag = c.gameObject.tag;
        if (cTag == "pickItem" && (!driver.clawState) && !graping && c.transform != triggObj)//se encostou e não está com a gaiola levantada, violou... fora
        {
            PersistentScript.incomingMessage = "Levante sua gaiola para pegar o objeto, sem empurrar!!";
            MenuManager_InGame.ReloadLevel();
        }else if (cTag == "walls")
        {
            PersistentScript.incomingMessage = "Mova sem colidir nas paredes!";
            MenuManager_InGame.ReloadLevel();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward);
    }

}

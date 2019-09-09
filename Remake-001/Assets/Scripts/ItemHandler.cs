using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{

    MovementDriver driver;

    Transform cageTransform;
    bool graping;


    private void Awake()
    {
        driver = GetComponent<MovementDriver>();

        cageTransform= transform.Find("cage");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "pickItem")
        {
            if (!driver.clawState)//gaiola levantada
            {
                graping = true;
            }
            else if(graping)
            {
                other.transform.SetParent(cageTransform);
                other.transform.localPosition = Vector3.zero;
            }

        }

    }

    private void OnCollisionStay(Collision c)
    {
        //Vector3 dir = c.GetContact(0).point - transform.position;
        string cTag = c.gameObject.tag;
        if (cTag == "pickItem" && (!driver.clawState) && !graping)//se encostou e não está com a gaiola levantada, violou... fora
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

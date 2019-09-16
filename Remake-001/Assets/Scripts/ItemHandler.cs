using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{


    MovementDriver driver;

    //public Transform ItemCollection;
    public Transform cageTransform;
    Transform ItemHeld;
    Transform triggObj;

    bool graping = true;




    private void Awake()
    {
        driver = GetComponent<MovementDriver>();
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
                else if (graping) //Item é Pego
                {
                    var cageCenter = cageTransform.GetChild(0).transform.position;
                    var c = new Vector3(cageCenter.x, 0.04f, cageCenter.z);

                    triggObj.SetParent(cageTransform.GetChild(0));
                    triggObj.localPosition = Vector3.zero;
                    triggObj.localRotation = Quaternion.identity;
                    ItemHeld = triggObj;
                    //graping = false;
                }
            }

        }

    }
    private void OnTriggerStay(Collider other)
    {

        //if (other.gameObject.tag == "pickItem")
        //{
        //    if (ItemHeld != null && other.gameObject != ItemHeld)
        //    {
        //        Debug.Log("tá cheio");
        //    }

        //}
    }
    private void OnTriggerExit(Collider other)
    {
        triggObj = null;
    }
    Vector3 dir;

    private void OnCollisionStay(Collision c)
    {
        //Vector3 dir = c.GetContact(0).point - transform.position;
        string cTag = c.gameObject.tag;
        //if (cTag == "pickItem" && (!driver.clawState) && !graping && c.transform != triggObj)//se encostou e não está com a gaiola levantada, violou... fora
        //{
        //    //PersistentScript.incomingMessage = "Levante sua gaiola para pegar o objeto, sem empurrar!!";
        //    //MenuManager_InGame.ReloadLevel();
        //}
        if (cTag == "pickItem") {
            dir = c.GetContact(0).normal;
            c.collider.transform.position -=new Vector3(dir.x, 0, dir.z)*Time.deltaTime*10f;
        }
        else if (cTag == "wall")
        {
            PersistentScript.incomingMessage = "Mova sem colidir nas paredes!\nSintoMuito, mas terá que tentar novamente";
            MenuManager_InGame.ReloadLevel();
        }
    }
    private void LateUpdate()
    {

        if (driver.clawState && ItemHeld != null) //se gaiola levantada e item dentro dropa
        {
            ItemHeld.SetParent(PersistentScript.persistentScript.ItemCollection);
            var p = ItemHeld.transform.position;
            p = new Vector3(p.x, 0.04f, p.z);
            ItemHeld.rotation = Quaternion.identity;
            ItemHeld = null;
        }
    }


    private void OnDrawGizmos()
    {
        //var r = 0.15f;
        //var cageCenter = cageTransform.GetComponent<Collider>().bounds.center;
        //var c = new Vector3(cageCenter.x, r + 1, cageCenter.z);

        //Gizmos.color = (ItemHeld == null) ? Color.red : Color.green;

        //Gizmos.DrawSphere(c, r);
        //Gizmos.DrawRay(c, transform.forward * .6f);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(c+transform.forward*0.6f, transform.forward*0.6f+dir * .6f);
    }

}

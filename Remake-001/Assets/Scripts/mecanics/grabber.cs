using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabber : MonoBehaviour
{
    public Transform Hand;
    public float GrabDuration;
    private Transform Item;



    public bool grabbing;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.GetComponent<Item>() != null)
        {
            Item = other.transform;
            Grab();
        }
            
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.transform == Item)
            Item = null;
    }
    public void Grab()
    {
        if (grabbing == false)
        {
            StartCoroutine(biezerProcess(Item, GrabDuration));   
        }
    }

    IEnumerator biezerProcess(Transform obj, float time)
    {
        grabbing = true;
        var t = 0f;
        var P0 = obj.position;
        var P2 = Hand.position;
        var flat = .1f;
        var P1 = P0 + new Vector3((P2.x - P0.x) * flat, (P2.y - P0.y) * 4f, (P2.z - P0.z) * flat);
        while (t < time)
        {
            t += Time.fixedDeltaTime;
            Hand.position = P2;
            var Q0 = Vector3.Lerp(P0, P1, t / time);
            var Q1 = Vector3.Lerp(P1, P2, t / time);
            var Pos = Vector3.Lerp(Q0, Q1, t / time);
            Debug.DrawLine(obj.position, Pos, Color.red, 5f);
            obj.position = Pos;
            yield return new WaitForFixedUpdate();
        }
        Item.SetParent(this.transform);
        grabbing = false;
        
    }
    
}

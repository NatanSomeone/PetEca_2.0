using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemTriggeredPad : MonoBehaviour
{
    //Esse Pad desabilita um objeto por determinada quantia de tempo
    public GameObject[] TargetObjs;
    public float timeOff;
    private bool running;
    private void Awake()
    {
        
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "pickItem")
        {
            //Debug.Log($"Item Parent:{other.transform.parent.name}");
            if (other.transform.parent.name == "ItemCollection" && running == false) // sendo um item solto no chão
            {
                running = true;
                StartCoroutine(WaitAndDo());
            }
        }

    }

    public IEnumerator WaitAndDo(float time)
    {
        TargetObjs.Select((o) => { o.SetActive(false);return true; });
        yield return new WaitForSeconds(time);
        TargetObjs.Select((o) => { o.SetActive(true);return true; });
    }
    public IEnumerator WaitAndDo()
    {
        var tScore = PersistentScript.currentScore;
        Debug.Log($"Waiting {tScore}");
        foreach (var obj in TargetObjs)
        {obj.SetActive(false);}
        yield return new WaitUntil(()=>PersistentScript.currentScore!=tScore);
        yield return new WaitForSeconds(5);
        foreach (var obj in TargetObjs)
        { obj.SetActive(true); }
        running = false;
    }
}

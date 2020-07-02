using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerPad : MonoBehaviour
{
    public GameObject Item;
    public int MaxSpawn;

    private void Start()
    {
        if (PersistentScript.playType == 1) { PersistentScript.currentScore+= MaxSpawn; }
        SpawnItem();
    }

    public void SpawnItem()
    {
        var o = Instantiate(Item, PersistentScript.instance.ItemCollection);
        o.transform.position = this.transform.position+ Vector3.up*0.04f;
        o.AddComponent<Item>().PadParent = this;
        o.tag = "pickItem";
    }
}

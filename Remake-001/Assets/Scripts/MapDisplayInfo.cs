using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName ="NewMapInfo",menuName ="_MapInfo")]
public class MapDisplayInfo : ScriptableObject
{
    public Sprite icon;
    public int scene = 1;
    public string description;
    public string longDescription;
    public GameObject MapPrefab;
    public float maxTime;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName ="NewMapInfo",menuName ="_MapInfo")]
public class MapDisplayInfo : ScriptableObject
{
    public Sprite icon;
    public int scene;
    public string description;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MapData : ScriptableObject
{
    public string mapName;
    public SceneField mapScene;
    public Sprite mapIcon;
}

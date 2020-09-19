using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

//各マップの地点のデータ
[System.SerializableAttribute]
public class MapData
{
    public int id;
    public GameObject MapObject;
    public List<int> NearMapData = new List<int>();
}

public class MapManager : MonoBehaviour
{
    // 道のコンポーネントリスト
    private List<LineRenderer> MapLoadList;
    public Material lineMaterial;
    public Color lineColor;
    [Range(0, 10)] public float lineWidth;

    public List<MapData> MapData = new List<MapData>();

    void Awake()
    {
        MapLoadList = new List<LineRenderer>();
    }

    void Start()
    {
        // 道を設定
        foreach (MapData MapPoint in MapData)
        {
            foreach (int near_index in MapPoint.NearMapData)
            {
                GameObject lineObject = new GameObject();
                lineObject.AddComponent<LineRenderer>();
                MapLoadList.Add(lineObject.GetComponent<LineRenderer>());
                MapLoadList.Last().positionCount = 0;
                MapLoadList.Last().material = this.lineMaterial;
                MapLoadList.Last().material.color = this.lineColor;
                MapLoadList.Last().startWidth = this.lineWidth;
                MapLoadList.Last().endWidth = this.lineWidth;
                MapLoadList.Last().positionCount++;
                MapLoadList.Last().SetPosition(MapLoadList.Last().positionCount - 1, MapPoint.MapObject.transform.position);
                MapLoadList.Last().positionCount++;
                MapLoadList.Last().SetPosition(MapLoadList.Last().positionCount-1, MapData.Find(x => x.id==near_index).MapObject.transform.position);
            }      
        }
    }
}

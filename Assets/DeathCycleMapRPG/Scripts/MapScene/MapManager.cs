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
    public GameObject mapObject;
    public List<int> nearMapData = new List<int>();
}

public class MapManager : MonoBehaviour
{
    // 道のコンポーネントリスト
    private List<LineRenderer> mapLoadList;
    public Material lineMaterial;
    public Color lineColor;
    [Range(0, 10)] public float lineWidth;

    public List<MapData> mapData = new List<MapData>();

    void Awake()
    {
        mapLoadList = new List<LineRenderer>();
    }

    void Start()
    {
        // 道を設定
        CreateLoad();
    }

    void CreateLoad()
    {
        foreach (MapData MapPoint in mapData)
        {
            foreach (int near_index in MapPoint.nearMapData)
            {
                GameObject lineObject = new GameObject();
                lineObject.AddComponent<LineRenderer>();
                mapLoadList.Add(lineObject.GetComponent<LineRenderer>());
                mapLoadList.Last().positionCount = 0;
                mapLoadList.Last().material = this.lineMaterial;
                mapLoadList.Last().material.color = this.lineColor;
                mapLoadList.Last().startWidth = this.lineWidth;
                mapLoadList.Last().endWidth = this.lineWidth;
                mapLoadList.Last().positionCount++;
                mapLoadList.Last().SetPosition(mapLoadList.Last().positionCount - 1, MapPoint.mapObject.transform.position);
                mapLoadList.Last().positionCount++;
                mapLoadList.Last().SetPosition(mapLoadList.Last().positionCount - 1, mapData.Find(x => x.id == near_index).mapObject.transform.position);
            }
        }
    }
}

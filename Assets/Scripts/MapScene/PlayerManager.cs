using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//プレイヤーのデータ
public class PlayerData
{
    //マップの現在地点
    private int current_point;
    public int CurrentPoint
    {
        get { return current_point; }
        set { current_point = value; }
    }

}

public class PlayerManager : MonoBehaviour
{
    PlayerData PlayerData = new PlayerData();
    public GameObject Player;
    public GameObject MapManager;
    public GameObject PlayerUI;
    public GameObject ArrowButtonPrefab;
    public List<GameObject> ArrowButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        PlayerData.CurrentPoint = 0;
        SetPosition(PlayerData.CurrentPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMoveClick()
    {
        PlayerUI.SetActive(false);
        MoveReady(PlayerData.CurrentPoint);
    }

    void MoveReady(int current_point)
    {
        MapManager Map = MapManager.GetComponent<MapManager>();
        List<int> NearData = Map.MapData[current_point].NearMapData;
        foreach (int next_point_id in NearData)
        {
            ArrowButtons.Add(Instantiate(ArrowButtonPrefab));
            Vector3 diff = Map.MapData[next_point_id].MapObject.transform.position - Map.MapData[current_point].MapObject.transform.position;
            ArrowButtons.Last().transform.position = Player.transform.position + 2 * diff.normalized;
            ArrowButtons.Last().transform.rotation = ArrowButtons.Last().transform.rotation * Quaternion.FromToRotation(Vector3.up, diff);
            ArrowButtons.Last().GetComponent<ArrowButtonManager>().next_point = next_point_id;
        }
    }

    public void MoveEnd()
    {
        for(int i=0;i<ArrowButtons.Count;i++)
        {
            Destroy(ArrowButtons[i]);
        }
        ArrowButtons.Clear();
    }

    public void SetPosition(int point)
    {
        MapManager Map = MapManager.GetComponent<MapManager>();
        Player.transform.position = Map.MapData[point].MapObject.transform.position;
        PlayerData.CurrentPoint = point;
    }

}

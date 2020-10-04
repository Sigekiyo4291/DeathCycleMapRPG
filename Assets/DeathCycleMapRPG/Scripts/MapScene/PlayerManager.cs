using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//プレイヤーのデータ
public class PlayerData
{
    //マップの現在地点
    private static int current_point = 0;
    public int CurrentPoint
    {
        get { return current_point; }
        set { current_point = value; }
    }

}

public class PlayerManager : MonoBehaviour
{
    PlayerData playerData;
    public GameObject player;
    public GameObject mapManager;
    public GameObject playerUI;
    public GameObject mapMoveUI;
    public GameObject mainCamara;
    public GameObject arrowButtonPrefab;
    public List<GameObject> arrowButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        playerData = new PlayerData();
        SetPosition(playerData.CurrentPoint);
        mainCamara = GameObject.Find("Main Camera");
        mainCamara.GetComponent<CameraManager>().CameraSetPositionToPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMoveSelectButtonClick()
    {
        MoveReady(playerData.CurrentPoint);
    }

    public void OnFormSelectButtonClick()
    {
        SceneManager.LoadScene("FormScene");
    }

    public void OnMoveCancelButtonClick()
    {
        MoveEnd();
    }

    void MoveReady(int current_point)
    {
        playerUI.SetActive(false);
        mapMoveUI.SetActive(true);
        MapManager Map = mapManager.GetComponent<MapManager>();
        List<int> NearData = Map.mapData[current_point].nearMapData;
        foreach (int next_point_id in NearData)
        {
            arrowButtons.Add(Instantiate(arrowButtonPrefab));
            Vector3 diff = Map.mapData[next_point_id].mapObject.transform.position - Map.mapData[current_point].mapObject.transform.position;
            arrowButtons.Last().transform.position = player.transform.position + 2 * diff.normalized;
            arrowButtons.Last().transform.rotation = arrowButtons.Last().transform.rotation * Quaternion.FromToRotation(Vector3.up, diff);
            arrowButtons.Last().GetComponent<ArrowButtonManager>().next_point = next_point_id;
        }
    }

    public void MoveEnd()
    {
        for(int i=0;i<arrowButtons.Count;i++)
        {
            Destroy(arrowButtons[i]);
        }
        arrowButtons.Clear();
        playerUI.SetActive(true);
        mapMoveUI.SetActive(false);
        mainCamara.GetComponent<CameraManager>().CameraSetPositionToPlayer();
    }

    public void SetPosition(int point)
    {
        MapManager map = mapManager.GetComponent<MapManager>();
        player.transform.position = map.mapData[point].mapObject.transform.position;
        playerData.CurrentPoint = point;
    }

    public void RandomEncount()
    {
        if (Random.value > 0.6)
        {
            SceneManager.LoadScene("BattleScene");
        }
    }
}

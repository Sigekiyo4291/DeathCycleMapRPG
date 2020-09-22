using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowButtonManager : MonoBehaviour
{
    public int next_point;
    private GameObject playerManagerObject;
    PlayerManager player;

    void Awake()
    {
        playerManagerObject = GameObject.Find("PlayerManager");
        player = playerManagerObject.GetComponent<PlayerManager>();
    }

    public void OnClick()
    {
        player.SetPosition(next_point);
        player.MoveEnd();
    }
}

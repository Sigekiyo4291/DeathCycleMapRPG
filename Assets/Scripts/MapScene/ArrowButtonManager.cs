using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowButtonManager : MonoBehaviour
{
    public int next_point;
    private GameObject PlayerManagerObject;
    PlayerManager Player;

    void Awake()
    {
        PlayerManagerObject = GameObject.Find("PlayerManager");
        Player = PlayerManagerObject.GetComponent<PlayerManager>();
    }

    public void OnClick()
    {
        Player.SetPosition(next_point);
        Player.MoveEnd();
        Player.PlayerUI.SetActive(true);
    }
}

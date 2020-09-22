using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[Serializable]
[CreateAssetMenu(fileName = "PartyStatus", menuName = "CreatePartyStatus")]
public class PartyStatus : ScriptableObject
{
    [SerializeField]
    private int money = 0;
    [SerializeField]
    private List<PlayerStatus> partyMembers = null;
 
    public void SetMoney(int money) {
        this.money = money;
    }
 
    public int GetMoney() {
        return money;
    }
 
    public void SetPlayerStatus(PlayerStatus PlayerStatus) {
        if (!partyMembers.Contains(PlayerStatus)) {
            partyMembers.Add(PlayerStatus);
        }
    }
 
    public List<PlayerStatus> GetPlayerStatus() {
        return partyMembers;
    }
 
}
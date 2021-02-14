using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "SaveData", menuName = "CreateSaveData")]
public class SaveData : ScriptableObject
{
    [SerializeField]
    private PartyStatus partyStatus = null;

    public void SetPartyStatus(PartyStatus partyStatus)
    {
        this.partyStatus = partyStatus;
    }

    public PartyStatus GetPartyStatus()
    {
        return partyStatus;
    }
}
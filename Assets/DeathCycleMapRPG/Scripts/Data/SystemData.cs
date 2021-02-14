using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "SystemData", menuName = "CreateSystemData")]
public class SystemData : ScriptableObject
{
    [SerializeField]
    private List<SaveData> SaveDatas = null;

    public void SetSaveData(SaveData PlayerStatus)
    {
        if (!SaveDatas.Contains(PlayerStatus))
        {
            SaveDatas.Add(PlayerStatus);
        }
    }

    public List<SaveData> GetSaveData()
    {
        return SaveDatas;
    }
}
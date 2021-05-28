using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
[CreateAssetMenu(fileName = "UserData", menuName = "CreateUserData")]
public class UserData : ScriptableObject
{
    [SerializeField]
    private SaveData saveData = null;
    [SerializeField]
    private SystemData systemData = null;
    [SerializeField]
    private bool testflag;

    public void SetSaveData(SaveData playingData)
    {
        saveData = playingData;
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void SetSystemData(SystemData playingData)
    {
        systemData = playingData;
    }

    public SystemData GetSystemData()
    {
        return systemData;
    }

    public bool GetTestFlag()
    {
        return testflag;
    }
}
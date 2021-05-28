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
    private int　bgm_volume = 1;

    [SerializeField]
    private int se_volume = 1;
    
}
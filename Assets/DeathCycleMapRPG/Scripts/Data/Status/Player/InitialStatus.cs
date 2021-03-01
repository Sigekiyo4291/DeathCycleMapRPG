using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "InitialStatus", menuName = "CreateInitialStatus")]
public class InitialStatus : CharacterStatus
{

    //　獲得経験値
    [SerializeField]
    private int earnedExperience = 0;
    //　装備している武器
    [SerializeField]
    private Item equipWeapon = null;
    //　装備している鎧
    [SerializeField]
    private Item equipArmor = null;
    //装備しているアクセサリ
    [SerializeField]
    private Item equipAccessory1 = null;
    [SerializeField]
    private Item equipAccessory2 = null;
}
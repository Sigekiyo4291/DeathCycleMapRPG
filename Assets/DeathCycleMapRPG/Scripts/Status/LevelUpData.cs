using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "LevelUpData", menuName = "CreateLevelUpData")]
public class LevelUpData : ScriptableObject
{
    // 任意のパラメーターの割り振りが可能かどうか
    [SerializeField]
    private bool isRaiseArbitrarily = false;
    //　素早さが上がった時のレベルごとのテーブル
    [SerializeField]
    private List<int> agilityRisingTable;
    public List<int> GetAgilityRisingTable()
    {
        return agilityRisingTable;
    }
    //　力が上がった時のレベルごとのテーブル
    [SerializeField]
    private List<int> powerRisingTable;
    public List<int> GetPowerRisingTable()
    {
        return powerRisingTable;
    }
    //　打たれ強さが上がった時のレベルごとのテーブル
    [SerializeField]
    private List<int> strikingStrengthRisingTable;
    public List<int> GetStrikingStrengthRisingTable()
    {
        return strikingStrengthRisingTable;
    }
    //　魔法力が上がった時のレベルごとのテーブル
    [SerializeField]
    private List<int> magicPowerRisingTable;
    public List<int> GetMagicPowerRisingTable()
    {
        return magicPowerRisingTable;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item
{
    //スキル
    [SerializeField]
    private Skill skill = null;
    // 命中率
    [SerializeField]
    private float HitRate = 100.0f;
    //ステータスアップの追加効果
    [SerializeField]
    private int upPower = 0;
    [SerializeField]
    private int upAgility = 0;
    [SerializeField]
    private int upMagic = 0;
    [SerializeField]
    private int upStrikingStrength = 0;
    //属性カット率
    [SerializeField]
    private int cutFlame = 0;
    [SerializeField]
    private int cutThunder = 0;
    [SerializeField]
    private int cutIce = 0;
        
    public Skill GetSkill()
    {
        return skill;
    }
    public int GetUpPower()
    {
        return upPower;
    }
    public int GetUpAgility()
    {
        return upAgility;
    }
    public int GetUpMagic()
    {
        return upMagic;
    }
    public int GetUpStrikingStrength()
    {
        return upStrikingStrength;
    }
    public int GetCutFlame()
    {
        return cutFlame;
    }
    public int GetCutThunder()
    {
        return cutThunder;
    }
    public int GetcutIce()
    {
        return cutIce;
    }
}

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterStatus : ScriptableObject
{
    //　キャラクターの名前
    [SerializeField]
    private string characterName = "";
    //　毒状態かどうか
    [SerializeField]
    private bool isPoisonState = false;
    //　痺れ状態かどうか
    [SerializeField]
    private bool isNumbnessState = false;
    //　キャラクターのレベル
    [SerializeField]
    private int level = 1;
    //　素早さ
    [SerializeField]
    private int agility = 5;
    //　力
    [SerializeField]
    private int power = 5;
    //　打たれ強さ
    [SerializeField]
    private int strikingStrength = 5;
    //　魔法力
    [SerializeField]
    private int magicPower = 5;
    // 攻撃力
    private int attackPower;
    // 守備力
    private int defencePower;
    //　最大HP
    [SerializeField]
    private int maxHp = 100;
    //　HP
    [SerializeField]
    private int hp = 100;
    //　最大MP
    [SerializeField]
    private int maxMp = 50;
    //　MP
    [SerializeField]
    private int mp = 50;
    //　持っているスキル
    [SerializeField]
    private List<Skill> skillList = null;
    //属性カット率
    [SerializeField]
    private int cutFlame = 0;
    [SerializeField]
    private int cutThunder = 0;
    [SerializeField]
    private int cutIce = 0;

    //キャラ名のGet,SET
    public void SetCharacterName(string characterName)
    {
        this.characterName = characterName;
    }

    public string GetCharacterName()
    {
        return characterName;
    }

    //毒状態のGet,SET
    public void SetPoisonState(bool poisonFlag)
    {
        isPoisonState = poisonFlag;
    }

    public bool IsPoisonState()
    {
        return isPoisonState;
    }

    //麻痺状態のGet,SET
    public void SetNumbness(bool numbnessFlag)
    {
        isNumbnessState = numbnessFlag;
    }

    public bool IsNumbnessState()
    {
        return isNumbnessState;
    }

    //レベルのGet,SET
    public void SetLevel(int level)
    {
        this.level = level;
    }

    public int GetLevel()
    {
        return level;
    }

    //最大HPのGet,SET
    public void SetMaxHp()
    {
        this.maxHp = 30 + this.level + this.strikingStrength * 4;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    //HPのGet,SET
    public void SetHp(int hp)
    {
        this.hp = Mathf.Max(0, Mathf.Min(GetMaxHp(), hp));
    }

    public int GetHp()
    {
        return hp;
    }

    //最大MPのGet,SET
    public void SetMaxMp()
    {
        this.maxMp = this.level + this.magicPower * 3;
    }

    public int GetMaxMp()
    {
        return maxMp;
    }

    //MPのGet,SET
    public void SetMp(int mp)
    {
        this.mp = Mathf.Max(0, Mathf.Min(GetMaxMp(), mp));
    }

    public int GetMp()
    {
        return mp;
    }

    //素早さのGet,SET
    public void SetAgility(int agility)
    {
        this.agility = agility;
    }

    public int GetAgility()
    {
        return agility;
    }

    //力のGet,SET
    public void SetPower(int power)
    {
        this.power = power;
    }

    public int GetPower()
    {
        return power;
    }

    //体力のGet,SET
    public void SetStrikingStrength(int strikingStrength)
    {
        this.strikingStrength = strikingStrength;
    }

    public int GetStrikingStrength()
    {
        return strikingStrength;
    }

    //魔力のGet,SET
    public void SetMagicPower(int magicPower)
    {
        this.magicPower = magicPower;
    }

    public int GetMagicPower()
    {
        return magicPower;
    }

    //攻撃力のGet,SET
    public void SetAttackPower(int attackPower)
    {
        this.attackPower = attackPower;
    }

    public int GetAttackPower()
    {
        return attackPower;
    }

    //防御力のGet,SET
    public void SetDefencePower(int defencePower)
    {
        this.defencePower = defencePower;
    }

    public int GetDefencePower()
    {
        return defencePower;
    }

    //スキルセットのGet,SET
    public void SetSkillList(List<Skill> skillList)
    {
        this.skillList = skillList;
    }

    public List<Skill> GetSkillList()
    {
        return skillList;
    }

    //属性耐性のGet,SET
    public void SetCutFlame(int cutFlame)
    {
        this.cutFlame = cutFlame;
    }
    public int GetCutFlame()
    {
        return cutFlame;
    }

    public void SetCutThunder(int cutThunder)
    {
        this.cutThunder = cutThunder;
    }
    public int GetCutThunder()
    {
        return cutThunder;
    }

    public void SetCutIce(int cutIce)
    {
        this.cutIce = cutIce;
    }
    public int GetcutIce()
    {
        return cutIce;
    }
}
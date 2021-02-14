using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Skill", menuName = "CreateSkill")]
public class Skill : ScriptableObject
{
    public enum Type
    {
        DirectAttack,
        Guard,
        GetAway,
        Item,
        SkillAttack,
        MagicAttack,
        RecoveryMagic,
        PoisonnouRecoveryMagic,
        NumbnessRecoveryMagic,
        IncreaseAttackPowerMagic,
        IncreaseDefencePowerMagic
    }

    public enum RangeType
    {
        None,
        SingleUnit,
        AllRange,
        Random
    }

    public enum AttributeType
    {
        None,
        Flame,
        Thunder,
        Ice
    }

    public enum SkillEffectType
    {
        None,
        AttackDown,
        DefenseDown,
        paralysis,
        poison,
        confusion
    }

    [SerializeField]
    private Type skillType = Type.DirectAttack;
    [SerializeField]
    private string kanjiName = "";
    [SerializeField]
    private string hiraganaName = "";
    [SerializeField]
    private string information = "";
    // 対象範囲
    [SerializeField]
    private RangeType rangeType = RangeType.None;
    // 属性
    [SerializeField]
    private AttributeType attributeType = AttributeType.None;
    // 倍率
    [SerializeField]
    private float magnification = 1.0f;
    // 実行回数
    [SerializeField]
    private int numOfExe = 1;
    // スキル効果
    [SerializeField]
    private SkillEffectType skillEffectType = SkillEffectType.None;
    //　命中率
    [SerializeField]
    private int hitRate = 100;

    //　使用者のエフェクト
    [SerializeField]
    private GameObject skillUserEffect = null;
    //　魔法を受ける側のエフェクト
    [SerializeField]
    private GameObject skillReceivingSideEffect = null;

    //　スキルの種類を返す
    public Type GetSkillType()
    {
        return skillType;
    }
    //　スキルの名前を返す
    public string GetKanjiName()
    {
        return kanjiName;
    }
    //　スキルの平仮名の名前を返す
    public string GetHiraganaName()
    {
        return hiraganaName;
    }
    //　スキル情報を返す
    public string GetInformation()
    {
        return information;
    }
    //　使用者のエフェクトを返す
    public GameObject GetSkillUserEffect()
    {
        return skillUserEffect;
    }
    //　魔法を受ける側のエフェクトを返す
    public GameObject GetSkillReceivingSideEffect()
    {
        return skillReceivingSideEffect;
    }

    // 対象範囲を返す
    public RangeType GetRangeType()
    {
        return rangeType;
    }
    // 属性を返す
    public AttributeType GetAttributeType()
    {
        return attributeType;
    }
    // 倍率を返す
    public float GetMagnification()
    {
        return magnification;
    }
    // 実行回数を返す
    public int GetNumOfExe()
    {
        return numOfExe;
    }
    // スキル効果を返す
    public SkillEffectType GetSkillEffectType()
    {
        return skillEffectType;
    }
    // 実行回数を返す
    public int GetHitRate()
    {
        return hitRate;
    }
}

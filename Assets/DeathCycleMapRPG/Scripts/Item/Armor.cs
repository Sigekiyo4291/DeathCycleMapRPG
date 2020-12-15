using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Armor", menuName = "CreateArmor")]
public class Armor : Equipment
{
    public enum ArmorType
    {
        None,
        Clothes,
        LightArmor,
        HeavyArmor
    }
    //武器の種類
    [SerializeField]
    private ArmorType armorType = ArmorType.None;

    private Armor()
    {
        itemType = Type.ArmorAll;
    }

    public ArmorType GetArmorType()
    {
        return armorType;
    }
}
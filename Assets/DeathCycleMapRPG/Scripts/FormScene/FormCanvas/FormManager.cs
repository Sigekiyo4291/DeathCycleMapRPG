using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormManager : MonoBehaviour
{
    public enum Status
    {
        HP,
        MP,
    }
    public enum EquipType
    {
        Weapon,
        Armor,
        Accessory1,
        Accessory2,
    }
    //　パーティーステータス
    [SerializeField]
    private PartyStatus partyStatus = null;
    // フォームキャンパス
    [SerializeField]
    private GameObject formCampas = null;
    // 装備選択画面
    [SerializeField]
    private GameObject equipmentSelectPanel = null;
    // トップ画面に戻るボタン
    [SerializeField]
    private GameObject ReturnToTopButton = null;
    // 装備更新のokボタン
    [SerializeField]
    private GameObject okButton = null;
    //　キャラクターステータス表示Transformリスト
    private Dictionary<AllyStatus, Transform> AllyStatusDictionary = new Dictionary<AllyStatus, Transform>();
    // 変更しようとしている装備の箇所の種類
    private EquipType chengingEquipType;
    //　変更する装備の種類
    Item.Type itemType;
    // 変更する装備
    private Equipment selectEquipment;
    // 装備変更中のキャラクター
    private AllyStatus changingEquipmentCharacter;
    // 装備解除ボタンのプレハブ
    [SerializeField]
    private GameObject unequipButtonPrefab = null;
    // 装備選択ボタンのプレハブ
    [SerializeField]
    private GameObject equipmentPanelButtonPrefab = null;
    //　装備アイテムボタンのリスト
    private List<GameObject> equipmentItemPanelButtonList = new List<GameObject>();
    // 装備アイテムボタン一覧
    private GameObject content;

    public void Start()
    {
        DisplayStatus();
        foreach (var member in partyStatus.GetAllyStatus())
        {
            UpdateEquipButton(member);
            AllyStatusDictionary[member].Find("ChangeWeaponPanel/ChangeEquipButton").GetComponent<Button>().onClick.AddListener(() => SelectEquip(member, EquipType.Weapon));
            AllyStatusDictionary[member].Find("ChangeArmorPanel/ChangeEquipButton").GetComponent<Button>().onClick.AddListener(() => SelectEquip(member, EquipType.Armor));
            AllyStatusDictionary[member].Find("ChangeAccessoryPanel1/ChangeEquipButton").GetComponent<Button>().onClick.AddListener(() => SelectEquip(member, EquipType.Accessory1));
            AllyStatusDictionary[member].Find("ChangeAccessoryPanel2/ChangeEquipButton").GetComponent<Button>().onClick.AddListener(() => SelectEquip(member, EquipType.Accessory2));
        }
        okButton.GetComponent<Button>().onClick.AddListener(() => UpdateEquip());
    }

    //　現在の装備ステータスデータの表示
    public void DisplayStatus()
    {
        AllyStatus member;
        Transform characterPanelTransform;
        for (int i = 0; i < 3; i++)
        {
            characterPanelTransform = formCampas.transform.Find("Form/FormPanel" + i);
            if (i < partyStatus.GetAllyStatus().Count)
            {
                member = partyStatus.GetAllyStatus()[i];

                AllyStatusDictionary.Add(member, characterPanelTransform);
            }
            else
            {
                characterPanelTransform.gameObject.SetActive(false);
            }
        }
    }

    // 装備ボタンの更新
    public void UpdateEquipButton(AllyStatus allyStatus)
    {
        AllyStatusDictionary[allyStatus].Find("CharacterNamePanel/CharacterName").GetComponent<Text>().text = allyStatus.GetCharacterName();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/HPText").GetComponent<Text>().text = "HP: " + allyStatus.GetHp().ToString() + "/" + allyStatus.GetMaxHp().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/MPText").GetComponent<Text>().text = "MP: " + allyStatus.GetMp().ToString() + "/" + allyStatus.GetMaxMp().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/AttackPowerText").GetComponent<Text>().text = "攻撃力: " + allyStatus.GetAttackPower().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/DefencePowerText").GetComponent<Text>().text = "守備力: " + allyStatus.GetDefencePower().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/PowerText").GetComponent<Text>().text = "力: " + allyStatus.GetPower().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/StrikingStrengthText").GetComponent<Text>().text = "体力: " + allyStatus.GetStrikingStrength().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/MagicPowerText").GetComponent<Text>().text = "知力: " + allyStatus.GetMagicPower().ToString();
        AllyStatusDictionary[allyStatus].Find("StatusParamPanel/AgilityText").GetComponent<Text>().text = "素早さ: " + allyStatus.GetAgility().ToString();
        AllyStatusDictionary[allyStatus].Find("ChangeWeaponPanel/ChangeEquipButton").Find("Text").GetComponent<Text>().text = allyStatus.GetEquipWeapon() ? allyStatus.GetEquipWeapon().GetKanjiName() : "なし";
        AllyStatusDictionary[allyStatus].Find("ChangeArmorPanel/ChangeEquipButton").Find("Text").GetComponent<Text>().text = allyStatus.GetEquipArmor() ? allyStatus.GetEquipArmor().GetKanjiName() : "なし";
        AllyStatusDictionary[allyStatus].Find("ChangeAccessoryPanel1/ChangeEquipButton").Find("Text").GetComponent<Text>().text = allyStatus.GetEquipAccessory1() ? allyStatus.GetEquipAccessory1().GetKanjiName() : "なし";
        AllyStatusDictionary[allyStatus].Find("ChangeAccessoryPanel2/ChangeEquipButton").Find("Text").GetComponent<Text>().text = allyStatus.GetEquipAccessory2() ? allyStatus.GetEquipAccessory2().GetKanjiName() : "なし";
    }

    //　変更する装備の選択
    public void SelectEquip(AllyStatus allyStatus, EquipType equipType)
    {
        ReturnToTopButton.GetComponent<Button>().interactable = false;
        chengingEquipType = equipType;
        changingEquipmentCharacter = allyStatus;
        content = formCampas.transform.Find("EquipmentSelectPanel/EquipmentPanel/Mask/Content").gameObject;

        if (chengingEquipType == EquipType.Weapon)
        {
            itemType = Item.Type.WeaponAll;
        }else if (chengingEquipType == EquipType.Armor) 
        {
            itemType = Item.Type.ArmorAll;
        }else if (chengingEquipType == EquipType.Accessory1 || chengingEquipType == EquipType.Accessory2)
        {
            itemType = Item.Type.AccessoryAll;
        }

        //　アイテムパネルボタンを何個作成したかどうか
        int itemPanelButtonNum = 0;
        GameObject itemButtonIns;
        // 装備解除ボタンのセット
        itemButtonIns = Instantiate<GameObject>(unequipButtonPrefab, content.transform);
        itemButtonIns.GetComponent<Button>().onClick.AddListener(() => CheckUpdateEquip());
        //　持っているアイテム分のボタンの作成とクリック時の実行メソッドの設定
        foreach (var item in partyStatus.GetItemDictionary().Keys)
        {
            // 変更しようとしている装備とアイテムの種類が一致している1個以上ある物を表示
            if (item.GetItemType() == itemType && item as Equipment && partyStatus.GetItemDictionary()[item] > 0)
            {
                Equipment equipment = item as Equipment;
                itemButtonIns = Instantiate<GameObject>(equipmentPanelButtonPrefab, content.transform);
                itemButtonIns.transform.Find("ItemNameText").GetComponent<Text>().text = item.GetKanjiName();
                itemButtonIns.GetComponent<Button>().onClick.AddListener(() => CheckUpdateEquip(equipment));

                //　アイテム数を表示
                itemButtonIns.transform.Find("NumText").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();

                //　アイテムボタンリストに追加
                equipmentItemPanelButtonList.Add(itemButtonIns);
                //　アイテムパネルボタン番号を更新
                itemPanelButtonNum++;
            }
        }

        equipmentSelectPanel.SetActive(true);
    }

    // 変更する装備候補の更新
    public void CheckUpdateEquip(Equipment item = null)
    {
        selectEquipment = item;
        okButton.GetComponent<Button>().interactable = true;
    }

    // 装備の更新
    public void UpdateEquip()
    {
        //　装備の種類ごとに装備を更新する
        if (chengingEquipType == EquipType.Weapon)
        {
            UnequipItemNumSet(changingEquipmentCharacter.GetEquipWeapon());
            changingEquipmentCharacter.SetEquipWeapon(selectEquipment as Weapon);
        }
        else if (chengingEquipType == EquipType.Armor)
        {
            UnequipItemNumSet(changingEquipmentCharacter.GetEquipArmor());
            changingEquipmentCharacter.SetEquipArmor(selectEquipment as Armor);
        }
        else if (chengingEquipType == EquipType.Accessory1)
        {
            UnequipItemNumSet(changingEquipmentCharacter.GetEquipAccessory1());
            changingEquipmentCharacter.SetEquipAccessory1(selectEquipment as Accessory);
        }
        else if (chengingEquipType == EquipType.Accessory2)
        {
            UnequipItemNumSet(changingEquipmentCharacter.GetEquipAccessory2());
            changingEquipmentCharacter.SetEquipAccessory2(selectEquipment as Accessory);
        }
        // 防御力と攻撃力を更新
        changingEquipmentCharacter.SetEquippedAttackPower();
        changingEquipmentCharacter.SetEquippedDefencePower();
        //　装備したアイテム数を減らす
        EquipItemNumSet(selectEquipment);
        // 装備変更ボタンの更新
        UpdateEquipButton(changingEquipmentCharacter);
        //選択アイテム一覧の子要素を全て削除
        EquipmentItemClear();
        equipmentSelectPanel.SetActive(false);
        okButton.GetComponent<Button>().interactable = false;
        ReturnToTopButton.GetComponent<Button>().interactable = true;
    }

    // 外した装備の数の調整
    private void UnequipItemNumSet(Item item)
    {
        // 装備していないなら何もしない
        if (item == null)
        {
            return;
        }
        if (partyStatus.GetItemDictionary().ContainsKey(item))
        {
            partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) + 1);
        }
        else
        {
            partyStatus.SetItemDictionary(item, 1);
        }
    }
    // 装備したアイテムの数の調整
    private void EquipItemNumSet(Item item)
    {
        // 装備するものがないなら何もしない
        if (item == null)
        {
            return;
        }
        partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
        //　アイテム数が0だったらキャラクターステータスからアイテムを削除
        if (partyStatus.GetItemNum(item) == 0)
        {
            partyStatus.GetItemDictionary().Remove(item);
        }
    }

    // アイテム一覧表示のクリア
    private void EquipmentItemClear()
    {
        //　アイテム選択ボタンがあれば全て削除
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        equipmentItemPanelButtonList.Clear();
    }
}
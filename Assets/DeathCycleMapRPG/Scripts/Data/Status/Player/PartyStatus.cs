using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "PartyStatus", menuName = "CreatePartyStatus")]
public class PartyStatus : ScriptableObject
{
    [SerializeField]
    private int money = 0;
    [SerializeField]
    private List<AllyStatus> partyMembers = null;

    //　アイテムと個数のDictionary
    [SerializeField]
    private ItemDictionary itemDictionary = null;

    public void SetMoney(int money) {
        this.money = money;
    }
 
    public int GetMoney() {
        return money;
    }
 
    public void SetAllyStatus(AllyStatus charaStatus) {
        if (!partyMembers.Contains(charaStatus)) {
            partyMembers.Add(charaStatus);
            //プレイヤーのレベルを取得
            int playerLevel = this.GetAllyStatus()[0].GetLevel();
            //加入するキャラのステータスを初期化
            charaStatus.StatusInit();
            //　変数を初期化
            int raisedAgility = 0;
            int raisedPower = 0;
            int raisedStrikingStrength = 0;
            int raisedMagicPower = 0;
            //ステータスアップのテーブル
            LevelUpData levelUpData = charaStatus.GetLevelUpData();
            List<int> agilityRisingTable = levelUpData.GetAgilityRisingTable();
            List<int> powerRisingTable = levelUpData.GetPowerRisingTable();
            List<int> strikingStrengthRisingTable = levelUpData.GetStrikingStrengthRisingTable();
            List<int> magicPowerRisingTable = levelUpData.GetMagicPowerRisingTable();
            //　プレイヤーとのレベル差分、ステータスアップを計算し反映する
            for (int i = 1; i < playerLevel; i++)
            {
                //　レベルを反映
                charaStatus.SetLevel(charaStatus.GetLevel() + 1);

                raisedAgility += agilityRisingTable[charaStatus.GetLevel() % agilityRisingTable.Count()];
                raisedPower += powerRisingTable[charaStatus.GetLevel() % powerRisingTable.Count()];
                raisedStrikingStrength += strikingStrengthRisingTable[charaStatus.GetLevel() % strikingStrengthRisingTable.Count()];
                raisedMagicPower += magicPowerRisingTable[charaStatus.GetLevel() % strikingStrengthRisingTable.Count];
            }

            charaStatus.StatusUpdate(raisedPower, raisedAgility, raisedStrikingStrength, raisedMagicPower);
            //HP、MPを満タンに
            charaStatus.SetHp(charaStatus.GetMaxHp());
            charaStatus.SetMp(charaStatus.GetMaxMp());
        }
    }

    public void RemoveAllyStatus(AllyStatus PlayerStatus)
    {
        if (partyMembers.Contains(PlayerStatus))
        {
            partyMembers.Remove(PlayerStatus);
        }
    }

    public List<AllyStatus> GetAllyStatus() {
        return partyMembers;
    }

    public void CreateItemDictionary(ItemDictionary itemDictionary)
    {
        this.itemDictionary = itemDictionary;
    }

    public void SetItemDictionary(Item item, int num = 0)
    {
        itemDictionary.Add(item, num);
    }

    //　アイテムが登録された順番のItemDictionaryを返す
    public ItemDictionary GetItemDictionary()
    {
        return itemDictionary;
    }
    //　平仮名の名前でソートしたItemDictionaryを返す
    public IOrderedEnumerable<KeyValuePair<Item, int>> GetSortItemDictionary()
    {
        return itemDictionary.OrderBy(item => item.Key.GetHiraganaName());
    }
    public int SetItemNum(Item tempItem, int num)
    {
        return itemDictionary[tempItem] = num;
    }
    //　アイテムの数を返す
    public int GetItemNum(Item item)
    {
        return itemDictionary[item];
    }
}
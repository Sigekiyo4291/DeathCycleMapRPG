using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStatusScript : MonoBehaviour
{
    public enum Status
    {
        HP,
        MP,
    }
    //　パーティーステータス
    [SerializeField]
    private PartyStatus partyStatus = null;
    //　キャラクターステータス表示Transformリスト
    private Dictionary<CharacterStatus, Transform> characterStatusDictionary = new Dictionary<CharacterStatus, Transform>();

    //　ステータスデータの表示
    public void DisplayStatus()
    {
        CharacterStatus member;
        Transform characterPanelTransform;
        for (int i = 0; i < 3; i++)
        {
            characterPanelTransform = transform.Find("CharacterPanel" + i);
            if (i < partyStatus.GetAllyStatus().Count)
            {
                member = partyStatus.GetAllyStatus()[i];
                characterPanelTransform.Find("CharacterName").GetComponent<Text>().text = member.GetCharacterName();
                characterPanelTransform.Find("HPSlider").GetComponent<Slider>().value = (float)member.GetHp() / member.GetMaxHp();
                characterPanelTransform.Find("HPText").GetComponent<Text>().text = "HP: " + member.GetHp().ToString() + "/" + member.GetMaxHp().ToString();
                characterPanelTransform.Find("MPSlider").GetComponent<Slider>().value = (float)member.GetMp() / member.GetMaxMp();
                characterPanelTransform.Find("MPText").GetComponent<Text>().text = "MP: " + member.GetMp().ToString() + "/" + member.GetMaxMp().ToString();
                characterStatusDictionary.Add(member, characterPanelTransform);
            }
            else
            {
                characterPanelTransform.gameObject.SetActive(false);
            }
        }
    }

    //　攻撃選択ボタンの更新
    public void UpdateSelect(CharacterStatus characterStatus, string skillName, Action<GameObject> action, GameObject allyObject)
    {
        characterStatusDictionary[characterStatus].Find("SelectActButton").Find("Text").GetComponent<Text>().text = skillName;
        characterStatusDictionary[characterStatus].Find("SelectActButton").GetComponent<Button>().onClick.AddListener(() => action(allyObject));
    }

    //　データの更新
    public void UpdateStatus(CharacterStatus characterStatus, Status status, int destinationValue)
    {
        if (status == Status.HP)
        {
            characterStatusDictionary[characterStatus].Find("HPSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxHp();
            characterStatusDictionary[characterStatus].Find("HPText").GetComponent<Text>().text = "HP: " + destinationValue.ToString() + "/" + characterStatus.GetMaxHp().ToString();
        }
        else if (status == Status.MP)
        {
            characterStatusDictionary[characterStatus].Find("MPSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxMp();
            characterStatusDictionary[characterStatus].Find("MPText").GetComponent<Text>().text = "HP: " + destinationValue.ToString() + "/" + characterStatus.GetMaxMp().ToString();
        }
    }
}   
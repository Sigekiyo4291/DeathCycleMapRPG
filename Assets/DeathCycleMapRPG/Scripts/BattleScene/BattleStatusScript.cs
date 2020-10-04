using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {
        //　最初の全データ表示
        DisplayStatus();
    }

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

    //　データの更新
    public void UpdateStatus(CharacterStatus characterStatus, Status status, int destinationValue)
    {
        if (status == Status.HP)
        {
            characterStatusDictionary[characterStatus].Find("HPSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxHp();
            characterStatusDictionary[characterStatus].Find("HPText").GetComponent<Text>().text = destinationValue.ToString();
        }
        else if (status == Status.MP)
        {
            characterStatusDictionary[characterStatus].Find("MPSlider").GetComponent<Slider>().value = (float)destinationValue / characterStatus.GetMaxMp();
            characterStatusDictionary[characterStatus].Find("MPText").GetComponent<Text>().text = destinationValue.ToString();
        }
    }
}   
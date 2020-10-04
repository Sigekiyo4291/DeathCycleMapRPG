using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIManager : MonoBehaviour
{
    //　パーティーステータス
    [SerializeField]
    private PartyStatus partyStatus = null;
    //　キャラクター毎のステータスパネルプレハブ
    [SerializeField]
    private GameObject characterPanelPrefab = null;
    //　キャラクターステータスパネル
    [SerializeField]
    private GameObject characterStatusPanel;

    private void Start()
    {
        GameObject characterPanel;
        //　パーティーメンバー分のステータスを作成
        foreach (var member in partyStatus.GetAllyStatus())
        {
            characterPanel = Instantiate<GameObject>(characterPanelPrefab, characterStatusPanel.transform);
            characterPanel.transform.Find("CharacterName").GetComponent<Text>().text = member.GetCharacterName();
            characterPanel.transform.Find("HPSlider").GetComponent<Slider>().value = (float)member.GetHp() / member.GetMaxHp();
            characterPanel.transform.Find("MPSlider").GetComponent<Slider>().value = (float)member.GetMp() / member.GetMaxMp();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TopManager : MonoBehaviour
{
    // 後でprivateに修正

    //　ステータス表示パネル
    public List<GameObject> statusPanel;
    //　キャラクター別ステータスパネル
    [SerializeField]
    private GameObject statusPanel1;
    [SerializeField]
    private GameObject statusPanel2;
    //　キャラクター名
    public List<Text> characterNameText;
    //　ステータスタイトルテキスト
    public List<Text> statusTitleText;
    //　ステータスパラメータテキスト1
    public List<Text> statusParam1Text;
    //　ステータスパラメータテキスト2
    public List<Text> statusParam2Text;
    //　パーティーステータス
    [SerializeField]
    public PartyStatus partyStatus = null;

    // Top画面
    [SerializeField]
    private GameObject TopCanvas;
    // Item画面
    [SerializeField]
    private GameObject ItemCanvas;
    // Form画面
    [SerializeField]
    private GameObject FormCanvas;
    // システム画面
    [SerializeField]
    private GameObject SystemCanvas;


    // Start is called before the first frame update
    void Start()
    {
        statusPanel.Add(statusPanel1);
        //　ステータス用テキスト
        characterNameText.Add(statusPanel[0].transform.Find("CharacterNamePanel/Text").GetComponent<Text>());
        statusTitleText.Add(statusPanel[0].transform.Find("StatusParamPanel/Title").GetComponent<Text>());
        statusParam1Text.Add(statusPanel[0].transform.Find("StatusParamPanel/Param1").GetComponent<Text>());
        statusParam2Text.Add(statusPanel[0].transform.Find("StatusParamPanel/Param2").GetComponent<Text>());
        ShowStatus(partyStatus.GetAllyStatus()[0], statusPanel[0], characterNameText[0], statusTitleText[0], statusParam1Text[0], statusParam2Text[0]);

        statusPanel.Add(statusPanel2);
        //　ステータス用テキスト
        characterNameText.Add(statusPanel[1].transform.Find("CharacterNamePanel/Text").GetComponent<Text>());
        statusTitleText.Add(statusPanel[1].transform.Find("StatusParamPanel/Title").GetComponent<Text>());
        statusParam1Text.Add(statusPanel[1].transform.Find("StatusParamPanel/Param1").GetComponent<Text>());
        statusParam2Text.Add(statusPanel[1].transform.Find("StatusParamPanel/Param2").GetComponent<Text>());
        ShowStatus(partyStatus.GetAllyStatus()[1], statusPanel[1], characterNameText[1], statusTitleText[1], statusParam1Text[1], statusParam2Text[1]);

        // 各画面への移動ボタンに関数をセット
        GameObject.Find("ToItemButton").GetComponent<Button>().onClick.AddListener(() => MoveToAnotherCanvas(TopCanvas, ItemCanvas));
        GameObject.Find("ToFormButton").GetComponent<Button>().onClick.AddListener(() => MoveToAnotherCanvas(TopCanvas, FormCanvas));
        GameObject.Find("ToSystemButton").GetComponent<Button>().onClick.AddListener(() => MoveToAnotherCanvas(TopCanvas, SystemCanvas));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnReturnMapButton()
    {
        SceneManager.LoadScene("MapScene");
    }

    //　キャラクターのステータス表示
    public void ShowStatus(AllyStatus allyStatus,GameObject statusPanel, Text characterNameText, Text statusTitleText, Text statusParam1Text, Text statusParam2Text)
    {
        statusPanel.SetActive(true);
        //　キャラクターの名前を表示
        characterNameText.text = allyStatus.GetCharacterName();

        //　タイトルの表示
        var text = "レベル\n";
        text += "HP\n";
        text += "MP\n";
        text += "経験値\n";
        text += "状態異常\n";
        text += "力\n";
        text += "素早さ\n";
        text += "打たれ強さ\n";
        text += "魔法力\n";
        text += "装備武器\n";
        text += "装備鎧\n";
        text += "攻撃力\n";
        text += "防御力\n";
        statusTitleText.text = text;

        //　HPとMPのDivision記号の表示
        text = "\n";
        text += allyStatus.GetHp() + "\n";
        text += allyStatus.GetMp() + "\n";
        statusParam1Text.text = text;

        //　ステータスパラメータの表示
        text = allyStatus.GetLevel() + "\n";
        text += allyStatus.GetMaxHp() + "\n";
        text += allyStatus.GetMaxMp() + "\n";
        text += allyStatus.GetEarnedExperience() + "\n";
        if (!allyStatus.IsPoisonState() && !allyStatus.IsNumbnessState())
        {
            text += "正常";
        }
        else
        {
            if (allyStatus.IsPoisonState())
            {
                text += "毒";
                if (allyStatus.IsNumbnessState())
                {
                    text += "、痺れ";
                }
            }
            else
            {
                if (allyStatus.IsNumbnessState())
                {
                    text += "痺れ";
                }
            }
        }

        text += "\n";
        text += allyStatus.GetPower() + "\n";
        text += allyStatus.GetAgility() + "\n";
        text += allyStatus.GetStrikingStrength() + "\n";
        text += allyStatus.GetMagicPower() + "\n";
        text += allyStatus?.GetEquipWeapon()?.GetKanjiName() ?? "";
        text += "\n";
        text += allyStatus.GetEquipArmor()?.GetKanjiName() ?? "";
        text += "\n";
        text += allyStatus.GetPower() + (allyStatus.GetEquipWeapon()?.GetAmount() ?? 0) + "\n";
        text += allyStatus.GetStrikingStrength() + (allyStatus.GetEquipArmor()?.GetAmount() ?? 0) + "\n";
        statusParam2Text.text = text;
    }

    // top画面に戻る
    public void ReturnTopCanvas()
    {
        TopCanvas.SetActive(true);
        ItemCanvas.SetActive(false);
        FormCanvas.SetActive(false);
        SystemCanvas.SetActive(false);
    }

    // 別の画面に移る
    public void MoveToAnotherCanvas(GameObject fromCanvas, GameObject toCanvas)
    {
        toCanvas.SetActive(true);
        fromCanvas.SetActive(false);
    }
}

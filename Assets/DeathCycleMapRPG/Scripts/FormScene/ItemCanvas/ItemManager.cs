using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public enum CommandMode
    {
        CommandPanel,
        StatusPanelSelectCharacter,
        StatusPanel,
        ItemPanelSelectCharacter,
        ItemPanel,
        UseItemPanel,
        UseItemSelectCharacterPanel,
        UseItemPanelToItemPanel,
        UseItemPanelToUseItemPanel,
        UseItemSelectCharacterPanelToUseItemPanel,
        NoItemPassed
    }

    //　パーティーステータス
    [SerializeField]
    private PartyStatus partyStatus = null;
    // アイテム関連のキャンバス
    [SerializeField]
    private GameObject itemCanvas = null;

    private CommandMode currentCommand;
    //　アイテム表示パネル
    private GameObject itemPanel;
    //　アイテムパネルボタンを表示する場所
    private GameObject content;
    //　アイテムを使う選択パネル
    private GameObject useItemPanel;
    //　アイテムを使う相手を誰にするか選択するパネル
    private GameObject useItemSelectCharacterPanel;
    //　情報表示パネル
    private GameObject itemInformationPanel;
    //　アイテム使用後の情報表示パネル
    private GameObject useItemInformationPanel;

    //　アイテムパネルのCanvas Group
    private CanvasGroup itemPanelCanvasGroup;
    //　アイテムを使う選択パネルのCanvasGroup
    private CanvasGroup useItemPanelCanvasGroup;
    //　アイテムを使うキャラクター選択パネルのCanvasGroup;
    private CanvasGroup useItemSelectCharacterPanelCanvasGroup;

    //　情報表示タイトルテキスト
    private Text informationTitleText;
    //　情報表示テキスト
    private Text informationText;

    //　アイテムのボタンのプレハブ
    [SerializeField]
    private GameObject itemPanelButtonPrefab = null;
    //　アイテム使用時のボタンのプレハブ
    [SerializeField]
    private GameObject useItemPanelButtonPrefab = null;
    //　キャラクター選択時のボタンのプレハブ
    [SerializeField]
    private GameObject characterPanelButtonPrefab = null;

    //　アイテムボタン一覧
    private List<GameObject> itemPanelButtonList = new List<GameObject>();

    //　最後に選択していたゲームオブジェクトをスタック
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        itemPanel = itemCanvas.transform.Find("ItemPanel").gameObject;
        content = itemPanel.transform.Find("Mask/Content").gameObject;
        useItemPanel = itemCanvas.transform.Find("UseItemPanel").gameObject;
        useItemSelectCharacterPanel = itemCanvas.transform.Find("UseItemSelectCharacterPanel").gameObject;
        itemInformationPanel = itemCanvas.transform.Find("ItemInfoPanel").gameObject;
        useItemInformationPanel = itemCanvas.transform.Find("UseItemInfoPanel").gameObject;

        itemPanelCanvasGroup = itemPanel.GetComponent<CanvasGroup>();
        useItemPanelCanvasGroup = useItemPanel.GetComponent<CanvasGroup>();
        useItemSelectCharacterPanelCanvasGroup = useItemSelectCharacterPanel.GetComponent<CanvasGroup>();

        //　情報表示用テキスト
        informationTitleText = itemInformationPanel.transform.Find("TitleText").GetComponent<Text>();
        informationText = itemInformationPanel.transform.Find("InfoText").GetComponent<Text>();

        //アイテムボタン一覧の作成
        CreateItemPanelButton();
    }

    /*
    private void OnEnable()
    {
        useItemPanel.SetActive(false);
        useItemSelectCharacterPanel.SetActive(false);
        itemInformationPanel.SetActive(false);
        useItemInformationPanel.SetActive(false);

        //　アイテムを使うキャラクター選択ボタンがあれば全て削除
        for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemPanel.transform.GetChild(i).gameObject);
        }
        //　アイテムを使う相手のキャラクター選択ボタンがあれば全て削除
        for (int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        itemPanelButtonList.Clear();

        itemPanelCanvasGroup.interactable = true;
        useItemPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanelCanvasGroup.interactable = false;
    }
    */

    // 選択用のボタンをリセットする
    public void ResetSelectButton()
    {
        useItemPanel.SetActive(false);
        useItemSelectCharacterPanel.SetActive(false);
        //itemInformationPanel.SetActive(false);

        //　アイテムを使うキャラクター選択ボタンがあれば全て削除
        for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemPanel.transform.GetChild(i).gameObject);
        }
        //　アイテムを使う相手のキャラクター選択ボタンがあれば全て削除
        for (int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }

        useItemPanelCanvasGroup.interactable = true;
        useItemSelectCharacterPanelCanvasGroup.interactable = false;
    }

    //　キャラクターが持っているアイテムのボタン表示
    public void CreateItemPanelButton()
    {
        itemInformationPanel.SetActive(true);

        //　アイテムパネルボタンを何個作成したかどうか
        int itemPanelButtonNum = 0;
        GameObject itemButtonIns;
        //　選択したキャラクターのアイテム数分アイテムパネルボタンを作成
        //　持っているアイテム分のボタンの作成とクリック時の実行メソッドの設定
        foreach (var item in partyStatus.GetItemDictionary().Keys)
        {
            itemButtonIns = Instantiate<GameObject>(itemPanelButtonPrefab, content.transform);
            itemButtonIns.transform.Find("ItemNameText").GetComponent<Text>().text = item.GetKanjiName();
            itemButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectItem(item));
            itemButtonIns.GetComponent<ItemButtonScript>().SetParam(item);

            //　アイテム数を表示
            itemButtonIns.transform.Find("NumText").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();

            //　アイテムボタンリストに追加
            itemPanelButtonList.Add(itemButtonIns);
            //　アイテムパネルボタン番号を更新
            itemPanelButtonNum++;
        }

        //　アイテムパネルの表示と最初のアイテムの選択
        if (content.transform.childCount != 0)
        {
            //　SelectCharacerPanelで最後にどのゲームオブジェクトを選択していたか
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);
            currentCommand = CommandMode.ItemPanel;
            itemPanel.SetActive(true);
            itemPanel.transform.SetAsLastSibling();
            itemPanelCanvasGroup.interactable = true;
            EventSystem.current.SetSelectedGameObject(content.transform.GetChild(0).gameObject);
        }
        else
        {
            informationTitleText.text = "";
            informationText.text = "アイテムを持っていません。";
        }
    }

    //　キャンセルボタンを押した時の処理
    public void OnCancelButton()
    {
        //　アイテムを選択し、どう使うかを選択している時
        if (currentCommand == CommandMode.UseItemPanel)
        {
            useItemPanelCanvasGroup.interactable = false;
            useItemPanel.SetActive(false);
            //　UseItemPanelでCancelボタンを押したらUseItemPanelの子要素のボタンの全削除
            for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }

            itemPanelCanvasGroup.interactable = true;
            currentCommand = CommandMode.ItemPanel;
            //　アイテムを使用する相手のキャラクターを選択している時
        }
        else if (currentCommand == CommandMode.UseItemSelectCharacterPanel)
        {
            useItemSelectCharacterPanelCanvasGroup.interactable = false;
            useItemSelectCharacterPanel.SetActive(false);
            //　UseItemSelectCharacterPanelでCancelボタンを押したらアイテムを使用するキャラクターボタンの全削除
            for (int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
            }

            EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
            useItemPanelCanvasGroup.interactable = true;
            currentCommand = CommandMode.UseItemPanel;
        }
    }



    //　アイテムをどうするかの選択
    public void SelectItem(Item item)
    {
        ResetSelectButton();
        GameObject itemMenuButtonIns;
        //　アイテムの種類に応じて出来る項目を変更する
        if (item.GetItemType() == Item.Type.ArmorAll ||
            item.GetItemType() == Item.Type.WeaponAll ||
            item.GetItemType() == Item.Type.AccessoryAll
            )
        {
            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "捨てる";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => ThrowAwayItem(item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "やめる";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => OnCancelButton());

        }
        else if (item.GetItemType() == Item.Type.NumbnessRecovery
          || item.GetItemType() == Item.Type.PoisonRecovery
          || item.GetItemType() == Item.Type.HPRecovery
          || item.GetItemType() == Item.Type.MPRecovery
          )
        {
            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "使う";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItem(item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "捨てる";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => ThrowAwayItem(item));

            itemMenuButtonIns = Instantiate<GameObject>(useItemPanelButtonPrefab, useItemPanel.transform);
            itemMenuButtonIns.GetComponentInChildren<Text>().text = "やめる";
            itemMenuButtonIns.GetComponent<Button>().onClick.AddListener(() => OnCancelButton());

        }
        else if (item.GetItemType() == Item.Type.Valuables)
        {
            informationTitleText.text = item.GetKanjiName();
            informationText.text = item.GetInformation();
        }

        if (item.GetItemType() != Item.Type.Valuables)
        {
            useItemPanel.SetActive(true);
            currentCommand = CommandMode.UseItemPanel;
          
            useItemPanel.transform.SetAsLastSibling();
            //EventSystem.current.SetSelectedGameObject(useItemPanel.transform.GetChild(0).gameObject);
            useItemPanelCanvasGroup.interactable = true;
        }
    }

    //　アイテムを使用するキャラクターを選択する
    public void UseItem(Item item)
    {
        useItemPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(true);
        //　UseItemPanelでどれを最後に選択していたか
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject characterButtonIns;
        //　パーティメンバー分のボタンを作成
        foreach (var member in partyStatus.GetAllyStatus())
        {
            characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, useItemSelectCharacterPanel.transform);
            characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
            characterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItemToCharacter(member, item));
        }
        characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, useItemSelectCharacterPanel.transform);
        characterButtonIns.GetComponentInChildren<Text>().text = "やめる";
        characterButtonIns.GetComponent<Button>().onClick.AddListener(() => OnCancelButton());
        //　UseItemSelectCharacterPanelに移行する
        currentCommand = CommandMode.UseItemSelectCharacterPanel;
        useItemSelectCharacterPanelCanvasGroup.interactable = true;
    }

    public void UseItemToCharacter(AllyStatus toChara, Item item)
    {
        useItemInformationPanel.SetActive(true);
        useItemSelectCharacterPanelCanvasGroup.interactable = false;
        useItemSelectCharacterPanel.SetActive(false);

        if (item.GetItemType() == Item.Type.HPRecovery)
        {
            if (toChara.GetHp() == toChara.GetMaxHp())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "は元気です。";
            }
            else
            {
                toChara.SetHp(toChara.GetHp() + item.GetAmount());
                //　アイテムを使用した旨を表示
                useItemInformationPanel.GetComponentInChildren<Text>().text = item.GetKanjiName() + "を" + toChara.GetCharacterName() + "に使用しました。\n" +
                    toChara.GetCharacterName() + "は" + item.GetAmount() + "回復しました。";
                //　持っているアイテム数を減らす
                partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
            }
        }
        else if (item.GetItemType() == Item.Type.MPRecovery)
        {
            if (toChara.GetMp() == toChara.GetMaxMp())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "のMPは最大です。";
            }
            else
            {
                toChara.SetMp(toChara.GetMp() + item.GetAmount());
                //　アイテムを使用した旨を表示
                useItemInformationPanel.GetComponentInChildren<Text>().text = item.GetKanjiName() + "を" + toChara.GetCharacterName() + "に使用しました。\n" +
                    toChara.GetCharacterName() + "はMPを" + item.GetAmount() + "回復しました。";
                //　持っているアイテム数を減らす
                partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
            }
        }
        else if (item.GetItemType() == Item.Type.PoisonRecovery)
        {
            if (!toChara.IsPoisonState())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "は毒状態ではありません。";
            }
            else
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "は毒から回復しました。";
                toChara.SetPoisonState(false);
                //　持っているアイテム数を減らす
                partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
            }
        }
        else if (item.GetItemType() == Item.Type.NumbnessRecovery)
        {
            if (!toChara.IsNumbnessState())
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "は痺れ状態ではありません。";
            }
            else
            {
                useItemInformationPanel.GetComponentInChildren<Text>().text = toChara.GetCharacterName() + "は痺れから回復しました。";
                toChara.SetNumbness(false);
                //　持っているアイテム数を減らす
                partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
            }
        }

        //　itemPanleButtonListから該当するアイテムを探し数を更新する
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemNameText").GetComponent<Text>().text == item.GetKanjiName());
        itemButton.transform.Find("NumText").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();

        //　アイテム数が0だったらボタンとキャラクターステータスからアイテムを削除
        if (partyStatus.GetItemNum(item) == 0)
        {
            DeleteLostItem(item, itemButton);
            //　ItemPanelに戻る為UseItemPanelの子要素のボタンを全削除
            ResetSelectButton();
        }
        else
        {
            //　アイテム数が残っている場合はUseItemPanelでアイテムをどうするかの選択に戻る
            currentCommand = CommandMode.UseItemSelectCharacterPanelToUseItemPanel;
        }
    }

    //　捨てる
    public void ThrowAwayItem(Item item)
    {
        //　アイテム数を減らす
        partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
 
        //　ItemPanelの子要素のアイテムパネルボタンから該当するアイテムのボタンを探して数を更新する
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemNameText").GetComponent<Text>().text == item.GetKanjiName());
        itemButton.transform.Find("NumText").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();
        useItemInformationPanel.GetComponentInChildren<Text>().text = item.GetKanjiName() + "を捨てました。";

        //　アイテム数が0だったらボタンとキャラクターステータスからアイテムを削除
        if (partyStatus.GetItemNum(item) == 0)
        {
            DeleteLostItem(item, itemButton);
            //　ItemPanelに戻る為UseItemPanelの子要素のボタンを全削除
            ResetSelectButton();
        }
        else
        {
            useItemPanelCanvasGroup.interactable = false;
            useItemInformationPanel.SetActive(true);
            currentCommand = CommandMode.UseItemPanelToUseItemPanel;
        }
    }

    // ０個になったアイテムとアイテムボタンの削除
    public void DeleteLostItem(Item item, GameObject itemButton)
    {
        itemPanelButtonList.Remove(itemButton);
        Destroy(itemButton);
        partyStatus.GetItemDictionary().Remove(item);

        currentCommand = CommandMode.NoItemPassed;
    }
}

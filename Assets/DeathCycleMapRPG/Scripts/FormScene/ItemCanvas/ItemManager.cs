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
        //アイテムボタン一覧の作成
        CreateItemPanelButton();
    }

    // Update is called once per frame
    void Update()
    {

        //　アイテムを装備、装備を外す情報表示後の処理
        if (currentCommand == CommandMode.UseItemPanelToItemPanel)
        {
            if (Input.anyKeyDown
                || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f)
                || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f)
                )
            {
                currentCommand = CommandMode.ItemPanel;
                useItemInformationPanel.SetActive(false);
                itemPanel.transform.SetAsLastSibling();
                itemPanelCanvasGroup.interactable = true;

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());

            }
            //　アイテムを使用する相手のキャラクター選択からアイテムをどうするかに移行する時
        }
        else if (currentCommand == CommandMode.UseItemSelectCharacterPanelToUseItemPanel)
        {
            if (Input.anyKeyDown
                || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f)
                || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f)
                )
            {
                currentCommand = CommandMode.UseItemPanel;
                useItemInformationPanel.SetActive(false);
                useItemPanel.transform.SetAsLastSibling();
                useItemPanelCanvasGroup.interactable = true;

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
            }
            //　アイテムを捨てるを選択した後の状態
        }
        else if (currentCommand == CommandMode.UseItemPanelToUseItemPanel)
        {
            if (Input.anyKeyDown
                || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f)
                || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f)
                )
            {
                currentCommand = CommandMode.UseItemPanel;
                useItemInformationPanel.SetActive(false);
                useItemPanel.transform.SetAsLastSibling();
                useItemPanelCanvasGroup.interactable = true;
            }
            //　アイテムを使用、渡す、捨てるを選択した後にそのアイテムの数が0になった時
        }
        else if (currentCommand == CommandMode.NoItemPassed)
        {
            if (Input.anyKeyDown
                || !Mathf.Approximately(Input.GetAxis("Horizontal"), 0f)
                || !Mathf.Approximately(Input.GetAxis("Vertical"), 0f)
                )
            {
                currentCommand = CommandMode.ItemPanel;
                useItemInformationPanel.SetActive(false);
                useItemPanel.SetActive(false);
                itemPanel.transform.SetAsLastSibling();
                itemPanelCanvasGroup.interactable = true;

                //　アイテムパネルボタンがあれば最初のアイテムパネルボタンを選択
                if (content.transform.childCount != 0)
                {
                    EventSystem.current.SetSelectedGameObject(content.transform.GetChild(0).gameObject);
                }
                else
                {
                    //　アイテムパネルボタンがなければ（アイテムを持っていない）ItemSelectPanelに戻る
                    currentCommand = CommandMode.ItemPanelSelectCharacter;
                    EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                }
            }
        }
    }

    private void Awake()
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

    }

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

        itemPanelCanvasGroup.interactable = false;
        useItemPanelCanvasGroup.interactable = false;
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

            EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
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
        GameObject itemMenuButtonIns;
        //　アイテムの種類に応じて出来る項目を変更する
        if (item.GetItemType() == Item.Type.ArmorAll
            || item.GetItemType() == Item.Type.WeaponAll
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
            itemPanelCanvasGroup.interactable = false;
            currentCommand = CommandMode.UseItemPanel;
            //　ItemPanelで最後にどれを選択していたか？
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            useItemPanel.transform.SetAsLastSibling();
            EventSystem.current.SetSelectedGameObject(useItemPanel.transform.GetChild(0).gameObject);
            useItemPanelCanvasGroup.interactable = true;
            Input.ResetInputAxes();

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
        //　UseItemSelectCharacterPanelに移行する
        currentCommand = CommandMode.UseItemSelectCharacterPanel;
        useItemSelectCharacterPanel.transform.SetAsLastSibling();
        EventSystem.current.SetSelectedGameObject(useItemSelectCharacterPanel.transform.GetChild(0).gameObject);
        useItemSelectCharacterPanelCanvasGroup.interactable = true;
        Input.ResetInputAxes();
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

        //　アイテムを使用したらアイテムを使用する相手のUseItemSelectCharacterPanelの子要素のボタンを全削除
        for (int i = useItemSelectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(useItemSelectCharacterPanel.transform.GetChild(i).gameObject);
        }
        //　itemPanleButtonListから該当するアイテムを探し数を更新する
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemNameText").GetComponent<Text>().text == item.GetKanjiName());
        itemButton.transform.Find("NumText").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();

        //　アイテム数が0だったらボタンとキャラクターステータスからアイテムを削除
        if (partyStatus.GetItemNum(item) == 0)
        {
            //　アイテムが0になったら一気にItemPanelに戻す為、UseItemPanel内とUseItemSelectCharacterPanel内でのオブジェクト登録を削除
            selectedGameObjectStack.Pop();
            selectedGameObjectStack.Pop();
            //　itemPanelButtonListからアイテムパネルボタンを削除
            itemPanelButtonList.Remove(itemButton);
            //　アイテムパネルボタン自身の削除
            Destroy(itemButton);
            //　アイテムを渡したキャラクター自身のItemDictionaryからそのアイテムを削除
            partyStatus.GetItemDictionary().Remove(item);
            //　ItemPanelに戻る為、UseItemPanel内に作ったボタンを全削除
            for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }
            //　アイテム数が0になったのでCommandMode.NoItemPassedに変更
            currentCommand = CommandMode.NoItemPassed;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
        else
        {
            //　アイテム数が残っている場合はUseItemPanelでアイテムをどうするかの選択に戻る
            currentCommand = CommandMode.UseItemSelectCharacterPanelToUseItemPanel;
            useItemInformationPanel.transform.SetAsLastSibling();
            Input.ResetInputAxes();
        }
    }

    //　捨てる
    public void ThrowAwayItem(Item item)
    {
        //　アイテム数を減らす
        partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) - 1);
        /*
        //　アイテム数が0になった時
        if (partyStatus.GetItemNum(item) == 0)
        {
            //　装備している武器を捨てる場合の処理
            if (item == partyStatus.GetEquipArmor())
            {
                var equipArmorButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetKanjiName());
                equipArmorButton.transform.Find("Equip").GetComponent<Text>().text = "";
                equipArmorButton = null;
                partyStatus.SetEquipArmor(null);
            }
            else if (item == partyStatus.GetEquipWeapon())
            {
                var equipWeaponButton = itemPanelButtonList.Find(itemPanelButton => itemPanelButton.transform.Find("ItemName").GetComponent<Text>().text == item.GetKanjiName());
                equipWeaponButton.transform.Find("Equip").GetComponent<Text>().text = "";
                equipWeaponButton = null;
                partyStatus.SetEquipWeapon(null);
            }
        }
        */
        //　ItemPanelの子要素のアイテムパネルボタンから該当するアイテムのボタンを探して数を更新する
        var itemButton = itemPanelButtonList.Find(obj => obj.transform.Find("ItemNameText").GetComponent<Text>().text == item.GetKanjiName());
        itemButton.transform.Find("NumText").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();
        useItemInformationPanel.GetComponentInChildren<Text>().text = item.GetKanjiName() + "を捨てました。";

        //　アイテム数が0だったらボタンとキャラクターステータスからアイテムを削除
        if (partyStatus.GetItemNum(item) == 0)
        {
            selectedGameObjectStack.Pop();
            itemPanelButtonList.Remove(itemButton);
            Destroy(itemButton);
            partyStatus.GetItemDictionary().Remove(item);

            currentCommand = CommandMode.NoItemPassed;
            useItemPanelCanvasGroup.interactable = false;
            useItemPanel.SetActive(false);
            useItemInformationPanel.transform.SetAsLastSibling();
            useItemInformationPanel.SetActive(true);
            //　ItemPanelに戻る為UseItemPanelの子要素のボタンを全削除
            for (int i = useItemPanel.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(useItemPanel.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            useItemPanelCanvasGroup.interactable = false;
            useItemInformationPanel.transform.SetAsLastSibling();
            useItemInformationPanel.SetActive(true);
            currentCommand = CommandMode.UseItemPanelToUseItemPanel;
        }

        Input.ResetInputAxes();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    //　パーティの情報
    [SerializeField]
    public PartyStatus partyStatus = null;
    //　敵パーティーリスト
    [SerializeField]
    public EnemyPartyStatusList enemyPartyStatusList = null;
    // 敵パーティ
    private EnemyPartyStatus enemyPartyStatus;
    //　キャラクターのベース位置
    [SerializeField]
    private Transform battleBasePosition;
    //　現在戦闘に参加しているキャラクター
    private List<GameObject> allCharacterList = new List<GameObject>();

    //　現在戦闘に参加している全キャラクター
    private List<GameObject> allCharacterInBattleList = new List<GameObject>();
    //　現在戦闘に参加している味方キャラクター
    private List<GameObject> allyCharacterInBattleList = new List<GameObject>();
    //　現在戦闘に参加している敵キャラクター
    private List<GameObject> enemyCharacterInBattleList = new List<GameObject>();
    //　現在の攻撃の順番
    private int currentAttackOrder;
    //　現在攻撃をしようとしている人が選択中
    private bool isChoosing;
    //　戦闘が開始しているかどうか
    private bool isStartBattle;
    //コマンド選択が終了しているかどうか
    private bool isStartTarn;
    // 前のターンに逃走に失敗しているか
    private bool isFailGetAway;
    //　戦闘シーンの最初の攻撃が始まるまでの待機時間
    [SerializeField]
    private float firstWaitingTime = 1f;
    //　戦闘シーンのキャラ移行時の間の時間
    [SerializeField]
    private float timeToNextCharacter = 1f;
    //　待ち時間
    private float waitTime;
    //　戦闘シーンの最初の攻撃が始まるまでの経過時間
    private float elapsedTime;
    //　戦闘が終了したかどうか
    private bool battleIsOver;
    //　現在のコマンド
    private CommandMode currentCommand;
    // 現在のターン数
    private int turn;

    // プレイヤー操作用のUI
    [SerializeField]
    private GameObject playerUI;
    // ターン開始ボタン
    [SerializeField]
    private Button attackButton = null;
    // コマンドキャンセルボタン
    [SerializeField]
    private Button cancelButton = null;
    // 逃げるボタン
    [SerializeField]
    private Button getAwayButton = null;
    //　味方パーティーのコマンドパネル
    [SerializeField]
    private Transform commandPanel = null;
    //　戦闘用キャラクター選択ボタンプレハブ
    [SerializeField]
    private GameObject battleCharacterButton = null;
    //　SelectCharacterPanel
    [SerializeField]
    private Transform selectCharacterPanel = null;
    //　魔法やアイテム選択パネル
    [SerializeField]
    private Transform magicOrItemPanel = null;
    //　魔法やアイテム選択パネルのContent
    private Transform magicOrItemPanelContent = null;
    // NoUseMPSkillボタンのプレハブ
    [SerializeField]
    private GameObject NoUseMPSkill = null;
    // UseMPSkillボタンのプレハブ
    [SerializeField]
    private GameObject UseMPSkill = null;
    //　BattleItemPanelButtonプレハブ
    [SerializeField]
    private GameObject battleItemPanelButton = null;
    //　BattleMagicPanelButtonプレハブ
    [SerializeField]
    private GameObject battleMagicPanelButton = null;
    //　最後に選択していたゲームオブジェクトをスタック
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();

    //　MagicOrItemPanelでどの番号のボタンから上にスクロールするか
    [SerializeField]
    private int scrollDownButtonNum = 8;
    //　MagicOrItemPanelでどの番号のボタンから下にスクロールするか
    [SerializeField]
    private int scrollUpButtonNum = 10;

    //　ScrollManager
    //private ScrollManager scrollManager;

    //ターン数表示用テキスト
    [SerializeField]
    private Text turnText;
    //　メッセージプレハブ
    [SerializeField]
    private GameObject messageImage;
    //　BattleUI
    [SerializeField]
    private Transform battleUI;
    //　メッセージパネルインスタンス
    private GameObject messagePanelIns;

    //　結果表示処理スクリプト
    private BattleResult battleResult;
    [SerializeField]
    private GameObject battleResultManager;

    private BattleStatusScript battleStatusScript;

    public class NextAct
    {
        public Skill skill { get; set; }
        public GameObject targetChara { get; set; }
        public Item item { get; set; }
    }

    //味方キャラクターの選択した行動リスト
    private Dictionary<string, NextAct> nextActs = new Dictionary<string, NextAct>();

    public enum CommandMode
    {
        SelectCommand,
        SelectDirectAttacker,
        SelectMagicTarget,
        SelectItem,
        SelectRecoveryItemTarget
    }

    // Start is called before the first frame update
    void Start()
    {
        magicOrItemPanelContent = magicOrItemPanel;
        int size = enemyPartyStatusList.GetPartyMembersList().Count;
        enemyPartyStatus = enemyPartyStatusList.GetPartyMembersList()[(int)(Random.value * 100) % size];
        //　同じ名前の敵がいた場合の処理に使うリスト
        List<string> enemyNameList = new List<string>();

        Transform characterTransform;
        List<GameObject> instances = new List<GameObject>();
        GameObject ins;
        string characterName;

        int positionNum = 0;
        //　味方パーティーのプレハブをインスタンス化
        foreach(AllyStatus allyChara in partyStatus.GetAllyStatus())
        {
            characterTransform = battleBasePosition.Find("AllyPos" + positionNum).transform;
            ins = Instantiate<GameObject>(allyChara.GetBattleObject(), characterTransform.position, characterTransform.rotation);        
            ins.AddComponent<CharacterBattleScript>();
            ins.GetComponent<CharacterBattleScript>().characterStatus = allyChara;
            ins.name = allyChara.GetCharacterName();
            if (allyChara.GetHp() > 0)
            {
                allyCharacterInBattleList.Add(ins);
                allCharacterList.Add(ins);
            }
            positionNum++;
        }
        if (enemyPartyStatus == null)
        {
            Debug.LogError("敵パーティーデータが設定されていません。");
        }
        //　敵パーティーのプレハブをインスタンス化
        for (int i = 0; i < enemyPartyStatus.GetEnemyGameObjectList().Count; i++)
        {
            characterTransform = battleBasePosition.Find("EnemyPos" + i).transform;
            ins = Instantiate<GameObject>(enemyPartyStatus.GetEnemyGameObjectList()[i], characterTransform.position, characterTransform.rotation);
            //　既に同じ敵が存在したら文字を付加する
            characterName = ins.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetCharacterName();
            if (!enemyNameList.Contains(characterName))
            {
                ins.name = characterName + 'A';
            }
            else
            {
                ins.name = characterName + (char)('A' + enemyNameList.Count(enemyName => enemyName == characterName));
            }
            enemyNameList.Add(characterName);
            enemyCharacterInBattleList.Add(ins);
            allCharacterList.Add(ins);
        }
        //　キャラクターリストをキャラクターの素早さの高い順に並べ替え
        allCharacterList = allCharacterList.OrderByDescending(character => character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetAgility()).ToList<GameObject>();
        //　現在の戦闘
        allCharacterInBattleList = allCharacterList.ToList<GameObject>();
        Debug.Log(allCharacterInBattleList.Count);
        //　確認の為並べ替えたリストを表示
        foreach (var character in allCharacterInBattleList)
        {
            Debug.Log(character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetCharacterName() + " : " + character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetAgility());
        }
        //　戦闘前の待ち時間を設定
        waitTime = firstWaitingTime;
        //　ランダム値のシードの設定
        Random.InitState((int)Time.time);

        battleResult = battleResultManager.GetComponent<BattleResult>();

        isStartTarn = false;

        isFailGetAway = false;

        //　最初の全データ表示
        battleStatusScript = GameObject.Find("BattleUICanvas/PlayerUI/StatusPanel").GetComponent<BattleStatusScript>();
        battleStatusScript.DisplayStatus();
        // 選択コマンドボタンを設定
        foreach (GameObject allyCharacterInBattle in allyCharacterInBattleList)
        {
            CharacterStatus allyCharacterStatus = allyCharacterInBattle.GetComponent<CharacterBattleScript>().GetCharacterStatus();
            SetSelectCommand(allyCharacterInBattle, allyCharacterStatus.GetSkillList()[0], enemyCharacterInBattleList[0]);
        }

        //ターンの初期化と表示
        turn = 1;
        turnText.text = "ターン: " + turn.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //　戦闘が終了していたらこれ以降何もしない
        if (battleIsOver)
        {
            return;
        }

        //　戦闘開始
        if (isStartBattle)
        {
            //味方のコマンド選択が終了しているか
            if (isStartTarn)
            {
                //攻撃ボタンを使用不可能に
                attackButton.interactable = false;
                //キャンセルボタンを使用不可能に
                cancelButton.interactable = false;
                //逃げるボタンを使用不可能に
                getAwayButton.interactable = false;
                //　現在のキャラクターの攻撃が終わっている
                if (!isChoosing)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime < waitTime)
                    {
                        return;
                    }
                    elapsedTime = 0f;
                    isChoosing = true;

                    //　キャラクターの攻撃の選択に移る
                    MakeAttackChoise(allCharacterInBattleList[currentAttackOrder]);
                    //　次のキャラクターのターンにする
                    currentAttackOrder++;
                    //　全員攻撃が終わったら最初から
                    if (currentAttackOrder >= allCharacterInBattleList.Count)
                    {
                        //前のターンに逃走に失敗しているなら戻す
                        isFailGetAway = false;
                        //攻撃ボタンを使用可能に
                        attackButton.interactable = true;
                        //キャンセルボタンを使用可能に
                        cancelButton.interactable = true;
                        //逃げるボタンを使用可能に
                        getAwayButton.interactable = true;
                        Debug.Log(allCharacterInBattleList.Count);
                        isStartTarn = false;
                        currentAttackOrder = 0;
                        //ターンの表示
                        turn++;
                        turnText.text = "ターン: " + turn.ToString();
                    }
                }
            }
        }
        else
        {
            Debug.Log("経過時間： " + elapsedTime);
            //　戦闘前の待機
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waitTime)
            {
                //　2回目以降はキャラ間の時間を設定
                waitTime = timeToNextCharacter;
                //　最初のキャラクターの待ち時間は0にする為にあらかじめ条件をクリアさせておく
                elapsedTime = timeToNextCharacter;
                isStartBattle = true;
            }
        }
    }

    //攻撃開始ボタンを押した時の処理
    public void OnClickAttacklButton()
    {
        isStartTarn = true;
    }

    // 逃げるボタンを押した時
    public void OnClickGetAwayButton()
    {
        var randomValue = Random.value;
        if (0f <= randomValue && randomValue <= 0.5f)
        {
            Debug.Log("逃げるのに成功した。");
            ShowMessage("逃げるのに成功した。");
            battleIsOver = true;
            commandPanel.gameObject.SetActive(false);
            CharacterBattleScript characterBattleScript;
            foreach (var character in allCharacterList)
            {
                //　味方キャラクターの戦闘で増減したHPとMPを通常のステータスに反映させる
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if (characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //　戦闘終了
            playerUI.SetActive(false);
            battleResult.InitialProcessingOfRanAwayResult();
        }
        else
        {
            Debug.Log("逃げるのに失敗した。");
            ShowMessage("逃げるのに失敗した。");
            commandPanel.gameObject.SetActive(false);
            isFailGetAway = true;
            isStartTarn = true;
        }
    }

    //　キャラクター選択パネルのリセット
    private void ResetCharacterPanel()
    {
        // キャラクター選択ボタンがあれば全て削除
        for (int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
        }

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
        selectCharacterPanel.gameObject.SetActive(false);
    }

    //キャンセルボタンを押した時の処理
    public void OnClickCancelButton()
    {
        if (isStartTarn)
        {
            return;
        }
        if (currentCommand == CommandMode.SelectCommand)
        {
            commandPanel.gameObject.SetActive(false);
        }
        else
        {
            if (currentCommand == CommandMode.SelectDirectAttacker || currentCommand == CommandMode.SelectMagicTarget)
            {
                currentCommand = CommandMode.SelectCommand;
            }
            else
            {
                // magicOrItemPanelにボタンがあれば全て削除
                for (int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
                }
                magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
                magicOrItemPanel.gameObject.SetActive(false);
                if (currentCommand == CommandMode.SelectItem)
                {
                    currentCommand = CommandMode.SelectCommand;
                }
                else if (currentCommand == CommandMode.SelectRecoveryItemTarget)
                {
                    currentCommand = CommandMode.SelectItem;
                }
            }
            ResetCharacterPanel();
            commandPanel.GetComponent<CanvasGroup>().interactable = true;
            EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
        }
    }

    //　キャラクターの攻撃の選択処理
    public void MakeAttackChoise(GameObject character)
    {
        CharacterStatus characterStatus = character.GetComponent<CharacterBattleScript>().GetCharacterStatus();
        //　EnemyStatusにキャスト出来る場合は敵の攻撃処理
        if (characterStatus as EnemyStatus != null)
        {
            //　敵の行動アルゴリズム
            int randomValue = (int)(Random.value * characterStatus.GetSkillList().Count);
            var targetNum = (int)(Random.value * allyCharacterInBattleList.Count); 
            AttackProcess(character, characterStatus.GetSkillList()[randomValue], allyCharacterInBattleList[targetNum]);
        }
        else
        {            
            AttackProcess(character, nextActs[characterStatus.GetCharacterName()].skill, nextActs[characterStatus.GetCharacterName()].targetChara, nextActs[characterStatus.GetCharacterName()].item);
        }
    }

    //　味方の攻撃処理
    public void AllySelectCommand(GameObject character)
    {
        //攻撃ボタンを使用不可能に
        attackButton.interactable = false;
        //逃げるボタンを使用不可能に
        getAwayButton.interactable = false;
        //キャンセルボタンを使用可能に
        cancelButton.interactable = true;

        currentCommand = CommandMode.SelectCommand;

        // コマンドセレクトボタンがあれば全て削除
        for (int i = commandPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(commandPanel.transform.GetChild(i).gameObject);
        }

        // 魔法やアイテムパネルの子要素のContentにボタンがあれば全て削除
        for (int i = magicOrItemPanelContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(magicOrItemPanelContent.transform.GetChild(i).gameObject);
        }

        commandPanel.GetComponent<CanvasGroup>().interactable = true;
        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = false;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;

        //　キャラクターがガード状態であればガードを解く
        /*
        if (character.GetComponent<Animator>().GetBool("Guard"))
        {
            character.GetComponent<CharacterBattleScript>().UnlockGuard();
        }
        */

        //　キャラクターの名前を表示
        //commandPanel.Find("CharacterName/Text").GetComponent<Text>().text = character.gameObject.name;

        var characterSkill = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();

        //　持っているスキルに応じてコマンドボタンの表示
        GameObject NoUseMPSkillIns;
        GameObject UseMPSkillIns;
        var skillList = character.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();

        foreach (var skill in skillList)
        {
            if (skill.GetSkillType() == Skill.Type.DirectAttack)
            {
                NoUseMPSkillIns = Instantiate<GameObject>(NoUseMPSkill, commandPanel);
                NoUseMPSkillIns.transform.Find("SkillName").GetComponent<Text>().text = skill.GetKanjiName();
                NoUseMPSkillIns.GetComponent<Button>().onClick.AddListener(() => SelectDirectAttacker(character));
            }
            if (skill.GetSkillType() == Skill.Type.Guard)
            {
                NoUseMPSkillIns = Instantiate<GameObject>(NoUseMPSkill, commandPanel);
                NoUseMPSkillIns.transform.Find("SkillName").GetComponent<Text>().text = skill.GetKanjiName();
                NoUseMPSkillIns.GetComponent<Button>().onClick.AddListener(() => SetSelectCommand(character, skill));
            }
            if (skill.GetSkillType() == Skill.Type.Item)
            {
                NoUseMPSkillIns = Instantiate<GameObject>(NoUseMPSkill, commandPanel);
                NoUseMPSkillIns.transform.Find("SkillName").GetComponent<Text>().text = skill.GetKanjiName();
                NoUseMPSkillIns.GetComponent<Button>().onClick.AddListener(() => SelectItem(character));
            }
            if (skill.GetSkillType() == Skill.Type.MagicAttack
                || skill.GetSkillType() == Skill.Type.RecoveryMagic
                || skill.GetSkillType() == Skill.Type.IncreaseAttackPowerMagic
                || skill.GetSkillType() == Skill.Type.IncreaseDefencePowerMagic
                || skill.GetSkillType() == Skill.Type.NumbnessRecoveryMagic
                || skill.GetSkillType() == Skill.Type.PoisonnouRecoveryMagic
                )
            {
                UseMPSkillIns = Instantiate<GameObject>(UseMPSkill, commandPanel);
                UseMPSkillIns.transform.Find("SkillName").GetComponent<Text>().text = skill.GetKanjiName();
                UseMPSkillIns.transform.Find("UseMagicPoints").GetComponent<Text>().text = ((Magic)skill).GetAmountToUseMagicPoints().ToString();

                //　MPが足りない時はボタンを押しても何もせず魔法の名前を暗くする
                if (character.GetComponent<CharacterBattleScript>().GetMp() < ((Magic)skill).GetAmountToUseMagicPoints())
                {
                    UseMPSkillIns.transform.Find("SkillName").GetComponent<Text>().color = new Color(0.4f, 0.4f, 0.4f);
                }
                else
                {
                    UseMPSkillIns.GetComponent<Button>().onClick.AddListener(() => SelectUseMagicTarget(character, skill));
                }
            }
        }
        
        EventSystem.current.SetSelectedGameObject(commandPanel.transform.GetChild(1).gameObject);
        commandPanel.gameObject.SetActive(true);
    }

    //　キャラクター選択
    public void SelectDirectAttacker(GameObject attackCharacter)
    {
        currentCommand = CommandMode.SelectDirectAttacker;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleCharacterButtonIns;

        foreach (var enemy in enemyCharacterInBattleList)
        {
            battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
            battleCharacterButtonIns.transform.Find("Text").GetComponent<Text>().text = enemy.gameObject.name;
            //　攻撃するキャラのDirectAttackスキルを取得する
            var characterSkill = attackCharacter.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
            Skill directAtatck = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack);
            battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => SetSelectCommand(attackCharacter, directAtatck, enemy));
        }

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }

    //　防御
    public void Guard(GameObject guardCharacter)
    {
        guardCharacter.GetComponent<CharacterBattleScript>().Guard();
        commandPanel.gameObject.SetActive(false);
        ChangeNextChara();
    }

    //　魔法を使う相手の選択
    public void SelectUseMagicTarget(GameObject user, Skill skill)
    {
        currentCommand = CommandMode.SelectMagicTarget;
        commandPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleCharacterButtonIns;

        if (skill.GetSkillType() == Skill.Type.MagicAttack)
        {
            foreach (var enemy in enemyCharacterInBattleList)
            {
                battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
                battleCharacterButtonIns.transform.Find("Text").GetComponent<Text>().text = enemy.gameObject.name;
                battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => SetSelectCommand(user, skill, enemy));
            }
        }
        else
        {
            foreach (var allyCharacter in allyCharacterInBattleList)
            {
                battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
                battleCharacterButtonIns.transform.Find("Text").GetComponent<Text>().text = allyCharacter.gameObject.name;
                battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => SetSelectCommand(user, skill, allyCharacter));
            }
        }

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }

    //　使用するアイテムの選択
    public void SelectItem(GameObject character)
    {

        var itemDictionary = partyStatus.GetItemDictionary();

        //　MagicOrItemPanelのスクロール値の初期化
        //scrollManager.Reset();
        var battleItemPanelButtonNum = 0;

        GameObject battleItemPanelButtonIns;

        foreach (var item in itemDictionary.Keys)
        {
            if (item.GetItemType() == Item.Type.HPRecovery
                || item.GetItemType() == Item.Type.MPRecovery
                || item.GetItemType() == Item.Type.NumbnessRecovery
                || item.GetItemType() == Item.Type.PoisonRecovery
                )
            {
                battleItemPanelButtonIns = Instantiate<GameObject>(battleItemPanelButton, magicOrItemPanelContent);
                battleItemPanelButtonIns.transform.Find("ItemName").GetComponent<Text>().text = item.GetKanjiName();
                battleItemPanelButtonIns.transform.Find("Num").GetComponent<Text>().text = partyStatus.GetItemNum(item).ToString();
                battleItemPanelButtonIns.GetComponent<Button>().onClick.AddListener(() => SelectItemTarget(character, item));

                //　指定した番号のアイテムパネルボタンにアイテムスクロール用スクリプトを取り付ける
                if (battleItemPanelButtonNum != 0
                    && (battleItemPanelButtonNum % scrollDownButtonNum == 0
                    || battleItemPanelButtonNum % (scrollDownButtonNum + 1) == 0)
                    )
                {
                    //　アイテムスクロールスクリプトの取り付けて設定値のセット
                    //battleItemPanelButtonIns.AddComponent<ScrollDownScript>();
                }
                else if (battleItemPanelButtonNum != 0
                  && (battleItemPanelButtonNum % scrollUpButtonNum == 0
                  || battleItemPanelButtonNum % (scrollUpButtonNum + 1) == 0)
                  )
                {
                    //battleItemPanelButtonIns.AddComponent<ScrollUpScript>();
                }
                //　ボタン番号を足す
                battleItemPanelButtonNum++;

                if (battleItemPanelButtonNum == scrollUpButtonNum + 2)
                {
                    battleItemPanelButtonNum = 2;
                }
            }
        }

        if (magicOrItemPanelContent.childCount > 0)
        {
            currentCommand = CommandMode.SelectItem;
            commandPanel.GetComponent<CanvasGroup>().interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            magicOrItemPanel.GetComponent<CanvasGroup>().interactable = true;
            EventSystem.current.SetSelectedGameObject(magicOrItemPanelContent.GetChild(0).gameObject);
            magicOrItemPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("使えるアイテムがありません。");
            ShowMessage("使えるアイテムがありません。");
        }
    }

    //　アイテムを使用する相手を選択
    public void SelectItemTarget(GameObject user, Item item)
    {
        currentCommand = CommandMode.SelectRecoveryItemTarget;
        magicOrItemPanel.GetComponent<CanvasGroup>().interactable = false;
        selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

        GameObject battleCharacterButtonIns;

        //　使用するキャラのItemスキルを取得する
        var characterSkill = user.GetComponent<CharacterBattleScript>().GetCharacterStatus().GetSkillList();
        Skill useItem = characterSkill.Find(skill => skill.GetSkillType() == Skill.Type.Item);

        foreach (var allyCharacter in allyCharacterInBattleList)
        {
            battleCharacterButtonIns = Instantiate<GameObject>(battleCharacterButton, selectCharacterPanel);
            battleCharacterButtonIns.transform.Find("Text").GetComponent<Text>().text = allyCharacter.gameObject.name;
            battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => SetSelectCommand(user, useItem, allyCharacter, item));　//回復アイテムのみ
            //battleCharacterButtonIns.GetComponent<Button>().onClick.AddListener(() => UseItem(user, allyCharacter, item));
        }

        selectCharacterPanel.GetComponent<CanvasGroup>().interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.GetChild(0).gameObject);
        selectCharacterPanel.gameObject.SetActive(true);
    }

    //　アイテム使用
    public void UseItem(GameObject user, GameObject targetCharacter, Item item)
    {
        var userCharacterBattleScript = user.GetComponent<CharacterBattleScript>();
        var targetCharacterBattleScript = targetCharacter.GetComponent<CharacterBattleScript>();
        var skill = userCharacterBattleScript.GetCharacterStatus().GetSkillList().Find(skills => skills.GetSkillType() == Skill.Type.Item);

        CharacterBattleScript.BattleState battleState = CharacterBattleScript.BattleState.Idle;

        if (item.GetItemType() == Item.Type.HPRecovery)
        {
            if (targetCharacterBattleScript.GetHp() == targetCharacterBattleScript.GetCharacterStatus().GetMaxHp())
            {
                Debug.Log(targetCharacter.name + "は全快です。");
                ShowMessage(targetCharacter.name + "は全快です。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.UseHPRecoveryItem;
        }
        else if (item.GetItemType() == Item.Type.MPRecovery)
        {
            if (targetCharacterBattleScript.GetMp() == targetCharacterBattleScript.GetCharacterStatus().GetMaxMp())
            {
                Debug.Log(targetCharacter.name + "はMP回復をする必要がありません。");
                ShowMessage(targetCharacter.name + "はMP回復をする必要がありません。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.UseMPRecoveryItem;
        }
        else if (item.GetItemType() == Item.Type.NumbnessRecovery)
        {
            if (!targetCharacterBattleScript.IsNumbness())
            {
                Debug.Log(targetCharacter.name + "は痺れ状態ではありません。");
                ShowMessage(targetCharacter.name + "は痺れ状態ではありません。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.UseNumbnessRecoveryItem;
        }
        else if (item.GetItemType() == Item.Type.PoisonRecovery)
        {
            if (!targetCharacterBattleScript.IsPoison())
            {
                Debug.Log(targetCharacter.name + "は毒状態ではありません。");
                ShowMessage(targetCharacter.name + "は毒状態ではありません。");
                return;
            }
            battleState = CharacterBattleScript.BattleState.UsePoisonRecoveryItem;
        }
        userCharacterBattleScript.ChooseAttackOptions(battleState, targetCharacter, skill, item);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        ResetCharacterPanel();
    }

    //選択したコマンドをセットする
    private void SetSelectCommand(GameObject allyCharacterInBattle, Skill skill, GameObject targetChara = null, Item item = null)
    {
        CharacterStatus allyCharacterStatus = allyCharacterInBattle.GetComponent<CharacterBattleScript>().GetCharacterStatus();
        NextAct nextAct = new NextAct();
        nextAct.skill = skill;
        nextAct.targetChara = targetChara;
        nextAct.item = item;
        nextActs[allyCharacterStatus.GetCharacterName()] = nextAct;
        battleStatusScript.UpdateSelect(allyCharacterStatus, skill.GetKanjiName(), AllySelectCommand, allyCharacterInBattle);
        commandPanel.gameObject.SetActive(false);
        magicOrItemPanel.gameObject.SetActive(false);
        ResetCharacterPanel();
        //キャンセルボタンを使用不可能に
        cancelButton.interactable = false;
        //攻撃ボタンを使用可能に
        attackButton.interactable = true;
        //逃げるボタンを使用可能に
        getAwayButton.interactable = true;
    }

    //　攻撃処理
    public void AttackProcess(GameObject character, Skill nowSkill, GameObject targetChara, Item item = null)
    {
        CharacterBattleScript characterBattleScript = character.GetComponent<CharacterBattleScript>();
        CharacterStatus characterStatus = characterBattleScript.GetCharacterStatus();

        if (characterStatus.GetSkillList().Count <= 0)
        {
            return;
        }
        //　敵がガード状態であればガードを解く
        /*
        if (character.GetComponent<Animator>().GetBool("Guard"))
        {
            character.GetComponent<CharacterBattleScript>().UnlockGuard();
        }
        */
        character.GetComponent<CharacterBattleScript>().UnlockGuard();

        //味方で逃走失敗なら何もしない
        if (isFailGetAway && characterStatus as AllyStatus != null)
        {
            ChangeNextChara();
            return;
        }

        ShowMessage(characterStatus.GetCharacterName() + "の攻撃");
        if (nowSkill.GetSkillType() == Skill.Type.DirectAttack)
        {
            //　攻撃相手のCharacterBattleScript
            characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, targetChara, nowSkill);
            Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            ShowMessage(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
        }
        else if (nowSkill.GetSkillType() == Skill.Type.MagicAttack)
        {

            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints())
            {
                //　攻撃相手のCharacterBattleScript
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.MagicAttack, targetChara, nowSkill);
                Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
                ShowMessage(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            }
            else
            {
                Debug.Log("MPが足りない！");
                ShowMessage("MPが足りない！");
                //　MPが足りない場合は直接攻撃を行う
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, targetChara, characterStatus.GetSkillList().Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack));
                Debug.Log(character.name + "は攻撃を行った");
                ShowMessage(character.name + "は攻撃を行った");
            }
        }
        else if (nowSkill.GetSkillType() == Skill.Type.RecoveryMagic)
        {
            if (characterBattleScript.GetMp() >= ((Magic)nowSkill).GetAmountToUseMagicPoints())
            {
                //　回復相手のCharacterBattleScript
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.Healing, targetChara, nowSkill);
                Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
                ShowMessage(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            }
            else
            {
                Debug.Log("MPが足りない！");
                ShowMessage("MPが足りない！");
                //　MPが足りない場合は直接攻撃を行う
                characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.DirectAttack, targetChara, characterStatus.GetSkillList().Find(skill => skill.GetSkillType() == Skill.Type.DirectAttack));
                Debug.Log(character.name + "は攻撃を行った");
                ShowMessage(character.name + "は攻撃を行った");
            }
        }
        else if (nowSkill.GetSkillType() == Skill.Type.Item)
        {
            //characterBattleScript.ChooseAttackOptions(CharacterBattleScript.BattleState.UseHPRecoveryItem, targetChara, nowSkill, item);
            UseItem(character, targetChara, item);
        }
        else if (nowSkill.GetSkillType() == Skill.Type.Guard)
        {
            characterBattleScript.Guard();
            // Guardアニメはboolなのでアニメーション遷移させたらすぐに次のキャラクターに移行させる
            ChangeNextChara();
            Debug.Log(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
            ShowMessage(character.name + "は" + nowSkill.GetKanjiName() + "を行った");
        }
    }

    //　次のキャラクターに移行
    public void ChangeNextChara()
    {
        isChoosing = false;
    }

    public void DeleteAllCharacterInBattleList(GameObject deleteObj)
    {
        var deleteObjNum = allCharacterInBattleList.IndexOf(deleteObj);
        allCharacterInBattleList.Remove(deleteObj);
        if (deleteObjNum < currentAttackOrder)
        {
            currentAttackOrder--;
        }
        //　全員攻撃が終わったら最初から
        if (currentAttackOrder >= allCharacterInBattleList.Count)
        {
            currentAttackOrder = 0;
        }
    }

    public void DeleteAllyCharacterInBattleList(GameObject deleteObj)
    {
        allyCharacterInBattleList.Remove(deleteObj);
        deleteObj.SetActive(false);
        if (allyCharacterInBattleList.Count == 0)
        {
            //Debug.Log("味方が全滅");
            ShowMessage("味方が全滅");
            battleIsOver = true;
            CharacterBattleScript characterBattleScript;
            foreach (var character in allCharacterList)
            {
                //　味方キャラクターの戦闘で増減したHPとMPを通常のステータスに反映させる
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if (characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //　敗戦時の結果表示
            battleResult.InitialProcessingOfDefeatResult();
        }
    }

    public void DeleteEnemyCharacterInBattleList(GameObject deleteObj)
    {
        enemyCharacterInBattleList.Remove(deleteObj);

        deleteObj.SetActive(false);
        if (enemyCharacterInBattleList.Count == 0)
        {
            Debug.Log("敵が全滅");
            ShowMessage("敵が全滅");
            battleIsOver = true;
            battleIsOver = true;
            playerUI.SetActive(false);
            CharacterBattleScript characterBattleScript;
            foreach (var character in allCharacterList)
            {
                //　味方キャラクターの戦闘で増減したHPとMPを通常のステータスに反映させる
                characterBattleScript = character.GetComponent<CharacterBattleScript>();
                if (characterBattleScript.GetCharacterStatus() as AllyStatus != null)
                {
                    characterBattleScript.GetCharacterStatus().SetHp(characterBattleScript.GetHp());
                    characterBattleScript.GetCharacterStatus().SetMp(characterBattleScript.GetMp());
                    characterBattleScript.GetCharacterStatus().SetNumbness(characterBattleScript.IsNumbness());
                    characterBattleScript.GetCharacterStatus().SetPoisonState(characterBattleScript.IsPoison());
                }
            }
            //　勝利時の結果表示
            battleResult.InitialProcessingOfVictoryResult(allCharacterList, allyCharacterInBattleList);
        }
        else
        {
            //選択対象のリセット
            foreach (GameObject allyCharacterInBattle in allyCharacterInBattleList)
            {
                CharacterStatus allyCharacterStatus = allyCharacterInBattle.GetComponent<CharacterBattleScript>().GetCharacterStatus();
                if (nextActs[allyCharacterStatus.GetCharacterName()].targetChara == deleteObj)
                {
                    nextActs[allyCharacterStatus.GetCharacterName()].targetChara = enemyCharacterInBattleList[0];
                }
            }
        }
    }

    //　メッセージ表示
    public void ShowMessage(string message) {
        if(messagePanelIns != null) {
            Destroy(messagePanelIns);
        }
        messagePanelIns = Instantiate<GameObject>(messageImage, battleUI);
        messagePanelIns.transform.Find("Text").GetComponent<Text>().text = message;
    }
}

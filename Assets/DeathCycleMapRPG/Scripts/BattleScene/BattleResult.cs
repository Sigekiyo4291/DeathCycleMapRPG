using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class BattleResult : MonoBehaviour
{

    //　結果を表示してからワールドマップに戻れるようになるまでの時間
    [SerializeField]
    private float timeToDisplay = 2f;
    [SerializeField]
    private GameObject resultPanel;
    [SerializeField]
    private Text resultText;
    [SerializeField]
    private PartyStatus partyStatus;
    //　戦闘結果表示をしているかどうか
    private bool isDisplayResult;
    //　結果を表示し戦闘から抜け出せるかどうか
    private bool isFinishResult;
    //　戦闘に勝利したかどうか
    private bool won;
    //　逃げたかどうか
    private bool ranAway;
    //　レベルアップに必要な経験値
    private const int LEVEL_UP_POINT = 100;

    //　MusicManager
    //[SerializeField]
    //private MusicManager musicManager;

    void Update()
    {
        //　結果表示前は何もしない
        if (!isDisplayResult)
        {
            return;
        }

        //　戦闘を抜け出すまでの待機時間を越えていない
        if (!isFinishResult)
        {
            return;
        }
    }

    //シーン移動
    public void OnCheckButton()
    {
        if (won || ranAway)
        {
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    //　勝利時の初期処理
    public void InitialProcessingOfVictoryResult(List<GameObject> allCharacterList, List<GameObject> allyCharacterInBattleList)
    {
        StartCoroutine(DisplayVictoryResult(allCharacterList, allyCharacterInBattleList));
    }

    //　勝利時の結果
    public IEnumerator DisplayVictoryResult(List<GameObject> allCharacterList, List<GameObject> allyCharacterInBattleList)
    {
        yield return new WaitForSeconds(timeToDisplay);
        won = true;
        resultPanel.SetActive(true);
        //　戦闘で獲得した経験値
        var earnedExperience = 0;
        //　戦闘で獲得したお金
        var earnedMoney = 0;
        //　戦闘で獲得したアイテムとその個数
        Dictionary<Item, int> getItemDictionary = new Dictionary<Item, int>();
        //　Floatのランダム値
        float randomFloat;
        //　アイテム取得確率
        float probability;
        //　キャラクターステータス
        CharacterStatus characterStatus;
        //　敵のアイテムディクショナリー
        ItemDictionary enemyItemDictionary;
        // プレイヤーのレベル
        int playerLv = allyCharacterInBattleList[0].GetComponent<CharacterBattleScript>().GetCharacterStatus().GetLevel();

        foreach (var character in allCharacterList)
        {
            characterStatus = character.GetComponent<CharacterBattleScript>().GetCharacterStatus();
            if (characterStatus as EnemyStatus != null)
            {
                earnedExperience += LevelDifferenceCorrection(((EnemyStatus)characterStatus).GetLevel(), playerLv, ((EnemyStatus)characterStatus).GetGettingExperience());
                earnedMoney += ((EnemyStatus)characterStatus).GetGettingMoney();
                enemyItemDictionary = ((EnemyStatus)characterStatus).GetDropItemDictionary();
                //　敵が持っているアイテムの種類の数だけ繰り返し
                foreach (var item in enemyItemDictionary.Keys)
                {
                    //　0～100の間のランダム値を取得
                    randomFloat = Random.Range(0f, 100f);
                    //　アイテムの取得確率を取得
                    probability = enemyItemDictionary[item];
                    //　ランダム値がアイテム取得確率以下の値であればアイテム取得
                    if (randomFloat <= probability)
                    {
                        if (getItemDictionary.ContainsKey(item))
                        {
                            getItemDictionary[item]++;
                        }
                        else
                        {
                            getItemDictionary.Add(item, 1);
                        }
                    }
                }
            }
        }
        resultText.text = earnedExperience + "の経験値を獲得した。\n";
        resultText.text += earnedMoney + "のお金を獲得した。\n";

        //　パーティーステータスにお金を反映する
        partyStatus.SetMoney(partyStatus.GetMoney() + earnedMoney);

        //　取得したアイテムを取得する
        foreach (var item in getItemDictionary.Keys)
        {
            //　既にアイテムを持っている時
            if (partyStatus.GetItemDictionary().ContainsKey(item))
            {
                partyStatus.SetItemNum(item, partyStatus.GetItemNum(item) + getItemDictionary[item]);
            }
            else
            {
                partyStatus.SetItemDictionary(item, getItemDictionary[item]);
            }
            resultText.text += item.GetKanjiName() + "を" + getItemDictionary[item] + "個手に入れた。\n";
            resultText.text += "\n";

        }

        //　上がったレベル
        var levelUpCount = 0;
        //　上がった素早さ
        var raisedAgility = 0;
        //　上がった力
        var raisedPower = 0;
        //　上がった打たれ強さ
        var raisedStrikingStrength = 0;
        //　上がった魔法力
        var raisedMagicPower = 0;
        //　レベルアップ前のHP
        var nowMaxHP = 0;
        //　レベルアップ前のMP
        var nowMaxMP = 0;
        //　LevelUpData
        LevelUpData levelUpData;
        // レベルアップのテーブル
        List<int> agilityRisingTable;
        List<int> powerRisingTable;
        List<int> strikingStrengthRisingTable;
        List<int> magicPowerRisingTable;

        //　レベルアップ等の計算
        foreach (var characterObj in allyCharacterInBattleList)
        {
            var character = (AllyStatus)characterObj.GetComponent<CharacterBattleScript>().GetCharacterStatus();
            //　変数を初期化
            levelUpCount = 0;
            raisedAgility = 0;
            raisedPower = 0;
            raisedStrikingStrength = 0;
            raisedMagicPower = 0;
            nowMaxHP = character.GetMaxHp();
            nowMaxMP = character.GetMaxMp();
            levelUpData = character.GetLevelUpData();

            agilityRisingTable = levelUpData.GetAgilityRisingTable();
            powerRisingTable = levelUpData.GetPowerRisingTable();
            strikingStrengthRisingTable = levelUpData.GetStrikingStrengthRisingTable();
            magicPowerRisingTable = levelUpData.GetMagicPowerRisingTable();

            //　キャラクターに経験値を反映
            character.SetEarnedExperience(character.GetEarnedExperience() + earnedExperience);

            //　そのキャラクターの経験値で何レベルアップしたかどうか
            levelUpCount = character.GetEarnedExperience() / LEVEL_UP_POINT;
            character.SetEarnedExperience(character.GetEarnedExperience() % LEVEL_UP_POINT);
            
            //　レベルアップ分のステータスアップを計算し反映する
            for (int i = 1; i <= levelUpCount; i++)
            {
                //　レベルを反映
                character.SetLevel(character.GetLevel() + 1);

                raisedAgility += agilityRisingTable[character.GetLevel() % agilityRisingTable.Count()];
                raisedPower += powerRisingTable[character.GetLevel() % powerRisingTable.Count()];
                raisedStrikingStrength += strikingStrengthRisingTable[character.GetLevel() % strikingStrengthRisingTable.Count()];
                raisedMagicPower += magicPowerRisingTable[character.GetLevel() % strikingStrengthRisingTable.Count];
            }
            
            if (levelUpCount > 0)
            {
                resultText.text += character.GetCharacterName() + "は" + levelUpCount + "レベル上がってLv" + character.GetLevel() + "になった。\n";
                if (raisedAgility > 0)
                {
                    resultText.text += "素早さが" + raisedAgility + "上がった。\n";
                }
                if (raisedPower > 0)
                {
                    resultText.text += "力が" + raisedPower + "上がった。\n";
                }
                if (raisedStrikingStrength > 0)
                {
                    resultText.text += "打たれ強さが" + raisedStrikingStrength + "上がった。\n";
                }
                if (raisedMagicPower > 0)
                {
                    resultText.text += "魔法力が" + raisedMagicPower + "上がった。\n";
                }
                resultText.text += "\n";
            }
            character.StatusUpdate(raisedPower, raisedAgility, raisedStrikingStrength, raisedMagicPower);
            //レベルの上がった分だけHP、MPを回復
            character.SetHp(character.GetHp() + (character.GetMaxHp() - nowMaxHP));
            character.SetMp(character.GetMp() + (character.GetMaxMp() - nowMaxMP));
        }
        //　結果を計算し終わった
        isDisplayResult = true;

        //　戦闘終了のBGMに変更する
        //musicManager.ChangeBGM();

        //　結果後に数秒待機
        yield return new WaitForSeconds(timeToDisplay);
        //　戦闘から抜け出す
        //resultPanel.transform.Find("FinishText").gameObject.SetActive(true);
        isFinishResult = true;
    }

    //　取得Expのレベル差の補正
    private int LevelDifferenceCorrection(int enemyLv, int playerLv, int Exp)
    {
        int levelDifference = playerLv - enemyLv;
        if (levelDifference > 5)
        {
            Exp = Mathf.Max(1, (int)(Exp*(float)((10-levelDifference)/10)));
        }
        return Exp;
    }

    //　敗戦時の初期処理
    public void InitialProcessingOfDefeatResult()
    {
        StartCoroutine(DisplayDefeatResult());
    }

    //　敗戦時の表示
    public IEnumerator DisplayDefeatResult()
    {
        yield return new WaitForSeconds(timeToDisplay);
        resultPanel.SetActive(true);
        resultText.text = "勇者達は全滅した。";
        isDisplayResult = true;
        yield return new WaitForSeconds(timeToDisplay);
        var finishText = resultPanel.transform.Find("FinishText");
        finishText.GetComponent<Text>().text = "タイトルへ";
        finishText.gameObject.SetActive(true);

        //　味方が全滅したのでHPを回復しておく
        var unityChanStatus = partyStatus.GetAllyStatus().Find(character => character.GetCharacterName() == "たつぽん");
        if (unityChanStatus != null)
        {
            unityChanStatus.SetHp(1);
        }

        isFinishResult = true;
    }

    //　逃げた時の初期処理
    public void InitialProcessingOfRanAwayResult()
    {
        StartCoroutine(DisplayRanAwayResult());
    }

    //　逃げた時の表示
    public IEnumerator DisplayRanAwayResult()
    {
        yield return new WaitForSeconds(timeToDisplay);
        ranAway = true;
        resultPanel.SetActive(true);
        resultText.text = "勇者たちは逃げ出した。";
        isDisplayResult = true;
        yield return new WaitForSeconds(timeToDisplay);
        isFinishResult = true;
    }
}

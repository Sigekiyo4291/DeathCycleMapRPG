using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager 
{
    // 使うノベルデータ
    private NovelData novelData;

    //　使うバトルシーンにおける敵のリスト
    private EnemyPartyStatusList enemyPartyStatusList;

    //　ノベルシーンの読み込み、使用するノベルデータ名を渡す
    public void LoadEventScene(string novelDataName)
    {
        //使用するノベルデータを取得
        novelData = Resources.Load<NovelData>("EventData/" + novelDataName);

        // イベントに登録
        SceneManager.sceneLoaded += EventSceneLoaded;

        // シーン切り替え
        SceneManager.LoadScene("EventScene");
    }

    private void EventSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameManager = GameObject.Find("System/NovelSystem").GetComponent<NovelSystem>();

        // データを渡す処理
        gameManager.novelData = novelData;

        // イベントから削除
        SceneManager.sceneLoaded -= EventSceneLoaded;
    }

    //　バトルシーンの読み込み、使用する敵データを渡す
    public void LoadBattleScene(EnemyPartyStatusList useEnemyPartyStatusList)
    {
        //使用する敵のリストデータを取得
        //enemyPartyStatusList = Resources.Load<EnemyPartyStatusList>("BattleEnemyData/" + enemyPartyStatusListName);
        enemyPartyStatusList = useEnemyPartyStatusList;

        // イベントに登録
        SceneManager.sceneLoaded += BattleSceneLoaded;

        // シーン切り替え
        SceneManager.LoadScene("BattleScene");
    }

    private void BattleSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        // データを渡す処理
        gameManager.enemyPartyStatusList = enemyPartyStatusList;

        // イベントから削除
        SceneManager.sceneLoaded -= BattleSceneLoaded;
    }
}
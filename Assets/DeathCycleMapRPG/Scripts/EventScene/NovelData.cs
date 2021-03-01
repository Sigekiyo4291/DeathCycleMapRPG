using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// ノベルデータ
/// </summary>
[CreateAssetMenu(fileName = "NovelData", menuName = "ScriptableObject/NovelData")]
public partial class NovelData : ScriptableObject
{
	/// <summary>
	/// ノベルデータの説明(注釈)
	/// </summary>
	[SerializeField]
	[Tooltip("ノベルデータの説明等(自由欄)")]
	public string comment;

	/// <summary>
	/// 敵データリストの集合
	/// </summary>
	[SerializeField]
	public List<EnemyPartyStatusList> enemyPartyStatusLists = new List<EnemyPartyStatusList>();

	/// <summary>
	/// アイテムリスト
	/// </summary>
	[SerializeField]
	public List<Item> items = new List<Item>();

	/// <summary>
	/// 加入する仲間
	/// </summary>
	[SerializeField]
	public List<AllyStatus> allyList = new List<AllyStatus>();

	/// <summary>
	/// ノベルコマンド
	/// </summary>
	[SerializeField]
	public List<Command> commands = new List<Command>();

	/// <summary>
	/// パーティステータス
	/// </summary>
	private PartyStatus partyStatus;
	public PartyStatus PartyStatus
    {
        get
        {
			if(partyStatus == null)
            {
				partyStatus = Resources.Load<PartyStatus>("PartyStatus/PartyStatus");
                if (partyStatus == null)
                {
					Debug.LogError("PartyStatus not found.");
                }
			}

			return partyStatus;
		}
    }

	/// <summary>
	/// コマンドデータ
	/// </summary>
	[System.Serializable]
	public class Command
	{
		/// <summary>
		/// コマンドID
		/// </summary>
		[SerializeField]
		public int id;

		/// <summary>
		/// コマンド引数
		/// </summary>
		[SerializeField]
		public string[] parameters;
	}

}

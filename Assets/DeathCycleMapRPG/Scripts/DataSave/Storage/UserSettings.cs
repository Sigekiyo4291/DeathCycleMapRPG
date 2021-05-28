//-----------------------------------------------------------------------------------
// File Name       : LocalStorage/UserSettings.cs
// Author          : Yugo Fujioka
// Creation Date   : 02/02/2018
 
// Copyright © 2018 Tanoshimasu CO., LTD.
//-----------------------------------------------------------------------------------

#define DEMO


/// <summary>
/// ユーザー設定（ローカルストレージに保存される）
/// ※頻繁に更新されるので大量のデータを格納しないこと
/// </summary>
[System.Serializable]
public sealed class UserSettings : Storage.ISerializer {
	// 文字列キャッシュ
	// プロパティに直接定義するとプロパティアクセスの度に無駄なメモリアロケートが発生する
	[System.NonSerialized]
	private const string magic_ = "UserSettings_180101";
	[System.NonSerialized]
	private const string fileName_ = "/UserSettings";

	// インターフェース実装
	public string magic     { get { return UserSettings.magic_; } }    // マジックNo.
	public int version      { get { return 1; } }                      // バージョンNo.
	public string fileName  { get { return UserSettings.fileName_; } } // 保存先
	public System.Type type { get { return typeof(UserSettings); } }   // JSONデシリアライズ用型宣言
#if DEMO
	public Storage.FORMAT format { get { return this.format_; }  set { this.format_ = value; } }  // 保存形式
	public bool encrypt          { get { return this.encrypt_; } set { this.encrypt_ = value; } } // 暗号化するか（任意）
	public bool backup           { get { return this.backup_; }  set { this.backup_ = value; } }  // バックアップを取るか（任意）
#else
	// 本来は決め打ってしまっていい
	public Storage.FORMAT format { get { return Storage.FORMAT.BINARY; } } // 保存形式
	public bool encrypt          { get { return true; } }                  // 暗号化するか（任意）
	public bool backup           { get { return true; } }                  // バックアップを取るか（任意）
#endif


	/// <summary>
	/// バージョン更新処理
	/// </summary>
	/// <param name="oldVersion">読み込まれたバージョン</param>
	/// <returns>破棄する場合はfalse</returns>
	public bool UpdateVersion(int oldVersion) {
		//switch (oldVersion) {
		//	case 1:
		//		break;
		//}
		return true;
	}

	/// <summary>
	/// 複製
	/// </summary>
	public Storage.ISerializer Clone() {
		return this.MemberwiseClone() as Storage.ISerializer;
	}

#if DEMO
	// デモ用に外部から書きかえれるようにしている
	[System.NonSerialized]
	private Storage.FORMAT format_ = Storage.FORMAT.BINARY;
	[System.NonSerialized]
	private bool encrypt_ = false;
	[System.NonSerialized]
	private bool backup_ = false;
#endif

	public long date = 0L;	// 更新時間(64bit)
	public int count = 0;   // 更新回数
	public SaveAllyStatus saveAllyStatus = new SaveAllyStatus();

	/// <summary>
	/// 簡易リセット
	/// </summary>
	public void Clear() {
#if DEMO
		this.format_ = Storage.FORMAT.BINARY;
		this.encrypt_ = false;
		this.backup_ = false;
#endif

		this.date = System.DateTime.MinValue.ToBinary();
		this.count = 0;
		this.saveAllyStatus = new SaveAllyStatus();
	}
}

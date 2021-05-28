//-----------------------------------------------------------------------------------
// File Name       : LocalStorage/Sample.cs
// Author          : Yugo Fujioka
// Creation Date   : 02/02/2018
 
// Copyright © 2018 Tanoshimasu CO., LTD.
//-----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using Storage;


/// <summary>
/// サンプルクラス
/// </summary>
public class Sample : MonoBehaviour {
	#region DEFINE
	public const string INFO_FORMAT =	"バージョン\t\t{0}\n" + 
										"暗号化\t\t\t{1}\n" +
										"形式\t\t\t{2}\n" +
										"バックアップ\t{3}\n" +
										"保存時刻\t\t{4}\n" +
										"保存回数\t\t{5}";

	/// <summary>
	/// 処理状態
	/// </summary>
	private enum STATE {
		IDLE,
		SAVING,
		LOADING,
		ASYNC_WAIT,
		FINISH,
	}
	#endregion


	#region MEMBER
	private StorageControl control = null;			// 各種設定表記

	private StorageManager storageManager = null;	// ストレージ制御
	private UserSettings usedSettings = null;		// 現在のデータ
	private UserSettings procSettings = null;       // 処理中のデータ
	private SaveAllyStatus saveAllyStatus = null;       // 現在のデータ
	private SaveAllyStatus procSaveAllyStatus = null;       // 処理中のデータ
	private FinishHandler ioHandler = null;			// ストレージアクセスコールバック
	private STATE state = STATE.IDLE;				// 処理状態
	private string accessMessage = "NOTHING";
	[SerializeField]
	private UserData userData = null; //プレイヤーのデータ
	[SerializeField]
	private SaveData saveData = null;
	[SerializeField]
	private SystemData systemData = null;
	[SerializeField]
	private AllyStatus allyStatus = null;
	private IO_RESULT result = IO_RESULT.NONE;		// 結果
	private float ioTime = 0f;                      // 処理開始時刻
	private float accessTime = 0f;
	#endregion


	#region MAIN FUNCTION
	/// <summary>
	/// 初期化
	/// </summary>
	void Start() {
		this.control = this.GetComponent<StorageControl>();

		this.ioHandler = new FinishHandler(this.IOHandler);
		this.storageManager = new StorageManager();
		this.usedSettings = this.procSettings = new UserSettings();
		this.saveAllyStatus = this.procSaveAllyStatus = new SaveAllyStatus();

		// 例外
		this.UpdateDataInfo((IO_RESULT)999);
	}

	/// <summary>
	/// 更新
	/// </summary>
	void Update() {
		// キャプチャ
#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetKeyDown(KeyCode.P))
			ScreenCapture.CaptureScreenshot("capture.png");
#endif

		switch (this.state) {
			case STATE.IDLE:
				break;
			case STATE.ASYNC_WAIT:
				StartCoroutine(this.AsyncWait());
				this.state = STATE.FINISH;
				break;
			case STATE.FINISH:
				this.accessTime += Time.deltaTime;

				this.control.access.text = this.accessMessage;
				int count = (int)(this.accessTime / 0.1f) % 4;
				for (int i = 0; i < count; ++i)
					this.control.access.text += ".";
				break;
		}
	}
	#endregion


	#region PRIVATE FUNCTION
	/// <summary>
	/// 完了コールバック
	/// </summary>
	/// <param name="ret">結果</param>
	/// <param name="dataInfo">結果情報</param>
	private void IOHandler(IO_RESULT ret, ref DataInfo dataInfo) {
		this.result = ret;
		switch (ret) {
			case IO_RESULT.SAVE_SUCCESS:
				// 保存成功
				if (dataInfo.async) {
					this.state = STATE.ASYNC_WAIT;
					break;
				}
				this.UpdateDataInfo(ret);
				this.state = STATE.IDLE;
				break;
			case IO_RESULT.SAVE_FAILED:
				// 保存失敗
				if (dataInfo.async) {
					this.state = STATE.ASYNC_WAIT;
					break;
				}
				this.state = STATE.IDLE;
				break;
			case IO_RESULT.LOAD_SUCCESS:
				// 読込成功
				/*
				this.procSettings = dataInfo.serializer as UserSettings;
				this.procSettings.encrypt = dataInfo.encrypt;
				this.procSettings.format = dataInfo.format;
				this.procSettings.backup = dataInfo.backup;
				*/
				this.procSaveAllyStatus = dataInfo.serializer as SaveAllyStatus;
				this.procSaveAllyStatus.encrypt = dataInfo.encrypt;
				this.procSaveAllyStatus.format = dataInfo.format;
				this.procSaveAllyStatus.backup = dataInfo.backup;
				this.allyStatus.StatusLoad(this.procSaveAllyStatus);
				if (dataInfo.async) {
					this.state = STATE.ASYNC_WAIT;
					break;
				}
				this.UpdateDataInfo(ret);
				this.state = STATE.IDLE;
				break;
			case IO_RESULT.LOAD_FAILED:
				// 読込失敗
				if (dataInfo.async) {
					this.state = STATE.ASYNC_WAIT;
					break;
				}
				this.state = STATE.IDLE;
				break;
			case IO_RESULT.NONE:
				// データ不備
				this.state = STATE.IDLE;
				break;
		}
	}

	/// <summary>
	/// 非同期完了待ち
	/// </summary>
	private IEnumerator AsyncWait() {
		// 非同期最低待ち時間（※保存や読込のアニメーションを一定時間表示させるため）
		while ((Time.realtimeSinceStartup - this.ioTime) < 1.8f)
			yield return null;

		this.UpdateDataInfo(this.result);
	}

	/// <summary>
	/// データ情報更新
	/// </summary>
	/// <param name="result">結果</param>
	private void UpdateDataInfo(IO_RESULT result) {
		UserSettings us = this.usedSettings = this.procSettings;
		SaveAllyStatus sas = this.saveAllyStatus = this.procSaveAllyStatus;
		//us.format = this.control.saveJson.isOn ? FORMAT.JSON : FORMAT.BINARY;
		//us.encrypt = this.control.saveEncrypt.isOn;
		//us.backup = this.control.saveBackup.isOn;
		if (us.count > 0) {
			this.control.dataInfo.text = string.Format(INFO_FORMAT, us.version, us.encrypt, us.format, us.backup, System.DateTime.FromBinary(us.date).ToString(), us.count);
			this.control.filePath.text = Application.persistentDataPath + us.fileName;
		} else {
			this.control.dataInfo.text = string.Format(INFO_FORMAT, "--", "--", "--", "--", "-/-/---- --:--:--", "--");
			this.control.filePath.text = "----";
		}
		this.state = STATE.IDLE;

		switch (result) {
			case IO_RESULT.NONE:
				this.accessMessage = "NOTHING";
				break;
			case IO_RESULT.SAVE_SUCCESS:
			case IO_RESULT.LOAD_SUCCESS:
				this.accessMessage = "SUCCESS";
				break;
			case IO_RESULT.SAVE_FAILED:
			case IO_RESULT.LOAD_FAILED:
				this.accessMessage = "FAILED";
				break;
			default:
				this.accessMessage = "NOTHING";
				break;
		}
		this.control.access.text = this.accessMessage;
	}
	#endregion


	#region CONTROL FUNCTION
	/// <summary>
	/// 保存
	/// </summary>
	public void Save() {
		this.ioTime = Time.realtimeSinceStartup;
		this.accessTime = 0f;

		// 保存設定（※デモ用の設定変更）
		this.procSettings = this.usedSettings.Clone() as UserSettings;
		this.saveAllyStatus = this.saveAllyStatus.Clone() as SaveAllyStatus;
		UserSettings us = this.procSettings;
		SaveAllyStatus sas= this.procSaveAllyStatus;
		us.format = this.control.saveJson.isOn ? FORMAT.JSON : FORMAT.BINARY;
		us.encrypt = this.control.saveEncrypt.isOn;
		us.backup = this.control.saveBackup.isOn;
		// 内容更新（適当に内容を更新）
		System.DateTime date = System.DateTime.Now;
		us.date = date.ToBinary();
		us.count += 1;
		us.saveAllyStatus.StatusCopy(this.allyStatus);

		sas.format = this.control.saveJson.isOn ? FORMAT.JSON : FORMAT.BINARY;
		sas.encrypt = this.control.saveEncrypt.isOn;
		sas.backup = this.control.saveBackup.isOn;
		// 内容更新（適当に内容を更新）
		sas.StatusCopy(this.allyStatus);
		// 保存（※FinishHandlerはnullでも可）
		bool async = this.control.saveAsync.isOn;
		if (async) {
			this.accessMessage = "Now Saving";
		}
		this.storageManager.Save(this.procSettings, this.ioHandler, async);
		this.storageManager.Save(this.procSaveAllyStatus, this.ioHandler, async);
	}

	/// <summary>
	/// 読込
	/// </summary>
	public void Load() {
		this.ioTime = Time.realtimeSinceStartup;
		this.accessTime = 0f;

		// 読込
		bool async = this.control.loadAsync.isOn;
		if (async) {
			this.accessMessage = "Now Loading";
		}
		//this.storageManager.Load(this.usedSettings, this.ioHandler, async);
		this.storageManager.Load(this.saveAllyStatus, this.ioHandler, async);
	}

	/// <summary>
	/// 削除
	/// </summary>
	public void Delete() {
		if (!this.storageManager.Exists(this.usedSettings)) {
			return;
		}

		this.storageManager.Delete(this.usedSettings);
		this.usedSettings.Clear();
		this.procSettings.Clear();

		this.storageManager.Delete(this.saveAllyStatus);
		this.saveAllyStatus.Clear();
		this.procSaveAllyStatus.Clear();
		this.UpdateDataInfo(IO_RESULT.NONE);
	}

	/// <summary>
	/// 初期化
	/// </summary>
	public void Clear() {
		if (this.usedSettings.count == 0 || !this.storageManager.Exists(this.usedSettings)) {
			return;
		}

		this.usedSettings.Clear();
		this.procSettings.Clear();

		this.saveAllyStatus.Clear();
		this.procSaveAllyStatus.Clear();
		this.UpdateDataInfo(IO_RESULT.NONE);
	}
	#endregion
}

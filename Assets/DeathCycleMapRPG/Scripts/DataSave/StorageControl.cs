//-----------------------------------------------------------------------------------
// File Name       : LocalStorage/StorageControl.cs
// Author          : Yugo Fujioka
// Creation Date   : 02/02/2018
 
// Copyright © 2018 Tanoshimasu CO., LTD.
//-----------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 各種設定ポインタ
/// </summary>
public class StorageControl : MonoBehaviour {
	[Tooltip("IOアクセス結果")]
	public Text access = null;
	[Tooltip("ファイルパス情報")]
	public Text filePath = null;
	[Tooltip("現在のデータ情報")]
	public Text dataInfo = null;
	[Tooltip("テストフラッグ")]
	public Text TestFlag = null;
	[Tooltip("保存非同期設定")]
	public Toggle saveAsync = null;
	[Tooltip("保存暗号化設定")]
	public Toggle saveEncrypt = null;
	[Tooltip("保存形式JSON設定")]
	public Toggle saveJson = null;
	[Tooltip("保存バックアップ設定")]
	public Toggle saveBackup = null;
	[Tooltip("読込非同期設定")]
	public Toggle loadAsync = null;
}

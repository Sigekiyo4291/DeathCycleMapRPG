#define DEMO
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveAllyStatus : Storage.ISerializer
{
    // 文字列キャッシュ
    // プロパティに直接定義するとプロパティアクセスの度に無駄なメモリアロケートが発生する
    [System.NonSerialized]
    private const string magic_ = "SaveAllyStatus_180101";
    [System.NonSerialized]
    private const string fileName_ = "/SaveAllyStatus";

    // インターフェース実装
    public string magic { get { return SaveAllyStatus.magic_; } }    // マジックNo.
    public int version { get { return 1; } }                      // バージョンNo.
    public string fileName { get { return SaveAllyStatus.fileName_; } } // 保存先
    public System.Type type { get { return typeof(SaveAllyStatus); } }   // JSONデシリアライズ用型宣言
#if DEMO
    public Storage.FORMAT format { get { return this.format_; } set { this.format_ = value; } }  // 保存形式
    public bool encrypt { get { return this.encrypt_; } set { this.encrypt_ = value; } } // 暗号化するか（任意）
    public bool backup { get { return this.backup_; } set { this.backup_ = value; } }  // バックアップを取るか（任意）
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
    public bool UpdateVersion(int oldVersion)
    {
        //switch (oldVersion) {
        //	case 1:
        //		break;
        //}
        return true;
    }

    /// <summary>
    /// 複製
    /// </summary>
    public Storage.ISerializer Clone()
    {
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

    //　キャラクターの名前
    [SerializeField]
    private string characterName = "";
    //　毒状態かどうか
    [SerializeField]
    private bool isPoisonState = false;
    //　痺れ状態かどうか
    [SerializeField]
    private bool isNumbnessState = false;
    //　キャラクターのレベル
    [SerializeField]
    private int level = 1;
    //　素早さ
    [SerializeField]
    private int agility = 5;
    //　力
    [SerializeField]
    private int power = 5;
    //　打たれ強さ
    [SerializeField]
    private int strikingStrength = 5;
    //　魔法力
    [SerializeField]
    private int magicPower = 5;
    // 攻撃力
    private int attackPower;
    // 守備力
    private int defencePower;
    //　最大HP
    [SerializeField]
    private int maxHp = 100;
    //　HP
    [SerializeField]
    private int hp = 100;
    //　最大MP
    [SerializeField]
    private int maxMp = 50;
    //　MP
    [SerializeField]
    private int mp = 50;
    //　持っているスキル
    [SerializeField]
    private List<Skill> skillList = null;
    //属性カット率
    [SerializeField]
    private int cutFlame = 0;
    [SerializeField]
    private int cutThunder = 0;
    [SerializeField]
    private int cutIce = 0;

    //　獲得経験値
    [SerializeField]
    private int earnedExperience = 0;
    //　装備している武器
    [SerializeField]
    private Weapon equipWeapon = null;
    //　装備している鎧
    [SerializeField]
    private Armor equipArmor = null;
    //装備しているアクセサリ
    [SerializeField]
    private Accessory equipAccessory1 = null;
    [SerializeField]
    private Accessory equipAccessory2 = null;

    //　レベルアップデータ
    [SerializeField]
    private LevelUpData levelUpData = null;

    //　初期ステータスデータ
    [SerializeField]
    private AllyStatus initialStatus = null;

    //　バトル時のオブジェクト
    [SerializeField]
    private GameObject BattleObject = null;

    //キャラ名のGet,SET
    public void SetCharacterName(string characterName)
    {
        this.characterName = characterName;
    }

    public string GetCharacterName()
    {
        return characterName;
    }

    //毒状態のGet,SET
    public void SetPoisonState(bool poisonFlag)
    {
        isPoisonState = poisonFlag;
    }

    public bool IsPoisonState()
    {
        return isPoisonState;
    }

    //麻痺状態のGet,SET
    public void SetNumbness(bool numbnessFlag)
    {
        isNumbnessState = numbnessFlag;
    }

    public bool IsNumbnessState()
    {
        return isNumbnessState;
    }

    //レベルのGet,SET
    public void SetLevel(int level)
    {
        this.level = level;
    }

    public int GetLevel()
    {
        return level;
    }

    //最大HPのGet,SET
    public void SetMaxHp()
    {
        this.maxHp = 30 + this.level + this.strikingStrength * 4;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    //HPのGet,SET
    public void SetHp(int hp)
    {
        this.hp = Mathf.Max(0, Mathf.Min(GetMaxHp(), hp));
    }

    public int GetHp()
    {
        return hp;
    }

    //最大MPのGet,SET
    public void SetMaxMp()
    {
        this.maxMp = this.level + this.magicPower * 3;
    }

    public int GetMaxMp()
    {
        return maxMp;
    }

    //MPのGet,SET
    public void SetMp(int mp)
    {
        this.mp = Mathf.Max(0, Mathf.Min(GetMaxMp(), mp));
    }

    public int GetMp()
    {
        return mp;
    }

    //素早さのGet,SET
    public void SetAgility(int agility)
    {
        this.agility = agility;
    }

    public int GetAgility()
    {
        return agility;
    }

    //力のGet,SET
    public void SetPower(int power)
    {
        this.power = power;
    }

    public int GetPower()
    {
        return power;
    }

    //体力のGet,SET
    public void SetStrikingStrength(int strikingStrength)
    {
        this.strikingStrength = strikingStrength;
    }

    public int GetStrikingStrength()
    {
        return strikingStrength;
    }

    //魔力のGet,SET
    public void SetMagicPower(int magicPower)
    {
        this.magicPower = magicPower;
    }

    public int GetMagicPower()
    {
        return magicPower;
    }

    //攻撃力のGet,SET
    public void SetAttackPower(int attackPower)
    {
        this.attackPower = attackPower;
    }

    public int GetAttackPower()
    {
        return attackPower;
    }

    //防御力のGet,SET
    public void SetDefencePower(int defencePower)
    {
        this.defencePower = defencePower;
    }

    public int GetDefencePower()
    {
        return defencePower;
    }

    //スキルセットのGet,SET
    public void SetSkillList(List<Skill> skillList)
    {
        this.skillList = skillList;
    }

    public List<Skill> GetSkillList()
    {
        return skillList;
    }

    //属性耐性のGet,SET
    public void SetCutFlame(int cutFlame)
    {
        this.cutFlame = cutFlame;
    }
    public int GetCutFlame()
    {
        return cutFlame;
    }

    public void SetCutThunder(int cutThunder)
    {
        this.cutThunder = cutThunder;
    }
    public int GetCutThunder()
    {
        return cutThunder;
    }

    public void SetCutIce(int cutIce)
    {
        this.cutIce = cutIce;
    }
    public int GetcutIce()
    {
        return cutIce;
    }

    //　レベルアップデータを返す
    public LevelUpData GetLevelUpData()
    {
        return levelUpData;
    }

    //　バトル時のオブジェクトを返す
    public GameObject GetBattleObject()
    {
        return BattleObject;
    }

    public void SetEarnedExperience(int earnedExperience)
    {
        this.earnedExperience = earnedExperience;
    }

    public int GetEarnedExperience()
    {
        return earnedExperience;
    }

    public void SetEquipWeapon(Weapon weaponItem)
    {
        //何か装備をしているならば
        if (this.equipWeapon != null)
        {
            RemoveSkill(this.equipWeapon.GetSkill());
        }
        this.equipWeapon = weaponItem;
        // 装備を外す時でなければ
        if (this.equipWeapon != null)
        {
            AddSkill(this.equipWeapon.GetSkill());
        }
    }

    public Weapon GetEquipWeapon()
    {
        return equipWeapon;
    }

    public void SetEquipArmor(Armor armorItem)
    {
        //何か装備をしているならば
        if (this.equipArmor != null)
        {
            RemoveSkill(this.equipArmor.GetSkill());
        }
        this.equipArmor = armorItem;
        // 装備を外す時でなければ
        if (this.equipArmor != null)
        {
            AddSkill(this.equipArmor.GetSkill());
        }
    }

    public Armor GetEquipArmor()
    {
        return equipArmor;
    }

    public void SetEquipAccessory1(Accessory equipAccessory1)
    {
        //何か装備をしているならば
        if (this.equipAccessory1 != null)
        {
            RemoveSkill(this.equipAccessory1.GetSkill());
        }
        this.equipAccessory1 = equipAccessory1;
        // 装備を外す時でなければ
        if (this.equipAccessory1 != null)
        {
            AddSkill(this.equipAccessory1.GetSkill());
        }
    }

    public Accessory GetEquipAccessory1()
    {
        return equipAccessory1;
    }

    public void SetEquipAccessory2(Accessory equipAccessory2)
    {
        //何か装備をしているならば
        if (this.equipAccessory2 != null)
        {
            RemoveSkill(this.equipAccessory2.GetSkill());
        }
        this.equipAccessory2 = equipAccessory2;
        // 装備を外す時でなければ
        if (this.equipAccessory2 != null)
        {
            AddSkill(this.equipAccessory2.GetSkill());
        }
    }

    public Accessory GetEquipAccessory2()
    {
        return equipAccessory2;
    }

    // スキルの追加
    public void AddSkill(Skill skill)
    {
        if (skill == null)
        {
            return;
        }
        List<Skill> newSkillList = this.GetSkillList();
        newSkillList.Add(skill);
        this.SetSkillList(newSkillList);
    }
    // スキルの削除
    public void RemoveSkill(Skill skill)
    {
        if (skill == null)
        {
            return;
        }
        List<Skill> newSkillList = this.GetSkillList();
        newSkillList.Remove(skill);
        this.SetSkillList(newSkillList);
    }

    /// <summary>
    /// 簡易リセット
    /// </summary>
    public void Clear()
    {
#if DEMO
        this.format_ = Storage.FORMAT.BINARY;
        this.encrypt_ = false;
        this.backup_ = false;
#endif

        //　キャラクターの名前
        this.SetCharacterName("");
        //　毒状態かどうか
        this.isPoisonState = this.IsPoisonState();
        //　痺れ状態かどうか
        this.isNumbnessState = this.IsNumbnessState();
        //　キャラクターのレベル
        this.SetLevel(1);
        //　素早さ
        this.SetAgility(5);
        //　力
        this.SetPower(10);
        //　打たれ強さ
        this.SetStrikingStrength(10);
        //　魔法力
        this.SetMagicPower(10);
        // 攻撃力
        this.SetAttackPower(10);
        // 守備力
        this.SetDefencePower(10);
        //　最大HP
        this.SetMaxHp();
        //　HP
        this.SetHp(1);
        //　最大MP
        this.SetMaxMp();
        //　MP
        this.SetMp(1);
        //属性カット率
        this.SetCutFlame(0);
        this.SetCutThunder(0);
        this.SetCutIce(0);
        //　獲得経験値
        this.SetEarnedExperience(0);
        //　装備している武器
        this.SetEquipWeapon(null);
        //　装備している鎧
        this.SetEquipArmor(null);
        //装備しているアクセサリ
        this.SetEquipAccessory1(null);
        this.SetEquipAccessory2(null);

    }


    // ステータスをセーブ用にコピー
    public void StatusCopy(AllyStatus allyStatus)
    {
        //　キャラクターの名前
        this.characterName = allyStatus.GetCharacterName();
        //　毒状態かどうか
        this.isPoisonState = allyStatus.IsPoisonState();
        //　痺れ状態かどうか
        this.isNumbnessState = allyStatus.IsNumbnessState();
        //　キャラクターのレベル
        this.level = allyStatus.GetLevel();
        //　素早さ
        this.agility = allyStatus.GetAgility();
        //　力
        this.power = allyStatus.GetPower();
        //　打たれ強さ
        this.strikingStrength = allyStatus.GetStrikingStrength();
        //　魔法力
        this.magicPower = allyStatus.GetMagicPower();
        // 攻撃力
        this.attackPower = allyStatus.GetAttackPower() ;
        // 守備力
        this.defencePower = allyStatus.GetDefencePower();
        //　最大HP
        this.maxHp = allyStatus.GetMaxHp();
        //　HP
        this.hp = allyStatus.GetHp();
        //　最大MP
        this.maxMp = allyStatus.GetMaxMp();
        //　MP
        this.mp = allyStatus.GetMp();
        //　持っているスキル
        this.skillList = allyStatus.GetSkillList();
        //属性カット率
        this.cutFlame = allyStatus.GetCutFlame();
        this.cutThunder = allyStatus.GetCutThunder();
        this.cutIce = allyStatus.GetcutIce();
        //　獲得経験値
        this.earnedExperience = allyStatus.GetEarnedExperience();
        //　装備している武器
        this.equipWeapon = allyStatus.GetEquipWeapon();
        //　装備している鎧
        this.equipArmor = allyStatus.GetEquipArmor();
        //装備しているアクセサリ
        this.equipAccessory1 = allyStatus.GetEquipAccessory1();
        this.equipAccessory2 = allyStatus.GetEquipAccessory2();
    }

}

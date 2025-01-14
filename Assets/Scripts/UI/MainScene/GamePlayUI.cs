using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    private Transform safeArea;
    private Button buttonBack;
    private Button buttonRefresh;
    private Button buttonSetting;
    private Text levelText;
    private GameObject levle1TipText;
    private Vector3 oriTextSize;

    private Transform addHoleTrans;
    private Transform rocketTrans;
    private Transform doubleBoxTrans;

    public Transform HoleTrans { get => addHoleTrans; }
    public Transform RocketTrans { get => rocketTrans; }
    public Transform DoubleBoxTrans { get => doubleBoxTrans; }

    private Transform buttonHolder;
    private Button buttonAddHole;
    private Button buttonRocket;
    private Button buttonDoubleBox;

    public ClockSlider clockSlider;
    private bool hasClock;

    private void Awake()
    {
        safeArea = transform.Find("bg/SafeArea").transform;
        buttonHolder = transform.Find("bg/Bottom Holder").transform;
        levle1TipText = transform.Find("bg/Tutorial lvl1").gameObject;

        buttonBack = safeArea.Find("Button Back").GetComponent<Button>();
        buttonBack.onClick.AddListener(ShowExitUI);
        buttonRefresh = safeArea.Find("Button Refresh").GetComponent<Button>();
        buttonRefresh.onClick.AddListener(RefreshGame);
        buttonSetting = safeArea.Find("Button Setting").GetComponent<Button>();
        buttonSetting.onClick.AddListener(SettingEvent);
        levelText = safeArea.Find("LevelCount").GetComponent<Text>();

        addHoleTrans = buttonHolder.Find("hint_AddHole").transform;
        rocketTrans = buttonHolder.Find("hint_Glass").transform;
        doubleBoxTrans = buttonHolder.Find("hint_DoubleBox").transform;

        buttonAddHole = addHoleTrans.Find("unlock").GetComponent<Button>();
        buttonAddHole.onClick.AddListener(AddHole);
        buttonRocket = rocketTrans.Find("unlock").GetComponent<Button>();
        buttonRocket.onClick.AddListener(RocketClick);
        buttonDoubleBox = doubleBoxTrans.Find("unlock").GetComponent<Button>();

        buttonDoubleBox.onClick.AddListener(DoubleBox);

        clockSlider = safeArea.Find("ClockSlider").GetComponent<ClockSlider>();
        oriTextSize = levelText.transform.localScale;
    }

    private void Start()
    {
        SetItemButtonFalse();
        InitButon();
    }
    private void ShowExitUI()
    {
        UIManager.Instance.ShowUI<ExitGameUI>();
    }
    private void SettingEvent()
    {
        UIManager.Instance.ShowUI<SettingUI>();
    }
    #region 道具按钮
    /// <summary>
    /// 洞口道具
    /// </summary>
    private void AddHole()
    {
        AudioManager.Instance.PlaySFX("Click");
        //没有足够的道具
        if (GameDataManager.CurrentGameData.holeItemCount <= 0)
        {
            UIManager.Instance.ShowUI<ExtraHole>();
            return;
        }

        if (GameManager.Instance.CurScrew != null && GameManager.Instance.CurScrew.IsMoving)
        {
            Debug.Log("当前小球正在移动，无法添加");
            return;
        }

        //没有达到洞口道具上线
        if (MainSceneUI.Instance.emptyHoleManager.CanAddExtraHole())
        {
            GameManager.Instance.AddExtraHole();
            GameDataManager.AddItemCount(ItemType.Hole, -1);
            UpdateHoleButtonText();
        }
        else
        {
            Debug.Log("洞口道具已经达到上线");
        }


    }

    /// <summary>
    /// 火箭道具
    /// </summary>
    private void RocketClick()
    {
        //没有足够的道具了
        AudioManager.Instance.PlaySFX("Click");
        if (GameDataManager.CurrentGameData.rocketItemCount <= 0)
        {
            UIManager.Instance.ShowUI<ExtraRocket>();
            return;
        }
        UIManager.Instance.ShowUI<RocketUI>();
        Invoke(nameof(DelaySetRocketClick), 0.1f);
    }

    //延迟设置为火箭点击状态(避免UI点击事件和Input发生冲突)
    private void DelaySetRocketClick()
    { 
        GameManager.Instance.SetISRocketClick(true);
    }

    /// <summary>
    /// 洞口道具
    /// </summary>
    private void DoubleBox()
    {
        AudioManager.Instance.PlaySFX("Click");
        //没有足够的道具了
        if (GameDataManager.CurrentGameData.doubleBoxItemCount <= 0)
        {
            UIManager.Instance.ShowUI<ExtraBox>();
            return;
        }

        if (GameManager.Instance.CurScrew != null && GameManager.Instance.CurScrew.IsMoving)
        {
            Debug.Log("当前小球正在移动，无法添加");
            return;
        }

        if (GameManager.Instance.GetCurBox().IsMoving)
        {
            Debug.Log("当前盒子正在移动，无法添加");
            return;
        }
        if (GameManager.Instance.CanAddExtraBox())
        {
            GameManager.Instance.ActiveExtraBox();
            GameDataManager.AddItemCount(ItemType.DoubleBox, -1);
            UpdateDoubleButtonText();
        }
    }

    public void UpdateRocketText()
    {
        GameDataManager.AddItemCount(ItemType.Rocket, -1);
        UpdateRocketButtonText();
    }

    #endregion
    /// <summary>
    ///  重新开始游戏
    /// </summary>
    private void RefreshGame()
    {
        UIManager.Instance.ShowUI<RefreshUI>();
    }

    /// <summary>
    /// 设置关卡数
    /// </summary>
    /// <param name="val"></param>
    public void SetLevelNum(int num)
    {
        if (LevelManager.Instance.CurLevel.HasClock)
            return;

        levelText.gameObject.SetActive(true);
        clockSlider.gameObject.SetActive(false);

        if (LevelManager.Instance.CurLevel.IsHard)
        {
            levelText.color = Color.red;

            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            { 
                levelText.text = "HARD LEVEL " + num.ToString();
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                levelText.text = "困难关卡 " + num.ToString();
            }

            levelText.transform.localScale = oriTextSize * 0.6f;
        }
        else
        {
            levelText.color = Color.white;
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                levelText.text = "LEVEL " + num.ToString();
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                levelText.text = "关卡 " + num.ToString();
            }
            levelText.transform.localScale = oriTextSize;
        }
    }

    /// <summary>
    /// 是否展示提示
    /// </summary>
    /// <param name="val"></param>
    public void SetTips(bool val, string tip = "")
    {
        if(levle1TipText != null)
            levle1TipText.SetActive(val);

        levle1TipText.GetComponentInChildren<Text>().text = tip;
    }

    /// <summary>
    /// 初始化所有的按钮为未解锁状态
    /// </summary>
    private void SetItemButtonFalse()
    {
        addHoleTrans.Find("Lock").gameObject.SetActive(true);
        buttonAddHole.gameObject.SetActive(false);

        rocketTrans.Find("Lock").gameObject.SetActive(true);
        buttonRocket.gameObject.SetActive(false);

        doubleBoxTrans.Find("Lock").gameObject.SetActive(true);
        buttonDoubleBox.gameObject.SetActive(false);
    }

    public void GameUIUnlocked(ItemType type)
    {
        if (type == ItemType.Hole)
        {
            addHoleTrans.Find("Lock").gameObject.SetActive(false);
            buttonAddHole.gameObject.SetActive(true);
        }
        else if (type == ItemType.Rocket)
        {
            rocketTrans.Find("Lock").gameObject.SetActive(false);
            buttonRocket.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 检查是否解锁了相应的道具
    /// </summary>
    public void InitButon()
    {
        //解锁了道具一
        if (GameDataManager.CurrentGameData.isHoleLocked == false)
        {
            addHoleTrans.Find("Lock").gameObject.SetActive(false);
            buttonAddHole.gameObject.SetActive(true);
            UpdateHoleButtonText();
        }
        //解锁了道具二
        if (GameDataManager.CurrentGameData.isRocketLocked == false)
        {
            rocketTrans.Find("Lock").gameObject.SetActive(false);
            buttonRocket.gameObject.SetActive(true);
            UpdateRocketButtonText();
        }
        //解锁了道具三
        if (GameDataManager.CurrentGameData.isDoubleBoxLocked == false)
        {
            doubleBoxTrans.Find("Lock").gameObject.SetActive(false);
            buttonDoubleBox.gameObject.SetActive(true);
            UpdateDoubleButtonText();
        }
    }

    private void UpdateDoubleButtonText()
    {
        buttonDoubleBox.transform.Find("btnAdd/Text Count").GetComponent<Text>().text
            = GameDataManager.CurrentGameData.doubleBoxItemCount == 0 ? "+" : GameDataManager.CurrentGameData.doubleBoxItemCount.ToString();
    }

    private void UpdateRocketButtonText()
    {
        buttonRocket.transform.Find("btnAdd/Text Count").GetComponent<Text>().text
            = GameDataManager.CurrentGameData.rocketItemCount == 0 ? "+" : GameDataManager.CurrentGameData.rocketItemCount.ToString();
    }

    private void UpdateHoleButtonText()
    {
        buttonAddHole.transform.Find("btnAdd/Text Count").GetComponent<Text>().text
            = GameDataManager.CurrentGameData.holeItemCount == 0 ? "+" : GameDataManager.CurrentGameData.holeItemCount.ToString();
    }

    /// <summary>
    /// 设置对应的道具文本
    /// </summary>
    /// <param name="type"></param>
    public void UpdateItemCount(ItemType type)
    {

        switch (type)
        {
            case ItemType.Hole:
                UpdateHoleButtonText();
                break;
            case ItemType.Rocket:
                UpdateRocketButtonText();
                break;
            case ItemType.DoubleBox:
                UpdateDoubleButtonText();
                break;
        }
    }

    //解锁道具
    public void UnlockedItem()
    {
        addHoleTrans.Find("Lock").gameObject.SetActive(false);
        buttonAddHole.gameObject.SetActive(true);
        rocketTrans.Find("Lock").gameObject.SetActive(false);
        buttonRocket.gameObject.SetActive(true);
        doubleBoxTrans.Find("Lock").gameObject.SetActive(false);
        buttonDoubleBox.gameObject.SetActive(true);
    }

    //显示倒计时
    public void ShowClockUI(int m, int s)
    {
        levelText.gameObject.SetActive(false);
        clockSlider.gameObject.SetActive(true);
        clockSlider.SetTime(m, s);
    }
}

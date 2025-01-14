using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RewardItemGroup
{
    public List<RewardItem> rewardItems = new List<RewardItem>();
}

public class DailyBonusUI : BaseUI
{
    private Button close;
    private Transform grid;
    private Text timeText;
    [SerializeField] private GameObject dailyRewardRefab;
    [SerializeField] private GameObject rewardPrefab;
    [Space]
    [SerializeField] private List<RewardItemGroup> rewardItemList = new List<RewardItemGroup>();//奖励配置
    [SerializeField] private List<DailyReward> dailyRewardList = new List<DailyReward>();//当前奖励物
    [SerializeField] private int curDailyIndex = 0;
    private TimedObject dailyReward;
    protected override void Awake()
    {
        base.Awake();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);
        grid = tableTransform.Find("ContainReward/Mask/Grid");
        timeText = tableTransform.Find("Top/time").GetComponent<Text>();
    }
    private void Start()
    {
        dailyReward = TimeManager.Instance.GetTimeObj(TimeEventType.DailyReward);
        //生成奖励
        CreateDailyReward();

        EventManager.Instance.RegisterEvent(GameEvent.RestartDailyEvent, () =>
        {
            GameDataManager.ResertRewardIndex();
            HomeSceneUI.Instance.homeUI.UpdateDailyNoti();
            CreateDailyReward();
        });
        
    }

    /// <summary>
    /// 生成每日奖励
    /// </summary>
    private void CreateDailyReward()
    {
        //清空
        TransFormUtility.DestroyAllChildren(grid);
        dailyRewardList.Clear();

        for (int i = 0; i < rewardItemList.Count; i++)
        {
            GameObject newDailyReward = Instantiate(dailyRewardRefab);
            newDailyReward.transform.SetParent(grid);
            newDailyReward.transform.localScale = Vector3.one;

            DailyReward curReward = newDailyReward.GetComponent<DailyReward>();
            curReward.InitDailyRewardItem(rewardItemList[i], rewardPrefab);

            dailyRewardList.Add(curReward);
        }

        curDailyIndex = GameDataManager.CurrentGameData.curDailyIndex;
        for (int i = 0; i < curDailyIndex; i++)
        {
            dailyRewardList[i].gameObject.SetActive(false);
        }

        if (curDailyIndex == dailyRewardList.Count)
        {
            Debug.Log("每日奖励已经领取完毕");
            return;
        }
        dailyRewardList[curDailyIndex].UnlockReward(false);
    }

    private void Update()
    {
        if (dailyReward != null)
        {
            UpdateDailyRewardText();
        }
    }
    public void ChangeDailyReward()
    {
        GameDataManager.AddDailyRewardIndex();
        curDailyIndex = GameDataManager.CurrentGameData.curDailyIndex;

        if (curDailyIndex == dailyRewardList.Count)
        {
            Debug.Log("每日奖励已经领取完毕");
            return;
        }
        dailyRewardList[curDailyIndex].UnlockReward(true);

    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Fade);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        HomeSceneUI.Instance.homeUI.UpdateDailyNoti();
        base.HideUI(delaytime, callback, UIEffectType.Fade);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<DailyBonusUI>();
    }

    private void UpdateDailyRewardText()
    {
        if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
        {
            timeText.text = $"{dailyReward.remainingTime.Hours}h{dailyReward.remainingTime.Minutes}m{dailyReward.remainingTime.Seconds}s";
        }
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
        { 
            timeText.text = $"{dailyReward.remainingTime.Hours}时{dailyReward.remainingTime.Minutes}分{dailyReward.remainingTime.Seconds}秒";
        }
    }
}

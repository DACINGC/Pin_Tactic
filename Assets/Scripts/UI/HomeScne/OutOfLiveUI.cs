using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutOfLiveUI : BaseUI
{
    private Button coinButton;
    private Button freeButton;
    private Button close;

    private Text heartText;
    private Text timeText;
    private TimedObject heartTime;
    protected override void Awake()
    {
        base.Awake();
        coinButton = tableTransform.Find("Button Coin").GetComponent<Button>();
        freeButton = tableTransform.Find("Button Video").GetComponent<Button>();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);
        coinButton.onClick.AddListener(CoinEvent);
        freeButton.onClick.AddListener(FreeEvent);

        heartText = tableTransform.Find("Icon Heart/Text").GetComponent<Text>();
        timeText = tableTransform.Find("Contain Time/Text Time").GetComponent<Text>();
    }

    private void Start()
    {
        EventManager.Instance.RegisterEvent(GameEvent.AddHearEvent, UpdateHeartText);
        heartTime = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);

        if (heartTime == null)
        {
            TimeManager.Instance.LoadTimedObjects();
            heartTime = TimeManager.Instance.GetTimeObj(TimeEventType.Heart);
        }

        //检查当前生命的数量
        if (GameDataManager.CurrentGameData.heartCount < 5 && heartTime.currentState == ActivityState.Stop)
        {
            heartTime.remainingTime = heartTime.startTime.ToTimeSpan();
            heartTime.currentState = ActivityState.Ongoing;
        }
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, effectType);
        UpdateHeartText();
    }
    private void UpdateHeartText()
    {
        int heartCount = GameDataManager.CurrentGameData.heartCount;
        if(YLocalization.lanaguage == YLocalization.Lanaguage.English)
            heartText.text = heartCount < 5 ? heartCount.ToString() : "FULL";
        else if(YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            heartText.text = heartCount < 5 ? heartCount.ToString() : "满了";

        if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            timeText.text = heartCount < 5 ? "" : "FULL";
        else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            timeText.text = heartCount < 5 ? "" : "满了";

    }

    private void Update()
    {
        if (bg.gameObject.activeSelf && GameDataManager.CurrentGameData.heartCount < 5)
        {
            timeText.text = $"{heartTime.remainingTime.Minutes}m{heartTime.remainingTime.Seconds}";
        }
    }
    private void CloseEvent()
    {
        UIManager.Instance.HideUI<OutOfLiveUI>();
        HomeSceneUI.Instance.homeUI.UpdateResourceText();
    }

    /// <summary>
    /// 添加生命
    /// </summary>
    private void CoinEvent()
    {
        if (GameDataManager.CurrentGameData.heartCount >= 5)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("FULL LIFE!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.Chinese)
            { 
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("生命值满了!");
            }
            return;
        }

        if (GameDataManager.DecreaseCoinCount(250))
        {
            GameDataManager.AddHeartCount(5);

            HomeSceneUI.Instance.homeUI.UpdateResourceText();
            UIManager.Instance.HideUI<OutOfLiveUI>();
            UpdateHeartText();
        }
    }

    private void FreeEvent()
    {
        GameDataManager.AddHeartCount(5);

        HomeSceneUI.Instance.homeUI.UpdateResourceText();
        UIManager.Instance.HideUI<OutOfLiveUI>();
        UpdateHeartText();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUpUI : BaseUI
{
    private Button freeButton;
    private Button coinButton;
    private Button closeButton;
    protected override void Awake()
    {
        base.Awake();
        closeButton = tableTransform.Find("Button Close").GetComponent<Button>();
        freeButton = tableTransform.Find("Contain/Button Free").GetComponent<Button>();
        coinButton = tableTransform.Find("Contain/Button Coin").GetComponent<Button>();

        freeButton.onClick.AddListener(FreeEvent);
        coinButton.onClick.AddListener(CoinEvent);
        closeButton.onClick.AddListener(CloseEvent);

    }

    private void FreeEvent()
    {
        MainSceneUI.Instance._GamePlayUI.clockSlider.SetTime(1, 0);
        UIManager.Instance.HideUI<TimeUpUI>();
    }

    private void CoinEvent()
    {
        if (GameDataManager.DecreaseCoinCount(150))
        {
            MainSceneUI.Instance._GamePlayUI.clockSlider.SetTime(1, 0);
            UIManager.Instance.HideUI<TimeUpUI>();
        }
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<TimeUpUI>();
        //如果当前连胜，并且解锁了连胜
        if (GameDataManager.CurrentGameData.isStreaklocked == false && GameDataManager.CurrentGameData.curStreakIndex != 0)
        {
            UIManager.Instance.ShowUI<LoseStreakUI>();
        }
        else
        {
            UIManager.Instance.ShowUI<LoseUI>();
        }
    }
}

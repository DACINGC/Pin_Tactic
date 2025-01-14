using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MainBaseUI
{
    private Button coinButton;
    private Button freeButton;
    private Button closeButton;

    protected override void Awake()
    {
        base.Awake();
        coinButton = tableTransform.Find("Button Gold").GetComponent<Button>();
        freeButton = tableTransform.Find("Button Free").GetComponent<Button>();
        closeButton = tableTransform.Find("Close").GetComponent<Button>();

        coinButton.onClick.AddListener(CoinEvent);
        freeButton.onClick.AddListener(FreeEvent);
        closeButton.onClick.AddListener(CloseEvent);
    }

    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        AudioManager.Instance.PlaySFX("Lose");
        base.ShowUI(callback, effectType);
    }
    private void CloseEvent()
    {
        UIManager.Instance.HideUI<ReviveUI>(0, () => 
        {
            //如果当前连胜，并且解锁了连胜
            if (GameDataManager.CurrentGameData.isStreaklocked == false && GameDataManager.CurrentGameData.curStreakIndex != 0)
            {
                UIManager.Instance.ShowUI<LoseStreakUI>();
            }
            else
            {
                UIManager.Instance.ShowUI<LoseUI>();
            }
        });
    }


    private void CoinEvent()
    {
        if (GameDataManager.DecreaseCoinCount(100))
        {
            UIManager.Instance.HideUI<ReviveUI>();
        }
    }

    private void FreeEvent()
    {
        UIManager.Instance.HideUI<ReviveUI>();
    }
}

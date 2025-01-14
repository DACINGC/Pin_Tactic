using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutOfSpaceUI : MainBaseUI
{
    private Button goldButton;
    private Button freeButton;
    private Button close;

    protected override void Awake()
    {
        base.Awake();
        goldButton = tableTransform.Find("Button Gold").GetComponent<Button>();
        freeButton = tableTransform.Find("Button Free").GetComponent<Button>();
        close = tableTransform.Find("Close").GetComponent<Button>();

        goldButton.onClick.AddListener(GoldEvent);
        freeButton.onClick.AddListener(FreeEvent);
        close.onClick.AddListener(CloseEvent);
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        AudioManager.Instance.PlaySFX("Lose");
        base.ShowUI(callback, effectType);
    }
    private void GoldEvent()
    {
        GameManager.Instance.AddSpaceToReviewGame();
    }

    private void FreeEvent()
    {
        GameManager.Instance.AddSpaceToReviewGame();
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<OutOfSpaceUI>(0, () => 
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
}

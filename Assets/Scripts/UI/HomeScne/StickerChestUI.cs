using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerChestUI : BaseUI
{
    private ChestEffect chest;
    private RectTransform rewardTrans;
    private ChestReward chestReward;
    private Vector3 orginPos;
    private Button close;

    protected override void Awake()
    {
        base.Awake();
        chest = tableTransform.Find("Chest Effect").GetComponent<ChestEffect>();
        rewardTrans = tableTransform.Find("ChestReward").GetComponent<RectTransform>();
        orginPos = rewardTrans.position;
        rewardTrans.gameObject.SetActive(false);
        chestReward = rewardTrans.GetComponent<ChestReward>();

        close = tableTransform.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);
    }

    //初始化奖励物品
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        //初始化奖励列表
        bg.gameObject.SetActive(true);
        chest.SetChest(false);
        close.interactable = false;

        //chestReward.InitRewardItems(StickerManager.Instance.CurSticker.RewardList);


        AnimationUtility.PlayScaleBounce(chest.transform, 1.4f, 0.3f, () =>
        {
            chest.SetChest(true);
            rewardTrans.gameObject.SetActive(true);

            Vector3 targetPosition = orginPos + Vector3.up * 400;

            // 动画效果：位移
            rewardTrans.DOAnchorPos(targetPosition, 0.5f).SetEase(Ease.InOutSine);

            // 动画效果：缩放
            rewardTrans.DOScale(1.4f, 0.5f).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                close.interactable = true;
            });
        });

    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        chest.SetChest(false);
        rewardTrans.position = orginPos;
        rewardTrans.localScale = Vector3.one;
        bg.gameObject.SetActive(false);

        //移动奖励物品
        chestReward.MoveRewardToTarget();
    }

    public void InitChestSprite(Sprite openSprite, Sprite closeSprite, List<RewardItem> items, System.Action callback = null)
    {
        chest.InitChestSprite(openSprite, closeSprite);
        chestReward.InitRewardItems(items, callback);
    }
    private void CloseEvent()
    {
        UIManager.Instance.HideUI<StickerChestUI>();
    }
}

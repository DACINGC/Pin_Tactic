using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerCompletedUI : BaseUI
{
    private Transform title;
    private Transform character;
    protected override void Awake()
    {
        base.Awake();
        title = tableTransform.Find("Title");
        character = tableTransform.Find("Char");
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        character.gameObject.SetActive(false);
        bg.gameObject.SetActive(true);
        AudioManager.Instance.PlaySFX("Win");
        AnimationUtility.PlayScaleBounce(title, 1.2f, 0.3f, () =>
        {
            character.gameObject.SetActive(true);
            AnimationUtility.PlayScaleBounce(character, 1.2f, 0.4f, () =>
            {
                Invoke(nameof(DelayHide), 2f);
            });
        });

    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        bg.gameObject.SetActive(false);

        //初始化宝箱图片
        UIManager.Instance.GetUI<StickerChestUI>().InitChestSprite(
            ResourceLoader.Instance.GetUnlockImageSprite("8open"), 
            ResourceLoader.Instance.GetUnlockImageSprite("8close"),
            StickerManager.Instance.CurSticker.RewardList,
            () => GameDataManager.SetStikcerChestOpen(true)//在开始宝箱之后，设置贴纸宝箱未已开启状态
            );

        UIManager.Instance.ShowUI<StickerChestUI>();
    }

    private void DelayHide()
    {
        UIManager.Instance.HideUI<StickerCompletedUI>();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowStickerUI : BaseUI
{
    private Button close;
    protected override void Awake()
    {
        base.Awake();
        close = tableTransform.Find("Button Close").GetComponent<Button>();
        close.onClick.AddListener(CloseEvent);
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        bg.gameObject.SetActive(true);
        HomeSceneUI.Instance.homeUI.Bg.SetActive(false);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        bg.gameObject.SetActive(false);
        //显示当前贴纸
        StickerManager.Instance.ShowCurSticker();
        //返回收集UI
        UIManager.Instance.ShowUI<CollectionUI>();
        UIManager.Instance.GetUI<CollectionUI>().SetIsClickFalse();
        HomeSceneUI.Instance.homeUI.Bg.SetActive(true);
    }


    private void CloseEvent()
    {
        UIManager.Instance.HideUI<ShowStickerUI>();
    }
}

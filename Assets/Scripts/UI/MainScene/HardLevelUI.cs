using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardLevelUI : MainBaseUI
{
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, UIEffectType.Scale);
        Invoke(nameof(DelayHide), 0.6f);
    }

    private void DelayHide()
    {
        UIManager.Instance.HideUI<HardLevelUI>();
    }
    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, callback, UIEffectType.Scale);
    }
}

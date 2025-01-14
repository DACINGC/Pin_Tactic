using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseUI : BaseUI
{
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.ShowUI(callback, effectType);
        GameManager.Instance.SetGameState(GameState.Stop);
    }
    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        callback += () =>
        {
            GameManager.Instance.SetGameState(GameState.Start);
        };
        base.HideUI(delaytime, callback, effectType);
    }
}

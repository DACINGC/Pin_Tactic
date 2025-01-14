using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseStreakUI : BaseUI
{
    private Button backBtn;//������һ��ui
    private Button tryaginBtn;
    protected Transform containTrans;
    private Button closeButton;
    protected override void Awake()
    {
        base.Awake();
        containTrans = tableTransform.Find("Table/Contain").transform;
        tryaginBtn = containTrans.Find("Button TRYAGAIN").transform.GetComponent<Button>();
        tryaginBtn.onClick.AddListener(TryAgain);
        backBtn = containTrans.Find("Button Back").transform.GetComponent<Button>();
        backBtn.onClick.AddListener(BackPreUI);
        closeButton = tableTransform.Find("Button Close").GetComponent<Button>();
        closeButton.onClick.AddListener(CloseEvent);
    }
    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        //AudioManager.Instance.PlaySFX("Lose");
        base.ShowUI(callback, effectType);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, () =>
        {
            if(!backBtn.gameObject.activeSelf)
                SetBackObj(true);
        }, effectType);
    }
    public void TryAgain()
    {
        Debug.Log("������ʤ");


        bool canRetruenHome = false;//�����ǰ����Ϊ0�����÷�����Ϸ������

        if (GameDataManager.CurrentGameData.heartCount <= 0)
        {
            canRetruenHome = true;
            LevelManager.Instance.ClearLevel();
        }
        UIManager.Instance.HideUI<LoseStreakUI>();
        GameDataManager.AddHeartCount(-1);
        
        //������ʤ
        GameDataManager.ResetStreak();
        //��������������ʾstreakUI
        GameManager.Instance.EnterHomeScene();
        EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);

        HomeSceneUI.Instance.homeUI.ResetStreakEvent(() =>
        {
            if (canRetruenHome == false)
            {
                GameManager.Instance.EnterMainScene();
                LevelManager.Instance.InitLevel();
            }
        });
    }


    private void CloseEvent()
    {
        Debug.Log("����������");

        LevelManager.Instance.ClearLevel();
        GameDataManager.AddHeartCount(-1);
        UIManager.Instance.HideUI<LoseStreakUI>();
        //������ʤ
        GameDataManager.ResetStreak();
        //��������������ʾstreakUI
        GameManager.Instance.EnterHomeScene();
        EventManager.Instance.TriggerEvent(GameEvent.UpdateStreakEvent);

        HomeSceneUI.Instance.homeUI.ResetStreakEvent(() =>
        {

        });
    }
    public void BackPreUI()
    {
        UIManager.Instance.SwitchToPreviousUI();
    }

    public void SetBackObj(bool val)
    {
        backBtn.gameObject.SetActive(val);
    }
}

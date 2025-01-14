using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseUI : MainBaseUI
{
    private Button backBtn;
    private Button tryaginBtn;
    protected Transform containTrans;
    private Button closeButton;
    protected override void Awake()
    {
        base.Awake();
        containTrans = tableTransform.Find("Contain").transform;
        tryaginBtn = containTrans.Find("Button TRYAGAIN").transform.GetComponent<Button>();
        tryaginBtn.onClick.AddListener(TryAgain);
        backBtn = containTrans.Find("Button Back").transform.GetComponent<Button>();
        backBtn.onClick.AddListener(BackPreUI);
        closeButton = tableTransform.Find("CloseButton").GetComponent<Button>();
        closeButton.onClick.AddListener(CloseEvent);
    }

    public override void ShowUI(Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        //AudioManager.Instance.PlaySFX("Lose");
        base.ShowUI(callback, effectType);
    }

    public override void HideUI(float delaytime = 0, Action callback = null, UIEffectType effectType = UIEffectType.Slide)
    {
        base.HideUI(delaytime, ()=>
        {
            if (!backBtn.gameObject.activeSelf)
                SetBackObj(true);
        }, effectType);
    }

    private void CloseEvent()
    {
        Debug.Log("����������");
        GameDataManager.AddHeartCount(-1);
        UIManager.Instance.HideUI<LoseUI>();
        GameManager.Instance.EnterHomeScene();
    }
    private void TryAgain()
    {
        //��������
        if (GameDataManager.CurrentGameData.heartCount <= 0)
        {
            LevelManager.Instance.ClearLevel();
            return;
        }
        GameDataManager.AddHeartCount(-1);
        UIManager.Instance.HideUI<LoseUI>();
        //���¿�ʼ��Ϸ
        LevelManager.Instance.ReStartGmae();
        
    }

    /// <summary>
    /// ����֮ǰ��UI
    /// </summary>
    private void BackPreUI()
    {
        UIManager.Instance.SwitchToPreviousUI();
    }

    /// <summary>
    /// �Ƿ���ʾ������һ��UI
    /// </summary>
    public void SetBackObj(bool val)
    {
        backBtn.gameObject.SetActive(val);
    }
}

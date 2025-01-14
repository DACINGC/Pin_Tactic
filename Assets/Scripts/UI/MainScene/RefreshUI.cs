using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshUI : MainBaseUI
{
    private Button buttonNO;
    private Button buttonYES;

    protected override void Awake()
    {
        base.Awake();
        buttonNO = tableTransform.Find("Contain/Button NO").GetComponent<Button>();
        buttonYES = tableTransform.Find("Contain/Button YES").GetComponent<Button>();

        buttonNO.onClick.AddListener(CloseEvent);
        buttonYES.onClick.AddListener(RefreshGame);
    }

    private void CloseEvent()
    {
        UIManager.Instance.HideUI<RefreshUI>();
    }

    private void RefreshGame()
    {
        //��������
        UIManager.Instance.HideUI<RefreshUI>();
        //���¿�ʼ��Ϸ
        GameDataManager.AddHeartCount(-1);

        if (GameDataManager.CurrentGameData.heartCount <= 0)
        {
            LevelManager.Instance.ClearLevel();
            return;
        }

        LevelManager.Instance.ReStartGmae();
    }
}

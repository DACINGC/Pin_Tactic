using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitGameUI : MainBaseUI
{
    private Button yesBtn;
    private Button noBtn;

    protected override void Awake()
    {
        base.Awake();
        yesBtn = tableTransform.Find("YESBtn").GetComponent<Button>();
        noBtn = tableTransform.Find("NOBtn").GetComponent<Button>();

        yesBtn.onClick.AddListener(ExitGame);
        noBtn.onClick.AddListener(NoButton);
    }

    private void ExitGame()
    {
        if (GameDataManager.CurrentGameData.levelNum <= 5)
        {
            UIManager.Instance.ShowUI<AlertUI>();
            if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            {
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("COMPLETE THE TUTORIAL FIRST!");
            }
            else if (YLocalization.lanaguage == YLocalization.Lanaguage.English)
            { 
                UIManager.Instance.GetUI<AlertUI>().SetAlertText("先完成教程关卡!");
            }
            return;
        }
        //减少生命
        GameManager.Instance.ExitGame();
        GameDataManager.AddHeartCount(-1);
        UIManager.Instance.HideUI<ExitGameUI>();
    }

    private void NoButton()
    {
        UIManager.Instance.HideUI<ExitGameUI>();
    }

}

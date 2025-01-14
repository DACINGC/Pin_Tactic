using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartEvnetStreakUI : BaseUI
{
    private Button startButton;
    protected override void Awake()
    {
        base.Awake();
        startButton = tableTransform.Find("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(StartSteak);
    }

    private void StartSteak()
    {
        HomeSceneUI.Instance.homeUI.ShowWinStreakUIFirst();
        UIManager.Instance.HideUI<StartEvnetStreakUI>();
    }
}

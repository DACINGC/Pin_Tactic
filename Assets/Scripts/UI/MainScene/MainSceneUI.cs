using UnityEngine;

public class MainSceneUI : MonoBehaviour
{
    // ��̬ʵ��
    public static MainSceneUI Instance { get; private set; }

    private GamePlayUI gamePlayUI;
    public EmptyHoleManager emptyHoleManager;

    public GamePlayUI _GamePlayUI
    {
        get => gamePlayUI;
    }

    private void Awake()
    {
        // ����Ƿ�����ʵ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ���õ���ʵ��
        Instance = this;

        gamePlayUI = transform.Find("Canvas Game/PopUpGamePlay").GetComponent<GamePlayUI>();
    }

    public void SetLevelNum(int val)
    {
        gamePlayUI.SetLevelNum(val);
    }

    public void SetLevelTip(bool val, string tip = "")
    {
        gamePlayUI.SetTips(val, tip);
    }

    public void SetMainScene(bool val)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Canvas>())
                transform.GetChild(i).gameObject.SetActive(val);
        }
    }


}

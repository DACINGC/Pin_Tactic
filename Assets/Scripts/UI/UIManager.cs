using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private List<BaseUI> uiInstances = new List<BaseUI>();
    [SerializeField] private Transform gamePopUpTrans;

    private BaseUI currentUI; // ��ǰ��ʾ�� UI
    private BaseUI previousUI; // ��һ����ʾ�� UI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �������м̳��� BaseUI ��ʵ�����洢���б���
        BaseUI[] foundUIs = FindObjectsOfType<BaseUI>();
        uiInstances.AddRange(foundUIs);

        for (int i = 0; i < gamePopUpTrans.childCount; i++)
        {
            if (gamePopUpTrans.GetChild(i).GetComponent<BaseUI>())
                uiInstances.Add(gamePopUpTrans.GetChild(i).GetComponent<BaseUI>());
        }
    }

    /// <summary>
    /// ��ʾָ�����͵�UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ShowUI<T>() where T : BaseUI
    {
        foreach (var ui in uiInstances)
        {
            if (ui is T)
            {
                // �����ǰ UI ���ڣ����� previousUI
                if (currentUI != null && currentUI != ui)
                {
                    previousUI = currentUI;
                }

                // ��ʾĿ�� UI
                currentUI = ui;
                currentUI.gameObject.SetActive(true);
                currentUI.ShowUI();
                break;
            }
        }
    }

    /// <summary>
    /// ����ָ�����͵�UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delayTime"></param>
    /// <param name="callback"></param>
    public void HideUI<T>(float delayTime = 0, System.Action callback = null) where T : BaseUI
    {
        foreach (var ui in uiInstances)
        {
            if (ui is T)
            {
                ui.HideUI(delayTime, callback);
                break;
            }
        }
    }

    /// <summary>
    /// ��ȡָ�����͵�UIʵ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUI<T>() where T : BaseUI
    {
        foreach (var ui in uiInstances)
        {
            if (ui is T)
            {
                return ui as T;
            }
        }
        return null;
    }

    /// <summary>
    /// �л�����һ�� UI
    /// </summary>
    public void SwitchToPreviousUI()
    {
        if (previousUI != null)
        {
            // ���ص�ǰ UI
            if (currentUI != null)
            {
                currentUI.HideUI();
            }

            // �л�����һ�� UI
            var temp = currentUI;
            currentUI = previousUI;
            previousUI = temp;

            // ��ʾ�µ� currentUI
            currentUI.gameObject.SetActive(true);
            currentUI.ShowUI();
        }
        else
        {
            Debug.LogWarning("û����һ�� UI �����л���");
        }
    }
}

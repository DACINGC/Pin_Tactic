using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class YLocalizationTool : EditorWindow
{
    // �洢�ҵ��Ĺ����� YLocalization �ű�������
    private List<GameObject> yLocalizationObjects = new List<GameObject>();

    // ��Ӳ˵���
    [MenuItem("MyTools/���ػ�����/���� YLocalization ����")]
    public static void ShowWindow()
    {
        // ��ʾ����
        GetWindow<YLocalizationTool>("���� YLocalization ����");
    }

    // ���ڻ���
    private void OnGUI()
    {
        // ��Ӱ�ť
        if (GUILayout.Button("���ҳ����е� YLocalization ����"))
        {
            FindAllYLocalizationObjects();
            ShowResultsWindow();
        }
    }

    // ���ҳ��������й����� YLocalization �ű������壨����δ����ģ�
    private void FindAllYLocalizationObjects()
    {
        // ���֮ǰ�ļ�¼
        yLocalizationObjects.Clear();

        // ���ҳ��������е� GameObject������δ����ģ�
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // ֻ�������еĶ����ų�Ԥ�Ƽ�����Դ��
            if (obj.scene.IsValid())
            {
                // ���ҹ����� YLocalization �ű�������
                YLocalization yLocalization = obj.GetComponent<YLocalization>();
                if (yLocalization != null)
                {
                    yLocalizationObjects.Add(obj);
                }
            }
        }

        // ��ӡ��־
        Debug.Log($"�ҵ� {yLocalizationObjects.Count} �������� YLocalization �ű�������");
    }

    // ��ʾ�������
    private void ShowResultsWindow()
    {
        // �����������
        YLocalizationResultsWindow resultsWindow = GetWindow<YLocalizationResultsWindow>("YLocalization ���ҽ��");
        resultsWindow.SetYLocalizationObjects(yLocalizationObjects);
    }
}

// �������
public class YLocalizationResultsWindow : EditorWindow
{
    private List<GameObject> yLocalizationObjects;
    private Vector2 scrollPosition; // ������λ��

    // ���� YLocalization ����
    public void SetYLocalizationObjects(List<GameObject> objects)
    {
        yLocalizationObjects = objects;
    }

    // ���ڻ���
    private void OnGUI()
    {
        if (yLocalizationObjects == null || yLocalizationObjects.Count == 0)
        {
            GUILayout.Label("δ�ҵ��κι����� YLocalization �ű������塣");
            return;
        }

        // ��ʾ�ҵ�����������
        GUILayout.Label($"�ҵ��� YLocalization ��������: {yLocalizationObjects.Count}");

        // ��ʼ������ͼ
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));

        // ��������ʾ�ҵ�������
        foreach (var obj in yLocalizationObjects)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(obj.name, GUILayout.Width(300));
            if (GUILayout.Button("ѡ��", GUILayout.Width(60)))
            {
                // ѡ�ж�Ӧ�� GameObject
                Selection.activeObject = obj;
            }
            GUILayout.EndHorizontal();
        }

        // ����������ͼ
        GUILayout.EndScrollView();
    }
}
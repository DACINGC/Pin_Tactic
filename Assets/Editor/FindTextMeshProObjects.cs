using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

public class FindTextMeshProObjects : EditorWindow
{
    private List<GameObject> textMeshProObjects = new List<GameObject>();
    private Vector2 scrollPosition; // ���ڻ�������λ��

    [MenuItem("Tools/Find TextMeshPro Objects")]
    public static void ShowWindow()
    {
        GetWindow<FindTextMeshProObjects>("Find TextMeshPro Objects");
    }

    private void OnGUI()
    {
        // ���Ұ�ť
        if (GUILayout.Button("Find TextMeshPro Objects"))
        {
            FindObjects();
        }

        // ��ʾ���ҽ��
        if (textMeshProObjects.Count > 0)
        {
            GUILayout.Label("Found TextMeshPro Objects:");

            // ����ʣ��߶�
            float remainingHeight = position.height - GUILayoutUtility.GetLastRect().height - 10; // ��ȥ��ť�ͱ�ǩ�ĸ߶ȣ�����һЩ�߾�

            // ��ʼ������ͼ������ˮƽ��������
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(remainingHeight));

            foreach (var obj in textMeshProObjects)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"{obj.name} - {GetHierarchyPath(obj)}");
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUILayout.EndHorizontal();
            }

            // ����������ͼ
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No TextMeshPro Objects found.");
        }
    }

    private void FindObjects()
    {
        textMeshProObjects.Clear();

        // �������� GameObject������δ����ģ�
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            // ����Ƿ���� TextMeshProUGUI �� TextMeshPro ���
            if (obj.GetComponent<TextMeshProUGUI>() != null || obj.GetComponent<TextMeshPro>() != null)
            {
                textMeshProObjects.Add(obj);
            }
        }
    }

    private string GetHierarchyPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
}
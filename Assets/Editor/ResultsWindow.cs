using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ResultsWindow : EditorWindow
{
    // �洢���ص��ı������
    private List<string> loadedTexts;
    private List<Component> loadedComponents;

    // ������λ��
    private Vector2 scrollPosition;

    // ���ü��ص��ı�
    public void SetLoadedTexts(List<string> texts)
    {
        loadedTexts = texts;
        loadedComponents = null; // �������б�
    }

    // ���ü��ص����
    public void SetTextComponents(List<Component> components)
    {
        loadedComponents = components;
        loadedTexts = null; // ����ı��б�
    }

    // ������н��
    public void ClearResults()
    {
        loadedTexts = null;
        loadedComponents = null;
    }

    // ���ڻ���
    private void OnGUI()
    {
        if ((loadedTexts == null || loadedTexts.Count == 0) &&
            (loadedComponents == null || loadedComponents.Count == 0))
        {
            GUILayout.Label("δ�����κ����ݡ�");
            return;
        }

        // ��ʾ���ص���������
        if (loadedTexts != null)
        {
            GUILayout.Label($"�Ѽ��� {loadedTexts.Count} ���ı���");
        }
        else if (loadedComponents != null)
        {
            GUILayout.Label($"���ҵ� {loadedComponents.Count} ���ı������");
        }

        // ��ʼ������ͼ
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));

        // ��ʾ�ı�����
        if (loadedTexts != null)
        {
            foreach (var text in loadedTexts)
            {
                GUILayout.Label(text);
            }
        }
        // ��ʾ�������
        else if (loadedComponents != null)
        {
            foreach (var component in loadedComponents)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(component.gameObject.name + " - " + component.GetType().Name, GUILayout.Width(300));
                if (GUILayout.Button("ѡ��", GUILayout.Width(60)))
                {
                    // ѡ�ж�Ӧ�� GameObject
                    Selection.activeObject = component.gameObject;
                }
                GUILayout.EndHorizontal();
            }
        }

        // ����������ͼ
        GUILayout.EndScrollView();
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LanguageLoaderTool : EditorWindow
{
    //// �����ļ���·��
    //private string dataFolderPath;

    //// �洢���ص��ı�
    //private List<string> loadedTexts = new List<string>();

    //// ��Ӳ˵���
    //[MenuItem("MyTools/���ػ�����/���� Language.txt")]
    //public static void ShowWindow()
    //{
    //    // ��ʾ������
    //    GetWindow<LanguageLoaderTool>("���� Language.txt");
    //}

    //// ��ʼ��·��
    //private void OnEnable()
    //{
    //    dataFolderPath = Path.Combine(Application.persistentDataPath, "Data");
    //}

    //// �����ڻ���
    //private void OnGUI()
    //{
    //    // ��Ӽ��ذ�ť
    //    if (GUILayout.Button("���� Language.txt"))
    //    {
    //        LoadLanguageFile();
    //        ShowResultsWindow();
    //    }
    //}

    //// ���� Language.txt �ļ�
    //private void LoadLanguageFile()
    //{
    //    // ���֮ǰ�ļ�¼
    //    loadedTexts.Clear();

    //    // �ļ�·��
    //    string filePath = Path.Combine(dataFolderPath, "Language.txt");

    //    // ����ļ��Ƿ����
    //    if (!File.Exists(filePath))
    //    {
    //        Debug.LogError($"�ļ������ڣ�{filePath}");
    //        return;
    //    }

    //    // ��ȡ�ļ�����
    //    string[] lines = File.ReadAllLines(filePath);

    //    // ����ÿһ��
    //    foreach (var line in lines)
    //    {
    //        if (!string.IsNullOrEmpty(line))
    //        {
    //            loadedTexts.Add(line);
    //        }
    //    }

    //    // ��ӡ��־
    //    Debug.Log($"�Ѽ��� {loadedTexts.Count} ���ı���");
    //}

    //// ��ʾ�������
    //private void ShowResultsWindow()
    //{
    //    // �����������
    //    ResultsWindow resultsWindow = GetWindow<ResultsWindow>("���ؽ��");
    //    resultsWindow.SetLoadedTexts(loadedTexts);
    //}
}
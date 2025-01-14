using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LocalizationTool : EditorWindow
{
    // �洢�ҵ����ı����
    private List<Component> textComponents = new List<Component>();

    // �洢���ص��ı�
    private List<string> loadedTexts = new List<string>();

    // �����ļ���·��
    private string dataFolderPath;

    // ��Ӳ˵���
    [MenuItem("MyTools/���ػ�����/���ػ�")]
    public static void ShowWindow()
    {
        Debug.Log("���ػ����ߴ����Ѵ�"); // ������־

        // ��ȡ�򴴽�����
        var window = GetWindow<LocalizationTool>("���ػ�����");

        // ���ô��ڴ�С��λ��
        window.minSize = new Vector2(400, 600); // ��С��С
        window.position = new Rect(100, 100, 400, 300); // ��ʼλ�úʹ�С

        // ��ʾ����
        window.Show();
    }

    // ��ʼ��·��
    private void OnEnable()
    {
        dataFolderPath = Path.Combine(Application.persistentDataPath, "Data");
    }

    // ���ڻ���
    private void OnGUI()
    {
        // ���ҳ����е��ı�
        if (GUILayout.Button("���ҳ����е��ı�"))
        {
            FindAllTextInScene();
            ShowResultsWindow();
        }

        // ���� Language.txt �ļ�
        if (GUILayout.Button("���� Language.txt"))
        {
            LoadLanguageFile();
            ShowResultsWindow();
        }

        // �����ı��� Language.txt
        if (GUILayout.Button("�����ı��� Language.txt"))
        {
            ExportTextToTxt();
            ShowResultsWindow();
        }

        // �����ص��ı���ֵ�� YLocalization ���
        if (GUILayout.Button("��ֵ�� YLocalization ���"))
        {
            AssignTextToYLocalization();
        }
    }

    // ���ҳ��������е��ı����������δ����ģ�
    private void FindAllTextInScene()
    {
        // ���֮ǰ�ļ�¼
        textComponents.Clear();

        // ���ҳ��������е� GameObject������δ����ģ�
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // ֻ�������еĶ����ų�Ԥ�Ƽ�����Դ��
            if (obj.scene.IsValid())
            {
                // ���� Text ���
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    textComponents.Add(text);
                }

                // ���� TextMeshPro ���
                TextMeshPro tmp = obj.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    textComponents.Add(tmp);
                }

                // ���� TextMeshProUGUI ���
                TextMeshProUGUI tmpUI = obj.GetComponent<TextMeshProUGUI>();
                if (tmpUI != null)
                {
                    textComponents.Add(tmpUI);
                }
            }
        }

        // ��ӡ��־
        Debug.Log($"�ҵ� {textComponents.Count} ���ı����");
    }

    // ���� Language.txt �ļ�
    private void LoadLanguageFile()
    {
        // ���֮ǰ�ļ�¼
        loadedTexts.Clear();

        // �ļ�·��
        string filePath = Path.Combine(dataFolderPath, "Language.txt");

        // ����ļ��Ƿ����
        if (!File.Exists(filePath))
        {
            Debug.LogError($"�ļ������ڣ�{filePath}");
            return;
        }

        // ��ȡ�ļ�����
        string[] lines = File.ReadAllLines(filePath);

        // ����ÿһ��
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                loadedTexts.Add(line);
            }
        }

        // ��ӡ��־
        Debug.Log($"�Ѽ��� {loadedTexts.Count} ���ı���");
    }

    // �����ı��� Language.txt
    private void ExportTextToTxt()
    {
        if (textComponents == null || textComponents.Count == 0)
        {
            Debug.LogWarning("û���ҵ��κ��ı���������ȵ�����Ұ�ť��");
            return;
        }

        // ���������ļ��У���������ڣ�
        if (!Directory.Exists(dataFolderPath))
        {
            Directory.CreateDirectory(dataFolderPath);
        }

        // �����ı�����
        StringBuilder txtContent = new StringBuilder();
        for (int i = 0; i < textComponents.Count; i++)
        {
            string textContent = GetTextContent(textComponents[i]);
            txtContent.AppendLine($"ID_{i + 1}: {textContent}#{textContent}");
        }

        // д���ļ�
        string filePath = Path.Combine(dataFolderPath, "Language.txt");
        File.WriteAllText(filePath, txtContent.ToString());

        // ��ӡ��־
        Debug.Log($"�ı��ѵ�����: {filePath}");

        // ������������ص� ResultsWindow
        loadedTexts.Clear();
        loadedTexts.AddRange(File.ReadAllLines(filePath));
    }

    // ��ʾ�������
    private void ShowResultsWindow()
    {
        // �����������
        ResultsWindow resultsWindow = GetWindow<ResultsWindow>("���ҽ��");

        // ���֮ǰ�Ľ��
        resultsWindow.ClearResults();

        // �����µĽ��
        if (loadedTexts.Count > 0)
        {
            resultsWindow.SetLoadedTexts(loadedTexts);
        }
        else if (textComponents.Count > 0)
        {
            resultsWindow.SetTextComponents(textComponents);
        }
    }

    // �����ص��ı���ֵ�� YLocalization ���
    private void AssignTextToYLocalization()
    {
        if (loadedTexts == null || loadedTexts.Count == 0)
        {
            Debug.LogWarning("û�м����κ��ı������ȵ�����ذ�ť��");
            return;
        }

        // �������ص��ı�
        for (int i = 0; i < loadedTexts.Count; i++)
        {
            // �����ı�����
            string[] parts = loadedTexts[i].Split(new[] { ':', '#' }, 3);
            if (parts.Length == 3)
            {
                string id = parts[0].Trim(); // ID
                string englishStr = parts[1].Trim(); // Ӣ���ַ�
                string chineseStr = parts[2].Trim(); // �����ַ�

                // ��ȡ��Ӧ���ı����
                if (i < textComponents.Count)
                {
                    var component = textComponents[i];

                    // ��ȡ YLocalization ���
                    YLocalization yLocalization = component.GetComponent<YLocalization>();
                    if (yLocalization != null)
                    {
                        // ��ֵ�� YLocalization ���
                        yLocalization.englishStr = englishStr;
                        yLocalization.chineseStr = chineseStr;
                        Debug.Log($"�Ѹ�ֵ�� {id}: {englishStr} -> {chineseStr}");
                    }
                    else
                    {
                        Debug.LogWarning($"δ�ҵ� {id} ��Ӧ�� YLocalization �����");
                    }
                }
            }
        }

        // ��ӡ��־
        Debug.Log("�ı��Ѹ�ֵ������ YLocalization �����");
    }

    // ��ȡ�ı�����
    private string GetTextContent(Component component)
    {
        if (component is Text text)
        {
            return text.text;
        }
        else if (component is TextMeshPro tmp)
        {
            return tmp.text;
        }
        else if (component is TextMeshProUGUI tmpUI)
        {
            return tmpUI.text;
        }
        return string.Empty;
    }
}
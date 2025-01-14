using UnityEditor;
using UnityEngine;

public class SetScrewColor : EditorWindow
{
    private string folderPath = "";

    [MenuItem("MyTools/Set Screw Attributes")]
    public static void ShowWindow()
    {
        GetWindow<SetScrewColor>("Set Screw Attributes");
    }

    private void OnGUI()
    {
        GUILayout.Label("���� Screw �ű�������ֵ", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("�ļ���·��:", GUILayout.Width(70));
        folderPath = GUILayout.TextField(folderPath, GUILayout.Width(300));
        if (GUILayout.Button("ѡ��·��", GUILayout.Width(100)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("ѡ���ļ���", "", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                folderPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("��ʼ����", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                EditorUtility.DisplayDialog("����", "����ѡ��һ����Ч��·����", "ȷ��");
            }
            else
            {
                SetAttributesInFolder(folderPath);
            }
        }
    }

    private void SetAttributesInFolder(string path)
    {
        string[] guids = AssetDatabase.FindAssets("", new[] { path });
        int modifiedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (obj != null)
            {
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);

                modifiedCount += SetScrewAttributes(prefabRoot);

                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        EditorUtility.DisplayDialog("���", $"������ {modifiedCount} �� Screw �ű������ԡ�", "ȷ��");
    }

    private int SetScrewAttributes(GameObject obj)
    {
        int count = 0;
        Transform[] allTransforms = obj.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in allTransforms)
        {
            Screw screw = t.GetComponent<Screw>();
            if (screw != null)
            {
                Transform imageChild = FindChildByName(t, "Image");
                if (imageChild != null)
                {
                    SpriteRenderer spriteRenderer = imageChild.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                    {
                        string spriteName = spriteRenderer.sprite.name.ToLower();

                        // ���� isStarHole
                        screw.SetIsStarHole(spriteName.Contains("star"));

                        // ���� color
                        screw.SetColor(DetermineScrewColor(spriteName));

                        EditorUtility.SetDirty(t.gameObject);
                        count++;
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return count;
    }

    private ScrewColor DetermineScrewColor(string spriteName)
    {
        if (spriteName.Contains("light_blue")) return ScrewColor.LightBlue;
        else if (spriteName.Contains("light_purple")) return ScrewColor.LightPurple;
        else if (spriteName.Contains("blue")) return ScrewColor.Blue;
        else if (spriteName.Contains("gray")) return ScrewColor.Grey;
        else if (spriteName.Contains("red")) return ScrewColor.Red;
        else if (spriteName.Contains("yellow")) return ScrewColor.Yellow;
        else if (spriteName.Contains("purple")) return ScrewColor.Purple;
        else if (spriteName.Contains("pink")) return ScrewColor.Pink;
        else if (spriteName.Contains("orange")) return ScrewColor.Orange;
        else if (spriteName.Contains("green")) return ScrewColor.Green;

        return ScrewColor.Grey; // Ĭ��ֵ
    }

    private Transform FindChildByName(Transform parent, string nameContains)
    {
        foreach (Transform child in parent)
        {
            if (child.name.ToLower().Contains(nameContains.ToLower()))
            {
                return child;
            }
        }
        return null;
    }
}

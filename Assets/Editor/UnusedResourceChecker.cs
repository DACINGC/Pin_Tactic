using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnusedResourceChecker : EditorWindow
{
    private string targetFolderPath = "Assets/"; // Ĭ��·��
    private Vector2 scrollPosition;
    private List<string> unusedResources = new List<string>();

    [MenuItem("Tools/Check Unused Resources")]
    public static void ShowWindow()
    {
        GetWindow<UnusedResourceChecker>("Unused Resource Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Check Unused Resources", EditorStyles.boldLabel);

        // ѡ���ļ���·��
        GUILayout.Label("Target Folder Path:");
        targetFolderPath = GUILayout.TextField(targetFolderPath);
        if (GUILayout.Button("Select Folder"))
        {
            targetFolderPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
        }

        // ��ʼ���
        if (GUILayout.Button("Check Unused Resources"))
        {
            CheckUnusedResources();
        }

        // ��ʾδ���õ���Դ�б�
        if (unusedResources.Count > 0)
        {
            GUILayout.Label("Unused Resources:", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            foreach (var resource in unusedResources)
            {
                GUILayout.BeginHorizontal();

                // ��ʾ��Դ·��
                GUILayout.Label(resource, GUILayout.Width(400));

                // ��ӡ�ѡ�񡱰�ť
                if (GUILayout.Button("Select", GUILayout.Width(80)))
                {
                    SelectResource(resource);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("All resources are used in the scene or prefabs.");
        }
    }

    private void CheckUnusedResources()
    {
        unusedResources.Clear();

        // ��ȡĿ���ļ����µ�����ͼƬ��Դ
        string[] imagePaths = Directory.GetFiles(targetFolderPath, "*.png", SearchOption.AllDirectories);

        // ��������ͼƬ��Դ
        foreach (var imagePath in imagePaths)
        {
            string assetPath = "Assets" + imagePath.Substring(Application.dataPath.Length);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture == null)
            {
                Debug.LogWarning($"Failed to load texture at path: {assetPath}");
                continue;
            }

            // ��鳡����Ԥ�����Ƿ����ø���Դ
            if (!IsTextureUsedInScene(texture) && !IsTextureUsedInPrefabs(texture))
            {
                unusedResources.Add(assetPath);
            }
        }
    }

    private bool IsTextureUsedInScene(Texture2D texture)
    {
        // ��ȡ������������Ϸ���󣨰���δ����ģ�
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            // ���Renderer���
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.mainTexture == texture)
                    {
                        return true;
                    }
                }
            }

            // ���UI Image���
            Image image = obj.GetComponent<Image>();
            if (image != null && image.sprite != null && image.sprite.texture == texture)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTextureUsedInPrefabs(Texture2D texture)
    {
        // ��ȡAssets�ļ���������Ԥ����
        string[] prefabPaths = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);

        foreach (var prefabPath in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Failed to load prefab at path: {prefabPath}");
                continue;
            }

            // ���Ԥ�����Ƿ����ø���Դ
            if (IsTextureUsedInGameObject(prefab, texture))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTextureUsedInGameObject(GameObject gameObject, Texture2D texture)
    {
        // ���Renderer���
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                if (material != null && material.mainTexture == texture)
                {
                    return true;
                }
            }
        }

        // ���UI Image���
        Image image = gameObject.GetComponent<Image>();
        if (image != null && image.sprite != null && image.sprite.texture == texture)
        {
            return true;
        }

        // �ݹ���������
        foreach (Transform child in gameObject.transform)
        {
            if (IsTextureUsedInGameObject(child.gameObject, texture))
            {
                return true;
            }
        }

        return false;
    }

    private void SelectResource(string resourcePath)
    {
        // ��λ����Դ
        Object resource = AssetDatabase.LoadAssetAtPath<Object>(resourcePath);
        if (resource != null)
        {
            EditorGUIUtility.PingObject(resource);
            Selection.activeObject = resource;
        }
        else
        {
            Debug.LogWarning($"Failed to locate resource at path: {resourcePath}");
        }
    }
}
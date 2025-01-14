using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class UnusedMaterialFinder : EditorWindow
{
    private string searchPath = "Assets"; // Ĭ������·��
    private Vector2 scrollPosition; // ���ڹ�����
    private List<Material> unusedMaterials = new List<Material>(); // �洢δ���õĲ���

    [MenuItem("Tools/Find Unused Materials")]
    public static void ShowWindow()
    {
        GetWindow<UnusedMaterialFinder>("Unused Material Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find Unused Materials", EditorStyles.boldLabel);

        // ��������·��
        searchPath = EditorGUILayout.TextField("Search Path", searchPath);

        // ���Ұ�ť
        if (GUILayout.Button("Find Unused Materials"))
        {
            FindUnusedMaterials();
        }

        // ��ʾδ���õĲ���
        GUILayout.Label("Unused Materials:", EditorStyles.boldLabel);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        foreach (var material in unusedMaterials)
        {
            GUILayout.BeginHorizontal();

            // ��ʾ��������
            GUILayout.Label(material.name, GUILayout.Width(200));

            // ��λ��ť
            if (GUILayout.Button("Select", GUILayout.Width(100)))
            {
                Selection.activeObject = material;
                EditorGUIUtility.PingObject(material);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void FindUnusedMaterials()
    {
        unusedMaterials.Clear();

        // ��ȡָ��·���µ����в���
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { searchPath });
        List<Material> materials = new List<Material>();

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            materials.Add(material);
        }

        // ��ȡ������Դ
        string[] allAssetGuids = AssetDatabase.FindAssets("t:Object");
        HashSet<Material> referencedMaterials = new HashSet<Material>();

        foreach (string guid in allAssetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencies = AssetDatabase.GetDependencies(path, false);

            foreach (string dependencyPath in dependencies)
            {
                // ����������Դ
                Object dependency = AssetDatabase.LoadAssetAtPath<Object>(dependencyPath);
                if (dependency is Material)
                {
                    referencedMaterials.Add(dependency as Material);
                }
            }
        }

        // �ҳ�δ���õĲ���
        unusedMaterials = materials.Except(referencedMaterials).ToList();
    }
}
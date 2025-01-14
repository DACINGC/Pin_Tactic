using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class NameUtility
{
    // �������ֵķ���
    public static void SaveDataByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Name cannot be null or empty.");
            return;
        }

        // ������תΪСд
        string lowerCaseName = name.ToLower();

        // ��������еĹؼ��ֲ�ִ�в���
        if (lowerCaseName.Contains("icon"))
        {
            HandleIcon();
        }

        if (lowerCaseName.Contains("heart"))
        {
            HandleHeart();
        }

        if (lowerCaseName.Contains("hole"))
        {
            HandleHole();
        }

        if (lowerCaseName.Contains("rocket"))
        {
            HandleRocket();
        }

        if (lowerCaseName.Contains("doublebox"))
        {
            HandleDoubleBox();
        }
    }

    // ���ֹؼ��ֵĴ�����
    private static void HandleIcon()
    {
        
        // ������ʵ�־���Ĳ����߼�
    }

    private static void HandleHeart()
    {
        Console.WriteLine("Handling 'heart' operation.");
        // ������ʵ�־���Ĳ����߼�
    }

    private static void HandleHole()
    {
        Console.WriteLine("Handling 'hole' operation.");
        // ������ʵ�־���Ĳ����߼�
    }

    private static void HandleRocket()
    {
        Console.WriteLine("Handling 'rocket' operation.");
        // ������ʵ�־���Ĳ����߼�
    }

    private static void HandleDoubleBox()
    {
        Console.WriteLine("Handling 'doubleBox' operation.");
        // ������ʵ�־���Ĳ����߼�
    }

    public static string GetStringByName(string name)
    {
        string curName = name.ToLower();
        if (curName.Contains("heart"))
        {
            return "m";
        }
        else if (curName.Contains("coin"))
        {
            return "";
        }
        else
            return "x";

    }

    public static void SortGameObjectsByName(List<GameObject> gameObjectList, bool sortByNumber = false)
    {
        if (sortByNumber)
        {
            gameObjectList.Sort((a, b) =>
            {
                int numberA = ExtractNumberFromName(a.name);
                int numberB = ExtractNumberFromName(b.name);
                return numberA.CompareTo(numberB);
            });
        }
        else
        {
            gameObjectList.Sort((a, b) => string.Compare(a.name, b.name));
        }
    }

    private static int ExtractNumberFromName(string name)
    {
        string numberPart = new string(name.Where(char.IsDigit).ToArray());
        if (int.TryParse(numberPart, out int number))
        {
            return number;
        }
        return 0; // Ĭ��ֵ
    }

    /// <summary>
    /// ��λ��ȥ0
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static int SetLastDigitToZero(int number)
    {
        // ȥ����λ����Ȼ����� 10
        return (number / 10) * 10;
    }
}

public static class TransFormUtility
{
    /// <summary>
    /// ����Чλ�����õ���Ӧ UI Ԫ���ϣ�Canvas Ϊ Screen Space - Overlay��
    /// </summary>
    /// <param name="effectTransform">��Ч�� Transform</param>
    /// <param name="uiElement">Ŀ�� UI Ԫ�ص� RectTransform</param>
    public static void SetEffectPosition(Transform effectTransform, RectTransform uiElement)
    {
        if (effectTransform == null || uiElement == null)
        {
            Debug.LogError("��������Ϊ�գ�effectTransform �� uiElement");
            return;
        }

        // �� RectTransform ����Ļ����ת��Ϊ��������
        Vector3 worldPosition = uiElement.position;

        // ������Ч����������
        effectTransform.position = worldPosition;
    }

    /// <summary>
    /// ��һ����Ϸ�����ƶ���UI�����λ�� (������ Canvas ��ȾģʽΪ Screen Space - Overlay)��
    /// </summary>
    /// <param name="gameObject">��Ҫ�ƶ�����Ϸ���� Transform��</param>
    /// <param name="uiObject">Ŀ��UI���� RectTransform��</param>
    /// <param name="zOffset">��Ϸ�����Z��ƫ������</param>
    public static void MoveGameObjectToUIPosition(Transform gameObject, RectTransform uiObject, float zOffset = 0f)
    {
        if (gameObject == null || uiObject == null)
        {
            Debug.LogError("GameObject or UIObject is null!");
            return;
        }

        // ��ȡUI��������Ļ�ϵ�����
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(null, uiObject.position);

        // ����Ļ����ת��Ϊ��������
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane + zOffset));

        // ������Ϸ�����λ��
        gameObject.position = worldPosition;
    }

    /// <summary>
    /// ɾ��ָ�������µ����������塣
    /// </summary>
    /// <param name="parent">������� Transform��</param>
    public static void DestroyAllChildren(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("������Ϊ�գ��޷�ɾ�������塣");
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}


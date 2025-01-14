using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// ���ļ����ݴ洢������
/// </summary>
public static class JsonDataManager
{
    // ����Ĭ�ϴ洢·����ʹ�� Application.persistentDataPath���ʺϿ�ƽ̨��
    private static readonly string DataFolderPath = Path.Combine(Application.persistentDataPath, "Data");
    private static readonly string GameDataFilePath = Path.Combine(DataFolderPath, "gamedata.json");
    public static readonly string TimeDataFilePath = Path.Combine(DataFolderPath, "timedata.json");

    /// <summary>
    /// �������ݵ�ָ��·��
    /// </summary>
    /// <typeparam name="T">�������������</typeparam>
    /// <param name="data">���ݶ���</param>
    /// <param name="filePath">�ļ�·��</param>
    public static void SaveData<T>(T data, string filePath)
    {
        EnsureDataFolderExists();

        try
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
            //Debug.Log($"[JsonDataManager] �����ѳɹ����浽: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonDataManager] ��������ʧ��: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// ��ָ��·����������
    /// </summary>
    /// <typeparam name="T">���ص���������</typeparam>
    /// <param name="filePath">�ļ�·��</param>
    /// <returns>���ص����ݶ����ļ������ڻ����ʧ��ʱ���� null</returns>
    public static T LoadData<T>(string filePath) where T : class
    {
        EnsureDataFolderExists();

        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                T data = JsonUtility.FromJson<T>(jsonData);
                Debug.Log($"[JsonDataManager] �����ѳɹ�����: {filePath}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonDataManager] ��������ʧ��: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"[JsonDataManager] �����ļ�δ�ҵ�: {filePath}");
            return null;
        }
    }

    /// <summary>
    /// ȷ�����ݴ洢�ļ��д���
    /// </summary>
    private static void EnsureDataFolderExists()
    {
        if (!Directory.Exists(DataFolderPath))
        {
            Directory.CreateDirectory(DataFolderPath);
            Debug.Log($"[JsonDataManager] �Ѵ��������ļ���: {DataFolderPath}");
        }
    }

    #region ��Ϸ����
    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public static void SaveGameData(GameData data)
    {
        SaveData(data, GameDataFilePath);
    }

    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public static GameData LoadGameData()
    {
        return LoadData<GameData>(GameDataFilePath);
    }

    #endregion

    #region ʱ������
    /// <summary>
    /// ����ʱ������
    /// </summary>
    public static void SaveTimeData(TimeData data)
    {
        SaveData(data, TimeDataFilePath);
    }

    /// <summary>
    /// ����ʱ������
    /// </summary>
    public static TimeData LoadTimeData()
    {
        return LoadData<TimeData>(TimeDataFilePath);
    }

    #endregion
}

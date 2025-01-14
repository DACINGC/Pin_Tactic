using System;
using System.Collections.Generic;
using UnityEngine;

public static class TimeDataManager
{
    // ��ǰ��ʱ������
    public static TimeData CurrentTimeData { get; private set; }

    // ��ʼ��
    public static void Initialize()
    {
        // ���Լ���ʱ������
        LoadTimeData();

        // �������ʧ�ܣ���ʼ������
        if (CurrentTimeData == null)
        {
            ResetTimeData();
            SaveTimeData();
        }
    }

    // ����ʱ������
    public static void SaveTimeData()
    {
        JsonDataManager.SaveTimeData(CurrentTimeData);
    }

    // ����ʱ������
    public static void LoadTimeData()
    {
        CurrentTimeData = JsonDataManager.LoadTimeData(); ;
    }

    // ����ʱ������
    public static void ResetTimeData()
    {
        CurrentTimeData = new TimeData
        {
            timedObjects = new List<TimedObjectData>(),
            lastLoginTime = new SerializableTime(DateTime.Now)
        };
    }

    #region ʱ�����
    public static void AddTimeDataList(TimedObjectData obj)
    {
        CurrentTimeData.timedObjects.Add(obj);
        //Save();
    }

    #endregion
}

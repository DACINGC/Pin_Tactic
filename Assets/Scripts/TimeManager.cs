using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ActivityState
{
    Ongoing, Ended, Stop
}

public enum TimeEventType
{ 
    Streak,
    LuckySpin,
    Heart,
    WireLessHeart,//��������
    DailyReward
}

public class TimeManager : MonoBehaviour
{
    private Dictionary<string, TimedObject> timedObjects = new Dictionary<string, TimedObject>();  // �洢�������ĵ���ʱ������
    public bool IsWireLessHeart { get; set; }
    public TimedObject GetTimeObj(TimeEventType type)
    {
        foreach (TimedObject obj in timedObjects.Values)
        {
            if (type == obj.eventType)
            {
                return obj;
            }
        }

        return null;
    }

    private bool isLoaded = false;

    private static TimeManager _instance;

    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // �����ڳ������ҵ�һ�����е�ʵ��
                _instance = FindObjectOfType<TimeManager>();

                // ���������û��ʵ�����򴴽�һ���µ�GameObject�����TimeManager���
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("TimeManager");
                    _instance = singletonObject.AddComponent<TimeManager>();

                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // ��ֹ�ظ�ʵ����
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // ����Ѿ�����һ��ʵ���������ظ���
        }
        //AddDefaultTime();
        if (isLoaded == false)
        {
            LoadTimedObjects();
        }

        StartCoroutine(nameof(SaveDataInterval));
    }

    private void Update()
    {
        foreach (TimedObject timedObject in timedObjects.Values)
        {
            if(timedObject.currentState != ActivityState.Stop)
                timedObject.UpdateCountdown();  // ���µ���ʱ
        }


    }

    private void OnApplicationQuit()
    {
        Debug.Log("��Ϸ�˳�����ʱ������");
        SaveTimedObjects();
        GameDataManager.Save();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("Ӧ�ó����Ѿ�ֹͣ����������.");
            SaveTimedObjects();
            GameDataManager.Save();

            StopCoroutine(SaveDataInterval());//��ͣ����ʱ�����ݵ�Э��
        }
        else
        {
            Debug.Log("Ӧ�ý����Ѿ��ָ�.");
            //foreach (TimedObject timeobj in timedObjects.Values)
            //{
            //    timeobj.UpdateRemainingTime(TimeDataManager.CurrentTimeData.lastLoginTime);
            //}
            ReLoadTimeObjct();
            StartCoroutine(nameof(SaveDataInterval));//��ʼ����ʱ�����ݵ�Э��
        }
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("Ӧ�ý���ʧȥ���㣬��������");
            SaveTimedObjects();
            GameDataManager.Save();
        }
    }

    private void ReLoadTimeObjct()
    {
        foreach (var timedObjectData in TimeDataManager.CurrentTimeData.timedObjects)
        {
            if (timedObjects.ContainsKey(timedObjectData.objectID))
            {
                timedObjects[timedObjectData.objectID].leftTime = timedObjectData.leftTime;
                timedObjects[timedObjectData.objectID].UpdateRemainingTime(TimeDataManager.CurrentTimeData.lastLoginTime);
            }
        }
    }
    public void LoadTimedObjects()
    {
        isLoaded = true;
        foreach (var timedObjectData in TimeDataManager.CurrentTimeData.timedObjects)
        {
            // ����һ���µ� TimedObject����ʹ�ôӱ��������м��صĲ�����ʼ��
            TimedObject timedObject = new TimedObject(
                timedObjectData.objectID,
                timedObjectData.startTime,
                timedObjectData.endTime,
                timedObjectData.leftTime,
                timedObjectData.activityStatus,
                timedObjectData.timeEventType
            );
            // ����ʼ����� timedObject ��ӵ� timedObjects �ֵ�
            timedObjects.Add(timedObjectData.objectID, timedObject);
            // ʹ�� lastLoginTime ����ʣ��ʱ��
            timedObject.UpdateRemainingTime(TimeDataManager.CurrentTimeData.lastLoginTime);
        }
        //Debug.Log(timedObjects.Count);
    }
    private void SaveTimedObjects()
    {
        List<TimedObjectData> timedObjectDataList = new List<TimedObjectData>();

        // �� timedObjects ת��Ϊ TimedObjectData �����浽�б�
        foreach (var timedObject in timedObjects.Values)
        {
            TimedObjectData timedObjectData = new TimedObjectData(
                timedObject.objectID,
                timedObject.startTime,
                timedObject.endTime,
                timedObject.currentState,
                new SerializableTime(timedObject.remainingTime), // ʹ�� leftTime ���д洢
                timedObject.eventType
            );
            timedObjectDataList.Add(timedObjectData);
        }
        if (timedObjectDataList.Count > 0)
        {
            // ���浽 CurrentGameData
            TimeDataManager.CurrentTimeData.timedObjects = timedObjectDataList;
        }

        // ���� lastLoginTime
        TimeDataManager.CurrentTimeData.lastLoginTime = new SerializableTime(DateTime.Now);

        // �������ݵ��ļ�
        TimeDataManager.SaveTimeData();
    }
    #region ���Ĭ������
    public void AddDefaultTime()
    {
        AddDefaultHeart();
        AddDefaultWirelessHeart();
        AddDefualtDailyReward();
        AddDefualtLuckySpinTime();
        AddDefualtStickerTime();
        TimeDataManager.SaveTimeData();
    }
    private void AddDefaultHeart()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(0, 15, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("Heart", time, time,
            ActivityState.Stop, time, TimeEventType.Heart));
    }
    private void AddDefaultWirelessHeart()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(0, 1, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("WirelessHeart", time, time,
            ActivityState.Stop, time, TimeEventType.WireLessHeart));
    }
    private void AddDefualtDailyReward()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(24, 0, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("DailyReward", time, time,
            ActivityState.Stop, time, TimeEventType.DailyReward));
    }
    private void AddDefualtStickerTime()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(7, 0, 0, 0));
        SerializableTime endtime = new SerializableTime(new TimeSpan(2, 0, 0, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("Streak", time, endtime,
            ActivityState.Stop, time, TimeEventType.Streak));
    }
    private void AddDefualtLuckySpinTime()
    {
        SerializableTime time = new SerializableTime(new TimeSpan(7, 0, 0, 0));
        SerializableTime endtime = new SerializableTime(new TimeSpan(2, 0, 0, 0));
        TimeDataManager.AddTimeDataList(new TimedObjectData("LuckySpin", time, endtime,
            ActivityState.Stop, time, TimeEventType.LuckySpin));
    }
    #endregion

    #region ���ʱ�䱣��ʱ������
    /// <summary>
    /// Э�̣�ÿ�� 2 ��ִ��һ�α������
    /// </summary>
    /// <returns></returns>
    private IEnumerator SaveDataInterval()
    {
        while (true)  // ����ѭ����ÿ�� 2 �뱣��һ��
        {
            // �������ݣ�����������Լ��ı������ݷ�����
            SaveTimedObjects();

            // �ȴ� 2 ��
            yield return new WaitForSeconds(2.5f);
        }
    }

    #endregion

    /// <summary>
    /// �ָ���������
    /// </summary>
    public void RecoverWirelessHeart(int h, int m, int s)
    {
        TimedObject wirelessTime = GetTimeObj(TimeEventType.WireLessHeart);
        wirelessTime.currentState = ActivityState.Ongoing;
        wirelessTime.remainingTime = new TimeSpan(h, m, s);
    }

    /// <summary>
    /// ��������������ʱ��
    /// </summary>
    /// <param name="h"></param>
    /// <param name="m"></param>
    /// <param name="s"></param>
    public void AddWirelessHeartTime(int h, int m, int s)
    {
        TimedObject wirelessTime = GetTimeObj(TimeEventType.WireLessHeart);

        //���Ϊֹͣ״̬������֮ǰ��ʣ��ʱ��
        if (wirelessTime.currentState == ActivityState.Stop)
            wirelessTime.remainingTime = TimeSpan.Zero;

        wirelessTime.currentState = ActivityState.Ongoing;
        wirelessTime.remainingTime += new TimeSpan(h, m, s);

        HomeSceneUI.Instance.homeUI.UpdateWirelessHeartIcon();
    }

    /// <summary>
    /// ��ʼ���Ƽ�ʱ
    /// </summary>
    public void StartStreakTime()
    {
        TimedObject streakTime = GetTimeObj(TimeEventType.Streak);
        streakTime.remainingTime = streakTime.startTime.ToTimeSpan();
        streakTime.currentState = ActivityState.Ongoing;
    }

    /// <summary>
    /// ��ʼ�齱��ʱ
    /// </summary>
    public void StartLuckySpinTime()
    {
        TimedObject streakTime = GetTimeObj(TimeEventType.LuckySpin);
        streakTime.remainingTime = streakTime.startTime.ToTimeSpan();
        streakTime.currentState = ActivityState.Ongoing;
    }

    /// <summary>
    /// ��ʼÿ�ռ�ʱ
    /// </summary>
    public void StartDailyTime()
    {
        TimedObject streakTime = GetTimeObj(TimeEventType.DailyReward);
        streakTime.remainingTime = streakTime.startTime.ToTimeSpan();
        streakTime.currentState = ActivityState.Ongoing;
    }
}


using System;
using UnityEngine;

public class TimedObject
{
    public string objectID; // Ψһ��ʶ��
    public TimeSpan remainingTime; // ʣ�൹��ʱ
    public ActivityState currentState; // ��ǰ�״̬
    public SerializableTime startTime; // ���ʼʱ�䣨���л�ʱ�䣩
    public SerializableTime endTime; // �������ĵ���ʱ�����л�ʱ�䣩
    public SerializableTime leftTime; // ʣ��ʱ�䣨���л�ʱ�䣩

    public TimeEventType eventType;

    // ���캯��
    public TimedObject(string id, SerializableTime startTime, SerializableTime endTime, SerializableTime leftTime, ActivityState state, TimeEventType type)
    {
        objectID = id;
        this.startTime = startTime;
        this.endTime = endTime;
        this.leftTime = leftTime;
        currentState = state;
        this.eventType = type;
    }

    // ��ʼ������ʱ��������Ϸ����ʱ�ָ�״̬

    // ���µ���ʱ
    public void UpdateCountdown()
    {
        if (remainingTime.TotalSeconds > 0)
        {
            remainingTime -= TimeSpan.FromSeconds(Time.deltaTime); // ÿ֡���ٵ���ʱ
        }
        else
        {
            if (currentState == ActivityState.Ongoing)
            {
                HandleCountdownEnd();
            }
            else if (currentState == ActivityState.Ended)
            {
                RestartActivity();
            }
            else if (currentState == ActivityState.Stop)
            {
                //return;
            }
        }
    }

    // �������Ĵ���
    private void HandleCountdownEnd()
    {
        //Debug.Log($"���� {objectID} ����������彫��ʧ");

        switch (eventType)
        {
            case TimeEventType.Streak:
                //�ر����ư�ť
                GameDataManager.SetStreakLockedVal(true);
                HomeSceneUI.Instance.homeUI.SetStreakButton(false);
                remainingTime = endTime.ToTimeSpan();
                GameDataManager.TimeRestartSteak();//���¿�ʼ�(���֮ǰ�Ļ����)
                currentState = ActivityState.Ended; // ״̬��Ϊ����
                break;

            case TimeEventType.LuckySpin:
                //�رճ齱��ť
                GameDataManager.SetLuckySpinLockedVal(false);
                HomeSceneUI.Instance.homeUI.SetLuckSpinButton(false);
                
                remainingTime = endTime.ToTimeSpan();
                currentState = ActivityState.Ended; // ״̬��Ϊ����
                break;

            case TimeEventType.Heart:
                if (GameDataManager.CurrentGameData.heartCount < 5)
                {
                    // �������С��5����Ҫһֱ����
                    // Heartһֱ���ڿ�ʼ״̬
                    Debug.Log("heart���¿�ʼ");
                    EventManager.Instance.TriggerEvent(GameEvent.AddHearEvent);
                    remainingTime = startTime.ToTimeSpan();
                    currentState = ActivityState.Ongoing; // ״̬��Ϊ���¿�ʼ
                }
                else
                {
                    Debug.Log("heart����");
                    remainingTime = startTime.ToTimeSpan();
                    currentState = ActivityState.Stop; // ״̬��Ϊֹͣ
                }
                break;

            case TimeEventType.WireLessHeart:
                // ���������ʱ�䣬ֹͣ��ʱ
                Debug.Log("��������ֹͣ");
                currentState = ActivityState.Stop;
                remainingTime = TimeSpan.Zero;
                HomeSceneUI.Instance.homeUI.UpdateWirelessHeartIcon();
                break;

            case TimeEventType.DailyReward:
                Debug.Log("����ÿ������");
                EventManager.Instance.TriggerEvent(GameEvent.RestartDailyEvent);
                remainingTime = startTime.ToTimeSpan();
                currentState = ActivityState.Ongoing;
                break;
            default:
                Debug.Log("û���¼����Ե���");
                break;
        }

        Debug.Log("����: " + eventType);

    }

    // ����¿�ʼ
    private void RestartActivity()
    {

        if (eventType == TimeEventType.Streak)
        {
            //�������ư�ť
            GameDataManager.SetStreakLockedVal(false);
            HomeSceneUI.Instance.homeUI.SetStreakButton(true);
            remainingTime = startTime.ToTimeSpan();
            GameDataManager.ReStartStreakEvent();
            currentState = ActivityState.Ongoing; // ״̬��Ϊ������
        }
        else if (eventType == TimeEventType.LuckySpin)
        {
            //�����齱��ť
            GameDataManager.SetLuckySpinLockedVal(true);
            HomeSceneUI.Instance.homeUI.SetLuckSpinButton(true);
            remainingTime = startTime.ToTimeSpan();
            GameDataManager.RestartLuckySpinEvent();
            currentState = ActivityState.Ongoing; // ״̬��Ϊ������
        }
        else
        {
            Debug.Log("û���¼����Ե���");
        }

        Debug.Log("���¿�ʼ����: " + eventType);

        // �������õ���ʱΪ���ʼʱ��

    }

    // ����ʣ��ʱ�䣨���ϴε�¼ʱ��ָ���
    public void UpdateRemainingTime(SerializableTime lastLoginTime)
    {
        //Ϊֹͣ״̬��ʱ�䣬����Ҫ����
        if (currentState == ActivityState.Stop)
            return;

        remainingTime = leftTime.ToTimeSpan();
        // ������ϴε�¼ʱ�䵽��ǰʱ���ʱ���
        TimeSpan timeDifference = DateTime.Now - lastLoginTime.ToDateTime();


        //���ʱ��û����ʣ��ʱ�����Ϊ0
        if ((remainingTime - timeDifference).TotalSeconds >= 0)
        {
            remainingTime -= timeDifference;
        }
        else
        {
            //ʣ���ʱ����Ѿ�Ϊ������
            if (eventType == TimeEventType.Heart)
            {
                //���������¼�
                timeDifference -= remainingTime;
                //ѭ�������������˶��ٸ�ʱ�䣬��Ҫ�������¼�
                while (timeDifference.TotalSeconds > 0 && GameDataManager.CurrentGameData.heartCount < 5)
                {
                    timeDifference -= startTime.ToTimeSpan();
                    GameDataManager.AddHeartCount(1);
                }
                //ѭ������ʱ��ʱ����Ϊ����
                remainingTime = startTime.ToTimeSpan() + timeDifference;
            }
            else if ((eventType == TimeEventType.Streak && currentState == ActivityState.Ongoing )
                || (eventType == TimeEventType.LuckySpin && currentState == ActivityState.Ongoing))
            {
                //���������¼����ǳ齱�¼����Զ�����ָ���ĵ���ʱ
                timeDifference -= remainingTime;

                //����Ѿ������˵���ʱ��ʱ��
                if ((endTime.ToTimeSpan() - timeDifference).TotalSeconds < 0)
                {
                    //������ʼ��״̬
                    currentState = ActivityState.Ongoing;
                    remainingTime = startTime.ToTimeSpan();
                }
                else
                {
                    //û�г�������ʱ��ʱ��
                    remainingTime = endTime.ToTimeSpan();
                    remainingTime -= timeDifference;
                    currentState = ActivityState.Ended;
                }
            }
            else
            {
                remainingTime = TimeSpan.Zero;
            }
        }

        if (remainingTime.TotalSeconds <= 0)
        {
            remainingTime = TimeSpan.Zero;
        }
    }
    // ��ȡ��ǰ����ʱ���ַ�����ʾ
    public string GetCountdownString()
    {
        return string.Format("{0:D2}�� {1:D2}ʱ {2:D2}�� {3:D2}��",
            remainingTime.Days, remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
    }
}

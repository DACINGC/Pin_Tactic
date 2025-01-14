using System;
using System.Collections.Generic;

/// <summary>
/// �¼����͵�ö�٣��������ֲ�ͬ���¼�
/// </summary>
public enum GameEvent
{
    UpdateDailyEvent,
    // ��Ӹ����¼�����...
    UpdateStreakEvent,
    OpenChestEvent,

    //����ʱ�����������
    AddHearEvent,
    RestartDailyEvent//����ÿ�ս���
}

/// <summary>
/// һ���򵥵��޲��¼�������
/// </summary>
public class EventManager
{
    private static readonly EventManager _instance = new EventManager();
    public static EventManager Instance => _instance;

    // �洢�¼����ֵ�
    private Dictionary<GameEvent, Action> eventDictionary;

    // ˽�й��캯����ȷ������ģʽ
    private EventManager()
    {
        eventDictionary = new Dictionary<GameEvent, Action>();
    }

    /// <summary>
    /// ע���¼�
    /// </summary>
    /// <param name="eventType">�¼�����</param>
    /// <param name="listener">��������</param>
    public void RegisterEvent(GameEvent eventType, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }

        eventDictionary[eventType] += listener;
    }

    /// <summary>
    /// ע���¼�
    /// </summary>
    /// <param name="eventType">�¼�����</param>
    /// <param name="listener">��������</param>
    public void UnregisterEvent(GameEvent eventType, Action listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] -= listener;

            // ���û�м������ˣ��Ƴ����¼�����
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventType">�¼�����</param>
    public void TriggerEvent(GameEvent eventType)
    {
        if (eventDictionary.ContainsKey(eventType) && eventDictionary[eventType] != null)
        {
            eventDictionary[eventType].Invoke();
        }
        else
        {
            Console.WriteLine($"[EventManager] û�м����߶����¼�: {eventType}");
        }
    }
}

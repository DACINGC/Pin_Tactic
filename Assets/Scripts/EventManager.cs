using System;
using System.Collections.Generic;

/// <summary>
/// 事件类型的枚举，用于区分不同的事件
/// </summary>
public enum GameEvent
{
    UpdateDailyEvent,
    // 添加更多事件类型...
    UpdateStreakEvent,
    OpenChestEvent,

    //用于时间管理器调用
    AddHearEvent,
    RestartDailyEvent//重置每日奖励
}

/// <summary>
/// 一个简单的无参事件管理器
/// </summary>
public class EventManager
{
    private static readonly EventManager _instance = new EventManager();
    public static EventManager Instance => _instance;

    // 存储事件的字典
    private Dictionary<GameEvent, Action> eventDictionary;

    // 私有构造函数，确保单例模式
    private EventManager()
    {
        eventDictionary = new Dictionary<GameEvent, Action>();
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void RegisterEvent(GameEvent eventType, Action listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }

        eventDictionary[eventType] += listener;
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    /// <param name="listener">监听函数</param>
    public void UnregisterEvent(GameEvent eventType, Action listener)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] -= listener;

            // 如果没有监听者了，移除该事件类型
            if (eventDictionary[eventType] == null)
            {
                eventDictionary.Remove(eventType);
            }
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventType">事件类型</param>
    public void TriggerEvent(GameEvent eventType)
    {
        if (eventDictionary.ContainsKey(eventType) && eventDictionary[eventType] != null)
        {
            eventDictionary[eventType].Invoke();
        }
        else
        {
            Console.WriteLine($"[EventManager] 没有监听者订阅事件: {eventType}");
        }
    }
}

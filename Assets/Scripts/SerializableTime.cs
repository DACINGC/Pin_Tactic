using System;
using Newtonsoft.Json;

[Serializable]
public class SerializableTime
{
    [JsonProperty] public long seconds; // ����Ϊ��λ�洢 TimeSpan
    [JsonProperty] public string dateTime; // �洢 DateTime ���ַ�����ʽ��ISO 8601 ��ʽ��

    // ���캯�������� TimeSpan ����
    public SerializableTime(TimeSpan timeSpan)
    {
        seconds = (long)timeSpan.TotalSeconds;
        dateTime = null; // ����Ҫ�洢���� DateTime
    }

    // ���캯�������� DateTime ����
    public SerializableTime(DateTime dateTime)
    {
        seconds = 0; // ����Ҫ�洢��������
        this.dateTime = dateTime.ToString("o"); // ISO 8601 ��ʽ
    }

    // ���� JSON �����л��Ĺ��캯��
    [JsonConstructor]
    public SerializableTime(long seconds, string dateTime)
    {
        this.seconds = seconds;
        this.dateTime = dateTime;
    }

    // �� SerializableTime ת��Ϊ TimeSpan
    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromSeconds(seconds);
    }

    // �� SerializableTime ת��Ϊ DateTime
    public DateTime ToDateTime()
    {
        if (!string.IsNullOrEmpty(dateTime))
        {
            return DateTime.Parse(dateTime); // �Ӵ洢�� ISO 8601 ��ʽ����
        }
        else
        {
            return DateTime.Now.AddSeconds(seconds); // ���ֻ�洢��������ӵ�ǰʱ������
        }
    }

    // ��̬�������� TimeSpan ���� SerializableTime
    public static SerializableTime FromTimeSpan(TimeSpan timeSpan)
    {
        return new SerializableTime(timeSpan);
    }

    // ��̬�������� DateTime ���� SerializableTime
    public static SerializableTime FromDateTime(DateTime dateTime)
    {
        return new SerializableTime(dateTime);
    }

    // ���ʱ����ַ�����ʾ
    public override string ToString()
    {
        if (!string.IsNullOrEmpty(dateTime))
        {
            return dateTime; // ����� DateTime���򷵻� DateTime ���ַ���
        }
        else
        {
            return ToTimeSpan().ToString(); // ���򷵻� TimeSpan ���ַ���
        }
    }
}

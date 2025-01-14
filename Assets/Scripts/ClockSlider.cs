using UnityEngine;
using UnityEngine.UI;

public class ClockSlider : MonoBehaviour
{
    private Text countdownText; // ������ʾ����ʱ���ı�
    private Slider countdownSlider; // ������ʾ����ʱ����
    [SerializeField]private int minutes = 0; // ����ʱ�ķ���
    [SerializeField]private int seconds = 30; // ����ʱ������

    private float totalTime; // ��ʱ�䣨�룩
    private float currentTime; // ��ǰʣ��ʱ��

    private void Awake()
    {
        countdownText = transform.Find("Fill Area/Text").GetComponent<Text>();
        countdownSlider = GetComponent<Slider>();
    }

    public void SetTime(int m, int s)
    {
        minutes = m;
        seconds = s;
        totalTime = minutes * 60 + seconds;
        currentTime = totalTime;

        // ��ʼ�� Slider ���ֵ�ͳ�ʼֵ
        countdownSlider.maxValue = totalTime;
        countdownSlider.value = totalTime;

        int minutesLeft = Mathf.FloorToInt(currentTime / 60);
        int secondsLeft = Mathf.FloorToInt(currentTime % 60);

        countdownText.text = $"{minutesLeft:00}:{secondsLeft:00}";
    }

    void Update()
    {
        if (currentTime > 0 && GameManager.Instance._GameState == GameState.Start)
        {
            // ����ʱ��
            currentTime -= Time.deltaTime;

            // ���µ���ʱ�ı�
            UpdateCountdownText();

            // ���� Slider ����
            countdownSlider.value = currentTime;

            // ȷ��ʱ�䲻Ϊ��ֵ
            if (currentTime <= 0)
            { 
                currentTime = 0;
                //����ʱ������Ϸ����
                UIManager.Instance.ShowUI<TimeUpUI>();
                //UIManager.Instance.ShowUI<LoseUI>();
            }
        }
    }

    /// <summary>
    /// ���µ���ʱ�ı�
    /// </summary>
    void UpdateCountdownText()
    {
        int minutesLeft = Mathf.FloorToInt(currentTime / 60);
        int secondsLeft = Mathf.FloorToInt(currentTime % 60);

        countdownText.text = $"{minutesLeft:00}:{secondsLeft:00}";
    }
}

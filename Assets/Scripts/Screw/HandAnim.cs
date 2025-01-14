using UnityEngine;

public class HandAnim : MonoBehaviour
{
    private GameObject child1; // ��һ��������
    private GameObject child2; // �ڶ���������
    [SerializeField] private float switchInterval = 0.2f; // �л���ʱ����
    [SerializeField] private float xoffset = 1;
    [SerializeField] private float yoffset = -1;
    private Glass level1Glass;
    private float timer = 0f; // ��ʱ��
    private bool isChild1Active = true; // ��ǰ״̬

    private void Awake()
    {
        level1Glass = transform.parent.GetChild(0).GetComponent<Glass>();
    }
    void Start()
    {
        child1 = transform.GetChild(0).gameObject;
        child2 = transform.GetChild(1).gameObject;

        // ��ʼ��ʱȷ��һ����ʾ��һ������
        child1.SetActive(true);
        child2.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            // ʱ�䵽���л�״̬
            isChild1Active = !isChild1Active;

            if (level1Glass.FirstScrew)
            {
                child1.SetActive(isChild1Active);
                child1.transform.position = level1Glass.FirstScrew.transform.position + new Vector3(xoffset, yoffset, 0);
                child2.SetActive(!isChild1Active);
                child2.transform.position = level1Glass.FirstScrew.transform.position + new Vector3(xoffset, yoffset, 0);
            }
            else
            {
                child1.SetActive(false);
                child2.SetActive(false);
            }

            timer = 0f; // ���ü�ʱ��
        }
    }
}

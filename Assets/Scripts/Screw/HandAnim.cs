using UnityEngine;

public class HandAnim : MonoBehaviour
{
    private GameObject child1; // 第一个子物体
    private GameObject child2; // 第二个子物体
    [SerializeField] private float switchInterval = 0.2f; // 切换的时间间隔
    [SerializeField] private float xoffset = 1;
    [SerializeField] private float yoffset = -1;
    private Glass level1Glass;
    private float timer = 0f; // 计时器
    private bool isChild1Active = true; // 当前状态

    private void Awake()
    {
        level1Glass = transform.parent.GetChild(0).GetComponent<Glass>();
    }
    void Start()
    {
        child1 = transform.GetChild(0).gameObject;
        child2 = transform.GetChild(1).gameObject;

        // 初始化时确保一个显示，一个隐藏
        child1.SetActive(true);
        child2.SetActive(false);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            // 时间到，切换状态
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

            timer = 0f; // 重置计时器
        }
    }
}

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GetNapGlass : MonoBehaviour
{
    [SerializeField] private List<Sprite> glassSprites = new List<Sprite>(); // �� 2 - 5����, ��Ӧ���� 0 - 3

    [SerializeField] private GameObject napglassPrefab;
    private static GetNapGlass instance;

    private float moveTime;
    public static GetNapGlass Instance
    {
        get
        {
            if (instance == null)
            {
                // �����Ѵ��ڵ�ʵ��
                instance = FindObjectOfType<GetNapGlass>();
                // ���û���ҵ�������һ���µ�ʵ��
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GetNapGlass");
                    instance = singletonObject.AddComponent<GetNapGlass>();
                }
            }
            return instance;
        }
    }
    private GameObject curNapGlass;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject); // ȷ���ڳ����л�ʱ���ᱻ����
        }
        else
        {
            Destroy(gameObject); // ���ٶ����ʵ��
        }
    }

    private void Start()
    {
        moveTime = 0.25f;
    }
    /// <summary>
    /// ���ɸ��ӵĺ���
    /// </summary>
    public void InitNapGlassPrefab(Box _box, System.Action callback = null)
    {
        GameObject newNapGlass = Instantiate(napglassPrefab);
        newNapGlass.GetComponent<SpriteRenderer>().sprite = GetFitNap(_box);
        newNapGlass.GetComponent<SpriteRenderer>().sortingOrder = 10;

        newNapGlass.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetFItGlass(_box);
        newNapGlass.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 9;
        newNapGlass.transform.SetParent(_box.transform);
        newNapGlass.transform.position = new Vector3(_box.transform.position.x, _box.transform.position.y + 5.5f);

        curNapGlass = newNapGlass;
        //�ǵ�λ����ʵ����ƫ��
        MoveToBox(new Vector3(_box.transform.position.x, _box.transform.position.y + 0.2f), callback);
    }
    private Sprite GetFItGlass(Box _box)
    {
        Sprite sprite = null;
        switch (_box.HoleCount)
        {
            case 2:
                sprite = glassSprites[0];
                break;
            case 3:
                sprite = glassSprites[1];
                break;
            case 4:
                sprite = glassSprites[2];
                break;
            case 5:
                sprite = glassSprites[3];
                break;
        }
        return sprite;
    }
    private Sprite GetFitNap(Box _box)
    {
        return ResourceLoader.Instance.GetBoxBoardSprite(_box.BoxColor, _box.HoleCount);
    }
    /// <summary>
    /// �������ƶ��ĺ���
    /// </summary>
    /// �����������װ��ĺ���
    private void MoveToBox(Vector3 targetPos, System.Action callback = null)
    {
        curNapGlass.transform.DOMove(targetPos, moveTime)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                callback?.Invoke();
            });
    }

}

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EmptyHoleManager : MonoBehaviour
{
    [SerializeField] private List<EmptyHole> holeList = new List<EmptyHole>(); // ���б�
    [SerializeField] private List<EmptyHole> extraHoles = new List<EmptyHole>(); // �����б�
    [SerializeField] private GameObject emptyHolePrefab; // ����ղ۵�Ԥ����
    [SerializeField] private ParticleSystem apearFx;
    [SerializeField] private int curScrewCount = 0; // ��ǰ��˿����

    private void Start()
    {
        // ��ʼ�����б�
        for (int i = 0; i < transform.childCount; i++)
        {
            holeList.Add(transform.GetChild(i).GetComponent<EmptyHole>());
        }
    }

    /// <summary>
    /// ���뵽�յĲ���
    /// </summary>
    public void AddToEmptyHole(Screw _screw)
    {
        // �������б�Ͷ����б�
        foreach (EmptyHole hole in GetAllHoles())
        {
            if (hole.IsEmpty())
            {

                if (_screw.CanRemoveFromGlass())
                { 
                    hole.Screw =_screw;
                    _screw.transform.SetParent(hole.transform);
                    curScrewCount++;
                }

                _screw.MoveUp(hole.transform.position);
                return;
            }
        }

        Debug.Log("�޷����뵽�ղ���");
    }

    /// <summary>
    /// �Ӵ�ѡ��ѡ��С�����Ŀ������
    /// </summary>
    /// <param name="targetBox"></param>
    public void AddToBox(Box targetBox)
    {
        ////�������û�����Ƕ���
        //if (targetBox.ISStarFull())
        //{
        //    SetScrewToBox(targetBox, true);
        //}
        //else
        //{
        //    //�����Ƕ���
        //    SetScrewToBox(targetBox, false);
        //}
        SetScrewToBox(targetBox);

    }
    /// <summary>
    /// ������������˿���뵽�����У��Ƿ�����������˿��
    /// </summary>
    public void SetScrewToBox(Box targetBox, bool skipStarBalls = false)
    {
        //Debug.Log("���ÿղۺ���");
        foreach (EmptyHole hole in GetAllHoles())
        {
            if (!hole.IsEmpty())
            {
                // ���ݴ���Ĳ��������Ƿ���������С��
                if (skipStarBalls && hole.Screw.IsStar())
                    continue;

                //Ŀ�����ͨС�����ˣ�����Ҫ���������ͨ��˿��
                if (targetBox.ISNormalFull() && !hole.Screw.IsStar())
                    continue;

                //Ŀ���������˿���ˣ�����Ҫ�������������˿��
                if (targetBox.ISStarFull() && hole.Screw.IsStar())
                    continue;

                if (targetBox.BoxColor == hole.Screw.ScrewColor)
                {
                    targetBox.SetHoleList(hole.Screw);
                    //Debug.Log(hole.Screw.ScrewColor + "���������");
                    // ����������Ϊ��
                    hole.Screw = null;
                    curScrewCount--;
                }
            }
        }
    }

    /// <summary>
    /// �����Ϸ�Ƿ�ʧ�ܵĺ���
    /// </summary>
    public bool CheckGameFailed()
    {
        if (curScrewCount >= GetAllHoles().Count)
        {
            //Debug.Log("��Ϸʧ��: �ղ��Ѿ�����");
            //Debug.Log("curCount: " + curScrewCount + " listCount: " + GetAllHoles().Count);
            return true;
        }

        return false;
    }

    /// <summary>
    /// ������пղ�
    /// </summary>
    public void ClearEmptyHole()
    {
        curScrewCount = 0;
        transform.position = Vector3.zero;

        foreach (EmptyHole hole in GetAllHoles())
        {
            if (!hole.IsEmpty())
            {
                Destroy(hole.Screw.gameObject);
                hole.Screw = null;
            }
        }

        foreach (EmptyHole hole in extraHoles)
        {
            Destroy(hole.gameObject);
        }
        extraHoles.Clear();
    }

    /// <summary>
    /// �������Ŀղ�Buff
    /// </summary>
    public void ActivateExtraHole()
    {
        GameObject newHole = Instantiate(emptyHolePrefab, transform);
        ParticleSystem fx = Instantiate(apearFx, transform);
        EmptyHole extraHole = newHole.GetComponent<EmptyHole>();

        // ��̬����λ�ã��ɸ������������
        newHole.transform.localPosition = new Vector3(extraHoles.Count + 3, 6.2f, 0);
        fx.transform.position = newHole.transform.position;
        fx.Play();
        Destroy(fx.gameObject, 1.2f);

        extraHoles.Add(extraHole);
        //Debug.Log("�������Ŀղ�Buff��");

        // ʹ��Dotween�ø������ƶ��Ա��־���
        transform.DOLocalMoveX(-(extraHoles.Count * 0.75f), 0.35f).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// �Ƴ�����Ŀղ�Buff
    /// </summary>
    public void DeactivateExtraHole()
    {
        foreach (EmptyHole hole in extraHoles)
        {
            if (!hole.IsEmpty())
            {
                Destroy(hole.Screw.gameObject); // ������ӵ���˿
            }
            Destroy(hole.gameObject); // ������ӵĿղ�
        }

        extraHoles.Clear();
        Debug.Log("�Ƴ�����Ŀղ�Buff��");
    }

    /// <summary>
    /// ��ȡ���пղۣ����б� + �����б�
    /// </summary>
    private List<EmptyHole> GetAllHoles()
    {
        List<EmptyHole> allHoles = new List<EmptyHole>(holeList);
        allHoles.AddRange(extraHoles);
        return allHoles;
    }

    /// <summary>
    /// ֻ�ܼ�2������
    /// </summary>
    /// <returns></returns>
    public bool CanAddExtraHole()
    {
        if (extraHoles.Count < 2)
            return true;

        return false;
    }
}

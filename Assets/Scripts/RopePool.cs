using UnityEngine;
using Obi;
using System.Collections.Generic;

public class RopePool : MonoBehaviour
{
    public ObiRope prefab; // Rope �� Chain ��Ԥ����
    public int poolSize = 4; // ����ش�С

    public ObiSolver solver; // ������ Solver ����
    private Queue<ObiRope> pool = new Queue<ObiRope>();

    private void Awake()
    {
        solver = GetComponent<ObiSolver>();
    }

    void Start()
    {
        InitializePool();
    }

    // ��ʼ�������
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ObiRope newObject = Instantiate(prefab, solver.transform);
            newObject.gameObject.SetActive(false);
            pool.Enqueue(newObject);
        }
    }

    // �Ӷ�����л�ȡ����
    public ObiRope GetObject()
    {
        if (pool.Count > 0)
        {
            ObiRope obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // ��Ϊ��ʱ��̬�����µĶ���
            ObiRope newObject = Instantiate(prefab, solver.transform);
            return newObject;
        }
    }

    // ������黹�������
    public void ReturnObject(ObiRope obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}

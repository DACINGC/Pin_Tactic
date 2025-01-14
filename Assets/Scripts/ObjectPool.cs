using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> pool;  // �洢����Ķ���
    private GameObject prefab;       // ����ʵ������Ԥ����
    private Transform parent; // ���ڵ㣬���ڹ������㼶
    /// <summary>
    /// ���캯����
    /// </summary>
    /// <param name="prefab">��Ҫʵ������Ԥ���塣</param>
    /// <param name="initialCount">��ʼ�������ʱ��������</param>
    /// <param name="parent">���ڵ㣨��ѡ����</param>
    public ObjectPool(GameObject prefab, int initialCount, Transform parent = null)
    {
        pool = new Queue<GameObject>();
        this.prefab = prefab;
        this.parent = parent;

        // ��ʼ�������
        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// ��ȡ�����������û�ж����򴴽��¶���
    /// </summary>
    /// <returns>���еĶ���</returns>
    public GameObject GetObj()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Object.Instantiate(prefab, parent);
        }

        return obj;
    }

    /// <summary>
    /// ���ն��󣬽������¼�����С�
    /// </summary>
    /// <param name="obj">��Ҫ���յĶ���</param>
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

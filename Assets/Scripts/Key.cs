using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Key : MonoBehaviour
{
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void InitKeyLayer(string layername, int order)
    {
        sr.sortingLayerName = layername;
        sr.sortingOrder = order;
    }

    //�ƶ�������λ��
    public void MoveToLock(Lock targetLock, System.Action callback = null)
    {
        transform.parent = GameManager.Instance.transform;
        // ����Ŀ�귽��
        Vector3 direction = (targetLock.transform.position - transform.position).normalized;

        // ������ת�Ƕȣ�����Կ�׵�ͷ�����£�Transform.down ��ͷ������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // ��ʼ�ƶ�
        transform.DOMove(targetLock.transform.position, 0.85f).SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                callback?.Invoke();
                targetLock.ShowUnLockFx();
                Destroy(gameObject, 0.01f);
            });
    }

}

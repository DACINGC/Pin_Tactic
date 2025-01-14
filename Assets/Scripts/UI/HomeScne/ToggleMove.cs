using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // ��Ҫʹ�� DOTween ���

public class ToggleMove : MonoBehaviour
{
    [Header("�ƶ������ʱ��")]
    private float moveDistance = 62f; // �����ƶ��ľ���
    private float moveDuration = 0.4f; // �ƶ���ʱ��

    private bool isMoved = false; // ��������Ƿ��Ѿ��ƶ�
    private bool isMoving = false;

    private Transform child;
    private RectTransform rectTrans;
    private void Start()
    {
        // Ϊ��ť��ӵ���¼�����
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX("Click");
                TogglePosition();
            } );
        }

        rectTrans = GetComponent<RectTransform>();
        child = transform.GetChild(0);
    }

    private void TogglePosition()
    {
        if (isMoving)
            return;

        if (isMoved)
        {
            isMoving = true;
            // ����Ѿ��ƶ����򷵻�ԭʼλ��
            rectTrans.DOLocalMoveX(-moveDistance, moveDuration)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        isMoved = false;
                        child.gameObject.SetActive(false);
                        isMoving = false;
                    });
        }
        else
        {
            isMoving = true;
            rectTrans.DOLocalMoveX(moveDistance, moveDuration)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        isMoved = true;
                        child.gameObject.SetActive(true);
                        isMoving = false;
                    });

        }

    }
}

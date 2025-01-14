using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public static class AnimationUtility
{
    private static float delayBetweenChildren = 0.08f;
    //private static float duration = 0.5f;
    /// <summary>
    /// ��һ��UIԪ�ز��ϵش�ԭʼλ���ƶ����Ҳ�ָ��λ��
    /// </summary>
    /// <param name="target">�ƶ���Ŀ������</param>
    /// <param name="offsetX">�����ƶ�����������ƫ����</param>
    /// <param name="duration">�ƶ���Ŀ��λ�õ�ʱ��</param>
    public static void RepeatMoveToRight(Transform target, float offsetX = 12f, float duration = 2f)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ��¼ԭʼλ�ã��������꣩
        Vector3 originalPosition = target.position;

        // ����Ŀ��λ�ã��������꣩
        Vector3 targetPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);

        // ѭ�������ƶ���������ԭʼλ���ƶ���Ŀ��λ�ã��ٻص�ԭʼλ������
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.Linear)           // �����ƶ�
              .SetLoops(-1, LoopType.Restart) // ���޴�ѭ������ͷ��ʼ
              .OnStepComplete(() => target.position = originalPosition); // ÿ��ѭ������λ�ã��������
    }
    /// <summary>
    /// Q�����������볡����
    /// </summary>
    public static void PlayEnterFromRight(Transform target, float startOffsetX = 20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ��¼Ŀ��λ��
        Vector3 targetPosition = target.position;

        // ���ó�ʼλ�����Ҳ�ƫ��
        target.position = new Vector3(targetPosition.x + startOffsetX, targetPosition.y, targetPosition.z);

        // ִ�ж��������Ҳ��ƶ���Ŀ��λ�ã�������Ч��
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.OutBack)
              .OnStart(() => FadeIn(target, duration))
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// Q�����������˳�����
    /// </summary>
    public static void PlayExitToRight(Transform target, float endOffsetX = 20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ��¼ԭʼλ��
        Vector3 originalPosition = target.position;

        // ����Ŀ���˳�λ��
        Vector3 exitPosition = new Vector3(originalPosition.x + endOffsetX, originalPosition.y, originalPosition.z);

        // ִ�ж������ӵ�ǰλ���ƶ����Ҳ࣬���쵽����Ч��
        target.DOMove(exitPosition, duration)
              .SetEase(Ease.InQuad)
              .OnStart(() => FadeOut(target, duration))
              .OnComplete(() =>
              {
                  target.position = originalPosition;
                  onComplete?.Invoke();
              });
    }

    /// <summary>
    /// Q�����������볡����
    /// </summary>
    public static void PlayEnterFromLeft(Transform target, float startOffsetX = -20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ��¼Ŀ��λ��
        Vector3 targetPosition = target.position;

        // ���ó�ʼλ�������ƫ��
        target.position = new Vector3(targetPosition.x + startOffsetX, targetPosition.y, targetPosition.z);

        // ִ�ж�����������ƶ���Ŀ��λ�ã�������Ч��
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.OutBack)
              .OnStart(() => FadeIn(target, duration))
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// Q�����������˳�����
    /// </summary>
    public static void PlayExitToLeft(Transform target, float endOffsetX = -20f, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ��¼ԭʼλ��
        Vector3 originalPosition = target.position;

        // ����Ŀ���˳�λ��
        Vector3 exitPosition = new Vector3(originalPosition.x + endOffsetX, originalPosition.y, originalPosition.z);

        // ִ�ж������ӵ�ǰλ���ƶ�����࣬���쵽����Ч��
        target.DOMove(exitPosition, duration)
              .SetEase(Ease.InQuad)
              .OnStart(() => FadeOut(target, duration))
              .OnComplete(() =>
              {
                  target.position = originalPosition;
                  onComplete?.Invoke();
              });
    }

    /// <summary>
    /// ����Ŀ������
    /// </summary>
    public static void FadeIn(Transform target, float duration = 0.5f, Action onComplete = null)
    {
        // ��ȡ����� CanvasGroup
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }
        target.gameObject.SetActive(true);
        // ���ó�ʼ͸����Ϊ 0
        canvasGroup.alpha = 0f;

        // ���Զ���
        canvasGroup.DOFade(1f, duration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// ����Ŀ������
    /// </summary>
    public static void FadeOut(Transform target, float duration = 0.5f, Action onComplete = null)
    {
        // ��ȡ����� CanvasGroup
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        onComplete += () => target.gameObject.SetActive(false);
        // ��������
        canvasGroup.DOFade(0f, duration)
                   .SetEase(Ease.Linear)
                   .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// ��������������볡��������Ч��
    /// </summary>
    /// <param name="parent">������ Transform</param>
    /// <param name="startOffsetY">��ʼλ�õ� Y ��ƫ��</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ��</param>
    /// <summary>
    public static void ChildrenDownToUp(Transform parent, List<Vector3> originPosList, float startOffsetY = -20f, float duration = 0.5f, System.Action onComplete = null)
    {
        // ��ȡ����������
        int childCount = parent.childCount;
        // ��������
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // ��¼��ʼλ�ú�Ŀ��λ��
            Vector3 originalPosition = originPosList[i];
            Vector3 startPosition = new Vector3(originalPosition.x, originalPosition.y + startOffsetY, originalPosition.z);

            // ���ó�ʼλ��
            child.position = startPosition;
            // �������ӳ�ʼλ�õ�Ŀ��λ�ã�������Ч��
            sequence.Insert(
                i * delayBetweenChildren, // ����ÿ����������ӳ�ʱ��
                child.DOMove(originalPosition, duration)
                     .SetEase(Ease.OutQuad) // ����
                     .OnStart(() => FadeIn(child, duration)) // ������ʼʱ����
            );
        }

        // ������ɺ�Ļص�
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// �������ԭλ�������˳�
    /// </summary>
    /// <param name="parent">������ Transform</param>
    /// <param name="endOffsetY">Ŀ��λ�õ� Y ��ƫ��</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ��</param>
    /// <summary>
    public static void ChildrenUpToDown(Transform parent, List<Vector3> originPosList, float endOffsetY = -20f, float duration = 0.5f, System.Action onComplete = null)
    {
        // ��ȡ����������
        int childCount = parent.childCount;
        // ��������
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // ��¼��ʼλ��
            Vector3 originalPosition = child.position;

            // ����Ŀ��λ��
            Vector3 endPosition = new Vector3(originalPosition.x, originalPosition.y + endOffsetY, originalPosition.z);

            // ��������ԭλ���ƶ���Ŀ��λ�ã�������Ч��
            sequence.Insert(
                i * delayBetweenChildren, // ������������ӳ�ʱ��
                child.DOMove(endPosition, duration)
                .OnStart(() => FadeOut(child, duration))
                     .SetEase(Ease.InQuad) // ����������ʧ
                     .OnComplete(() =>
                     {
                         // ������ɺ�����Ϊ��ʼλ��
                         child.position = originalPosition;
                     })
            );
        }

        // ������ɺ�Ļص�
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// �����������������
    /// </summary>
    /// <param name="parent">������</param>
    /// <param name="duration">ÿ��������Ľ���ʱ��</param>
    /// <param name="delayBetweenChildren">ÿ��������֮����ӳ�ʱ��</param>
    /// <param name="onComplete">���������嶯����ɺ�Ļص�</param>
    public static void ShowChildrenOneByOne(Transform parent, float duration = 0.5f, float delayBetweenChildren = 0.08f, Action onComplete = null)
    {
        int childCount = parent.childCount;

        parent.gameObject.SetActive(true);
        // ����һ�����У����ڰ�˳��ִ��ÿ��������Ľ��Զ���
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // ��ȡ����� CanvasGroup ���
            CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = child.gameObject.AddComponent<CanvasGroup>();
            }

            // ���ó�ʼ͸����Ϊ 0
            canvasGroup.alpha = 0f;

            // ����ִ��ÿ��������Ľ��Զ���
            sequence.Insert(
                i * delayBetweenChildren, // ����ÿ��������֮����ӳ�
                canvasGroup.DOFade(1f, duration).SetEase(Ease.Linear)
            );
        }

        // ���������嶯����ɺ�Ļص�
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// �Ŵ�Ȼ����С�Ķ���Ч����
    /// </summary>
    /// <param name="target">��Ҫִ�ж��������� Transform��</param>
    /// <param name="scaleMultiplier">�Ŵ�ı���������</param>
    /// <param name="scaleDuration">�Ŵ����С�Ķ���ʱ����</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ����</param>
    public static void PlayScaleBounce(Transform target, float scaleMultiplier = 1.2f, float scaleDuration = 0.3f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ��¼ԭʼ�����ű���
        Vector3 originalScale = target.localScale;

        // �������ж���
        Sequence sequence = DOTween.Sequence();

        // �Ŵ󶯻�
        sequence.Append(target.DOScale(originalScale * scaleMultiplier, scaleDuration).SetEase(Ease.OutQuad));

        // ��С����
        sequence.Append(target.DOScale(originalScale, scaleDuration).SetEase(Ease.InQuad));

        // ������ɻص�
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// �������ƶ���ָ��λ��
    /// </summary>
    /// <param name="target">��Ҫ�ƶ������� Transform</param>
    /// <param name="targetPosition">Ŀ��λ��</param>
    /// <param name="duration">�ƶ��ĳ���ʱ��</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ��</param>
    public static void MoveToPosition(Transform target, Vector3 targetPosition, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            return;
        }

        // ʹ�� DOTween ����λ�ö���
        target.DOMove(targetPosition, duration)
              .SetEase(Ease.InQuad)
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// �����ƶ��������Ķ���
    /// </summary>
    /// <param name="target">Ҫ���ж�����Ŀ�� RectTransform</param>
    /// <param name="moveDistance">�����ƶ��ľ���</param>
    /// <param name="duration">��������ʱ��</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ</param>
    public static void MoveUpAndFadeOut(RectTransform target, float duration = 0.8f, float moveDistance = 100, System.Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot perform animation.");
            return;
        }
        Vector3 initialPosition = target.position;
        Vector3 initialScale = target.localScale;

        // ȷ��Ŀ���� CanvasGroup�����ڿ���͸����
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        // ��ʼ״̬��ȷ����ȫ��ʾ
        canvasGroup.alpha = 1f;

        // ��¼��ʼλ��
        Vector3 startPosition = target.anchoredPosition;

        // ���� DOTween ����
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOAnchorPosY(startPosition.y + moveDistance, duration).SetEase(Ease.OutQuad)) // �����ƶ�
                .Join(canvasGroup.DOFade(0, duration).SetEase(Ease.Linear)) // ����
                .OnComplete(() =>
                {
                    target.position = initialPosition;
                    target.localScale = initialScale;
                    // ������ɺ�Ĵ���
                    onComplete?.Invoke();
                });

        // ���ö���
        sequence.Play();
    }

    /// <summary>
    /// ��С�������Ķ���
    /// </summary>
    /// <param name="target">Ҫ���ж�����Ŀ�� Transform</param>
    /// <param name="duration">��������ʱ��</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ</param>
    public static void ScaleDownAndFadeOut(Transform target, float duration = 0.8f, System.Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot perform animation.");
            return;
        }

        // ��ȡ��ʼλ�úʹ�С
        Vector3 initialPosition = target.position;
        Vector3 initialScale = target.localScale;

        // ȷ��Ŀ���� CanvasGroup�����ڿ���͸����
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        // ��ʼ״̬��ȷ����ȫ��ʾ
        canvasGroup.alpha = 1f;

        // ���� DOTween ����
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOScale(Vector3.zero, duration).SetEase(Ease.InQuad)) // ��С
                .Join(canvasGroup.DOFade(0, duration).SetEase(Ease.Linear)) // ����
                .OnComplete(() =>
                {
                    // ������ɺ�ָ���ԭ����λ�úʹ�С
                    target.position = initialPosition;
                    target.localScale = initialScale;

                    // ������ɺ�Ļص�
                    onComplete?.Invoke();
                });

        // ���ö���
        sequence.Play();
    }

    /// <summary>
    /// �Ŵ󲢽��ԵĶ�����������Ч������
    /// </summary>
    /// <param name="target">Ҫ���ж�����Ŀ�� Transform</param>
    /// <param name="duration">��������ʱ��</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ</param>
    public static void ScaleUpAndFadeIn(Transform target, float duration = 0.8f, System.Action onComplete = null)
    {
        if (target == null)
        {
            Debug.LogError("Target is null. Cannot perform animation.");
            return;
        }

        // ��ȡ��ʼλ�úʹ�С
        Vector3 initialScale = target.localScale;

        // ȷ��Ŀ���� CanvasGroup�����ڿ���͸����
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
        }

        // ��ʼ״̬��ȷ����ȫ����
        canvasGroup.alpha = 0f;
        target.localScale = Vector3.zero;

        // ���� DOTween ����
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOScale(initialScale, duration).SetEase(Ease.OutBounce)) // �Ŵ󣨴�����Ч����
                .Join(canvasGroup.DOFade(1, duration * 0.5f).SetEase(Ease.Linear)) // ���ԣ�ʱ��Ϊ��ʱ����һ�룩
                .OnComplete(() =>
                {
                    // ȷ��������ɺ�͸���Ⱥ����Ŷ�������ȷ
                    canvasGroup.alpha = 1f;
                    target.localScale = initialScale;

                    // ������ɺ�Ļص�
                    onComplete?.Invoke();
                });

        // ���ö���
        sequence.Play();
    }

    /// <summary>
    /// �������ƶ���ָ��λ�ã����Ҷ�̬��������
    /// </summary>
    public static void MoveToPositionWithParentChange(Transform target, Transform newParent, Vector3 localTargetPosition, float duration = 0.5f, Action onComplete = null)
    {
        if (target == null || newParent == null)
        {
            Debug.LogError("Target or newParent is null!");
            return;
        }

        //Vector3 oriScale = target.localScale;
        // �ı丸��
        target.SetParent(newParent);

        //target.localScale = oriScale;
        // �ƶ����¸����µ�Ŀ��λ��
        target.DOLocalMove(localTargetPosition, duration)
              .SetEase(Ease.InQuad)
              .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// ���������б��е����壬����ӷŴ�Ч����
    /// </summary>
    /// <param name="objects">�����б�</param>
    /// <param name="fadeDuration">ÿ������Ľ���ʱ����</param>
    /// <param name="scaleDuration">ÿ������ķŴ�ʱ����</param>
    /// <param name="delayBetweenObjects">����֮��������ӳ�ʱ�䡣</param>
    /// <param name="onComplete">���������������Ļص���</param>
    public static void ShowObjectsOneByOneWithScale(List<GameObject> objects, float fadeDuration = 0.5f, float scaleDuration = 0.5f, float delayBetweenObjects = 0.1f, System.Action onComplete = null)
    {
        if (objects == null || objects.Count == 0)
        {
            Debug.LogError("Object list is empty or null.");
            return;
        }

        // ����һ�����У����ڿ��ƶ���
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];

            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

            // ���û�� CanvasGroup ��������һ��
            if (canvasGroup == null)
            {
                canvasGroup = obj.AddComponent<CanvasGroup>();
            }

            // ȷ�����忪ʼʱ��͸������С��
            canvasGroup.alpha = 0f;

            Vector3 oriScale = obj.transform.localScale;
            obj.transform.localScale = Vector3.zero; // ����С��ʼ

            // ���ԺͷŴ󶯻�
            sequence.Insert(i * delayBetweenObjects, canvasGroup.DOFade(1f, fadeDuration));
            sequence.Insert(i * delayBetweenObjects, obj.transform.DOScale(oriScale, scaleDuration).SetEase(Ease.OutBack)); // �Ŵ�����Ի���
        }

        // ���ö�����ɺ�Ļص�
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// ��UI�����ƶ�����һ��UI�����λ�ã�֧�ֿ�Canvas������
    /// </summary>
    /// <param name="movingObject">��Ҫ�ƶ������� Transform��</param>
    /// <param name="targetObject">Ŀ��λ�õ����� Transform��</param>
    /// <param name="duration">�ƶ�������ʱ����</param>
    /// <param name="onComplete">������ɺ�Ļص�����ѡ��</param>
    public static void MoveUIObjectToTarget(Transform movingObject, Transform targetObject, float duration = 0.5f, System.Action onComplete = null)
    {
        if (movingObject == null || targetObject == null)
        {
            Debug.LogError("MovingObject or TargetObject is null!");
            return;
        }

        // ��ȡĿ����������Ļ�ϵ�����
        RectTransform targetRect = targetObject as RectTransform;
        RectTransform movingRect = movingObject as RectTransform;

        if (targetRect == null || movingRect == null)
        {
            Debug.LogError("Both MovingObject and TargetObject must be RectTransforms.");
            return;
        }

        Canvas targetCanvas = targetObject.GetComponentInParent<Canvas>();
        Canvas movingCanvas = movingObject.GetComponentInParent<Canvas>();

        if (targetCanvas == null || movingCanvas == null)
        {
            Debug.LogError("Both MovingObject and TargetObject must belong to a Canvas.");
            return;
        }

        // ��ȡĿ���������Ļ����
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, targetRect.position);

        // ����Ļ����ת��Ϊ�ƶ���������Canvas�ľֲ�����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            movingRect.parent as RectTransform,
            screenPosition,
            movingCanvas.worldCamera,
            out Vector2 localPosition
        );

        // ִ���ƶ�����
        movingRect.DOLocalMove(localPosition, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// �����������ı����ݡ�
    /// </summary>
    /// <param name="text">Ҫ���µ��ı������</param>
    /// <param name="startValue">��ʼ���֡�</param>
    /// <param name="endValue">�������֡�</param>
    /// <param name="duration">��������ʱ�䣨�룩��</param>
    public static void AnimateText(Text text, float startValue, float endValue, float duration, Action onComplete = null)
    {
        if (text == null)
        {
            Debug.LogError("Text component is null. Please provide a valid Text component.");
            return;
        }

        float currentValue = startValue;

        DOTween.To(() => currentValue, x => currentValue = x, endValue, duration)
            .OnUpdate(() =>
            {
                text.text = $"{Mathf.RoundToInt(currentValue)}%";
            })
            .OnComplete(() => onComplete?.Invoke());
    }
}

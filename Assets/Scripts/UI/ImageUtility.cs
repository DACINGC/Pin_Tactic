using UnityEngine;
using UnityEngine.UI;

public static class ImageUtility
{
    /// <summary>
    /// ����ͼƬ��Sprite��ͬʱ�������С�Ա���ԭʼ���������������ߴ硣
    /// </summary>
    /// <param name="image">Ŀ��Image�����</param>
    /// <param name="sprite">Ҫ���õ�Sprite��</param>
    /// <param name="maxWidth">ͼƬ������ȡ�</param>
    /// <param name="maxHeight">ͼƬ�����߶ȡ�</param>
    public static void SetImageSpriteWithAspect(Image image, Sprite sprite, float maxWidth, float maxHeight)
    {
        if (image == null || sprite == null)
        {
            Debug.LogWarning("Image or Sprite is null!");
            return;
        }

        // ����ͼƬ��Sprite
        image.sprite = sprite;

        // ��ȡSprite��ԭʼ�ߴ�
        float spriteWidth = sprite.rect.width;
        float spriteHeight = sprite.rect.height;

        // ����ԭʼ����
        float aspectRatio = spriteWidth / spriteHeight;

        // ����Ŀ��ߴ磬���ֱ������������������
        float targetWidth = spriteWidth;
        float targetHeight = spriteHeight;

        if (targetWidth > maxWidth)
        {
            targetWidth = maxWidth;
            targetHeight = targetWidth / aspectRatio;
        }

        if (targetHeight > maxHeight)
        {
            targetHeight = maxHeight;
            targetWidth = targetHeight * aspectRatio;
        }

        // ����Image��RectTransform��С
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
    }
}

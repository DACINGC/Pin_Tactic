using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    private SpriteRenderer icesr;
    [SerializeField] private Sprite[] iceSprites;
    [Space]
    [SerializeField] private int curIndex;
    private ParticleSystem iceParticle;
    private void Awake()
    {
        icesr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        iceParticle = transform.Find("Ice Break").GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        icesr.sprite = iceSprites[0];
        iceParticle.Stop();
        iceParticle.Clear();
    }

    /// <summary>
    /// �������������Ч��
    /// </summary>
    private void PlayIceParticle()
    {
        if (iceParticle.isPlaying)
        {
            iceParticle.Stop();
            iceParticle.Clear();
        }

        iceParticle.Play();
    }

    public void SetSortingLayer(string name)
    {
        icesr.sortingLayerName = name;
        icesr.sortingOrder = 20;
    }

    /// <summary>
    /// ���Ƿ��Ѿ��ƻ���ȫ
    /// </summary>
    /// <returns></returns>
    public bool IceBreak()
    {
        //�Ѿ������һ��ͼƬ��
        bool icebreak = false;
        if (curIndex == iceSprites.Length - 1)
        {
            icesr.color = new Color(1, 1, 1, 0);
            Destroy(gameObject, 1.2f);
            //PlayIceParticle();
            icebreak = true;
        }
        else
        { 
            curIndex++;
            icesr.sprite = iceSprites[curIndex];
        }    

        PlayIceParticle();
        return icebreak;
    }
}

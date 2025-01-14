using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewHole : MonoBehaviour
{
    private Screw screw;
    private SpriteRenderer sr;
    private Collider2D cd;
    public Screw HoleScrew
    {
        get => screw;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        cd = GetComponent<Collider2D>();

    }
    public void InitScrewHole()
    {
        foreach (Transform trans in transform)
        {
            if (trans.GetComponent<Screw>())
            {
                screw = trans.GetComponent<Screw>();
                return;
            }
        }
    }

    public void SetHoleFlase()
    {
        if (cd != null)
        {
            cd.isTrigger = true;
            cd.enabled = false;
        }
    }

    public void RocketExplotion()
    {
        if (cd != null)
        {
            sr.enabled = false;
            cd.isTrigger = true;
            cd.enabled = false;
        }
    }
}

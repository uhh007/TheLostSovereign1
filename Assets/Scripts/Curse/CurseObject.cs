using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseObject
{
    public GameObject gameObject;

    public bool Works = false;
    public float TimeAfterTeleportation = 0f;
    public float TimeAfterDamage = 0f;
    public float TimeAfterStack = 0f;
    public int StacksNum = 0;


    public int TypeOfCurse;
    public int CurseEnterTime;

    public bool Invisible = false;
    public bool SeeNegativeEffect = true;

    public GameObject AntidoteObject;
    public GameObject CurseArea;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabIDOfAttribute : Attribute
{
    public Type PrefabType;

    public PrefabIDOfAttribute(Type PrefabType)
    {
        this.PrefabType = PrefabType;
    }
}

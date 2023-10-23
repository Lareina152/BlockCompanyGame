#if FISHNET

using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class UniqueNetworkBehaviour<T> : NetworkBehaviour where T : UniqueNetworkBehaviour<T>
{
    public static T instance;

    private void Awake()
    {
        instance = this as T;
    }
}

#endif

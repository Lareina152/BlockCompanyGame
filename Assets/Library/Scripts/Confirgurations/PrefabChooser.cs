using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
[Serializable]
public class PrefabChooser<TComponent> : BaseConfigClass where TComponent : Behaviour
{
    [AssetSelector(Paths = "Assets/Resources/Prefabs"), Required, AssetsOnly]
    [SerializeField]
    [HideLabel]
    private TComponent prefab;

    public TComponent GetPrefab()
    {
        return prefab;
    }

    public static implicit operator TComponent(PrefabChooser<TComponent> chooser)
    {
        return chooser.prefab;
    }
}

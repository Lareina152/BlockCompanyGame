using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
public class ContainerChooser : BaseConfigClass, IEnumerable<Transform>
{
    [LabelText("容器ID")]
    [StringIsNotNullOrEmpty]
    [SerializeField]
    private string containerID;

    [LabelText("容器")]
    [SerializeField]
    [ReadOnly]
    private Transform container;

    [LabelText("父容器")]
    public ContainerChooser parentContainer;

    [Button("测试获取容器", ButtonStyle.Box)]
    public Transform GetContainer()
    {

        if (container == null)
        {
            container = containerID.FindOrCreateObject().transform;
        }

        if (parentContainer != null)
        {
            container.SetParent(parentContainer.GetContainer());
        }

        return container;
    }

    public void SetDefaultContainerID(string defaultContainerID)
    {
        if (containerID.IsNullOrEmptyAfterTrim())
        {
            containerID = defaultContainerID;
        }
    }

    public static implicit operator Transform(ContainerChooser chooser)
    {
        return chooser.GetContainer();
    }

    public IEnumerator<Transform> GetEnumerator()
    {
        return container.Cast<Transform>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

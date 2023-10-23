using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

[AddComponentMenu("EventsCustomed/ScrollWheelEvent", 0)]
public class ScrollWheelEvent : MonoBehaviour
{
    [FoldoutGroup("滚动事件")]
    [Header("向前滚触发")]
    public UnityEvent<float> scrollInEvent;
    [FoldoutGroup("滚动事件")]
    [Header("向后滚触发")]
    public UnityEvent<float> scrollBackEvent;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            scrollInEvent.Invoke(Input.GetAxis("Mouse ScrollWheel"));
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            scrollBackEvent.Invoke(Input.GetAxis("Mouse ScrollWheel"));
        }
    }
}

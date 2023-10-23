using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBase : Selectable
{
    private SelectableEvents events;
    private SelectableAnimation buttonAnimations;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        buttonAnimations = GetComponent<SelectableAnimation>();

        if (buttonAnimations != null && transition == Transition.Animation)
        {
            transition = Transition.None;
        }
    }
#endif

    protected override void Awake()
    {
        base.Awake();

        events = GetComponent<SelectableEvents>();
        buttonAnimations = GetComponent<SelectableAnimation>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (events != null)
        {
            UISystemProfilerApi.AddMarker("Button.onPointerDown", this);
            events.onPointerDown?.Invoke();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (events != null)
        {
            UISystemProfilerApi.AddMarker("Button.onPointerUp", this);
            events.onPointerUp?.Invoke();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (events != null)
        {
            UISystemProfilerApi.AddMarker("Button.onPointerEnter", this);
            events.onPointerEnter?.Invoke();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (events != null)
        {
            UISystemProfilerApi.AddMarker("Button.onPointerExit", this);
            events.onPointerExit?.Invoke();
        }
    }
}

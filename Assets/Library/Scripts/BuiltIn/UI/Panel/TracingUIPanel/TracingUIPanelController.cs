using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TracingUIPanelController<T> : UIToolkitPanelController where T : TracingUIPanelController<T>
{
    protected static T instance;

    protected TracingUIPanelPreset tracingUIPanelPreset { get; private set; }

    protected Vector2 defaultPivot;

    protected bool overflowEnable, autoPivotCorrection;

    [ShowInInspector]
    protected VisualElement tooltipContainer;

    [ShowInInspector]
    protected Vector2 mousePositionOnOpen;

    [ShowInInspector]
    protected Vector2 screenSizeOnOpen;

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        tracingUIPanelPreset = preset as TracingUIPanelPreset;

        Note.note.AssertIsNotNull(tracingUIPanelPreset, nameof(tracingUIPanelPreset));
    }

    protected override void OnInit(UIPanelPreset preset)
    {
        base.OnInit(preset);

        if (preset.isUnique)
        {
            instance = this as T;
        }

        defaultPivot = tracingUIPanelPreset.defaultPivot;
        overflowEnable = tracingUIPanelPreset.overflowEnable;
        autoPivotCorrection = tracingUIPanelPreset.autoPivotCorrection;
    }

    protected virtual void Update()
    {
        if (preset != null && tracingUIPanelPreset.continuousMouseTracing)
        {
            UpdatePos();
        }
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        tooltipContainer = uiDocument.rootVisualElement.Q(tracingUIPanelPreset.containerVisualElementName);

        mousePositionOnOpen = Input.mousePosition.To2D();
        screenSizeOnOpen = new(Screen.width, Screen.height);
    }

    protected override void OnClose()
    {
        base.OnClose();

        tooltipContainer = null;
    }

    protected override void OnLayoutChange()
    {
        base.OnLayoutChange();

        SetPivot(defaultPivot);
        UpdatePos();
    }

    public new static void Close()
    {
        if (instance != null)
        {
            ((UIPanelController)instance).Close();
        }
    }

    public static bool IsOpen()
    {
        return instance.isOpen;
    }

    #region Caculate Position

    [Button("更新位置")]
    protected void UpdatePos()
    {
        if (isOpen == false)
        {
            return;
        }

        Vector2 mousePosition, screenSize;

        if (tracingUIPanelPreset.continuousMouseTracing)
        {
            mousePosition = Input.mousePosition.To2D();
            screenSize = new(Screen.width, Screen.height);
        }
        else
        {
            mousePosition = mousePositionOnOpen;
            screenSize = screenSizeOnOpen;
        }

        var boundsSize = uiDocument.panelSettings.referenceResolution.F();
        var position = mousePosition.Divide(screenSize).
                Multiply(boundsSize);

        var width = tooltipContainer.resolvedStyle.width;
        var height = tooltipContainer.resolvedStyle.height;

        //if (isDebugging)
        //{
        //    Debug.Log($"{nameof(mousePosition)}: {mousePosition}, " +
        //              $"screenSize: {new Vector2(Screen.width, Screen.height)}" +
        //              $"screenOrientation: {Screen.orientation}" +
        //              $"{nameof(position)}: {position}, " +
        //              $"{nameof(boundsSize)}: {boundsSize}" + 
        //              $"size:{new Vector2(width, height)}");
        //}

        if (overflowEnable == false)
        {
            position = position.Clamp(boundsSize);

            if (autoPivotCorrection)
            {
                var pivot = defaultPivot;

                if (position.x < defaultPivot.x * width)
                {
                    pivot.x = (position.x / width).ClampMin(0);
                }
                else if (position.x > boundsSize.x - (1 - defaultPivot.x) * width)
                {
                    pivot.x = (1 - (boundsSize.x - position.x) / width).ClampMax(1);
                }

                if (position.y < defaultPivot.y * height)
                {
                    pivot.y = (position.y / height).ClampMin(0);
                }
                else if (position.y > boundsSize.y - (1 - defaultPivot.y) * height)
                {
                    pivot.y = (1 - (boundsSize.y - position.y) / height).ClampMax(1);
                }

                SetPivot(pivot);
            }
        }

        if (tracingUIPanelPreset.useRightPosition)
        {
            tooltipContainer.style.right = boundsSize.x - position.x - width;
        }
        else
        {
            tooltipContainer.style.left = position.x;
        }

        if (tracingUIPanelPreset.useTopPosition)
        {
            tooltipContainer.style.top = boundsSize.y - position.y - height;
        }
        else
        {
            tooltipContainer.style.bottom = position.y;
        }
    }

    private void SetPivot(Vector2 pivot)
    {
        tooltipContainer.style.translate = new StyleTranslate(new Translate(
            new Length(-100 * pivot.x, LengthUnit.Percent),
            new Length(100 * pivot.y, LengthUnit.Percent)));
    }

    #endregion
}

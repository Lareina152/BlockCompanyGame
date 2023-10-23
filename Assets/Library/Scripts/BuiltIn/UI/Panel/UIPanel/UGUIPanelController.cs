using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

public class UGUIPanelController : UIPanelController
{
    protected GameObject visualObject { get; private set; }

    protected RectTransform rectTransform { get; private set; }

    protected UGUIPanelPreset uguiPanelPreset { get; private set; }

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        uguiPanelPreset = preset as UGUIPanelPreset;

        Note.note.AssertIsNotNull(uguiPanelPreset, nameof(uguiPanelPreset));

        rectTransform = gameObject.AddComponent<RectTransform>();

        var canvas = UIPanelManager.GetCanvas(preset.sortingOrder);

        transform.SetParent(canvas.transform);

        transform.ResetLocalArguments();

        visualObject = Instantiate(uguiPanelPreset.prefab, transform);
    }

    protected override void OnOpen()
    {
        base.OnOpen();

        visualObject.SetActive(true);
    }

    protected override void OnClose()
    {
        base.OnClose();

        visualObject.SetActive(false);
    }
}

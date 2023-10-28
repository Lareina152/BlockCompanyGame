using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameMainUIPanelController : UIToolkitPanelController
{
    public InGameMainUIPanelPreset inGameMainUIPanelPreset { get; private set; }

    private VisualElement resetButton;

    protected override void OnPreInit(UIPanelPreset preset)
    {
        base.OnPreInit(preset);

        inGameMainUIPanelPreset = preset as InGameMainUIPanelPreset;
        
        Note.note.AssertIsNotNull(inGameMainUIPanelPreset, nameof(inGameMainUIPanelPreset));

        resetButton = rootVisualElement.Q(inGameMainUIPanelPreset.resetButtonName);

        Note.note.AssertIsNotNull(resetButton, nameof(resetButton));

        resetButton.style.backgroundImage = 
            inGameMainUIPanelPreset.resetButtonReleasedIcon;

        resetButton.RegisterCallback<MouseDownEvent>(e =>
        {
            resetButton.style.backgroundImage =
                inGameMainUIPanelPreset.resetButtonPressedIcon;
        });

        resetButton.RegisterCallback<MouseUpEvent>(e =>
        {
            resetButton.style.backgroundImage =
                inGameMainUIPanelPreset.resetButtonReleasedIcon;
        });
    }
}

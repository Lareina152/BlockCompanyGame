using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class InGameMainUIPanelPreset : UIToolkitPanelPreset
{
    public const string registeredID = "in_game_main_ui";

    public override bool autoAddToPrefabList => true;

    public override Type controllerType => typeof(InGameMainUIPanelController);

    [LabelText("重置按钮名称")]
    [ValueDropdown(nameof(GetVisualTreeNames))]
    public string resetButtonName;

    [LabelText("重置按钮按下的图标")]
    [Required]
    public Texture2D resetButtonPressedIcon;

    [LabelText("重置按钮释放的图标")]
    [Required]
    public Texture2D resetButtonReleasedIcon;
}

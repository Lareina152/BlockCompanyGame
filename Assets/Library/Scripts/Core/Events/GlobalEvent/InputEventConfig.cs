using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputEventConfig : GlobalEventConfig
{
    public const string INPUT_MAPPING_SETTING_CATEGORY = "输入映射设置";

    [LabelText("需要鼠标在屏幕内才触发"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY, Expanded = false)]
    [JsonProperty]
    public bool requireMouseInScreen = true;

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        requireUpdate = true;
    }

    public override void CheckSettings()
    {
        base.CheckSettings();

        requireUpdate = true;
    }

    protected InputActionGroup AddActionGroupGUI()
    {
        return new InputActionGroup();
    }

    #endregion

    public virtual IEnumerable<string> GetInputMappingContent()
    {
        yield return "";
    }

    protected override bool CanUpdate()
    {
        if (base.CanUpdate() == false)
        {
            return false;
        }

        if (requireMouseInScreen)
        {
            if (Input.mousePosition.x.BetweenInclusive(0, Screen.width) == false ||
                Input.mousePosition.y.BetweenInclusive(0, Screen.height) == false)
            {
                return false;
            }
        }

        return true;
    }

    protected static bool CheckGroups(List<InputActionGroup> groups)
    {
        if (groups == null || groups.Count == 0)
        {
            return false;
        }

        foreach (var group in groups)
        {
            bool thisGroupResult = true;

            foreach (var action in group.actions)
            {
                switch (action.type)
                {
                    case InputType.KeyBoardOrMouseOrJoyStick:

                        switch (action.keyBoardTriggerType)
                        {
                            case KeyBoardTriggerType.KeyStay:

                                if (Input.GetKey(action.keyCode) == false)
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                break;
                            case KeyBoardTriggerType.KeyDown:

                                if (Input.GetKeyDown(action.keyCode) == false)
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                break;
                            case KeyBoardTriggerType.KeyUp:

                                if (Input.GetKeyUp(action.keyCode) == false)
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                break;
                            case KeyBoardTriggerType.OnHolding:
                                if (Input.GetKey(action.keyCode) == false)
                                {
                                    action.runtimeData.heldTime = 0;
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                action.runtimeData.heldTime += Time.deltaTime;

                                if (action.runtimeData.heldTime < action.holdThreshold)
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }
                                break;
                            case KeyBoardTriggerType.HoldDown:
                                if (Input.GetKey(action.keyCode) == false)
                                {
                                    action.runtimeData.heldTime = 0;
                                    action.runtimeData.hasTriggeredHoldDown = false;
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                action.runtimeData.heldTime += Time.deltaTime;

                                if (action.runtimeData.heldTime < action.holdThreshold)
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                if (action.runtimeData.hasTriggeredHoldDown)
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }

                                action.runtimeData.hasTriggeredHoldDown = true;
                                break;
                            case KeyBoardTriggerType.HoldAndRelease:
                                if (Input.GetKey(action.keyCode))
                                {
                                    action.runtimeData.heldTime += Time.deltaTime;
                                }

                                if (Input.GetKeyUp(action.keyCode))
                                {
                                    if (action.runtimeData.heldTime > action.holdThreshold)
                                    {
                                        action.runtimeData.heldTime = 0;
                                    }
                                    else
                                    {
                                        action.runtimeData.heldTime = 0;
                                        thisGroupResult = false;
                                        goto NextGroup;
                                    }
                                }
                                else
                                {
                                    thisGroupResult = false;
                                    goto NextGroup;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            NextGroup:

            if (thisGroupResult)
            {
                return true;
            }
        }

        return false;
    }
}

public sealed class InputEventConfigOfBoolArg : InputEventConfig
{
    [LabelText("输入动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> boolActionGroups = new();

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        boolActionGroups ??= new();
    }

    #endregion

    public override IEnumerable<string> GetInputMappingContent()
    {
        if (boolActionGroups.Count == 0)
        {
            yield return "";
            yield break;
        }

        foreach (var actionGroup in boolActionGroups)
        {
            var contentList = new List<string>();

            foreach (var action in actionGroup.actions)
            {
                contentList.Add(GameCoreSettingBase.globalEventSystemGeneralSetting.GetKeyCodeName(action.keyCode));
            }

            yield return "+".Join(contentList);
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        InvokeAction(CheckGroups(boolActionGroups));
    }
}

public sealed class InputEventConfigOfFloatArg : InputEventConfig
{
    [LabelText("是否输入传入Axis"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [JsonProperty]
    public bool isFloatFromAxis = false;

    [HideLabel, FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [ShowIf(nameof(isFloatFromAxis))]
    [JsonProperty]
    public InputAxisType floatInputAxisType;

    [LabelText("正值动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [HideIf(nameof(isFloatFromAxis))]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> floatPositiveActionGroups = new();

    [LabelText("负值动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [HideIf(nameof(isFloatFromAxis))]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> floatNegativeActionGroups = new();

    [FoldoutGroup(ONLY_FOR_DEBUGGING_CATEGORY)]
    [ReadOnly, EnableGUI, DisplayAsString, NonSerialized, ShowInInspector]
    public float floatValue = 0;

    public event Action<float> floatAction;

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        floatPositiveActionGroups ??= new();
        floatNegativeActionGroups ??= new();
    }

    #endregion

    protected override void OnInit()
    {
        base.OnInit();

        if (isDebugging)
        {
            floatAction += arg => Debug.Log($"{name}被触发:{arg}");
        }
    }

    public void InvokeAction(float arg)
    {
        floatValue = arg;

        InvokeAction(arg != 0);

        floatAction?.Invoke(arg);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        float argFloat;

        if (isFloatFromAxis)
        {
            argFloat = floatInputAxisType switch
            {
                InputAxisType.MouseWheelScroll => Input.GetAxis("Mouse ScrollWheel"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            argFloat = 0;

            if (CheckGroups(floatPositiveActionGroups))
            {
                argFloat += 1;
            }

            if (CheckGroups(floatNegativeActionGroups))
            {
                argFloat -= 1;
            }
        }

        InvokeAction(argFloat);
    }
}

public sealed class InputEventConfigOfVector2Arg : InputEventConfig
{
    [LabelText("参数模值不超过1"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [JsonProperty]
    public bool argMagnitudeLessThan1 = true;

    [LabelText("X正值动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> vector2XPositiveActionGroups = new();

    [LabelText("X负值动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> vector2XNegativeActionGroups = new();

    [LabelText("Y正值动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> vector2YPositiveActionGroups = new();

    [LabelText("Y负值动作组"), FoldoutGroup(INPUT_MAPPING_SETTING_CATEGORY)]
    [ListDrawerSettings(CustomAddFunction = nameof(AddActionGroupGUI))]
    [JsonProperty]
    public List<InputActionGroup> vector2YNegativeActionGroups = new();

    [FoldoutGroup(ONLY_FOR_DEBUGGING_CATEGORY)]
    [ReadOnly, EnableGUI, DisplayAsString, NonSerialized, ShowInInspector]
    public Vector2 vector2Value = Vector2.zero;

    public event Action<Vector2> vector2Action;

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        vector2XPositiveActionGroups ??= new();
        vector2XNegativeActionGroups ??= new();
        vector2YPositiveActionGroups ??= new();
        vector2YNegativeActionGroups ??= new();
    }

    [Button("快速添加WASD输入动作组", ButtonSizes.Medium)]
    private void AddWASDActionGroupGUI()
    {
        vector2XPositiveActionGroups.Clear();
        vector2XPositiveActionGroups.Add(new(KeyCode.D, KeyBoardTriggerType.KeyStay));
        vector2XPositiveActionGroups.Add(new(KeyCode.RightArrow, KeyBoardTriggerType.KeyStay));

        vector2XNegativeActionGroups.Clear();
        vector2XNegativeActionGroups.Add(new(KeyCode.A, KeyBoardTriggerType.KeyStay));
        vector2XNegativeActionGroups.Add(new(KeyCode.LeftArrow, KeyBoardTriggerType.KeyStay));

        vector2YPositiveActionGroups.Clear();
        vector2YPositiveActionGroups.Add(new(KeyCode.W, KeyBoardTriggerType.KeyStay));
        vector2YPositiveActionGroups.Add(new(KeyCode.UpArrow, KeyBoardTriggerType.KeyStay));

        vector2YNegativeActionGroups.Clear();
        vector2YNegativeActionGroups.Add(new(KeyCode.S, KeyBoardTriggerType.KeyStay));
        vector2YNegativeActionGroups.Add(new(KeyCode.DownArrow, KeyBoardTriggerType.KeyStay));
    }

    #endregion

    protected override void OnInit()
    {
        base.OnInit();

        if (isDebugging)
        {
            vector2Action += arg => Debug.Log($"{name}被触发:{arg}");
        }
    }

    public void InvokeAction(Vector2 arg)
    {
        if (arg == Vector2.zero)
        {
            InvokeAction(false);
        }
        else
        {
            if (argMagnitudeLessThan1 && arg.sqrMagnitude > 1)
            {
                arg = arg.normalized;
            }

            InvokeAction(true);
        }

        vector2Value = arg;

        vector2Action?.Invoke(arg);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        Vector2 argVector2 = Vector2.zero;

        if (CheckGroups(vector2XPositiveActionGroups))
        {
            argVector2.x += 1;
        }

        if (CheckGroups(vector2XNegativeActionGroups))
        {
            argVector2.x -= 1;
        }

        if (CheckGroups(vector2YPositiveActionGroups))
        {
            argVector2.y += 1;
        }

        if (CheckGroups(vector2YNegativeActionGroups))
        {
            argVector2.y -= 1;
        }

        InvokeAction(argVector2);
    }
}

public enum InputAxisType
{
    [LabelText("鼠标滚轮")]
    MouseWheelScroll
}

[Serializable]
[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
public class InputActionGroup : BaseConfigClass
{
    [LabelText("输入动作")]
    [ListDrawerSettings(ShowFoldout = false, DefaultExpandedState = true, 
        CustomAddFunction = nameof(AddInputActionToListGUI))]
    [JsonProperty]
    public List<InputAction> actions = new();

    #region GUI

    private InputAction AddInputActionToListGUI()
    {
        return new InputAction()
        {
            holdThreshold = 0.5f,
            runtimeData = new()
        };
    }

    #endregion

    public InputActionGroup()
    {
        actions.Add(new());
    }

    public InputActionGroup(KeyCode keyCode, KeyBoardTriggerType keyBoardTriggerType)
    {
        InputAction action;
        action.type = InputType.KeyBoardOrMouseOrJoyStick;
        action.keyCode = keyCode;
        action.keyBoardTriggerType = keyBoardTriggerType;
        action.holdThreshold = 0;
        action.runtimeData = new();

        actions.Add(action);
    }
}

[Serializable]
public struct InputAction
{
    [LabelText("输入种类")]
    [EnumToggleButtons]
    [OnInspectorInit(nameof(OnInspectorInit))]
    public InputType type;

    [LabelText("键盘码")]
    [ShowIf(nameof(type), InputType.KeyBoardOrMouseOrJoyStick)]
    public KeyCode keyCode;

    [LabelText("键盘触发类型")]
    [ShowIf(nameof(type), InputType.KeyBoardOrMouseOrJoyStick)]
    [EnumToggleButtons]
    public KeyBoardTriggerType keyBoardTriggerType;

    [LabelText("长按时间阈值")]
    [ShowIf(nameof(DisplayHoldThresholdGUI))]
    [MinValue(0)]
    public float holdThreshold;

    [LabelText("运行时数据")]
    [HideInEditorMode]
    public InputActionRuntimeData runtimeData;

    #region GUI

    private void OnInspectorInit()
    {
        runtimeData ??= new();
    }

    #endregion

    private bool DisplayHoldThresholdGUI()
    {
        return type == InputType.KeyBoardOrMouseOrJoyStick && 
               keyBoardTriggerType is KeyBoardTriggerType.OnHolding or KeyBoardTriggerType.HoldDown 
                   or KeyBoardTriggerType.HoldAndRelease;
    }
}

[Serializable]
public class InputActionRuntimeData : BaseConfigClass
{
    [LabelText("已经按下的时间")]
    public float heldTime = 0;

    [LabelText("是否已经触发了按压瞬间")]
    public bool hasTriggeredHoldDown = false;
}

public enum InputType
{
    [LabelText("键盘、鼠标或操纵杆")]
    KeyBoardOrMouseOrJoyStick
}

public enum KeyBoardTriggerType
{
    [LabelText("正在按压")]
    KeyStay,
    [LabelText("按下瞬间")]
    KeyDown,
    [LabelText("松开瞬间")]
    KeyUp,
    [LabelText("正在长按")]
    OnHolding,
    [LabelText("长按瞬间")]
    HoldDown,
    [LabelText("长按松开后触发")]
    HoldAndRelease
}
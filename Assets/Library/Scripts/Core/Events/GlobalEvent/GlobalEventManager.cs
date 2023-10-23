using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public sealed class GlobalEventManager : UniqueMonoBehaviour<GlobalEventManager>
{
    private static GlobalEventSystemGeneralSetting setting => GameCoreSettingBase.globalEventSystemGeneralSetting;

    [ShowInInspector]
    private static GlobalEventConfig[] updateableGlobalEventConfigs;

    [ShowInInspector]
    private static bool initialized = false;

    private void Update()
    {
        if (initialized == false)
        {
            return;
        }

        foreach (var eventConfig in updateableGlobalEventConfigs)
        {
            eventConfig.Update();
        }
    }

    public static void Init()
    {
        updateableGlobalEventConfigs = setting.GetAllPrefabs().Where(config => config.requireUpdate).ToArray();

        initialized = true;
    }

    public static void AddEvent(string mappingID, Action action)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).action += action;
    }

    public static void AddEvent(string mappingID, Action<bool> action)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).boolAction += action;
    }

    public static void AddTriggerEvent(string mappingID, Action<bool> action)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).boolTriggerAction += action;
    }

    public static void AddEvent(string mappingID, Action<float> action)
    {
        GlobalEventConfig.GetPrefabStrictly<InputEventConfigOfFloatArg>(mappingID).floatAction += action;
    }

    public static void AddEvent(string mappingID, Action<Vector2> action)
    {
        GlobalEventConfig.GetPrefabStrictly<InputEventConfigOfVector2Arg>(mappingID).vector2Action += action;
    }

    public static void RemoveEvent(string mappingID, Action action)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).action -= action;
    }

    public static void RemoveEvent(string mappingID, Action<bool> action)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).boolAction -= action;
    }

    public static void RemoveTriggerEvent(string mappingID, Action<bool> action)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).boolTriggerAction -= action;
    }

    public static void RemoveEvent(string mappingID, Action<float> action)
    {
        GlobalEventConfig.GetPrefabStrictly<InputEventConfigOfFloatArg>(mappingID).floatAction -= action;
    }

    public static void RemoveEvent(string mappingID, Action<Vector2> action)
    {
        GlobalEventConfig.GetPrefabStrictly<InputEventConfigOfVector2Arg>(mappingID).vector2Action -= action;
    }

    public static bool GetState(string mappingID)
    {
        return GlobalEventConfig.GetPrefabStrictly(mappingID).state;
    }

    public static float GetFloatValue(string mappingID)
    {
        return GlobalEventConfig.GetPrefabStrictly<InputEventConfigOfFloatArg>(mappingID).floatValue;
    }

    public static Vector2 GetVector2State(string mappingID)
    {
        return GlobalEventConfig.GetPrefabStrictly<InputEventConfigOfVector2Arg>(mappingID).vector2Value;
    }

    public static void EnableEvent(string mappingID)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).EnableEvent();
    }

    public static void DisableEvent(string mappingID)
    {
        GlobalEventConfig.GetPrefabStrictly(mappingID).DisableEvent();
    }
}

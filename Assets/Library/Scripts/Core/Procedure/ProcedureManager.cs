using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public sealed class ProcedureManager : UniqueMonoBehaviour<ProcedureManager>
{
    private static ProcedureGeneralSetting setting => GameCoreSettingBase.procedureGeneralSetting;

    [ShowInInspector]
    private static List<IManagerBehaviour> managerBehaviours = new();

    [ShowInInspector]
    [DisplayAsString]
    public static readonly HashSet<string> currentProcedureIDs = new();

    [ShowInInspector]
    [DisplayAsString]
    public static bool initialized = false;

    private static event Action<string> OnEnterProcedureEvent;
    private static event Action<string> OnLeaveProcedureEvent;

    protected override void Awake()
    {
        base.Awake();

        GameCoreSettingBase.Init();

        var allGameObjects = 
            FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (GameObject go in allGameObjects)
        {
            var behaviours = go.GetComponents<IManagerBehaviour>();

            if (behaviours.Length > 0)
            {
                managerBehaviours.AddRange(behaviours);
            }
        }

        foreach (IManagerBehaviour behaviour in managerBehaviours)
        {
            behaviour.Init();
        }

        initialized = true;

        currentProcedureIDs.Clear();
    }

    private void Start()
    {
        EnterProcedure(setting.initialProcedureID);
    }

    public static void EnterProcedure(string procedureID)
    {
        if (currentProcedureIDs.Contains(procedureID))
        {
            return;
        }

        Note.note.Log($"进入流程:{procedureID}");

        currentProcedureIDs.Add(procedureID);

        Procedure.GetPrefabStrictly(procedureID).OnEnter();

        OnEnterProcedureEvent?.Invoke(procedureID);
    }

    public static void LeaveProcedure(string procedureID)
    {
        if (currentProcedureIDs.Contains(procedureID) == false)
        {
            return;
        }

        Note.note.Log($"离开流程:{procedureID}");

        Procedure.GetPrefabStrictly(procedureID).OnLeave();

        currentProcedureIDs.Remove(procedureID);

        OnLeaveProcedureEvent?.Invoke(procedureID);
    }

    public static void AddOnEnterEvent(string procedureID, Action action)
    {
        if (currentProcedureIDs.Contains(procedureID))
        {
            action();
        }
        
        if (initialized)
        {
            Procedure.GetPrefabStrictly(procedureID).OnEnterEvent += action;
        }
        else
        {
            GameCoreSettingBase.procedureGeneralSetting.GetPrefabStrictly(procedureID).OnEnterEvent += action;
        }
    }

    public static void AddOnLeaveEvent(string procedureID, Action action)
    {
        if (initialized)
        {
            Procedure.GetPrefabStrictly(procedureID).OnLeaveEvent += action;
        }
        else
        {
            GameCoreSettingBase.procedureGeneralSetting.GetPrefabStrictly(procedureID).OnLeaveEvent += action;
        }
    }

    public static void AddOnEnterEvent(Action<string> action)
    {
        OnEnterProcedureEvent += action;
    }

    public static void AddOnLeaveEvent(Action<string> action)
    {
        OnLeaveProcedureEvent += action;
    }
}

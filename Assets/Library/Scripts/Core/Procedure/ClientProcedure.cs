using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLoadingProcedure : LoadingProcedure
{
    public const string registeredID = "client_loading_procedure";

    public override bool autoAddToPrefabList => true;

    public override void OnEnter()
    {
        base.OnEnter();

        EnterNextProcedure();
    }

    public override void OnLeave()
    {
        base.OnLeave();
    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        name = new()
        {
            { "Chinese", "客户端加载流程" },
            { "English", "Client Loading Procedure" }
        };
    }
}

public class ClientRunningProcedure : Procedure
{
    public const string registeredID = "client_running_procedure";

    public override bool autoAddToPrefabList => true;

    public override void OnEnter()
    {
        base.OnEnter();

    }

    public override void OnLeave()
    {
        base.OnLeave();


    }

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        name = new()
        {
            { "Chinese", "客户端运行流程" },
            { "English", "Client Running Procedure" }
        };
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLoadingProcedure : LoadingProcedure
{
    public const string registeredID = "server_loading_procedure";

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
            { "Chinese", "服务器加载流程" },
            { "English", "Server Loading Procedure" }
        };
    }
}

public class ServerRunningProcedure : Procedure
{
    public const string registeredID = "server_running_procedure";

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
            { "Chinese", "服务器运行流程" },
            { "English", "Server Running Procedure" }
        };
    }
}
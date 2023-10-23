#if FISHNET

using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
public class NetworkProcedureTriggerController : UniqueNetworkBehaviour<NetworkProcedureTriggerController>
{
    public override void OnStartServer()
    {
        base.OnStartServer();

        ProcedureManager.LeaveProcedure(MainMenuProcedure.registeredID);
        ProcedureManager.EnterProcedure(ServerLoadingProcedure.registeredID);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        ProcedureManager.LeaveProcedure(MainMenuProcedure.registeredID);
        ProcedureManager.EnterProcedure(ClientLoadingProcedure.registeredID);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        ProcedureManager.LeaveProcedure(ClientRunningProcedure.registeredID);

        if (IsServer == false)
        {
            ProcedureManager.EnterProcedure(MainMenuProcedure.registeredID);
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        ProcedureManager.LeaveProcedure(ServerRunningProcedure.registeredID);

        if (IsClient == false)
        {
            ProcedureManager.EnterProcedure(MainMenuProcedure.registeredID);
        }
    }
}

#endif

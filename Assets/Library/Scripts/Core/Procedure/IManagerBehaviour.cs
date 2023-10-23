using Basis;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface IManagerBehaviour
{

    public void Init()
    {
        OnInit();
        ProcedureManager.AddOnEnterEvent(OnEnterProcedure);
        ProcedureManager.AddOnLeaveEvent(OnLeaveProcedure);
    }

    protected void OnInit()
    {
        
    }

    protected void OnEnterProcedure(string procedureID)
    {

    }

    protected void OnLeaveProcedure(string procedureID)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using Basis;
using FishNet;
using FishNet.Managing.Timing;
using UnityEngine;

public class PingDebugEntry : DebugEntry
{
    public const string registeredID = "ping";

    public override bool autoAddToPrefabList => true;

    public override bool ShouldDisplay() => InstanceFinder.IsClient;

    public override string OnHandleContentUpdate()
    {
        long ping = 0;
        TimeManager tm = InstanceFinder.TimeManager;
        if (tm != null)
        {
            ping = tm.RoundTripTime;
            var deduction = (long)(tm.TickDelta * 2000d);
            
            ping = (ping - deduction).Max(1);
        }

        return ping.ToString() + "ms";
    }
}

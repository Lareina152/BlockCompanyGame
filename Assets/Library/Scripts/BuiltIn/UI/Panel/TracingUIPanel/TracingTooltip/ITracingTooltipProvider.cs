using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface ITracingTooltipProvider
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetTooltipBindGlobalEvent(out GlobalEventConfig globalEvent)
    {
        globalEvent = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetTooltipID() => GameCoreSettingBase.uiPanelGeneralSetting.universalTooltipID;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetTooltipTitle();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<(string attributeName, Func<string> attributeValueGetter, bool isStatic)> GetTooltipProperties();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetTooltipDescription();
}

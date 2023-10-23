using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using DG.Tweening;
using UnityEngine;

public class UIParticleBase : ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>
{
    protected override Vector3 GetBindTargetPosition()
    {
        return bindTarget.GetRealPositionOnScreen(tracingOffset, GameCoreSettingBase.uiParticleGeneralSetting.camera);
    }

    public static UIParticleBase Create(string id, GameObject target, Vector2 tracingOffset = default)
    {
        var prefab = GameCoreSettingBase.uiParticleGeneralSetting.GetPrefabStrictly(id);
        
        var result = (UIParticleBase)Create(prefab, target, tracingOffset);

        result.transform.SetParent(GameCoreSettingBase.uiParticleGeneralSetting.container);

        return result;
    }
}

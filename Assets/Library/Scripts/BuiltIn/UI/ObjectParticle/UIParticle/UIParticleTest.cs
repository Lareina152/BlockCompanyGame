using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using ConfigurationBasis;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class UIParticleTest : MonoBehaviour
{
    //[LabelText("所有按ID分类的激活的数值显示UI")]
    //[ShowInInspector]
    //public Dictionary<GameObject, Dictionary<string, List<ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>>>> allValueHUDs 
    //    => UIParticleBase.allValueHUDs;
    //[LabelText("数值显示UI全局对象池")]
    //[ShowInInspector]
    //public Dictionary<string, GameObjectLimitPool<ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>>> globalPools 
    //    => UIParticleBase.globalPools;
    //[LabelText("全局激活的数值显示UI")]
    //[ShowInInspector]
    //public Dictionary<string, List<ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>>> globalValueHuDs 
    //    => UIParticleBase.globalValueHUDs;

    [LabelText("测试生成点")]
    [Required, SceneObjectsOnly]
    public GameObject testSpawnPoint;

    [LabelText("数值显示ID")]
    [ValueDropdown(@"@GameCoreSettingBase.uiParticleGeneralSetting.GetPrefabNameList()")]
    public string valueHUDID;

    [LabelText("生成数量")]
    [MinValue(1)]
    public IntegerSetter amount = 20;

    [LabelText("生成间隔")]
    [MinValue(0.001)]
    public FloatSetter gapTime = 0.1f;

    [LabelText("ShortText传入的值")]
    public FloatSetter shortTextValue = new();

    [Button("生成UIParticleBase", ButtonSizes.Medium)]
    public void GenerateUIParticleBase()
    {
        amount.GetValue().DoFuncNTimes(gapTime, () => UIParticleBase.Create(valueHUDID, testSpawnPoint));
    }

    [Button("生成ShortText", ButtonSizes.Medium)]
    public void GenerateValueHUDShortText()
    {
        amount.GetValue().DoFuncNTimes(gapTime, () =>
        {
            ValueHUDShortText.Create(valueHUDID, testSpawnPoint).Set(shortTextValue);
        });
    }

    public float radius = 3;
    public float durationPerCircle = 2;

    [Button("测试生成点转圈")]
    public void Circle(Vector3 center = default)
    {
        DOTween.Kill(testSpawnPoint);
        testSpawnPoint.transform.DOCircle(center, radius, 0, durationPerCircle).SetLoops(-1).SetEase(Ease.Linear);
    }
}

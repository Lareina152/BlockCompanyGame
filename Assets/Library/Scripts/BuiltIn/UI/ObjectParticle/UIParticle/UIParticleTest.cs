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
    //[LabelText("���а�ID����ļ������ֵ��ʾUI")]
    //[ShowInInspector]
    //public Dictionary<GameObject, Dictionary<string, List<ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>>>> allValueHUDs 
    //    => UIParticleBase.allValueHUDs;
    //[LabelText("��ֵ��ʾUIȫ�ֶ����")]
    //[ShowInInspector]
    //public Dictionary<string, GameObjectLimitPool<ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>>> globalPools 
    //    => UIParticleBase.globalPools;
    //[LabelText("ȫ�ּ������ֵ��ʾUI")]
    //[ShowInInspector]
    //public Dictionary<string, List<ObjectParticleBase<UIParticlePrefab, UIParticleGeneralSetting>>> globalValueHuDs 
    //    => UIParticleBase.globalValueHUDs;

    [LabelText("�������ɵ�")]
    [Required, SceneObjectsOnly]
    public GameObject testSpawnPoint;

    [LabelText("��ֵ��ʾID")]
    [ValueDropdown(@"@GameCoreSettingBase.uiParticleGeneralSetting.GetPrefabNameList()")]
    public string valueHUDID;

    [LabelText("��������")]
    [MinValue(1)]
    public IntegerSetter amount = 20;

    [LabelText("���ɼ��")]
    [MinValue(0.001)]
    public FloatSetter gapTime = 0.1f;

    [LabelText("ShortText�����ֵ")]
    public FloatSetter shortTextValue = new();

    [Button("����UIParticleBase", ButtonSizes.Medium)]
    public void GenerateUIParticleBase()
    {
        amount.GetValue().DoFuncNTimes(gapTime, () => UIParticleBase.Create(valueHUDID, testSpawnPoint));
    }

    [Button("����ShortText", ButtonSizes.Medium)]
    public void GenerateValueHUDShortText()
    {
        amount.GetValue().DoFuncNTimes(gapTime, () =>
        {
            ValueHUDShortText.Create(valueHUDID, testSpawnPoint).Set(shortTextValue);
        });
    }

    public float radius = 3;
    public float durationPerCircle = 2;

    [Button("�������ɵ�תȦ")]
    public void Circle(Vector3 center = default)
    {
        DOTween.Kill(testSpawnPoint);
        testSpawnPoint.transform.DOCircle(center, radius, 0, durationPerCircle).SetLoops(-1).SetEase(Ease.Linear);
    }
}

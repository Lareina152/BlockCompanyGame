using System;
using Basis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIParticlePrefab : ObjectParticlePrefab<UIParticlePrefab, UIParticleGeneralSetting>
{
    protected override Type requireType => typeof(UIParticleBase);
}

[CreateAssetMenu(fileName = "UIParticleGeneralSetting", menuName = "GameConfiguration/UIParticleGeneralSetting")]
public class UIParticleGeneralSetting : ObjectParticleGeneralSetting<UIParticlePrefab, UIParticleGeneralSetting>
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "UI粒子" },
        { "English", "UI Particle" }
    };

    public override StringTranslation folderPath => GameCoreSettingBase.uiPanelGeneralSetting.fullSettingName;

    public override StringTranslation prefabName => new()
    {
        { "Chinese", "UI粒子" },
        { "English", "UI Particle" }
    };

    public override bool gameTypeAbandoned => true;

    #region Interface

    public Transform container
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_container == null)
            {
                CreateContainer();
            }
            return _container;
        }
    }

    public Camera camera
    {
        get
        {
            if (_camera == null)
            {
                FindCamera();
            }
            return _camera;
        }
    }

    #endregion

    [LabelText("容器名字")]
    public string containerName = "UIParticle Container";

    [LabelText("主相机名字")]
    public string cameraName = "Main Camera";

    [LabelText("测试生成点名字")]
    public string testSpawnPointName = "UIParticle TestSpawnPoint";

    [LabelText("容器")]
    [Required, SceneObjectsOnly, SerializeField]
    protected Transform _container;

    [LabelText("相机")]
    [Required, SceneObjectsOnly, SerializeField]
    protected Camera _camera;

    protected override void Awake()
    {
        base.Awake();

        CreateContainer();

        FindCamera();
    }

    #region GUI

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        CreateContainer();

        FindCamera();
    }

    [Button("创建容器")]
    [ShowIf(@"@_container == null")]
    private void CreateContainer()
    {
        //var uiObject = GameObjectFunc.FindOrCreateUIContainer();

        //var testSpawnPointObject = testSpawnPointName.FindOrCreateObject(uiObject);
        //testSpawnPointObject.FindOrCreateComponent<SpriteRenderer>();

        //var canvas = uiObject.transform.FindOrCreateCanvas();

        //GameObjectFunc.FindOrCreateEventSystem();

        //var containerObject = containerName.FindOrCreateObject(canvas.transform);

        //containerObject.FindOrCreateComponent<RectTransform>();

        //containerObject.RT().ResetLocalArguments();

        //var test = containerObject.FindOrCreateComponent<UIParticleTest>();
        //test.testSpawnPoint = testSpawnPointObject;

        //_container = containerObject.transform;
    }

    [Button("寻找相机")]
    [ShowIf(@"@_camera == null")]
    private void FindCamera()
    {
        if (_camera == null)
        {
            var newCameraObject = GameObject.Find(cameraName);

            if (newCameraObject != null)
            {
                _camera = newCameraObject.GetComponent<Camera>();
            }

            if (_camera == null)
            {
                _camera = FindObjectOfType<Camera>();
            }
        }
    }

    #endregion

    public override void CheckSettings()
    {
        base.CheckSettings();

        FindCamera();
    }
}

using Basis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MouseEventGeneralSetting", menuName = "GameConfiguration/MouseEventGeneralSetting")]
public class MouseEventGeneralSetting : GeneralSettingBase, IManagerCreationProvider
{
    public override StringTranslation settingName => new()
    {
        { "Chinese", "鼠标事件" },
        { "English", "MouseEvent" }
    };

    public override bool hasIcon => true;

    public override SdfIconType icon => SdfIconType.Mouse2;

    public override StringTranslation folderPath => coreCategory;

    //[LabelText("绑定相机")]
    //[SceneObjectsOnly, Required]
    //public Camera bindCamera;

    [LabelText("2D检测的射线长度")]
    [Range(0, 100)]
    public float detectDistance2D = 100;


    public IEnumerable<ManagerType> GetManagerTypes()
    {
        yield return ManagerType.EventCore;
    }

    public void HandleManagerCreation(ManagerType managerType, Transform managerContainer)
    {
        managerContainer.GetOrAddComponent<MouseEventManager>();
    }
}

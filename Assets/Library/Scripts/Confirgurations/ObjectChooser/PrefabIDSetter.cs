using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis;
using Basis.GameItem;
using ConfigurationBasis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;


[JsonObject(MemberSerialization.OptIn)]
[Serializable]
[HideDuplicateReferenceBox]
[HideReferenceObjectPicker]
public class PrefabIDSetter<TPrefab, TGeneralSetting> : 
    ObjectChooser<PrefabIDSetter<TPrefab, TGeneralSetting>.PrefabID>
where TPrefab : GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemPrefab
where TGeneralSetting : GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemGeneralSetting
{
    [Serializable]
    public struct PrefabID : ICloneable
    {
        [ValueDropdown(nameof(GetPrefabNameList))]
        [HideLabel]
        [StringIsNotNullOrEmpty]
        public string id;

        private GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemGeneralSetting GetGeneralSetting()
        {
            return GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemPrefab.GetStaticGeneralSetting();
        }

        private IEnumerable GetPrefabNameList()
        {
            return GetGeneralSetting().GetPrefabNameList();
        }

        public override string ToString()
        {
            return GetGeneralSetting()?.GetPrefabName(id);
        }

        public object Clone()
        {
            return new PrefabID() { id = id };
        }
    }

    protected override bool valueAlwaysPreviewed => true;

    public static implicit operator string(PrefabIDSetter<TPrefab, TGeneralSetting> setter)
    {
        return setter.GetValue().id;
    }

    public static implicit operator PrefabIDSetter<TPrefab, TGeneralSetting>(string id)
    {
        return new()
        {
            value = new()
            {
                id = id,
            }
        };
    }
}

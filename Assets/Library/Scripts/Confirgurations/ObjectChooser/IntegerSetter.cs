using Basis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConfigurationBasis
{
    [JsonObject(MemberSerialization.OptIn, Description = "Integer Setter")]
    [Serializable]
    public class IntegerSetter : NumberChooser<int, RangeInteger>
    {
        protected override string valueName => "整数";

        #region GUI

        #endregion

        public static implicit operator IntegerSetter(int fixedInt)
        {
            return new IntegerSetter()
            {
                isRandomValue = false,
                value = fixedInt
            };
        }
    }
}


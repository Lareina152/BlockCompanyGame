using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor.AssetImporters;

[ScriptedImporter(1, "xlsx")]
public class ExcelImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {

    }
}

#endif
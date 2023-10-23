//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Basis;
//using UnityEditor;
//using UnityEngine;

//using UnityEditor.Compilation;

//public class MustOverrideAttributeCheckHandler
//{
//    [InitializeOnLoadMethod]
//    static void RegisterCallback()
//    {
//        CompilationPipeline.compilationFinished += OnCompilationFinished;
//    }

//    private static void OnCompilationFinished(object obj)
//    {
//        Debug.Log(obj);

//        var assemblyCSharp = "Assembly-CSharp".GetAssemblyStrictly();

//        foreach (var type in assemblyCSharp.GetTypes())
//        {
//            if (type.HasMethodByAttribute<MustOverrideAttribute>())
//            {
//                Debug.Log(type.Name);
//            }

//            //if (type.HasPropertyByAttribute<MustOverrideAttribute>(BindingFlags.DeclaredOnly | BindingFlags.Public |
//            //                                                       BindingFlags.NonPublic))
//            //{
//            //    Debug.Log(type.Name);
//            //}

//            foreach (var propertyInfo in type.GetPropertyByAttribute<MustOverrideAttribute>())
//            {
//                if (propertyInfo.IsOverride())
//                {
//                    Debug.Log($"{type.Name} : {propertyInfo.Name}");
//                }
//            }
//        }
//    }
//}

//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
//public class MustOverrideAttribute : Attribute
//{
    
//}

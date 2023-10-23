using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Test;
using UnityEngine;

public class ReflectionTestBench : MonoBehaviour
{
    private A a;

    [Button("Test", ButtonStyle.Box)]
    public void Test()
    {
        a.Demo();
    }
}

namespace Test
{
    public class A
    {

        public void Demo()
        {
            
        }
    }
}

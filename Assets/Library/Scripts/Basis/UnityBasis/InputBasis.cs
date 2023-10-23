using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Basis
{
    public static class InputFunc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetWASDKeyDown()
        {
            return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                   Input.GetKeyDown(KeyCode.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetWASDKeyUp()
        {
            return Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) ||
                   Input.GetKeyUp(KeyCode.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetWASDKeyStay()
        {
            return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
                   Input.GetKey(KeyCode.A);
        }
    }
}
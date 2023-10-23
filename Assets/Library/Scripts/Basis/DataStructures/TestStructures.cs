using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;

public class TestStructures : MonoBehaviour
{
    public RangeIntegerSet rangeIntegerSet;

    void Start()
    {
        rangeIntegerSet = new RangeIntegerSet();

        foreach (var i in 30.SeveralRandomRangeInteger(0, 100))
        {
            rangeIntegerSet.AddRange(i);
        }

        Note.note.Warning(rangeIntegerSet.ToString());
    }
}

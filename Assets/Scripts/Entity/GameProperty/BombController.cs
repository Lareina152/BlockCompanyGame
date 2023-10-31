using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class BombController : GamePropertyController
{
    public Bomb bomb { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        bomb = entity as Bomb;

        Note.note.AssertIsNotNull(bomb, nameof(bomb));
    }
}

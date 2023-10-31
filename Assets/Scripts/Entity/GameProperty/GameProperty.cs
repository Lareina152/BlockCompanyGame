using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProperty : Entity, IResettable
{
    bool IResettable.isLeft { get; set; }

    bool IResettable.areaInitialized { get; set; }
}

public class GamePropertyPrefab : EntityPrefab
{
    protected static Type bindInstanceType = typeof(GameProperty);
}

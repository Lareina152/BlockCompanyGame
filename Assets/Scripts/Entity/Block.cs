using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Entity
{
    
}

public class BlockPrefab : EntityPrefab
{
    protected static Type bindInstanceType = typeof(Block);
}

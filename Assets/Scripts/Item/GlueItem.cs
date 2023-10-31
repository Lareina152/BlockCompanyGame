using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueItem : GamePropertyItem
{
    public const string registeredID = "glue_item";

    public override bool CanPlace(Player player)
    {
        if (base.CanPlace(player) == false)
        {
            return false;
        }

        var playerMovement = player.controller.GetComponent<PlayerMovementController>();

        return playerMovement.isGround;
    }
}

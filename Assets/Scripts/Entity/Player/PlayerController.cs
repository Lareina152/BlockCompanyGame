using System;
using System.Collections;
using System.Collections.Generic;
using Basis;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : EntityController
{
    [SerializeField]
    private PlayerMovementController _movementController;

    public PlayerMovementController movementController => _movementController;

    public Player player { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();

        player = entity as Player;
        
        Note.note.AssertIsNotNull(player, nameof(player));

        _movementController = GetComponent<PlayerMovementController>();

        _movementController.SetInputMapping(player.horizontalMovementInputMappingID, 
                       player.jumpInputMappingID);
    }

    
}

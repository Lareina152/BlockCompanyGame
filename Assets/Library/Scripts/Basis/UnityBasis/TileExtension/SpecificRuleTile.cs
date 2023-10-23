using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SpecificRuleTile")]
public class SpecificRuleTile : RuleTile<SpecificRuleTile.Neighbor>
{
    public class Neighbor : TilingRuleOutput.Neighbor
    {
        public const int TileGroup1 = 3;
        public const int NotTileGroup1 = 4;
    }

    [LabelText("Tile×é1")]
    public List<TileBase> tileGroup1 = new();

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        return neighbor switch
        {
            Neighbor.TileGroup1 => tile == null || tileGroup1.Contains(tile),
            Neighbor.NotTileGroup1 => tile != null && !tileGroup1.Contains(tile),
            _ => base.RuleMatch(neighbor, tile)
        };
    }
}

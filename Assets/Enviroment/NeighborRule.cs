using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A custom rule tile that defines neighbor-based rules for tile placement.
/// </summary>
[CreateAssetMenu]
public class NeighborRule : RuleTile<NeighborRule.Neighbor>
{
    /// <summary>
    /// If enabled, the tile will connect to these tiles too when the mode is set to "This".
    /// </summary>
    [Header("Advanced Tile")]
    [Tooltip("If enabled, the tile will connect to these tiles too when the mode is set to \"This\"")]
    public bool AlwaysConnect;

    /// <summary>
    /// Tiles to connect to.
    /// </summary>
    [Tooltip("Tiles to connect to")]
    public TileBase[] TilesToConnect;

    /// <summary>
    /// Check itself when the mode is set to "any".
    /// </summary>
    [Space]
    [Tooltip("Check itself when the mode is set to \"any\"")]
    public bool CheckSelf = true;

    /// <summary>
    /// Nested class defining neighbor constants.
    /// </summary>
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    /// <summary>
    /// Determines if a rule matches based on the neighbor type and tile.
    /// </summary>
    /// <param name="neighbor">The type of neighbor.</param>
    /// <param name="tile">The tile to check against.</param>
    /// <returns>True if the rule matches, otherwise false.</returns>
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Any: return CheckAny(tile);
            case Neighbor.Specified: return CheckSpecified(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    /// <summary>
    /// Checks if the given tile is the same as this tile.
    /// </summary>
    /// <param name="tile">The tile to check.</param>
    /// <returns>True if the tile is the same as this tile, otherwise false.</returns>
    private bool CheckThis(TileBase tile)
    {
        if (!AlwaysConnect) return tile == this;
        else return TilesToConnect.Contains(tile) || tile == this;
    }

    /// <summary>
    /// Checks if the given tile is not the same as this tile.
    /// </summary>
    /// <param name="tile">The tile to check.</param>
    /// <returns>True if the tile is not the same as this tile, otherwise false.</returns>
    private bool CheckNotThis(TileBase tile)
    {
        if (!AlwaysConnect) return tile != this;
        else return !TilesToConnect.Contains(tile) && tile != this;
    }

    /// <summary>
    /// Checks if the given tile is not null.
    /// </summary>
    /// <param name="tile">The tile to check.</param>
    /// <returns>True if the tile is not null, otherwise false.</returns>
    private bool CheckAny(TileBase tile)
    {
        if (CheckSelf) return tile != null;
        else return tile != null && tile != this;
    }

    /// <summary>
    /// Checks if the given tile is in the list of tiles to connect.
    /// </summary>
    /// <param name="tile">The tile to check.</param>
    /// <returns>True if the tile is in the list of tiles to connect, otherwise false.</returns>
    private bool CheckSpecified(TileBase tile)
    {
        return TilesToConnect.Contains(tile);
    }

    /// <summary>
    /// Checks if the given tile is null.
    /// </summary>
    /// <param name="tile">The tile to check.</param>
    /// <returns>True if the tile is null, otherwise false.</returns>
    private bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
}
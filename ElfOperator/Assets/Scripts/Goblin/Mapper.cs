using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BlockType
{
    Air,
    Solid,
    Spike,
    Despawn
}

public class Mapper : MonoBehaviour
{
    [SerializeField] private List<TileBase> solids;
    [SerializeField] private List<TileBase> spikes;
    [SerializeField] private List<TileBase> buttonUp;
    [SerializeField] private List<TileBase> buttonDown;
    [SerializeField] private List<TileBase> doorClosed;
    [SerializeField] private List<TileBase> doorOpen;
    
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Vector2Int offset = new Vector2Int(1, 1);
    
    private Dictionary<Vector2Int, BlockType> _blockTypes;
    private List<Vector2Int> _goblinPositions;

    public void Setup(List<Vector2Int> startingPositions)
    {
        _goblinPositions = startingPositions;
        _blockTypes = new Dictionary<Vector2Int, BlockType>();
        for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile == null) continue;
                
                var tilePosition = new Vector2Int(x, y) + offset;
                
                if (solids.Contains(tile))
                {
                    _blockTypes[tilePosition] = BlockType.Solid;
                    Debug.Log(tilePosition +  ", " + tile + BlockType.Solid);
                    continue;
                }

                if (spikes.Contains(tile))
                {
                    _blockTypes[tilePosition] = BlockType.Spike;
                    Debug.Log(tilePosition +  ", " + tile + BlockType.Spike);
                    continue;
                }
            }
        }
    }
    
    public void UpdateGoblinPosition(int index, Vector2Int goblinPosition)
    {
        _goblinPositions[index] = goblinPosition;
    }

    public bool IsSolid(Vector2Int position)
    {
        if (_goblinPositions.Contains(position))
        {
            return true;
        }
        
        _blockTypes.TryGetValue(position, out var blockType);
        return  blockType == BlockType.Solid;
    }

    public bool IsSolidSkipGoblin(Vector2Int position, int goblinIndex)
    {
        Debug.Log(position);
        if (_goblinPositions.Contains(position) && _goblinPositions.IndexOf(position) != goblinIndex)
        {
            return true;
        }
        
        _blockTypes.TryGetValue(position, out var blockType);
        return  blockType == BlockType.Solid;
    }
    
    public BlockType GetBlockType(Vector2Int position)
    {
        if (_blockTypes.ContainsKey(position))
        {
            return _blockTypes[position];
        }
        if (_goblinPositions.Contains(position))
        {
            return BlockType.Solid;
        }
        
        return BlockType.Air;
    }
}

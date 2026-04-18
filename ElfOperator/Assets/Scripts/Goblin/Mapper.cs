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
    
    private Dictionary<Vector2, BlockType> _blockTypes = new Dictionary<Vector2, BlockType>();
    
    void Start()
    {
        
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

    public BlockType GetBlockType(Vector2 position)
    {
        return _blockTypes.ContainsKey(position) ? _blockTypes[position] : BlockType.Air;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

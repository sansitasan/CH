using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MyTileMap : MonoBehaviour
{
    [SerializeField]
    private Tilemap[] _walkables;
    [SerializeField]
    private Tilemap[] _walltiles;

    private BoundsInt _size;

    public int[,] Weights { get; private set; }

    private void Awake()
    {
        int y, x;
        Vector3Int pos, size;
        foreach (var tilemap in _walkables)
        {
            tilemap.CompressBounds();
        }

        _size = _walkables[0].cellBounds;
        Weights = new int[_size.size.y, _size.size.x];

        for (y = 0; y < _size.size.y; ++y)
            for (x = 0; x < _size.size.x; ++x)
                Weights[y, x] = 1;

        foreach (var tilemap in _walltiles)
        {
            tilemap.CompressBounds();
            pos = tilemap.cellBounds.position;
            size = tilemap.cellBounds.size;

            var tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
            for (y = 0; y < size.y; ++y)
            {
                for (x = 0; x < size.x; ++x)
                {
                    var tile = tiles[x + y * size.x];
                    if (tile != null)
                    {
                        Weights[y + pos.y - _size.position.y, x + pos.x - _size.position.x] = ushort.MaxValue;
                    }
                }
            }
        }
    }
}

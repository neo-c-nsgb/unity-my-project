// Assets/Scripts/ShapeGenerator.cs
using System.Collections.Generic;
using UnityEngine;

public static class ShapeGenerator
{
    private static System.Random rng = new System.Random();
    private const int BoxSize = 4;

    /// <summary>
    /// Returns a list of 4 connected cells (x,y in [0,BoxSize)) that form a random tetromino.
    /// </summary>
    public static List<Vector2Int> RandomTetromino()
    {
        var shape = new List<Vector2Int>();
        var occupied = new HashSet<Vector2Int>();

        // seed in box
        var start = new Vector2Int(rng.Next(BoxSize), rng.Next(BoxSize));
        shape.Add(start);
        occupied.Add(start);

        while (shape.Count < BoxSize)
        {
            var candidates = new List<Vector2Int>();
            foreach (var c in shape)
            {
                foreach (var d in new[] {
                    Vector2Int.up, Vector2Int.down,
                    Vector2Int.left, Vector2Int.right })
                {
                    var nb = c + d;
                    if (nb.x >= 0 && nb.x < BoxSize &&
                        nb.y >= 0 && nb.y < BoxSize &&
                        occupied.Add(nb)) // tries add, returns true if new
                    {
                        candidates.Add(nb);
                    }
                }
            }
            if (candidates.Count == 0) break;
            var next = candidates[rng.Next(candidates.Count)];
            shape.Add(next);
            // occupied already contains next
        }

        return shape;
    }
}
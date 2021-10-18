using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetBounds
{
    // Cache
    private Bounds bounds;

    public TargetBounds(Bounds b)
    {
        bounds = b;
    }

    public Rect FocusOnBounds(Camera cam)
    {
        Vector3 c = bounds.center;
        Vector3 e = bounds.extents;

        // Get all 8 points of the renderer's box in 3D space
        Vector3[] worldCorners = new[] {
            new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
        };

        // Convert all 8 points to screen space and get the max and min corners of the box
        // Use these 4 points as the bounds for the 2D box shown in the screen
        IEnumerable<Vector3> screenCorners = worldCorners.Select(corner => cam.WorldToScreenPoint(corner));
        float maxX = screenCorners.Max(corner => corner.x);     // Right
        float minX = screenCorners.Min(corner => corner.x);     // Left
        float maxY = screenCorners.Max(corner => corner.y);     // Top
        float minY = screenCorners.Min(corner => corner.y);     // Bottom

        /* RectTransform Position */
        Rect newRect = Rect.MinMaxRect(minX, minY, maxX, maxY);
        return newRect;
    }

}
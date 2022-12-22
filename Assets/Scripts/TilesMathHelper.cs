using System;
using System.Collections.Generic;
using UnityEngine;

public static class TilesMathHelper
{
    private readonly struct LineInfo
    {
        private readonly double _a;
        private readonly double _b;
        private readonly double _c;

        public LineInfo(Vector2 dot1, Vector2 dot2)
        {
            var diffX = dot2.x - dot1.x;
            var diffY = dot2.y - dot1.y;
            if (diffX == 0)
            {
                _a = 1;
                _b = 0;
                _c = -dot1.x;
            }
            else
            {
                _a = diffY;
                _b = -diffX;
                _c = -dot1.x * dot2.y + dot2.x * dot1.y;
            }
        }

        public bool TryGetIntersectPoint(in LineInfo otherLine, out Vector2 intersectPoint)
        {
            if (IsParallel(in otherLine))
            {
                intersectPoint = default;
                return false;
            }

            var x = (_b * otherLine._c - _c * otherLine._b) / (_a * otherLine._b - _b * otherLine._a);
            var y = _b != 0
                ? GetY(in this, x)
                : GetY(in otherLine, x);

            intersectPoint = new Vector2((float) x, (float) y);
            return true;

            static double GetY(in LineInfo line, double x)
            {
                return - (line._a * x + line._c) / line._b;
            }
        }

        public bool IsParallel(in LineInfo other)
        {
            return _a == other._a && _b == other._b;
        }

        public override string ToString()
        {
            return $"{_a}x + {_b}y + {_c} = 0";
        }
    }

    private static readonly List<Vector2Int> ResultBuffer = new(100);
    public static IReadOnlyList<Vector2Int> GetIntersectedTiles(Vector2Int startPos, Vector2Int endPos)
    {
        ResultBuffer.Clear();

        var diff = endPos - startPos;
        var line = new LineInfo(startPos, endPos);

        var stepX = (int)Mathf.Sign(diff.x);
        var stepY = (int)Mathf.Sign(diff.y);
        var borderX = diff.x + stepX;
        var borderY = diff.y + stepY;
        for (int i = 0; i != borderY; i += stepY)
        {
            for (int j = 0; j != borderX; j += stepX)
            {
                var posToCheck = startPos + new Vector2Int(j, i);
                if (IsTileIntersected(posToCheck, in line))
                {
                    ResultBuffer.Add(posToCheck);
                }
            }
        }

        return ResultBuffer;
    }

    private static bool IsTileIntersected(Vector2Int tilePos, in LineInfo line)
    {
        var upperLeftCorner = new Vector2(tilePos.x - 0.5f, tilePos.y + 0.5f);
        var upperRightCorner = new Vector2(tilePos.x + 0.5f, tilePos.y + 0.5f);
        var bottomLeftCorner = new Vector2(tilePos.x - 0.5f, tilePos.y - 0.5f);
        var bottomRightCorner = new Vector2(tilePos.x + 0.5f, tilePos.y - 0.5f);
        return IsSegmentIntersected(bottomLeftCorner, upperLeftCorner, in line)
               || IsSegmentIntersected(upperLeftCorner, upperRightCorner, in line)
               || IsSegmentIntersected(upperRightCorner, bottomRightCorner, in line)
               || IsSegmentIntersected(bottomRightCorner, bottomLeftCorner, in line);
    }

    private static bool IsSegmentIntersected(Vector2 startPos, Vector2 endPos, in LineInfo line)
    {
        var segmentLine = new LineInfo(startPos, endPos);
        if (!line.TryGetIntersectPoint(segmentLine, out var intersectPoint))
        {
            return false;
        }

        var minX = Math.Min(startPos.x, endPos.x);
        var minY = Math.Min(startPos.y, endPos.y);
        var maxX = Math.Max(startPos.x, endPos.x);
        var maxY = Math.Max(startPos.y, endPos.y);
        var checkX1 = minX <= intersectPoint.x;
        var checkX2 = intersectPoint.x <= maxX;
        var checkX = (checkX1 && checkX2);
        var checkY1 = minY <= intersectPoint.y;
        var checkY2 = intersectPoint.y <= maxY;
        var checkY = (checkY1 && checkY2);
        var isSegmentIntersect = checkX
                                 && checkY;

        return isSegmentIntersect;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public static class TilesMathHelper
{
    public readonly struct LineInfo
    {
        public Vector2 NormalVector => new((float) _a, (float) _b);
        
        private readonly double _a;
        private readonly double _b;
        private readonly double _c;

        public static LineInfo BuildFromTwoPoints(Vector2 point1, Vector2 point2)
        {
            return new LineInfo(point1, point2, ' ');
        }

        public static LineInfo BuildFromNormalAndPoint(Vector2 normalVector, Vector2 point)
        {
            return new LineInfo(normalVector, point, 0);
        }

        private LineInfo(Vector2 normalVector, Vector2 point, int _)
        : this(normalVector.x, normalVector.y, point)
        { }

        public LineInfo(Vector2 point1, Vector2 point2, char _) : this(
            point2.x - point1.x,
            point2.y - point1.y,
            point1)
        { }

        private LineInfo(double diffX, double diffY, Vector2 point)
        {
            if (diffX == 0)
            {
                _a = 1;
                _b = 0;
                _c = -point.x;
            }
            else
            {
                _a = Math.Round(diffY, 6);
                _b = Math.Round(-diffX, 6);
                _c = Math.Round(-diffY * point.x + diffX * point.y, 6);
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
            var aCft = _a / other._a;
            var bCft = _b / other._b;
            return Math.Abs(aCft - bCft) < 0.00001;
        }

        public override string ToString()
        {
            return $"{_a}x + {_b}y + {_c} = 0";
        }
    }

    private static readonly List<Vector2Int> ResultBuffer = new(100);
    public static IReadOnlyList<Vector2Int> GetIntersectedTiles(Vector2Int startPos, Vector2Int endPos,
        int lineWidth = 0)
    {
        ResultBuffer.Clear();

        var diff = endPos - startPos;
        var line = LineInfo.BuildFromTwoPoints(startPos, endPos);

        var stepX = (int)Mathf.Sign(diff.x);
        var stepY = (int)Mathf.Sign(diff.y);
        var borderX = diff.x + stepX;
        var borderY = diff.y + stepY;
        var lines = GetLinesToProcess(startPos, endPos, lineWidth, line);
        var processInfo = new ProcessInfo(
            lines
        );
        for (int i = 0; i != borderY; i += stepY)
        {
            for (int j = 0; j != borderX; j += stepX)
            {
                var posToCheck = startPos + new Vector2Int(j, i);
                ProcessPosition(processInfo, posToCheck);
            }
        }

        return ResultBuffer;
    }

    private static List<LineInfo> GetLinesToProcess(Vector2Int startPos, Vector2Int endPos, int lineWidth, LineInfo line)
    {
        var lines = new List<LineInfo>(lineWidth * 2);
        if (lineWidth == 0)
        {
            lines.Add(line);
        }
        else
        {
            lines.Add(line);
            var normalVector = line.NormalVector.normalized;

            const double lenStep = 0.5d;
            for (var normalLen = lenStep; normalLen <= lineWidth / 2d; normalLen += lenStep)
            {
                var currNormalVec = normalVector * (float) normalLen;
                var lineInfo = GetShiftedLine(startPos, endPos, currNormalVec);
                lines.Add(lineInfo);

                currNormalVec = normalVector * (float) -normalLen;
                lineInfo = GetShiftedLine(startPos, endPos, currNormalVec);
                lines.Add(lineInfo);
            }
        }

        return lines;
    }

    private static LineInfo GetShiftedLine(Vector2Int startPos, Vector2Int endPos, Vector2 shift)
    {
        var newStartPos = startPos + shift;
        var newEndPos = endPos + shift;
        var lineInfo = LineInfo.BuildFromTwoPoints(newStartPos, newEndPos);
        return lineInfo;
    }

    private readonly ref struct ProcessInfo
    {
        public readonly List<LineInfo> Lines;

        public ProcessInfo(
            List<LineInfo> lines
        )
        {
            Lines = lines;
        }
    }

    private static void ProcessPosition(in ProcessInfo processInfo, Vector2Int posToCheck)
    {
        foreach (var lineInfo in processInfo.Lines)
        {
            if (IsTileIntersected(posToCheck, in lineInfo))
            {
                ResultBuffer.Add(posToCheck);
                break;
            }
        }
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
        var segmentLine = LineInfo.BuildFromTwoPoints(startPos, endPos);
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
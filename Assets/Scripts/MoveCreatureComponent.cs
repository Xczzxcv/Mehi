using System.Collections.Generic;
using UnityEngine;

namespace Ecs.Components
{
public struct MoveCreatureComponent
{
    public struct PathPart
    {
        public Vector2Int DestPos;
    }

    public Queue<PathPart> Path;
}
}
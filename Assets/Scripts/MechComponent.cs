using System.Collections.Generic;

namespace Ecs.Components
{
public struct MechComponent
{
    public List<string> WeaponIds;
    public int RightHandsAmount;
    public int LeftHandsAmount;
    public int RightLegsAmount;
    public int LeftLegsAmount;
}
}
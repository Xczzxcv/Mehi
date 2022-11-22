using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;

public struct MechSystemComponent
{
    public MechSystemType Type;
    public int Level;
    public bool IsActive;
    public EcsPackedEntity MechEntity;

    public static bool IsWeaponHandlingSystem(MechSystemType type)
    {
        switch (type)
        {
            case MechSystemType.LeftHandSystem:
            case MechSystemType.RightHandSystem:
                return true;
            default:
                return false;
        }
    }

    public static bool CanMechMove(List<MechSystemType> mechSystems)
    {
        var leftLegsCount = mechSystems.Count(systemType => systemType == MechSystemType.LeftLegSystem);
        var rightLegsCount = mechSystems.Count(systemType => systemType == MechSystemType.RightLegSystem);
        
        return leftLegsCount > 0 && rightLegsCount > 0;
    }
}
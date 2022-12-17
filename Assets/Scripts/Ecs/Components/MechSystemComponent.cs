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
}
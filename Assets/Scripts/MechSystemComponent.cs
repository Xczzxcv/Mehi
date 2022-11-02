using Leopotam.EcsLite;

public struct MechSystemComponent
{
    public MechSystemType Type;
    public int Level;
    public bool IsActive;
    public EcsPackedEntity MechEntity;
}
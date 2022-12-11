using System.Linq;
using Ecs.Components;
using UnityEngine;

public class UseWeaponManager
{
    private bool _useWeaponModeActive;
    private int _weaponUserEntity;
    private BattleMechManager.WeaponInfo _usedWeaponInfo;
    private InputWeaponTarget _weaponTarget;

    private readonly BattleManager _battleManager;

    public UseWeaponManager(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }
    
    public void Init()
    {
        GlobalEventManager.BattleField.UseWeaponBtnClicked.Event += OnUseWeaponBtnClicked;
    }
    
    private void OnUseWeaponBtnClicked(int unitEntity, BattleMechManager.WeaponInfo weaponInfo)
    {
        _weaponUserEntity = unitEntity;
        _usedWeaponInfo = weaponInfo;
        _useWeaponModeActive = true;
        _weaponTarget = InputWeaponTarget.BuildFromTargetType(_usedWeaponInfo.WeaponTarget.TargetType);
    }

    private bool CanSelectTargetForAttackBase(WeaponTargetType targetType)
    {
        if (!_useWeaponModeActive)
        {
            return false;
        }

        if (_usedWeaponInfo.WeaponTarget.TargetType != targetType)
        {
            return false;
        }

        return true;
    }

    private bool CanSelectTargetForAttack(WeaponTargetType targetType, 
        BattleMechManager.BattleUnitInfo battleUnitInfo)
    {
        if (!CanSelectTargetForAttackBase(targetType))
        {
            return false;
        }
        
        var attackerInfo = _battleManager.GetBattleUnitInfo(_weaponUserEntity);
        var distanceToTarget = Vector2Int.Distance(attackerInfo.Position, battleUnitInfo.Position);
        if (distanceToTarget > _usedWeaponInfo.UseDistance)
        {
            return false;
        }
        
        var attackerControl = attackerInfo.ControlledBy;
        var victimControl = battleUnitInfo.ControlledBy;
        return BattleMechManager.CanAttack(attackerControl, victimControl);
    }

    public bool CanSelectUnitRoomAsTargetForAttack(BattleMechManager.BattleUnitInfo battleUnitInfo)
    {
        return CanSelectTargetForAttack(WeaponTargetType.Rooms, battleUnitInfo);
    }

    public void ProcessUnitRoomAsTargetForAttack(int roomEntity)
    {
        bool selectedAsTarget;
        if (_weaponTarget.TargetMechRoomEntities.Contains(roomEntity))
        {
            _weaponTarget.DelTargetRoom(roomEntity);
            selectedAsTarget = false;
        }
        else
        {
            _weaponTarget.AddTargetRoom(roomEntity);
            selectedAsTarget = true;
        }
        
        GlobalEventManager.BattleField.RoomSelectedAsWeaponTarget.HappenedWith(roomEntity, selectedAsTarget);
    }

    public bool TrySelectUnitBodyAsTargetForAttack(BattleMechManager.BattleUnitInfo battleUnitInfo)
    {
        if (!CanSelectTargetForAttack(WeaponTargetType.Unit, battleUnitInfo))
        {
            return false;
        }

        bool selected;
        var targetUnitEntity = battleUnitInfo.Entity;
        if (_weaponTarget.TargetMechEntities.Contains(targetUnitEntity))
        {
            _weaponTarget.DelTargetUnit(targetUnitEntity);
            selected = false;
        }
        else
        {
            _weaponTarget.AddTargetUnit(targetUnitEntity);
            selected = true;
        }

        GlobalEventManager.BattleField.UnitSelectedAsWeaponTarget.HappenedWith(targetUnitEntity, selected);
        return true;
    }

    public bool TryProcessTileAsTargetForAttack(Vector2Int targetTilePos)
    {
        if (!CanSelectTargetForAttackBase(WeaponTargetType.BattleFieldTiles))
        {
            return false;
        }

        var attackerInfo = _battleManager.GetBattleUnitInfo(_weaponUserEntity);
        var distanceToTarget = Vector2Int.Distance(attackerInfo.Position, targetTilePos);
        if (distanceToTarget > _usedWeaponInfo.UseDistance)
        {
            return false;
        }

        bool selected;
        if (_weaponTarget.TargetTiles.Contains(targetTilePos))
        {
            _weaponTarget.DelTargetTile(targetTilePos);
            selected = false;
        }
        else
        {
            _weaponTarget.AddTargetTile(targetTilePos);
            selected = true;
        }

        GlobalEventManager.BattleField.TileSelectedAsWeaponTarget.HappenedWith(targetTilePos, selected);
        return true;
    }

    public void BuildUseWeaponOrder()
    {
        _battleManager.BuildUseWeaponOrder(
            _weaponUserEntity,
            _usedWeaponInfo,
            _weaponTarget
        );

        Reset();
    }

    private void Reset()
    {
        _weaponUserEntity = -1;
        _usedWeaponInfo = default;
        _useWeaponModeActive = false;
        _weaponTarget = default;
    }

    public bool CanConfirmWeaponTargets()
    {
        return CheckTargetsCount();
    }

    private bool CheckTargetsCount()
    {
        int selectedTargetsCount;
        int maxTargetsCount;
        switch (_usedWeaponInfo.WeaponTarget.TargetType)
        {
            case WeaponTargetType.Rooms:
                selectedTargetsCount = _weaponTarget.TargetMechRoomEntities.Count;
                var roomsSelectionSize = _usedWeaponInfo.WeaponTarget.RoomTargetsSize;
                maxTargetsCount = roomsSelectionSize.x * roomsSelectionSize.y;
                break;
            case WeaponTargetType.Unit:
                selectedTargetsCount = _weaponTarget.TargetMechEntities.Count;
                maxTargetsCount = _usedWeaponInfo.WeaponTarget.UnitTargetsCount;
                break;
            case WeaponTargetType.BattleFieldTiles:
                selectedTargetsCount = _weaponTarget.TargetTiles.Count;
                var tilesSelectionSize = _usedWeaponInfo.WeaponTarget.TileTargetsSize;
                maxTargetsCount = tilesSelectionSize.x * tilesSelectionSize.y;
                break;
            case WeaponTargetType.NonTargeted:
                return true;
            default:
                selectedTargetsCount = 0;
                maxTargetsCount = 0;
                Debug.LogError(
                    $"[{nameof(UseWeaponManager)}] Wrong used weapon target type {_usedWeaponInfo.WeaponTarget.TargetType}");
                break;
        }

        return selectedTargetsCount > 0 && selectedTargetsCount <= maxTargetsCount;
    }
}
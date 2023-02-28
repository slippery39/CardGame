﻿using System.Collections.Generic;

//NOTE - Unity does not support null in serialization. If we want things to be serialized, we should use the null object pattern instead.
//Don't use System.Serializable unless we are 100% sure nothing will be set to null.
public class Lane : CardGameEntity, IZone, IDeepCloneable<Lane>
{        
    private CardInstance _unitInLane;
    #region Public Properties
    public CardInstance UnitInLane { get { return _unitInLane; } set { _unitInLane = value; } }
    public int LaneId { get; set; }
    public int LaneIndex { get; set; }

    public List<CardInstance> Cards
    {
        get
        {
            if (_unitInLane == null)
            {
                return new List<CardInstance>() { };
            }
            else
            {
                return new List<CardInstance>() { UnitInLane };
            }
        }
    }

    public override string Name => "Lane";
    public ZoneType ZoneType => ZoneType.InPlay;
    #endregion

    public Lane()
    {
        _unitInLane = null;
    }

    #region Public Methods

    public Lane DeepClone(CardGame cardGame)
    {
        Lane clone = this.Clone();
        clone.EntityId = EntityId;
        clone.ContinuousEffects = ContinuousEffects.Clone();
        clone.Modifications = Modifications.Clone();
        clone.EntityId = EntityId;
        clone.Name = Name;

        if (_unitInLane != null)
        {
            clone.UnitInLane = _unitInLane.DeepClone(cardGame);
        }

        return clone;

    }

    public bool CanBattle()
    {
        if (UnitInLane == null)
        {
            return false;
        }
        //Note : We should possibly have all our objects contain a convenience reference to a card game.
        return UnitInLane.CardGame.ActivePlayerId == UnitInLane.OwnerId && UnitInLane.CardGame.BattleSystem.CanBattle(LaneIndex);
    }

    public bool IsEmpty()
    {
        //return _isEmpty;
        return _unitInLane == null;
    }

    public void RemoveUnitFromLane()
    {
        //_isEmpty = true;
        _unitInLane = null;
    }

    public void Add(CardInstance card)
    {
        _unitInLane = card;
    }

    public void Remove(CardInstance card)
    {
        if (_unitInLane != card)
        {
            //Debug.LogError("Trying to remove a card that does not exist in lane");
        }
        else
        {
            _unitInLane = null;
        }
    }
    #endregion
}
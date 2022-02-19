using UnityEngine;

[System.Serializable]
public class Lane
{

    private CardInstance? _unitInLane;

    #region Public Properties
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public CardInstance? UnitInLane { get { return _unitInLane; } set { _unitInLane = value; } }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public int LaneId { get; set; }
    #endregion

    #region Public Methods

    public bool IsEmpty()
    {        
        return _unitInLane == null;
    }

    public void RemoveUnitFromLane()
    {
        _unitInLane = null;
    }
    #endregion
}
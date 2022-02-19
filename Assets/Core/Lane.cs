using UnityEngine;

[System.Serializable]
public class Lane
{

#nullable enable
    private CardInstance? _unitInLane;

    #region Public Properties
    public CardInstance? UnitInLane { get { return _unitInLane; } set { _unitInLane = value; } }
#nullable restore
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
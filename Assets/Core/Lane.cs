using UnityEngine;

[System.Serializable]
public class Lane
{
    private CardInstance _unitInLane;
    private bool _isEmpty = true; //fixes the bug where unity thinks there is something in here even when it should be null? not sure why it is doing that.
    //at some point, but not right after initialization, _unitInLane gets replaced with a dummy CardInstance with null data.
    //this breaks the IsEmpty() method as it checks for null.
    //need to find out why that is still. or perhaps just avoid using nulls and use the null object pattern.
    #region Public Properties
    public CardInstance UnitInLane { get { return _unitInLane; } set { _unitInLane = value; _isEmpty = false; } }
    public int LaneId { get; set; }
    #endregion

    public Lane()
    {
        _unitInLane = null;
    }

    #region Public Methods

    public bool IsEmpty()
    {
        return _isEmpty;
        //return _unitInLane == null;
    }

    public void RemoveUnitFromLane()
    {
        _isEmpty = true;
        _unitInLane = null;
    }
    #endregion
}
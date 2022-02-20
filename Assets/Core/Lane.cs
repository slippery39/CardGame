using UnityEngine;

//NOTE - Unity does not support null in serialization. If we want things to be serialized, we should use the null object pattern instead.
//Don't use System.Serializable unless we are 100% sure nothing will be set to null.
public class Lane
{
    private CardInstance _unitInLane;
    //private bool _isEmpty = true; //fixes the bug where unity thinks there is something in here even when it should be null? not sure why it is doing that.
    //at some point, but not right after initialization, _unitInLane gets replaced with a dummy CardInstance with null data.
    //this breaks the IsEmpty() method as it checks for null.
    //need to find out why that is still. or perhaps just avoid using nulls and use the null object pattern.
    #region Public Properties
    public CardInstance UnitInLane { get { return _unitInLane; } set { _unitInLane = value;} }
    public int LaneId { get; set; }
    #endregion

    public Lane()
    {
        _unitInLane = null;
    }

    #region Public Methods

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
    #endregion
}
using System.Collections.Generic;
using UnityEngine;

public class Lanes3D : MonoBehaviour
{
    [SerializeField] List<Lane3D> _lanes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Lane3D> GetLanes()
    {
        return _lanes;
    }
}

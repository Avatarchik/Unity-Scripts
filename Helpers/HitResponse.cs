using UnityEngine;

public class HitResponse
{
    public HitResponse(bool hasHit, RaycastHit rayHit)
    {
        HasHit = hasHit;
        RayHit = rayHit;
    }

    public bool HasHit { get; set; }
    public RaycastHit RayHit { get; set; }
}

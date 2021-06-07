using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Capturable : MonoBehaviour
{
    // Info for Item Photo Metadata
    public string Name;
    public CapturableType CapturableType;
    public PhotoImportance Importance;

    public bool IsVisibleOnCameraPlanes(IList<Plane> planes)
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        if (GeometryUtility.TestPlanesAABB(planes.ToArray(), bounds))
        {
            Importance = new PhotoImportance();

            float leastDistance = planes.ElementAt(0).GetDistanceToPoint(bounds.center);
            int indexOfClosestPlane = 0;
            for(int i = 0; i < planes.Count(); ++i)
            {
                float distance = planes.ElementAt(i).GetDistanceToPoint(bounds.center);
                if (distance < 0) distance = distance * -1;
                if(distance < leastDistance)
                {
                    leastDistance = distance;
                    indexOfClosestPlane = i;
                }
            }

            Importance.IndexOfClosestPlane = indexOfClosestPlane;
            Importance.DistanceFromPlaneToObjectCenter = leastDistance;

            return true;
        }
        else
        {
            return false;
        }
    }
}

public class PhotoImportance
{
    public int IndexOfClosestPlane { get; set; }
    public float DistanceFromPlaneToObjectCenter { get; set; }

}
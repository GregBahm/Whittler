using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfIntersectionTest : MonoBehaviour
{
    public Transform PlaneA;
    public Transform PlaneB;
    
    struct PlanePlaneIntersection
    {
        public static readonly PlanePlaneIntersection NoIntersection = new PlanePlaneIntersection(false, Vector3.zero, Vector3.zero);
             
        public readonly bool PlanesIntersect;
        public readonly Vector3 PointOnLine;
        public readonly Vector3 NormalOfLine;

        public PlanePlaneIntersection(bool planesIntersect, Vector3 pointOnLine, Vector3 normalOfLine)
        {
            PlanesIntersect = planesIntersect;
            PointOnLine = pointOnLine;
            NormalOfLine = normalOfLine;
        }
    }
    
    PlanePlaneIntersection GetIntersectionBetweenTwoPlanes(
            Plane p1, Plane p2)
    {
        Vector3 normalOfLine = Vector3.Cross(p1.normal, p2.normal);
        float determinant = normalOfLine.sqrMagnitude;

        if (determinant > float.Epsilon)
        {
            Vector3 crossA = Vector3.Cross(normalOfLine, p2.normal);
            Vector3 crossB = Vector3.Cross(p1.normal, normalOfLine);
            Vector3 pointOnLine = ((crossA * p1.distance) + (crossB * p2.distance)) / determinant;
            return new PlanePlaneIntersection(true, pointOnLine, normalOfLine);
        }
        else
        {
            return PlanePlaneIntersection.NoIntersection;
        }
    }

	void Update ()
    {
        Plane planeA = new Plane(PlaneA.forward, PlaneA.position);
        Plane planeB = new Plane(PlaneB.forward, PlaneB.position);
        PlanePlaneIntersection intersectionResult = GetIntersectionBetweenTwoPlanes(planeA, planeB);
        if(intersectionResult.PlanesIntersect)
        {
            Debug.DrawRay(intersectionResult.PointOnLine, intersectionResult.NormalOfLine, Color.red);
        }
    }
}

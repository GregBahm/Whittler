using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedLineOfIntersectionTest : MonoBehaviour
{
    public Transform TriPointA1;
    public Transform TriPointA2;
    public Transform TriPointA3;

    public Transform TriPointB1;
    public Transform TriPointB2;
    public Transform TriPointB3;

    struct MyPlane
    {
        public Vector3 normal;
        public float distance;

        public MyPlane(Vector3 normal, float distance)
        {
            this.normal = normal;
            this.distance = distance;
        }
    }

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
            MyPlane p1, MyPlane p2)
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

    private static MyPlane PlaneFromThreePoints(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 dir = Vector3.Cross(pointB - pointA, pointC - pointA);
        Vector3 norm = dir.normalized;
        float dist = Vector3.Dot(norm, -pointA);
        return new MyPlane(norm, dist);
    }

	void Update ()
    {
        MyPlane planeA = PlaneFromThreePoints(TriPointA1.position, TriPointA2.position, TriPointA3.position);
        MyPlane planeB = PlaneFromThreePoints(TriPointB1.position, TriPointB2.position, TriPointB3.position);
        PlanePlaneIntersection intersectionResult = GetIntersectionBetweenTwoPlanes(planeA, planeB);
        if(intersectionResult.PlanesIntersect)
        {
            Debug.DrawRay(intersectionResult.PointOnLine, intersectionResult.NormalOfLine, Color.red);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatLinesTestScript : MonoBehaviour
{
    public Transform PlaneTestProjector;
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

    private static PlanePlaneIntersection GetIntersectionBetweenTwoPlanes(
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

    private static Vector3 DoTheThing(MyPlane trianglePlane, Vector3 triEdgeStart, Vector3 triEdgeEnd, PlanePlaneIntersection lineOfIntersection)
    {
        Vector3 u;
        Vector3 uBasis = new Vector3(trianglePlane.normal.y, -trianglePlane.normal.x, 0);
        if(uBasis.sqrMagnitude < float.Epsilon)
        {
            u = new Vector3(0, 0, 1);
        }
        else
        {
            u = uBasis.normalized;
        }
        Vector3 v = Vector3.Cross(trianglePlane.normal, u);

        Vector2 flatEdgeStart = FlattenPoint(u, v, triEdgeStart);
        Vector2 flatEdgeEnd = FlattenPoint(u, v, triEdgeEnd);
        Vector2 flatIntersectStart = FlattenPoint(u, v, lineOfIntersection.PointOnLine);
        Vector2 flatIntersectEnd = FlattenPoint(u, v, lineOfIntersection.PointOnLine + lineOfIntersection.NormalOfLine);
        Vector2 flatRet = Vector2.zero;

        bool intersects = LineLineIntersect(flatEdgeStart, flatEdgeEnd, flatIntersectStart, flatIntersectEnd, out flatRet);


        Debug.DrawLine(flatEdgeStart, flatEdgeEnd, Color.blue);
        Debug.DrawLine(flatIntersectStart, flatIntersectEnd, Color.white);
        Debug.DrawLine(flatRet, flatRet + Vector2.up, Color.yellow);

        Vector3 ret = ProjectPointOnPlane(flatRet, trianglePlane.normal, trianglePlane.distance);
        return ret;
    }

    private static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeNormal, float planeDistance)
    {
        Vector3 start = Vector3.ProjectOnPlane(point, planeNormal);
        return start - planeNormal * planeDistance;
    }

    private static Vector2 FlattenPoint(Vector3 u, Vector3 v, Vector3 point)
    {
        float x = Vector3.Dot(u, point);
        float y = Vector3.Dot(v, point);
        return new Vector2(x, y);
    }

    void Update()
    {
        MyPlane planeA = PlaneFromThreePoints(TriPointA1.position, TriPointA2.position, TriPointA3.position);
        MyPlane planeB = PlaneFromThreePoints(TriPointB1.position, TriPointB2.position, TriPointB3.position);
        PlanePlaneIntersection intersectionResult = GetIntersectionBetweenTwoPlanes(planeA, planeB);
        if (intersectionResult.PlanesIntersect)
        {
            Vector3 theThing = DoTheThing(planeB, TriPointB1.position, TriPointB2.position, intersectionResult);
            Debug.DrawRay(theThing, intersectionResult.NormalOfLine, Color.red);
            Debug.DrawRay(theThing, -intersectionResult.NormalOfLine, Color.green);
        }

        Vector3 test = ProjectPointOnPlane(PlaneTestProjector.position, planeB.normal, planeB.distance);

        //Debug.DrawRay(test, intersectionResult.NormalOfLine, Color.red);
        //Debug.DrawRay(test, -intersectionResult.NormalOfLine, Color.green);
    }

    private static bool LineLineIntersect(Vector2 segAStart, Vector2 segAEnd,
                                        Vector2 segBStart, Vector2 segBEnd,
                                        out Vector2 intersectionPoint)
    {
        float determinant = (segBEnd.y - segBStart.y) * (segAEnd.x - segAStart.x)
           - (segBEnd.x - segBStart.x) * (segAEnd.y - segAStart.y);

        float n_a = (segBEnd.x - segBStart.x) * (segAStart.y - segBStart.y)
           - (segBEnd.y - segBStart.y) * (segAStart.x - segBStart.x);

        float n_b = (segAEnd.x - segAStart.x) * (segAStart.y - segBStart.y)
           - (segAEnd.y - segAStart.y) * (segAStart.x - segBStart.x);

        if (determinant == 0)
        {
            intersectionPoint = Vector2.zero;
            return false;
        }

        float ua = n_a / determinant;
        float ub = n_b / determinant;

        //if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1) // This is not useful
        //{
        float retX = segAStart.x + (ua * (segAEnd.x - segAStart.x));
        float retY = segAStart.y + (ua * (segAEnd.y - segAStart.y));
        intersectionPoint = new Vector2(retX, retY);
        return true;
        //}
        //return false;
    }
}

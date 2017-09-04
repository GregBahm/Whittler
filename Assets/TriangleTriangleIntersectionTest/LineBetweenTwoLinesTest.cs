using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TestShit
{
    /// <summary>
    /// Calculates the intersection line segment between 2 lines (not segments).
    /// Returns false if no solution can be found.
    /// </summary>
    /// <returns></returns>
    public static bool CalculateLineLineIntersection(Vector3 line1Point1, Vector3 line1Point2,
        Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2)
    {
        resultSegmentPoint1 = Vector3.zero;
        resultSegmentPoint2 = Vector3.zero;

        Vector3 p1 = line1Point1;
        Vector3 p2 = line1Point2;
        Vector3 p3 = line2Point1;
        Vector3 p4 = line2Point2;
        Vector3 p13 = p1 - p3;
        Vector3 p43 = p4 - p3;

        if (p43.sqrMagnitude < Mathf.Epsilon)
        {
            return false;
        }
        Vector3 p21 = p2 - p1;
        if (p21.sqrMagnitude < Mathf.Epsilon)
        {
            return false;
        }

        double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
        double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
        double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
        double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
        double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

        double denom = d2121 * d4343 - d4321 * d4321;
        if (Math.Abs(denom) < Mathf.Epsilon)
        {
            return false;
        }
        double numer = d1343 * d4321 - d1321 * d4343;

        double mua = numer / denom;
        double mub = (d1343 + d4321 * (mua)) / d4343;

        resultSegmentPoint1.x = (float)(p1.x + mua * p21.x);
        resultSegmentPoint1.y = (float)(p1.y + mua * p21.y);
        resultSegmentPoint1.z = (float)(p1.z + mua * p21.z);
        resultSegmentPoint2.x = (float)(p3.x + mub * p43.x);
        resultSegmentPoint2.y = (float)(p3.y + mub * p43.y);
        resultSegmentPoint2.z = (float)(p3.z + mub * p43.z);

        return true;
    }
}
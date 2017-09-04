using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleTriangleIntersectionScript : MonoBehaviour
{
    public Transform TriPointA1;
    public Transform TriPointA2;
    public Transform TriPointA3;

    public Transform TriPointB1;
    public Transform TriPointB2;
    public Transform TriPointB3;

    public Material TriangleMat;
    public ComputeShader Computer;

    private int _pointSetterKernel;
    private ComputeBuffer _triangleABuffer;
    private ComputeBuffer _triangleBBuffer;
    private const int PointsBufferStride = 3 * sizeof(float);

    private int _intersectionKernel;
    private ComputeBuffer _intersectionBuffer;
    private const int IntersectionBufferStride = sizeof(int) + 3 * sizeof(float) + 3 * sizeof(float);

    private struct TriangleIntersection
    {
        bool Present;
        Vector3 Start;
        Vector3 End;
    }

    void Start ()
    {
        _pointSetterKernel = Computer.FindKernel("PointSetter");
        _intersectionKernel = Computer.FindKernel("LineOfIntersection");
        _triangleABuffer = new ComputeBuffer(3, PointsBufferStride);
        _triangleBBuffer = new ComputeBuffer(3, PointsBufferStride);
        _intersectionBuffer = new ComputeBuffer(1, IntersectionBufferStride);
    }

    void Update ()
    {
        UpdatePointPositionProperties();
        Computer.SetBuffer(_pointSetterKernel, "_TriangleABuffer", _triangleABuffer);
        Computer.SetBuffer(_pointSetterKernel, "_TriangleBBuffer", _triangleBBuffer);
        Computer.Dispatch(_pointSetterKernel, 1, 1, 1);

        Computer.SetBuffer(_intersectionKernel, "_TriangleABuffer", _triangleABuffer);
        Computer.SetBuffer(_intersectionKernel, "_TriangleBBuffer", _triangleBBuffer);
        Computer.SetBuffer(_intersectionKernel, "_IntersectionBuffer", _intersectionBuffer);
        Computer.Dispatch(_intersectionKernel, 1, 1, 1);
    }

    private void UpdatePointPositionProperties()
    {
        Computer.SetVector("_TriPointA1Pos", TriPointA1.position);
        Computer.SetVector("_TriPointA2Pos", TriPointA2.position);
        Computer.SetVector("_TriPointA3Pos", TriPointA3.position);

        Computer.SetVector("_TriPointB1Pos", TriPointB1.position);
        Computer.SetVector("_TriPointB2Pos", TriPointB2.position);
        Computer.SetVector("_TriPointB3Pos", TriPointB3.position);
    }

    private void OnRenderObject()
    {
        TriangleMat.SetBuffer("_IntersectionBuffer", _intersectionBuffer);

        TriangleMat.SetPass(0);
        TriangleMat.SetBuffer("_PointsBuffer", _triangleABuffer);
        TriangleMat.SetColor("_Color", Color.red);
        Graphics.DrawProcedural(MeshTopology.Triangles, 3, 1);

        TriangleMat.SetPass(0);
        TriangleMat.SetBuffer("_PointsBuffer", _triangleBBuffer);
        TriangleMat.SetColor("_Color", Color.white);
        Graphics.DrawProcedural(MeshTopology.Triangles, 3, 1);
    }

    private void OnDestroy()
    {
        _triangleABuffer.Dispose();
        _triangleBBuffer.Dispose();
        _intersectionBuffer.Dispose();
    }
}

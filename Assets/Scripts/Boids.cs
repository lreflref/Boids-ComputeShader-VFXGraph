//Management of Boids particle

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using Random = Unity.Mathematics.Random;




[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
public struct boidBoid
{

    public Vector3 position; 
    public Vector3 forward;
}


[RequireComponent(typeof(VisualEffect))]
public class Boids : MonoBehaviour
{


    [SerializeField]
    int numBoids;

 
    [SerializeField]
    ComputeShader boidCompute;
    

    [SerializeField]
    float alignWeight;
    [SerializeField]
    float separateWeight;
    [SerializeField]
    float targetWeight;


    [SerializeField]
    Transform targetTrans;

    int kernelIndex;

    GraphicsBuffer boidsBuffer;


    VisualEffect boidVFX;

    [SerializeField]
    float speed;

    uint threadGroupSize;



    void Start()
    {
        //Initialize Boids particles.
        SetBoidBoids();

        kernelIndex = boidCompute.FindKernel("Boids");

        CreateBuffer();

        boidCompute.SetInt("numBoid", numBoids);
        boidCompute.SetBuffer(kernelIndex, "_boidBoids", boidsBuffer);


         boidVFX = GetComponent<VisualEffect>();

         boidVFX.SetGraphicsBuffer("bufferBoids", boidsBuffer);
        
        
    }


    void Update()
    {
        boidCompute.SetFloat("deltaTime", Time.deltaTime);
        boidCompute.SetFloat("speed", speed);
        boidCompute.SetFloat("alignWeight", alignWeight);
        boidCompute.SetFloat("separationWeight", separateWeight);
        boidCompute.SetFloat("targetWeight", targetWeight);
        boidCompute.SetVector("targetP", targetTrans.position);

        boidCompute.GetKernelThreadGroupSizes(kernelIndex, out threadGroupSize, out _, out _);
        int threadGroups = (int)((numBoids + (threadGroupSize - 1)) / threadGroupSize);
        boidCompute.Dispatch(kernelIndex, threadGroups, 1, 1);



    }

    void CreateBuffer()
    {
        boidsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, numBoids, Marshal.SizeOf<boidBoid>());
    

        var boidArray =
            new NativeArray<boidBoid>(numBoids, Allocator.Temp, NativeArrayOptions.UninitializedMemory);


        for (int i = 0; i < numBoids; i++)
        {
            boidArray[i] = new boidBoid
            {
                position = Points.positions[i],
                forward = Points.forwards[i]
            };

        }



        boidsBuffer.SetData(boidArray);
        boidArray.Dispose();
    }

    

    private void OnDisable()
    {
        boidsBuffer?.Dispose();
   
    }

    void SetBoidBoids()
    {
        
        Points.positions = new float3[numBoids];
        Points.forwards = new float3[numBoids];

        for(int i = 0; i< numBoids; i++)
        {
            Points.positions[i] = new float3(UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f), UnityEngine.Random.Range(-10.0f, 10.0f));
            Points.forwards[i] = new float3(0,0,0);
        }
    }



}

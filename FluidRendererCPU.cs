using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FluidRendererCPU : MonoBehaviour
{
    public float influenceRadius;

    private Particle[] data;
    private List<GameObject> particleList;
    public GameObject particlePrefab;
    private List<Vector3> velocityList;

    //public ComputeShader surfaceShader;
    public int particleNumber;
    public float gravity;
    public float boxwidth;
    public float pressureForce;
    public float collisionDamping;
    public float targetDensity;
    public float massPerParticle;
    public float viscosity;
    public float inputRadius;

    private float[] densityData;
    void Start()
    {
        data = new Particle[particleNumber * 10];
        particleList = new List<GameObject>();
        velocityList = new List<Vector3>();

        for (int i = 0; i < data.Length; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
            Particle p = new Particle();
            p.mass = massPerParticle;
            p.position = randomPosition;
            p.velocity = Vector2.zero;
            p.force = Vector2.zero;
            data[i] = p;

            GameObject p2 = Instantiate(particlePrefab, randomPosition, Quaternion.identity);
            particleList.Add(p2);
            velocityList.Add(Vector3.zero);

        }
    }

    void RenderParticles()
    {
        for (int i = 0; i < data.Length; i++)
        {
            Vector3 velocity = new Vector3(data[i].velocity.x, data[i].velocity.y, 0) + new Vector3(data[i].force.x, data[i].force.y, velocityList[i].z) * Time.deltaTime / data[i].mass;

            

            data[i].velocity = velocity;
            particleList[i].transform.position += Time.deltaTime * velocity;
            data[i].position = particleList[i].transform.position;
        }


    }

    private float SmoothingKernel(float r)
    {
        float q = r / influenceRadius;
        float a = 10 / (7 * Mathf.PI * (influenceRadius * influenceRadius));

        if (0 <= q && q < 1)
        {
            return a * (1 - (3 / 2) * (q*q) + (3 / 4) * (q*q*q));

        }
        else if (1 <= q && q < 2)
        {
            return a * ((1 / 4) * Mathf.Pow((2 - q), 3));

        }
        else
        {
            return 0;
        }
    }


    private float CalculateDensity(Vector2 position)
    {

        float density = 0;

        for (int i = 0; i < data.Length; i++)
        {
            density += data[i].mass * SmoothingKernel(Vector2.Distance(position, data[i].position));
        }

        return density;
    }
    Vector2 ApplyGravity(Vector2 position, Vector2 velocity)
    {
        return position + (velocity + Vector2.down * gravity * Time.deltaTime) * Time.deltaTime;

    }
    float LaplacianSmoothingKernel(float r)
    {
        float q = r / influenceRadius;
        float a = 10 / (7 * Mathf.PI * Mathf.Pow(influenceRadius, 2));

        if (0 <= q && q < 1)
        {
            return a * ((3 / Mathf.Pow(influenceRadius, 2)) + ((9 * r) / (2 * Mathf.Pow(influenceRadius, 3))));
        }
        else if (1 <= q && q < 2)
        {
            return a * ((3 / (2 * Mathf.Pow(influenceRadius, 2))) * (2 - (r / influenceRadius)));
        }
        else
        {
            return 0;
        }

    }
    Vector2 DerivativeSmoothingKernel(Vector2 u, Vector2 v)
    {
        Vector2 r_vec = u - v;
        float r = Vector2.Distance(u, v);
        float q = r / influenceRadius;
        float a = 10 / (7 * Mathf.PI * Mathf.Pow(influenceRadius, 2));

        if (0 <= q && q < 1)
        {
            return a * ((-3 * q / Mathf.Pow(influenceRadius, 2)) + ((9 * Mathf.Pow(q, 2)) / (4 * Mathf.Pow(influenceRadius, 2)))) * (r_vec / r);

        }
        else if (1 <= q && q < 2)
        {
            return a * (-3 * Mathf.Pow((2 - q), 2) / (4 * Mathf.Pow(influenceRadius, 2))) * (r_vec / r);

        }
        else
        {
            return Vector2.zero;
        }

    }

    float CalculatePressure(float density)
    {
        return pressureForce * (density - targetDensity);
    }
    Vector2 CalculateNetForce(int particleIndex, Vector2 nextpos)
    {
        Particle currParticle = data[particleIndex];
        Vector2 pressureForce = Vector2.zero;


        float currDensity = densityData[particleIndex];
        //float currDensity = CalculateDensity(currParticle.position);
        float currPressure = CalculatePressure(currDensity);
        //float2 curvature = float2(0, 0);

        for (uint i = 0; i < data.Length; i++)
        {
            if (particleIndex != i && (nextpos[0] != data[i].position[0] || nextpos[1] != data[i].position[1]))
            {
                Particle otherParticle = data[i];
                float otherDensity = densityData[i];
                //float otherDensity = CalculateDensity(otherParticle.position);
                float otherPressure = CalculatePressure(otherDensity);
                Vector2 slope = DerivativeSmoothingKernel(otherParticle.position, nextpos);
                pressureForce += otherParticle.mass * ((currPressure / Mathf.Pow(currDensity, 2)) + (otherPressure / Mathf.Pow(otherDensity, 2))) * slope;

                //viscosity
                pressureForce += otherParticle.mass * ((otherParticle.velocity - currParticle.velocity) / otherDensity) * LaplacianSmoothingKernel(Vector2.Distance(nextpos, otherParticle.position));

                //surface tension
                //curvature += otherParticle.mass * (colorGradients[i] - colorGradients[particleIndex]) * slope / otherPressure;
            }
        }

        //curvature *= colorGradients[particleIndex] * surfaceConstant;


        return viscosity * pressureForce;
    }

    float deltaTime = 0.0f;

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        Debug.Log(fps);
        densityData = new float[particleNumber * 10];
        for (int i = 0; i < densityData.Length; i++) 
        {
            densityData[i] = CalculateDensity(data[i].position);
        }

        for (int i = 0; i < data.Length; i++)
        {
            data[i].velocity += Vector2.down * gravity * Time.deltaTime;
            Vector2 nextPosition = ApplyGravity(data[i].position, data[i].velocity);
            data[i].force = CalculateNetForce(i, nextPosition);
        }
        RenderParticles();
    }
}

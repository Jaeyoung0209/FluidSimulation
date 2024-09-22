using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// The main c# script to run the compute shaders and render particles

public struct Particle
{
    public float mass;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 force;
}

public struct cgData
{
    public float x;
    public float y;
}

public class FluidRenderer : MonoBehaviour
{

    public float influenceRadius;

    private Particle[] data;
    private List<GameObject> particleList;
    public GameObject particlePrefab;
    private List<Vector3> velocityList;

    public ComputeShader computeShader;
    public ComputeShader densityShader;
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
    //public float surfaceTension;


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

            if (Mathf.Abs(particleList[i].transform.position.y) >= boxwidth)
            {
                velocity = new Vector3(velocity.x, -velocity.y, velocity.z) * collisionDamping;
            }
            if (Mathf.Abs(particleList[i].transform.position.x) >= boxwidth)
            {
                velocity = new Vector3(-velocity.x * collisionDamping, velocity.y, velocity.z);
            }

            data[i].velocity = velocity;
            particleList[i].transform.position += Time.deltaTime * velocity;
            data[i].position = particleList[i].transform.position;
        }


    }

    Vector2 HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;    
            return Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.z));
        }

        return new Vector2(-boxwidth * 2, -boxwidth * 2);
    }

    
    float deltaTime = 0.0f;
    void Update()
    {

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        Debug.Log(fps);
        
        Vector2 inputPosition = HandleInput();
        float[] inputArray = new float[2];
        inputArray[0] = -inputPosition.x;
        inputArray[1] = -inputPosition.y;

        ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, sizeof(float) * 7);
        ComputeBuffer densityBuffer = new ComputeBuffer(data.Length, sizeof(float));
        //ComputeBuffer colorGradientBuffer = new ComputeBuffer (data.Length, sizeof(float) * 2);

        densityShader.SetBuffer(0, "densityBuffer", densityBuffer);
        densityShader.SetBuffer(0, "particles", computeBuffer);
        densityShader.SetFloat("influenceRange", influenceRadius);

        //surfaceShader.SetBuffer(0, "colorGradients", colorGradientBuffer);
        //surfaceShader.SetBuffer(0, "particles", computeBuffer);
        //surfaceShader.SetFloat("influenceRange", influenceRadius);
        //surfaceShader.SetFloat("targetDensity", targetDensity);
        //surfaceShader.SetFloat("pressureConstant", pressureForce);
        
        computeShader.SetBuffer(0, "particles", computeBuffer);
        computeShader.SetFloat("influenceRange", influenceRadius);
        computeShader.SetFloat("pressureConstant", pressureForce);
        computeShader.SetFloat("targetDensity", targetDensity);
        computeShader.SetFloat("gravityConstant", gravity);
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("boxSize", boxwidth);
        computeShader.SetFloat("collisionDamping", collisionDamping);
        computeShader.SetFloat("viscosity", viscosity);
        computeShader.SetFloats("inputPosition", inputArray);
        computeShader.SetFloat("inputRadius", inputRadius);
        //computeShader.SetFloat("surfaceConstant", surfaceTension);


        computeBuffer.SetData(data);


        float[] densityData = new float[particleNumber * 10];
        cgData[] colorGradientData = new cgData[particleNumber * 10];
        
        densityBuffer.SetData(densityData);
        densityShader.Dispatch(0, data.Length / 10, 1, 1);
        //densityBuffer.GetData(densityData);
        
        //surfaceShader.SetBuffer(0, "densityBuffer", densityBuffer);
        //colorGradientBuffer.SetData(colorGradientData);
        //surfaceShader.Dispatch(0, data.Length / 10, 1, 1);
        //colorGradientBuffer.GetData(colorGradientData);


        computeShader.SetBuffer(0, "densities", densityBuffer);
        //computeShader.SetBuffer(0, "colorGradients", colorGradientBuffer);
        computeShader.Dispatch(0, data.Length / 10, 1, 1);

        computeBuffer.GetData(data);
        RenderParticles();


        //colorGradientBuffer.Release();
        densityBuffer.Release();
        computeBuffer.Release();
    }
}

// 3-body Starter Code
// Fall 2025. IMDM 327
// Instructor. Myungin Lee

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.Random;
using Color = UnityEngine.Color;

public class ThreeBody : MonoBehaviour
{
    [SerializeField] private DataCSV dataCSV;
    
    public float G = 500f; // Gravity constant https://en.wikipedia.org/wiki/Gravitational_constant
    public float initvelo = 300f;
    public float r = 200f;
    public float distScale = 1f / 10000000000f;
    public float massScale = 1f / 10000000000f;
    GameObject[] body;
    BodyProperty[] bp;
    public int numberOfSphere = 10;
    TrailRenderer trailRenderer;

    public struct BodyProperty
    {
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
        public float radius;
    }
    void Start()
    {
        /*
        // Just like GO, computer should know how many room for struct is required:
        bp = new BodyProperty[numberOfSphere];
        body = new GameObject[numberOfSphere];

        // Loop generating the gameobject and assign initial conditions (type, position, (mass/velocity/acceleration)
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            body[i] = GameObject.CreatePrimitive(PrimitiveType.Cube); // why sphere? try different options.

            // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html

            // initial conditions
            // position is (x,y,z). In this case, I want to plot them on the circle with r

            // ******** Fill in this part ********
            // body[i].transform.position = new Vector3( ***, *** , 180);
            var angle = Mathf.PI * 2 / numberOfSphere * i;

            body[i].transform.position = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 180);
            // z = 180 to see this happen in front of me. Try something else (randomize) too.

            bp[i].velocity =
                new Vector3(
                    Mathf.Cos(angle + ((float)Mathf.PI * 0.5f)) * UnityEngine.Random.Range(0.8f, 1.2f) * initvelo,
                    Mathf.Sin(angle + ((float)Mathf.PI * 0.5f)) * UnityEngine.Random.Range(0.8f, 1.2f) * initvelo,
                    0); // Try different initial condition

            bp[i].mass = 1; // Simplified. Try different initial condition

        }
        */
        
        // override solar system
        
        

        numberOfSphere = dataCSV.getBP().Length;
        Debug.Log(numberOfSphere);
        bp = new BodyProperty[numberOfSphere];
        body = new GameObject[numberOfSphere];
        for (int i = 0; i < numberOfSphere; i++)
        {
            
            var angle = Mathf.PI * 2 / numberOfSphere * i;
            
            var distance = Mathf.Sqrt(dataCSV.getBP()[i].distance);
            
            
            body[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body[i].transform.localScale = Vector3.one * 10f;
            body[i].transform.position = new Vector3(Mathf.Cos(angle) * distance, Mathf.Sin(angle) * distance, 0);
            bp[i].velocity =
                new Vector3(
                    Mathf.Cos(angle + (Mathf.PI * 0.5f)) * dataCSV.getBP()[i].initial_velocity * initvelo,
                    Mathf.Sin(angle + (Mathf.PI * 0.5f)) * dataCSV.getBP()[i].initial_velocity * initvelo,
                    0); 
            
            bp[i].mass = massScale * dataCSV.getBP()[i].mass;
            
            
            //trails
            
        
            trailRenderer = body[i].AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the trail color over time
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(new Color (Mathf.Cos(Mathf.PI * 2 / numberOfSphere * i), Mathf.Sin(Mathf.PI * 2 / numberOfSphere * i), Mathf.Tan(Mathf.PI * 2 / numberOfSphere * i)), 0.80f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            trailRenderer.colorGradient = gradient;

        }
        
        
    }

        void Update()
    {
        // timeflow += Time.deltaTime;
        
        // Loop for N-body gravity
        // How should we design the loop?
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Something
            bp[i].acceleration = Vector3.zero;
        }

        var count = 0;
        
        for (int i = 0; i < numberOfSphere; i++)
        {
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                
                // F = G * m1 * m2 / r^2
                // A1 = G * m1 * m2 / r^2 ) / m1
                float distance = Vector3.Distance(body[i].transform.position, body[j].transform.position);
                Vector3 itoj = (body[i].transform.position - body[j].transform.position).normalized;
                var gmoverr = G * bp[j].mass / (distance * distance);
                bp[i].acceleration += gmoverr * itoj;
                bp[j].acceleration += gmoverr * -1 * itoj;
                count++;
            }
        }
        
        for (int i = 0; i < numberOfSphere; i++)
        {
            bp[i].velocity -= bp[i].acceleration * Time.deltaTime;
            body[i].transform.position += bp[i].velocity * Time.deltaTime;
            
        }
        



    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    {
        Vector3 gravity = Vector3.zero; // note this is also Vector3
        gravity = (m1 * m2 / (distanceVector.magnitude * distanceVector.magnitude)) * distanceVector.normalized * G;
        return gravity;
    }
}

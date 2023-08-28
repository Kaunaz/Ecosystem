using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum states{
    patrol,
    chase,
    reproduce,
    evade
}

public class Behaviour : MonoBehaviour
{
    public states state = states.patrol;

    // Movement variables
    public float speed = 5.0f;
    public float rotationSpeed = 1.0f;
    private float rotationInterval = 2.0f;
    private float rotationTimer = 0f;
    public float energy = 70f; // para que no se reproduzcan al empezar el juego
    public float energy_loss = 3f;

    // Detection variables
    public float maxDetectionDistance = 10f;
    public float fov = 90f;
    private float nRays = 20;

    public int boxDistance = 50;

    public float forniqueTime = 5f;
    public bool recentFornique = true;
    private float timer = 0f;


    List<Ray> rays = new List<Ray>();
    private GameObject target;

    void Start()
    {

    }

    void Boundaries(){
        if (transform.position.x > boxDistance){
            transform.position = new Vector3(boxDistance, transform.position.y, transform.position.z);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
        else if (transform.position.x < -boxDistance){
            transform.position = new Vector3(-boxDistance, transform.position.y, transform.position.z);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);

        }
        if (transform.position.z > boxDistance){
            transform.position = new Vector3(transform.position.x, transform.position.y,boxDistance);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);

        }
        else if (transform.position.z < -boxDistance){
            transform.position = new Vector3(transform.position.x, transform.position.y, -boxDistance);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);

        }
    }

    private void RotateEntity()
    {
        // Generate a random rotation angle
        float randomAngle = UnityEngine.Random.Range(0.0f, 30.0f);

        // Rotate around the y-axis by the random angle
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, randomAngle, 0.0f), rotationSpeed * Time.deltaTime);
    }

    void Patrol(){
        if (rotationTimer < 0f){
            RotateEntity();
            rotationTimer =  UnityEngine.Random.Range(1.0f, 3.0f);
        }
        rotationTimer -= Time.deltaTime;

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void Chase(){
        if (target != null){
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            transform.LookAt(target.transform.position);

            if (Vector3.Distance(transform.position, target.transform.position) < 2.0f){
                Destroy(target);
                energy += 20;
            }
        }
        else{
            Patrol();
        }
    }
    void Reproduce(){
        if(target != null && !recentFornique){
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            transform.LookAt(target.transform.position);
            target.GetComponent<Behaviour>().recentFornique = true;
            if (Vector3.Distance(transform.position, target.transform.position) < 2.0f){
                recentFornique = true;
                GameObject offspring = Instantiate(gameObject, transform.position + Vector3.forward * 2f, Quaternion.identity);
                Behaviour offspringScript = offspring.GetComponent<Behaviour>();
                energy -= 20.0f;
                offspringScript.energy = UnityEngine.Random.Range(50f, 100f);
                offspringScript.speed = (target.GetComponent<Behaviour>().speed + speed) / 2.0f;
                offspringScript.recentFornique = true;
            }
        }
        else{
            Patrol();
        }
    }

    void Update()
    {

        if (energy >= 300 && !recentFornique){
            energy = 100;
            GameObject offspring = Instantiate(gameObject, new Vector3(transform.position.x +2f, 1, transform.position.z + 2f), Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0));
            offspring.GetComponent<Behaviour>().energy = UnityEngine.Random.Range(50f, 75f);
            offspring.GetComponent<Behaviour>().speed = speed;
            offspring.GetComponent<Behaviour>().recentFornique = true;
            recentFornique = true;


        }
        if (recentFornique){
            timer += Time.deltaTime;
            if (timer >= forniqueTime){
                recentFornique = false;
                timer = 0f;
            }
        }

        energy -= energy_loss * Time.deltaTime * speed / 5;
        if (energy <= 0){
            Destroy(gameObject);
        }


        rays.Clear();
        for (int i = 0; i < nRays; i++) // For loop to create multiple raycasts
        {
            float angle = i / nRays * fov - fov / 2; // Calculate the angle of the raycast
            Quaternion rot = Quaternion.AngleAxis(angle, transform.up); // Calculate the rotation of the raycast
            Vector3 dir = rot * transform.forward; // Calculate the direction of the raycast
            Ray ray = new Ray(transform.position, dir); // Create the raycast
            rays.Add(ray);
            Debug.DrawRay(transform.position, dir * maxDetectionDistance, Color.red); // Draw the raycast in the editor
        }

        state= states.patrol;
        float minDistance = maxDetectionDistance;
        // Si tiene hambre, buscar comida
        foreach (Ray ray in rays)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDetectionDistance))
            {
                // Queremos que coma 
                
                    if (hit.collider.tag == "prey")
                    {
                        target = hit.collider.gameObject;
                        state = states.chase;

                        target.GetComponent<Herbivore>().state = statesHerbivore.evade;
                        
                        target.GetComponent<Herbivore>().predator = gameObject;
                    }       
                    // reproduccion
                    if (hit.collider.tag == "predator" && energy >= 60)
                    {   
                        if(Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
                            target = hit.collider.gameObject;
                            state = states.reproduce;
                        }
                    }
                    
                // }
                
            }
        }
        Boundaries();
        if (state == states.patrol){
            Patrol();
        }
        else if (state == states.chase){
            Chase();
        }
        else if (state == states.reproduce){
            Reproduce();
        }
    }
}

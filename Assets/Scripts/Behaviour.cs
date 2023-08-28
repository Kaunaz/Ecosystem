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
    public float speed = 1.0f;
    public float rotationSpeed = 1.0f;
    private float rotationInterval = 2.0f;
    private float nextRotationTime;
    private float energy = 70f; // para que no se reproduzcan al empezar el juego
    public float energy_loss = 1f;

    // Detection variables
    public float maxDetectionDistance = 20f;
    public float fov = 90f;
    private float nRays = 20;

    public int boxDistance = 20;

    List<Ray> rays = new List<Ray>();
    private GameObject target;

    void Start()
    {
        nextRotationTime = Time.time + rotationInterval;
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
        float randomAngle = Random.Range(0.0f, 360.0f);

        // Rotate around the y-axis by the random angle
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, randomAngle, 0.0f), rotationSpeed * Time.deltaTime);
    }

    void Patrol(){
        if (Time.time >= nextRotationTime){
            RotateEntity();
            nextRotationTime = Time.time + rotationInterval;
            rotationInterval = Random.Range(1.0f, 3.0f);
        }

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
        if(target != null){
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            transform.LookAt(target.transform.position);

            if (Vector3.Distance(transform.position, target.transform.position) < 2.0f){
                GameObject offspring = Instantiate(gameObject, transform.position + Vector3.forward * 2f, Quaternion.identity);
                Behaviour offspringScript = offspring.GetComponent<Behaviour>();
                float offspringEnergy = energy * 0.5f;
                energy -= 10.0f;
                offspringScript.energy = offspringEnergy;
            }
        }
        else{
            Patrol();
        }
    }

    void Update()
    {

        energy -= energy_loss * Time.deltaTime;
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
                if (energy <= 75){
                    if (hit.collider.tag == "prey")
                    {
                        target = hit.collider.gameObject;
                        state = states.chase;

                        target.GetComponent<Herbivore>().state = statesHerbivore.evade;
                        
                        target.GetComponent<Herbivore>().predator = gameObject;
                    }       
                }
                else{
                    // reproduccion
                    if (hit.collider.tag == "predator")
                    {
                        if(Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
                            target = hit.collider.gameObject;
                            state = states.reproduce;
                        }
                    }
                    
                }
                
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

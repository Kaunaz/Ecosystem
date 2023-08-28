using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour
{
    public float start_rabbit;
    public float start_fox;
    public float start_grass;

    public GameObject Rabbit;
    public GameObject Fox;
    public GameObject Grass;

    public float grassRate = 2f;


    // Start is called before the first frame update
    void Start()
    {
        // Spawn 20 Foxes at random positions and random rotations
        for (int i = 0; i < start_fox; i++)
        {
            Instantiate(Fox, new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            Fox.GetComponent<Behaviour>().energy = Random.Range(50f, 100f);
            Fox.GetComponent<Behaviour>().speed = Random.Range(5f, 10f);
        }

        // Spawn 20 Rabbits at random positions and random rotations
        for (int i = 0; i < start_rabbit; i++)
        {
            Instantiate(Rabbit, new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            Rabbit.GetComponent<Herbivore>().energy = Random.Range(50f, 100f);
            Rabbit.GetComponent<Herbivore>().speed = Random.Range(3f, 8f);
        }

        for (int i = 0; i < start_grass; i++){
            // grass puede no tener rotacion.
            Instantiate(Grass, new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
}

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0f, 100f) < grassRate){
            Instantiate(Grass, new Vector3(Random.Range(-50f, 50f), 1, Random.Range(-50f, 50f)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
    }

}

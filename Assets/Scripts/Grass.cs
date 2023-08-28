using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public GameObject go_Grass;
    private float timer = 0;
    public float reproduction_time = 5;
    public float radio = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= reproduction_time){
            int childs = Random.Range(1, 2);
            for(int i = 0; i < childs+1; i++){
                // Instantiate(go_Grass, transform.position + new Vector3(Random.Range(-radio, radio), 0, Random.Range(-radio, radio)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
            // Destroy(gameObject);
            // timer = 0;
        }
        
    }
}

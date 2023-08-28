using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFracture : MonoBehaviour
{
    public Collider[] colliders;

    private PressureSensorInputController pressure; //presure data값 평균 받아오기 위함
    
    void Awake()
    {   
        pressure = FindObjectOfType<PressureSensorInputController>();

        colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            if (c.name == "FracturedEgg") continue; //부모, 즉 완전한 egg만 renderer를 킴
            
            c.gameObject.GetComponent<Renderer>().enabled = false;
            Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
            rb.constraints = (RigidbodyConstraints)126;
        }
        colliders[11].gameObject.GetComponent<Renderer>().enabled = true;
    }

    void Update()
    {
        if(pressure.pressure_avg > 400) //400이상일때 깨짐
        {
            GetComponent<Renderer>().enabled = false;
            foreach (Collider c in colliders)
            {
                if (c.name == "FracturedEgg") continue;

                c.gameObject.GetComponent<Renderer>().enabled = true;
                Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
                rb.constraints = (RigidbodyConstraints)0;
            }
        }
    }
}

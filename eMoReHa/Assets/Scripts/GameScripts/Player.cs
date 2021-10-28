using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed=1;
    private float Accelaration= 1;
    void Start()
    {
        StartCoroutine(decelerate());

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Accelaration <= 10)
                Accelaration *= 1.5f;


        }
 
        Vector3 Target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Target.z =0;
        transform.position = Vector3.MoveTowards(transform.position, Target, Accelaration* Speed * Time.deltaTime / transform.localScale.x);

    }
    IEnumerator decelerate()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            if (Accelaration>1) Accelaration /= 1.5f;
        }


    }
    float getAccelaration()
    {
        return Accelaration;
    }
    float getSpeed()
    {
        return Speed;
    }
}

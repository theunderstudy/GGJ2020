using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillRotator : MonoBehaviour
{
    public float rotationRate = 10;
    // Update is called once per frame
    void Update()
    {
        Vector3 angle = transform.eulerAngles;
        angle.z -= Time.deltaTime * rotationRate;
        transform.eulerAngles = angle;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Background Settings")]
    [SerializeField] private GameObject camera;
    [SerializeField] private float parallaxEffect;
    private float length, startPosition;

    void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = (camera.transform.position.x * (1 - parallaxEffect));
        float distance = (camera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if (temp > startPosition + length) startPosition += length;
        else if (temp < startPosition - length) startPosition -= length;
    }
}

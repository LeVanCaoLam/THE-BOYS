using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public float timeDayNight = 1f; // thời gian quy định
    public Light lights;

    public Gradient gradient;

    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = 360f / (timeDayNight * 60f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        if (lights != null && gradient != null)
        {
            float time = Mathf.PingPong(Time.time / (timeDayNight * 60f), 1f);
            lights.color = gradient.Evaluate(time);
        }
    }
}

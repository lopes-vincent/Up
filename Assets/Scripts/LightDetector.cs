using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetector : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> lights = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject light in lights)
            {
                light.SetActive(true);
            }
        }

    }
}

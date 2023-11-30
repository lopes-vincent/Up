using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundable : MonoBehaviour
{
    [SerializeField] private AudioSource grabAudioSource;
    [SerializeField] private AudioSource movingAudioSource;
    
    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (movingAudioSource)
        {
            Vector2 velocity = _rigidbody2D.velocity;   
            float speed = velocity.magnitude;
            movingAudioSource.volume = speed * 0.08f;
        }
    }

    public void PlayGrabSound()
    {
        if (grabAudioSource)
        {
            grabAudioSource.Play();
        }
    }
}

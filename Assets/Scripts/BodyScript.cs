using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BodyScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> endDisabling;
    [SerializeField] private Animator endLightAnimator;
    [SerializeField] private GameObject endWall;
    
    public float MomentumFactor = 25f;
    public Vector2 MomentumValue;
    public bool MomentumLaunch;
    
    [SerializeField] private GameObject lightHat;
    
    public bool GrabbingHard;
    
    private float moveSpeed = 5f;
    
    public Vector2 Movement;
    private Rigidbody2D _rigidbody2D;

    public int grabForce = 0;

    public HandScript leftHand;
    public HandScript rightHand;

    private List<Vector2> lastPositions = new List<Vector2>();

    private int positionsMaxLength = 10;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        StartCoroutine(VelocityDebug());
    }

    private void Update()
    {
        if (MomentumLaunch)
        {
            MomentumLaunch = false;
            StartCoroutine(LaunchMomentum(MomentumValue));
        }
    }

    void FixedUpdate()
    {
        // Debug.Log(Movement);
        GrabbingHard = leftHand.GrabbingHard || rightHand.GrabbingHard;
        _rigidbody2D.mass = GrabbingHard ? 2 : 150;
        
        if (GrabbingHard)
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + Movement * moveSpeed * Time.fixedDeltaTime);
        }
        
        lastPositions.Add(_rigidbody2D.position);
        if (lastPositions.Count > positionsMaxLength)
        {
            lastPositions.RemoveAt(0);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("lightSwitch"))
        {
            switchLight();
        }
        
        if (other.CompareTag("end"))
        {
            StartCoroutine(End());
        }
    }

    public void switchLight()
    {
        lightHat.SetActive(true);
    }

    public void Momentum()
    {
        Vector2 baseMomentum = lastPositions.Last() - lastPositions.First();
        float realXMomentum;
        float realYMomentum;
        if (Mathf.Abs(baseMomentum.x) > Mathf.Abs(baseMomentum.y))
        {
            realXMomentum = 1f;
            realYMomentum = baseMomentum.y / baseMomentum.x;
        }
        else
        {
            realYMomentum = 1f;
            realXMomentum = baseMomentum.x / baseMomentum.y;
        }
    
        Vector2 momentum = new Vector2(realXMomentum, realYMomentum);
        Debug.DrawLine(lastPositions.First(), lastPositions.Last(), Color.blue, 80f);
        Vector2 velocity = baseMomentum * MomentumFactor;
        _rigidbody2D.velocity = new Vector2(velocity.x, velocity.y + 0.7f);
        MomentumValue = momentum;
    }
    
    private IEnumerator LaunchMomentum(Vector2 force)
    {
        _rigidbody2D.velocity = force;
        yield return null;

        // Debug.Log(force);
        // float countDown = 1.5f;
        // for (int i=0; i < 10000; i++) 
        // {
        //     while (countDown >= 0)
        //     {
        //         // Debug.Log("Launch");
        //         _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        //         countDown -= Time.smoothDeltaTime;
        //         // force *= 0.9f;
        //         yield return null;
        //     }
        // }
    }
    
    private IEnumerator VelocityDebug() {
        while(true) {
            yield return new WaitForSeconds(0.2f);
            // Debug.Log(lastPositions.Count);
            if (lastPositions.Count >= positionsMaxLength)
            {
                // Debug.Log("DRAW");
                // Debug.DrawLine(lastPositions.First(), lastPositions.Last(), Color.green, 5f);
            }
        }
    }
    
    private IEnumerator End() {
        endWall.SetActive(true);
        foreach (GameObject gameObjectToDisable in endDisabling)
        {
            gameObjectToDisable.SetActive(false);
        }
        endLightAnimator.SetTrigger("End");
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene(2);
    }
}

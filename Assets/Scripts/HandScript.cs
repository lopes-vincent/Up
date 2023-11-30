using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class HandScript : MonoBehaviour
{
    [SerializeField] private BodyScript body;
    [SerializeField] private Color idleColor;
    [SerializeField] private Color grabColor;
    [SerializeField] private Color grabHardColor;
    
    [SerializeField] private HandScript otherHand;
    [SerializeField] private Sprite idleSprite;
    
    [SerializeField] private Sprite grabSprite;
    
    [SerializeField] private Animator grabAnimator;
    
    [SerializeField] private List<AudioClip> gradAudioClips;
    [SerializeField] private List<AudioClip> gradHardAudioClips;

    private Vector2 _movement;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private HingeJoint2D _hingeJoint2D;
    private AudioSource _audioSource;

    public bool _canGrab;
    public Rigidbody2D _grabbable;
    
    public bool Grabbing;
    public bool GrabbingHard;

    private Rigidbody2D take;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hingeJoint2D = GetComponent<HingeJoint2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _spriteRenderer.color = !Grabbing ? idleColor : !GrabbingHard ? grabColor : grabHardColor;
        GrabbingHard = Grabbing && (_grabbable.CompareTag("Grabbable") ||
                                    (_grabbable.CompareTag("Takeable") && _grabbable.GetComponent<ObjectScript>().Grounded));
    }

    void FixedUpdate()
    {
        if (Grabbing && !_hingeJoint2D.enabled)
        {
            _hingeJoint2D.connectedAnchor = _grabbable.transform.InverseTransformPoint(transform.position);
        }
        _hingeJoint2D.enabled = Grabbing;
        _hingeJoint2D.connectedBody = _grabbable;
        if (!GrabbingHard)
        {
            float moveSpeed = otherHand.GrabbingHard ? 25f : 10f;
            _rigidbody2D.MovePosition(_rigidbody2D.position + _movement * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            body.Movement = _movement;
        }
    }
    
    public void Move(InputAction.CallbackContext movement)
    {
        // Debug.Log(movement);
        _movement = movement.ReadValue<Vector2>();
    }
    
    public void Grab(InputAction.CallbackContext grab)
    {
        if (grab.performed)
        {
            grabAnimator.SetTrigger("Grab");
            List<AudioClip> audioClips = _canGrab && _grabbable.CompareTag("Grabbable") ? gradHardAudioClips : gradAudioClips;
            Soundable grabSound = _grabbable.GetComponent<Soundable>();
            if (_canGrab && grabSound)
            {
                grabSound.PlayGrabSound();
            }
            else
            {
                _audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)]);
            }
            _spriteRenderer.sprite = grabSprite;
        }
        
        Grabbing = _canGrab && grab.performed;

        if (grab.canceled)
        {
            _spriteRenderer.sprite = idleSprite;
        }
        
        if (GrabbingHard && grab.canceled && !otherHand.GrabbingHard)
        {
            body.Momentum();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!Grabbing && _canGrab != true && (other.CompareTag("Grabbable") || other.CompareTag("Takeable")))
        {
            _canGrab = true;
            _grabbable = other.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grabbable") || other.CompareTag("Takeable"))
        {
            _canGrab = false;
        }
    }
}

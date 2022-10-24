using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class PlayerMovement : MonoBehaviour
{
    private int KeyCount = 0;
    public GameObject Wall;
    public GameEnding gameEnding;
    public float hearRadius = 10f;
    private float originalHearRadius;
    [Range(0, 50)]
    public int segments = 50;
    private float xradius;
    private float yradius;
    LineRenderer line;
    public enum State { Run, Walk, Stealth };
    public float turnSpeed = 20f;
    public float movementSpeed = 2f;
    public State playerState = State.Walk;
    public bool isHiding = false;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start ()
    {
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        line = gameObject.GetComponent<LineRenderer>();
        originalHearRadius = hearRadius;
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        DrawCirecle();
    }

    void FixedUpdate ()
    {
        if(KeyCount >= 3){
            Destroy(Wall);
        }
        float horizontal = Input.GetAxis ("Horizontal");
        float vertical = Input.GetAxis ("Vertical");
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool ("IsWalking", isWalking);
        
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop ();
        }

        Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation (desiredForward);
    }

    private void Update()
    {
        //DrawCirecle(100, 1);
        if (Input.GetKey(KeyCode.LeftControl))
        {
            playerState = State.Run;
            movementSpeed = 2f;
            hearRadius = originalHearRadius * 1.5f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            playerState = State.Stealth;
            movementSpeed = .5f;
            hearRadius = originalHearRadius / 3;
        }
        else
        {
            playerState = State.Walk;
            movementSpeed = 1f;
            hearRadius = originalHearRadius;
        }
        DrawCirecle();
    }

    void OnAnimatorMove ()
    {
        m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * movementSpeed);
        m_Rigidbody.MoveRotation (m_Rotation);
    }

    public void changeHide()
    {
        isHiding = !isHiding;
    }


    void DrawCirecle()
    {
        xradius = hearRadius;
        yradius = hearRadius;
        CreatePoints(xradius, yradius);
    }
    void CreatePoints(float xrad, float yrad)
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xrad;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yrad;

            line.SetPosition(i, new Vector3(x, 0, y));

            angle += (360f / segments);
        }
    }

    public float GetHearRadius()
    {
        return hearRadius;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ghost")
        {
            gameEnding.CaughtPlayer();
        }
        
        if (collision.gameObject.tag == "Key")
        {            KeyCount++;
        }
        
            
    
    }
}
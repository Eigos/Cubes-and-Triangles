using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BulletMovement : MonoBehaviour
{
    [Range(0.0f, 10.0f)] [SerializeField] float mDefaultBulletSpeed = 1.0f;

    [Range(0, 2.0f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [Range(0.0f, 10.0f)] [SerializeField] float mBulletSpeed = 1.0f;

    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private Vector2 mDirection;


    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move.x * mBulletSpeed, move.y * mBulletSpeed);

        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

    }


    public void Init()
    {
        mDirection = new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));

    }

    public void setAsDefault()
    {
        mBulletSpeed = mDefaultBulletSpeed;
    }

    public void setBulletSpeed(float newVal)
    {
        mBulletSpeed = newVal;
        
    }

    // Update is called once per frame
    void Update()
    {

        Move(mDirection * mBulletSpeed);
    }


}

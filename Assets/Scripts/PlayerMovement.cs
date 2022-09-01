using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    enum PlayerMovementDirections : int
    {
        NoMovement = 0,
        Left,
        Right
    }


    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [Range(0.0f, 10.0f)] [SerializeField] float mMovementSpeedMultiplier = 1.0f;

    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;

    [SerializeField] Button MovementLeftButton;

    [SerializeField] Button MovementRightButton;

    private int mMovementDirection = (int)PlayerMovementDirections.NoMovement;

    public int MovementDirection
    {
        get
        {
            return mMovementDirection;
        }

        set
        {
            mMovementDirection = value;
        }
    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

#if UNITY_EDITOR || UNITY_ANDROID
        InitPlayerMovementAndroid();
#endif

    }


    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        PlayerMovementPC();
#endif

#if UNITY_EDITOR || UNITY_ANDROID
        PlayerMovementAndroid();
#endif
    }


    #region ANDROID_METHODS

#if UNITY_EDITOR || UNITY_ANDROID
    private void InitPlayerMovementAndroid()
    {

    }

    private void PlayerMovementAndroid()
    {
        if (mMovementDirection == (int)PlayerMovementDirections.NoMovement)
            return;

        else if (mMovementDirection == (int)PlayerMovementDirections.Left)
        {
            Move(-1.0f);
        }
        else
        {
            Move(1.0f);
        }
    }
#endif
    #endregion


    #region STANDALONE_METHODS

#if UNITY_EDITOR || UNITY_STANDALONE
    private void PlayerMovementPC()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(1.0f);
        }

        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(-1.0f);
        }

        else
        {
            Move(0.0f);
        }
    }
#endif

    #endregion

    public void Move(float move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * mMovementSpeedMultiplier, m_Rigidbody2D.velocity.y);

        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

    }

}

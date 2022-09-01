using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    float mBulletSpeed;

    float mBulletIncrement;

    [SerializeField] private float mDefBulletSpeed;

    [SerializeField] private float mDefBulletIncrement;

    // Start is called before the first frame update
    void Start()
    {

    }


    public void Init()
    {

        GetComponent<Rigidbody2D>().velocity =
            new Vector2(0 * Mathf.Sign(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)),
            mBulletSpeed * Mathf.Sign(Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)));
    }

    public void setBulletSpeed(float newVal)
    {
        mBulletSpeed = newVal;
    }

    public void setBulletIncrement(float newVal)
    {
        mBulletIncrement = newVal;
    }

    public void setAsDefault()
    {

        mBulletSpeed = mDefBulletSpeed;

        mBulletIncrement = mDefBulletIncrement;
    }

    // Update is called once per frame
    void Update()
    {
        //Move to same direction
        GetComponent<Rigidbody2D>().velocity += new Vector2(0 * Mathf.Sign(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)) * Time.deltaTime,
            mBulletIncrement * Mathf.Sign(Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)) * Time.deltaTime);
        
        // new Vector2(0 * Mathf.Sign(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)) * Time.deltaTime,
        // mBulletIncrement * Mathf.Sign(Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad)) * Time.deltaTime);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{

    [SerializeField] float mShootingDelay = 1.0f;

    public GameObject mBulletPrefab;

    public List<BulletTypeScript> mBulletTypeList;

    public List<Material> mMaterialList;

    int mSelectedBulletIndex;

    private float lastTimeShot = 0;

    private Canvas mCanvasGame;

    public GameManager gameManager;

    public delegate GameObject Shoot(float direction, int bulletType = -1, GameObject existingBullet = null);

    public Shoot ShootPlayer;

    public delegate GameObject Bullet();

    public Bullet GetBullet;

    [SerializeField] Button mButtonCircle;

    [SerializeField] Button mButtonBox;
    
    [SerializeField] Button mButtonTriangle;


    void Start()
    {
        mSelectedBulletIndex = 0;

        mCanvasGame = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (CanShoot() )
            {
                lastTimeShot = Time.realtimeSinceStartup;

                ShootFunc(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (CanShoot())
            {
                lastTimeShot = Time.realtimeSinceStartup;

                ShootFunc(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (CanShoot())
            {
                lastTimeShot = Time.realtimeSinceStartup;

                ShootFunc(2);
            }
        }
    }

    private bool CanShoot()
    {
        if (lastTimeShot + mShootingDelay < Time.realtimeSinceStartup)
            return true;

        return false;

    }

    private void ShootFunc(int bulletType)
    {

        GameObject bullet = gameManager.Shoot(90.0f, bulletType, gameManager.GetFromPool());

        bullet.transform.position = gameObject.transform.position;

        Quaternion rotation = bullet.transform.rotation;

        rotation.z = 0;

        bullet.transform.rotation = rotation;

        bullet.GetComponent<BulletInformation>().InitOwner(gameObject);

        bullet.GetComponent<BulletInformation>().InitCollider(0);

        bullet.GetComponent<CollisionCallback>().CollisionFunction = null;

        bullet.GetComponent<BulletMovement>().setBulletIncrement(5);

        bullet.GetComponent<BulletMovement>().setBulletSpeed(7);

        bullet.GetComponent<BulletMovement>().Init();
    }

}

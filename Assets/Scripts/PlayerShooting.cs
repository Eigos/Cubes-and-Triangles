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

    [SerializeField] Button mButtonCircle = null;

    [SerializeField] Button mButtonBox = null;

    [SerializeField] Button mButtonTriangle = null;

    [SerializeField] Button mButtonAuto = null;


    void Start()
    {
        mSelectedBulletIndex = 0;

        mCanvasGame = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

#if  UNITY_EDITOR || UNITY_ANDROID
        InitPlayerShootAndroid();
#endif
    }


    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        PlayerShootPC();
#endif
    }
# if UNITY_EDITOR || UNITY_ANDROID
    #region ANDROID_METHODS

    private void InitPlayerShootAndroid()
    {
        if (mButtonAuto != null)
        {
            mButtonAuto.onClick.AddListener(ShootCasted);
        }
    }

    private void ShootCasted()
    {
        RaycastHit2D[] raycastAll = Physics2D.RaycastAll(transform.position, Vector2.up);
        RaycastHit2D hit;

        if(raycastAll.Length > 1)
        {
            hit = raycastAll[1];

            for(int i = 1; i < raycastAll.Length; i++)
            {
                RaycastHit2D ray = raycastAll[i];

                if(hit.distance > ray.distance)
                {
                    hit = ray;

                }
            }
        }
        else
        {
            ShootFunc(Random.Range(0, 3));
            Debug.DrawLine(transform.position, transform.position + new Vector3(0,Mathf.Infinity,0), Color.red, 20.0f);

            return;
        }

        if (hit.collider.gameObject.GetComponent<BulletInformation>() == null)
        {
            ShootFunc(Random.Range(0, 3));
            Debug.DrawLine(transform.position, hit.point, Color.red, 20.0f);

            return;
        }

        int hitBulletIndex = hit.collider.gameObject.GetComponent<BulletInformation>().GetBulletIndex() - 1;

        ShootFunc(hitBulletIndex);


    }

    #endregion
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
    #region STANDALONE_METHODS
    private void PlayerShootPC()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (CanShoot())
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
    #endregion
#endif

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

        bullet.GetComponent<BulletInformation>().InitOwner(gameObject);

        bullet.GetComponent<CollisionCallback>().CollisionFunction = null;

        bullet.GetComponent<BulletMovement>().setBulletSpeed(3.2f);

        bullet.GetComponent<BulletMovement>().Init();

        Quaternion rotation = bullet.transform.rotation;

        rotation.z = 0;

        bullet.transform.rotation = rotation;
    }

}

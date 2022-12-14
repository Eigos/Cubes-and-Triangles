using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
#E05A46
#BADB4F
#DB3995
#23DB87
#A42EDB
*/

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mWorldBoundLeft;

    [SerializeField] GameObject mWorldBoundRight;

    [SerializeField] GameObject mWorldBoundBottom;

    [SerializeField] GameObject mWorldBoundTop;

    [SerializeField] GameObject mCanvas;

    [SerializeField] GameObject mBulletPrefab;

    [SerializeField] List<BulletTypeScript> mBulletTypeList;

    [SerializeField] List<Material> mMaterialList;

    [SerializeField] int mReservedBulletAmount = 40;

    [SerializeField] GameObject mPlayer;

    [SerializeField] GameObject mParticleHit;

    [SerializeField] Material[] mParticleHitMaterials;

    [SerializeField] GameObject mPlayerHPVisualObj;

    [SerializeField] GameObject[] mTipsObj;

    private Canvas mCanvasGame;

    [SerializeField] float mBulletSpawnRate = 6.0f;

    [SerializeField] Text mScoreText;

    [SerializeField] GameObject mBulletsParent;

    [SerializeField] GameObject mParticlesParent;

    private float mPlayerHP = 100.0f;

    float mLastSpawnTime = 0;

    private GameObject mObjectPoolObj;

    private Queue<GameObject> mObjectPool;

    private GameObject mParticlePoolObj;

    private Queue<GameObject> mParticlePool;

    private bool mIsGameRunning = true;

    private int mShapeDifficulty = 3;

    int mPlayerScore = 0;

    // Start is called before the first frame update
    void Awake()
    {
        mWorldBoundTop.GetComponent<CollisionCallback>().CollisionFunction = TopBoundCollision;

        mWorldBoundBottom.GetComponent<CollisionCallback>().CollisionFunction = BottomBoundCollision;

        mCanvasGame = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        mPlayer.GetComponent<PlayerShooting>().ShootPlayer = Shoot;

        mPlayer.GetComponent<PlayerShooting>().GetBullet = GetFromPool;

        mPlayer.GetComponent<PlayerShooting>().gameManager = this;

        mPlayer.GetComponent<CollisionCallback>().CollisionFunction = PlayerCollision;

        mObjectPool = new Queue<GameObject>();

        mParticlePool = new Queue<GameObject>();

        mObjectPoolObj = new GameObject("ObjectPool");

        mParticlePoolObj = new GameObject("ParticlePool");

        float fadeTime = 5.0f;

        foreach (GameObject obj in mTipsObj)
        {
            Image image = obj.GetComponent<Image>();

            if (image)
            {
                image.CrossFadeAlpha(0, fadeTime, true);

                continue;
            }

            Text text = obj.GetComponent<Text>();

            if (text)
            {
                text.CrossFadeAlpha(0, fadeTime, true);

                continue;
            }
        }

        StartCoroutine(DeactiveAfterSeconds(mTipsObj[0], fadeTime));

    }

    void PlayerCollision(GameObject thisObj, GameObject collisionObj)
    {
        if (collisionObj.GetComponent<BulletInformation>().getOwner() != thisObj)
        {

            mPlayerHP -= 16.5f;

            UpdatePlayerHPVisual();

            StartCoroutine(ParticleHitSequence(GetFromParticlePool(), collisionObj));

            ReturnToPool(collisionObj);


        }
    }

    void BulletCollision(GameObject thisObj, GameObject collisionObj)
    {
        if (collisionObj.GetComponent<BulletInformation>() == null)
        {

            return;
        }

        if (thisObj.GetComponent<BulletInformation>().getOwner() != collisionObj.GetComponent<BulletInformation>().getOwner())
        {
            if (thisObj.GetComponent<BulletInformation>().GetBulletIndex() == collisionObj.GetComponent<BulletInformation>().GetBulletIndex())
            {

                mPlayerScore += 5;

                UpdateScoreText(mPlayerScore);

                StartCoroutine(ParticleHitSequence(GetFromParticlePool(), thisObj, collisionObj));

                ReturnToPool(thisObj);
                ReturnToPool(collisionObj);

                thisObj.GetComponent<CollisionCallback>().CollisionFunction = null;
                collisionObj.GetComponent<CollisionCallback>().CollisionFunction = null;

            }
            else
            {
                StartCoroutine(ParticleHitSequence(GetFromParticlePool(), collisionObj));

                ReturnToPool(collisionObj);
            }

        }

    }

    IEnumerator ParticleHitSequence(GameObject particleObject, GameObject destObj, GameObject collisionObject = null)
    {
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        particleObject.transform.SetParent(mParticlesParent.transform);
        particleObject.transform.localScale = Vector3.one;
        particleObject.transform.position = destObj.transform.position;
        particleObject.transform.SetAsLastSibling();
        Vector3 newPos = new Vector3(destObj.transform.position.x, destObj.transform.position.y, -5);
        particleObject.transform.position = newPos;

        if (collisionObject != null)
        {

            GradientColorKey[] colorKey = new GradientColorKey[2];
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            colorKey[0].color = destObj.GetComponent<BulletInformation>().getColor();
            colorKey[1].color = collisionObject.GetComponent<BulletInformation>().getColor();
            colorKey[0].time = 1.0f;
            colorKey[1].time = 1.0f;

            //alphaKey[0].alpha = 1.0f;
            //alphaKey[0].time = 0.5f;
            //alphaKey[1].alpha = 1.0f;
            //alphaKey[1].time = 1.0f;

            //particleSystem.main.startColor.gradientMax.alphaKeys = alphaKey;
            //particleSystem.main.startColor.gradientMax.colorKeys = colorKey;
            //
            //particleSystem.main.startColor.gradient.colorKeys = colorKey;
            //particleSystem.main.startColor.gradient.alphaKeys = alphaKey;

            ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(colorKey[0].color, colorKey[1].color);

            //gradient.mode = ParticleSystemGradientMode.RandomColor; NOT WORKING

            var main = particleSystem.main;
            main.startColor = gradient;

            particleObject.GetComponent<ParticleSystemRenderer>().material = mParticleHitMaterials[destObj.GetComponent<BulletInformation>().GetBulletIndex() - 1];

        }
        else
        {
            GradientColorKey colorKey = new GradientColorKey();
            colorKey.color = destObj.GetComponent<BulletInformation>().getColor();
            ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(colorKey.color, colorKey.color);

            var main = particleSystem.main;
            main.startColor = gradient;

            particleObject.GetComponent<ParticleSystemRenderer>().material = mParticleHitMaterials[destObj.GetComponent<BulletInformation>().GetBulletIndex() - 1];
        }

        particleSystem.Play();
        yield return new WaitUntil(() => !particleSystem.isPlaying);
        ReturnToParticlePool(particleObject);

    }

    IEnumerator DeactiveAfterSeconds(GameObject obj, float sec){
        yield return new WaitForSeconds(sec);
        obj.SetActive(false);
    }

    void UpdatePlayerHPVisual()
    {
        const float maxAlphaValue = 80.0f;
        float alphaVal = (100.0f /*max player hp*/ - mPlayerHP) * maxAlphaValue / 100.0f;
        Color color = mPlayerHPVisualObj.GetComponent<Image>().color;
        color.a = Mathf.Clamp(alphaVal, 0.0f, 80.0f) / 100.0f;
        mPlayerHPVisualObj.GetComponent<Image>().color = color;
    }

    void UpdateScoreText(int newScore)
    {
        mScoreText.text = "Score:" + newScore;
    }

    void TopBoundCollision(GameObject thisObj, GameObject collisionObj)
    {

        if (!collisionObj.gameObject.activeSelf)
            return;

        ReturnToPool(collisionObj);

        return;
    }


    void BottomBoundCollision(GameObject thisObj, GameObject collisionObj)
    {

        mPlayerHP -= 16.5f;

        Mathf.Clamp(mPlayerHP, 0, 100);

        UpdatePlayerHPVisual();

        if (!collisionObj.gameObject.activeSelf)
            return;

        ReturnToPool(collisionObj);

        return;
    }

    public GameObject Shoot(float direction, int bulletType = -1, GameObject existingBullet = null)
    {

        if (bulletType == -1)
        {
            bulletType = Random.Range(0, mBulletTypeList.Count);
        }

        GameObject newBullet;

        if (existingBullet == null)
        {
            newBullet = Instantiate<GameObject>(mBulletPrefab);
        }
        else
        {
            newBullet = existingBullet;
        }

        newBullet.GetComponent<BulletInformation>()
            .Init(mBulletTypeList[bulletType])
            .InitMaterial(mMaterialList[(int)Random.Range(0, mMaterialList.Count)])
            .InitCollider(bulletType);

        newBullet.transform.position = gameObject.transform.position;

        newBullet.transform.eulerAngles = new Vector3(0, 0, direction);

        newBullet.GetComponent<BulletMovement>().setAsDefault();

        newBullet.GetComponent<BulletMovement>().Init();

        newBullet.transform.SetParent(mBulletsParent.transform);

        newBullet.transform.SetAsLastSibling();

        newBullet.transform.localScale = new Vector3(1, 1, 1);

        //Init particle (trail) - Removed
        /* 
        ParticleSystem particleSystem = newBullet.GetComponent<ParticleSystem>();
        GradientColorKey colorKey = new GradientColorKey();
        colorKey.color = newBullet.GetComponent<BulletInformation>().getColor();
        ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(colorKey.color);

        var main = newBullet.GetComponent<ParticleSystem>().main;
        main.startColor = gradient;

        
        newBullet.GetComponent<ParticleSystemRenderer>().material = mParticleHitMaterials[newBullet.GetComponent<BulletInformation>().GetBulletIndex() - 1];
        */


        return newBullet;
    }

    private GameObject CreateParticleObject(Vector2 pos = new Vector2(), Texture2D texture = null)
    {
        GameObject newObj = Instantiate(mParticleHit);
        newObj.transform.position = new Vector3(pos.x, pos.y, 0);
        ParticleSystem.ShapeModule shape = newObj.GetComponent<ParticleSystem>().shape;

        if (texture != null)
        {
            shape.texture = texture;
        }


        return newObj;
    }


    void SpawnBullet()
    {

        GameObject newBullet = Shoot(-90, (int)Random.Range(0.0f, (float)mShapeDifficulty), GetFromPool());

        Vector3 bulletPos = new Vector3(
            Random.Range(newBullet.GetComponent<Image>().sprite.rect.width / 2,
            mCanvasGame.GetComponent<RectTransform>().rect.width - newBullet.GetComponent<Image>().sprite.rect.width / 2)
            - (mCanvasGame.GetComponent<RectTransform>().rect.width / 2),
            (mCanvasGame.GetComponent<RectTransform>().rect.height / 2) + 150, 0.0f);

        newBullet.transform.localPosition = bulletPos;

        newBullet.GetComponent<BulletMovement>().setAsDefault();

        newBullet.GetComponent<BulletMovement>().Init();

        newBullet.transform.eulerAngles = new Vector3(0, 0, -180);

        newBullet.GetComponent<BulletInformation>().InitOwner(gameObject);

        newBullet.GetComponent<CollisionCallback>().CollisionFunction = BulletCollision;

    }

    private void ReturnToPool(GameObject poolObject)
    {

        mObjectPool.Enqueue(poolObject);

        poolObject.transform.SetParent(mObjectPoolObj.transform);

        poolObject.SetActive(false);
    }

    private void ReturnToParticlePool(GameObject poolObject)
    {
        mParticlePool.Enqueue(poolObject);

        poolObject.transform.SetParent(mParticlePoolObj.transform);

        poolObject.SetActive(false);
    }

    private GameObject GetFromParticlePool()
    {
        if (mParticlePool.Count != 0)
        {
            GameObject existingParticle = mParticlePool.Dequeue();
            existingParticle.SetActive(true);
            return existingParticle;
        }

        return CreateParticleObject();
    }

    public GameObject GetFromPool()
    {
        if (mObjectPool.Count != 0)
        {
            GameObject existingBullet = mObjectPool.Dequeue();
            existingBullet.SetActive(true);
            return existingBullet;
        }

        return Shoot(0, 0);
    }

    private bool IsGameRunning()
    {
        return mIsGameRunning;
    }

    private void EndGame()
    {
        mIsGameRunning = false;
    }

    private void DisplayScoreboard()
    {

    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private float EaseInOutQuad(float x)
    {
        return x > 0.5 ? -2 * x * x + 1 : - (1 - Mathf.Pow(-2 * x + 2, 2) / 2) + 1;

    }

    private float EaseInSine(float x){
        return 1 - Mathf.Cos((x* Mathf.PI) / 2);
    }

    private void UpdateGameDifficultySpeed()
    {
        float timeMin = 0.0f;
        float timeMax = 60.0f * 10;

        //new block per given second
        float difficultyMin = 1.8f;  // lowest difficulty   
        float difficultyMax = 0.36f; // highest difficlty 


        float fx = EaseInOutQuad(Mathf.Clamp(Time.realtimeSinceStartup / timeMax, timeMin , timeMax));
        float newDifficulty = Mathf.Clamp(Remap(fx, 1, 0, difficultyMin, difficultyMax), difficultyMax, difficultyMin);

        mBulletSpawnRate = newDifficulty;
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsGameRunning())
            return;

        UpdateGameDifficultySpeed();

        if (mLastSpawnTime + mBulletSpawnRate < Time.realtimeSinceStartup)
        {
            SpawnBullet();
            mLastSpawnTime = Time.realtimeSinceStartup;
        }

        if (mPlayerHP < 100)
        {
            mPlayerHP += 10 * Time.deltaTime;

            UpdatePlayerHPVisual();
        }

        else if (mPlayerHP < 0)
        {
            EndGame();

            RestartGame();

            DisplayScoreboard();
        }
    }
}

enum BulletIndex
{
    NoIndex = -1,
    Circle = 0,
    Box,
    Triangle
}
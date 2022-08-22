using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletInformation : MonoBehaviour
{

    private int mBulletIndex = -1;

    private Collider2D mCollider = null;

    private int mColliderIndex = -1;

    private GameObject mOwner = null;

    public int GetBulletIndex()
    {
        return mBulletIndex;
    }

    public Color getColor()
    {
        return GetComponent<Image>().color;
    }

    void Start()
    {

    }

    public BulletInformation Init(BulletTypeScript bulletType)
    {
        mBulletIndex = bulletType.BulletIndex;
        GetComponent<Image>().sprite = bulletType.mSprite;

        return this;
    }

    public BulletInformation InitMaterial(Material material)
    {
        //GetComponent<Image>().material = material;
        GetComponent<Image>().color = material.color;

        return this;
    }

    public BulletInformation InitOwner(GameObject owner)
    {
        mOwner = owner;

        return this;
    }

    public BulletInformation InitCollider(int bulletIndex = -1)
    {
        if (bulletIndex == -1)
        {
            RemoveCollider();

            return this;
        }

        RemoveCollider();

        switch (bulletIndex)
        {

            //Circle
            case (int)BulletIndex.Circle:
                {
                    CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();

                    mCollider = collider;

                    collider.offset = new Vector2(0, 0);

                    collider.radius = GetComponent<RectTransform>().rect.width / 2;

                    collider.isTrigger = true;

                    mColliderIndex = bulletIndex;

                    break;
                }

            //Square
            case (int)BulletIndex.Box:
                {
                    BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();

                    mCollider = collider;

                    collider.offset = new Vector2(0, 0);

                    collider.size = new Vector2(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);

                    collider.isTrigger = true;

                    mColliderIndex = bulletIndex;

                    break;
                }

            //Triangle
            case (int)BulletIndex.Triangle:
                {
                    PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();

                    mCollider = collider;

                    collider.offset = new Vector2(0, 0);

                    float w = GetComponent<RectTransform>().rect.width;

                    float h = GetComponent<RectTransform>().rect.height;

                    //p0 : bottom left --- p1 : top middle --- p2 : bottom right
                    collider.points = new Vector2[] { new Vector2(-1 * (w / 2), -1 * (h / 2)), new Vector2(0, h / 2), new Vector2(w / 2, -1 * (h / 2)) };

                    collider.isTrigger = true;

                    mColliderIndex = bulletIndex;

                    break;
                }
        }

        return this;
    }

    void RemoveCollider()
    {
        if (mCollider == null)
        {
            return;
        }

        switch (mColliderIndex)
        {
            case (int)BulletIndex.Circle:
                {
                    Destroy(GetComponent<CircleCollider2D>());
                    break;
                }

            case (int)BulletIndex.Box:
                {
                    Destroy(GetComponent<BoxCollider2D>());
                    break;
                }

            case (int)BulletIndex.Triangle:
                {
                    Destroy(GetComponent<PolygonCollider2D>());
                    break;
                }
        }

        mColliderIndex = -1;
        mCollider = null;
    }

    public GameObject getOwner()
    {
        return mOwner;
    }
}

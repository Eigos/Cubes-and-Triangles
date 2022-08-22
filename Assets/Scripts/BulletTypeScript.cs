using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletType", menuName = "ScriptableObjects")]
public class BulletTypeScript : ScriptableObject
{
    public Sprite mSprite;

    public int BulletIndex = -1;
}

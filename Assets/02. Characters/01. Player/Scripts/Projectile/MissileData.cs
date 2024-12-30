using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissileData", menuName = "ScriptableObjects/MissileData", order = 2)]
public class MissileData : ScriptableObject
{
    public int missileID;
    public Sprite missileIcon;
    public bool isBoomActivate;
}
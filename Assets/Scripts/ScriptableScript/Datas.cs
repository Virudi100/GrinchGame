using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data")]
public class Datas : ScriptableObject 
{
    //C'est ici que le score et highScore est stocké

    public int score = 0;
    public int highScore = 0;
}

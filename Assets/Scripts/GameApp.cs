using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : Patterns.Singleton<GameApp>
{
    public bool MovementEnabled { get; set; } = false;
    public double SecondsSinceStart { get; set; } = 0;
    public int TotalCorrectTiles { get; set; } = 0;

    [SerializeField]
    List<string> jigsawnames = new List<string>();

    

    public string getJigsaw()
    {
        int num = Random.Range(1, 5);
        string name = jigsawnames[num];
        if(num == jigsawnames.Count)
        {
            num = 0;
        }
        return name;
    }
    
}

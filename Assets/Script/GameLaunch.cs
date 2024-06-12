using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLaunch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.AddComponent<ResMgr>();

        this.gameObject.AddComponent<GameApp>();

        GameApp.Instance.GameStart();
    }
}

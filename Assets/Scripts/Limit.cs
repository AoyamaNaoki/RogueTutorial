using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Limit : MonoBehaviour {

    float seconds = 10;
   // GameManager gameManager;

	// Use this for initialization
	void Start () {
       // gameManager = new GameManager();
	}
	
	// Update is called once per frame
	void Update () {


        seconds -= Time.deltaTime;
        this.GetComponent<Text>().text = "TimeLimit=" + seconds;
        if(seconds <= 0){
            GameManager.instance.GameOver();
        }
    }
}

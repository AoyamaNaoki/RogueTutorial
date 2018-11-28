using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Limit : MonoBehaviour {

    float seconds = 5;
    GameManager gameManager = new GameManager();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        seconds -= Time.deltaTime;
        this.GetComponent<Text>().text = "TimeLimit=" + seconds;
        if(seconds <= 0){
            gameManager.GameOver();
        }  else{
            return;
        }
    }
}

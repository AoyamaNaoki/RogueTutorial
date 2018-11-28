using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;       //Allows us to use Lists. 
using UnityEngine.UI;                   //Allows us to use UI.

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int playerFoodPoints = 100;                      //Starting value for Player food points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.


    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
    private int level = 1;                                  //Current level number, expressed in game as "Day 1".
    private List<Enemy> enemies;                          //List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;                             //Boolean to check if enemies are moving.
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
    
    private Text timeText;


    //Awake is always called before any Start functions - Start関数が呼び出される前に毎回呼び出される
    void Awake()
    {
        //  インスタンスが既に存在しているかチェックする
        if (instance == null)   //  もしもインスタンスがnullなら

            //  インスタンスにGameManagerに設定する
            instance = this;

        //  もしインスタンスが存在していてGameManagerでないなら
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //  シーンをリロードしたときにDontDestroyOnLoadに引数gameObjectを渡して実行
        DontDestroyOnLoad(gameObject);

        //  敵オブジェクトのリストに敵を割り当てる
        enemies = new List<Enemy>();

        //  boardScriptにBoardManagerコンポーネントを取得させる
        boardScript = GetComponent<BoardManager>();

        //  1stレベルを初期化するためにInit関数を呼び出す
        InitGame();
    }



    //  引数にScene scene, LoadSceneMode sceneModeを設定する
    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        level++;
        InitGame();
    }

    //　それぞれのレベルを初期化する
    void InitGame()
    {
        //  doingSetupにtrueを代入し、プレイヤーが動けないようにする
        doingSetup = true;

        //  evelImageに"LevelImage"の名前を見つけて代入する
        levelImage = GameObject.Find("LevelImage");

        //　LevelTextに"LevelText"を探してTextコンポーネントを代入する
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //  levelTextのtextに"day level"のようにレベル数を表示する
        levelText.text = "Day " + level;

        
        
        //　gameboardの設定中はレベルイメージを表示させる
        levelImage.SetActive(true);

        //  HideLevelImage関数をlevelStartDelay秒後に呼び出す
        Invoke("HideLevelImage", levelStartDelay);

        //　リストに入ってる敵をすべてクリアーする
        enemies.Clear();

        //　現在のレベル数をBoardManagerスクリプトのSetupScene関数を呼ぶ
        boardScript.SetupScene(level);

            

    }


    //  HideLevelImage関数の定義
    void HideLevelImage()
    {
        //　levelImageのアクティブをfalseに設定する
        levelImage.SetActive(false);

        //　doingSetupをfalseに設定する
        doingSetup = false;
    }

    void Start()//  スタート時に呼び出す
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Update()// 毎フレーム呼び出す
    {


        //　playersTurn、enemiesMovin、doingSetupがtrueなら
        if (playersTurn || enemiesMoving || doingSetup)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //　moveEnemies関数を呼び出す
        StartCoroutine(MoveEnemies());

        

    }

    //  Enemy型のscript変数を引数に関数を定義する
    public void AddEnemyToList(Enemy script)
    {
        //引数のscriptを渡してenemiesにaddする
        enemies.Add(script);
    }


    //GameOver is called when the player reaches 0 food points
    public void GameOver()
    {
        //Set levelText to display number of levels passed and game over message
        levelText.text = "After " + level + " days, you starved.";

        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }


}
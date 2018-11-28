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


    //Awake is always called before any Start functions - Start�֐����Ăяo�����O�ɖ���Ăяo�����
    void Awake()
    {
        //  �C���X�^���X�����ɑ��݂��Ă��邩�`�F�b�N����
        if (instance == null)   //  �������C���X�^���X��null�Ȃ�

            //  �C���X�^���X��GameManager�ɐݒ肷��
            instance = this;

        //  �����C���X�^���X�����݂��Ă���GameManager�łȂ��Ȃ�
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //  �V�[���������[�h�����Ƃ���DontDestroyOnLoad�Ɉ���gameObject��n���Ď��s
        DontDestroyOnLoad(gameObject);

        //  �G�I�u�W�F�N�g�̃��X�g�ɓG�����蓖�Ă�
        enemies = new List<Enemy>();

        //  boardScript��BoardManager�R���|�[�l���g���擾������
        boardScript = GetComponent<BoardManager>();

        //  1st���x�������������邽�߂�Init�֐����Ăяo��
        InitGame();
    }



    //  ������Scene scene, LoadSceneMode sceneMode��ݒ肷��
    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        level++;
        InitGame();
    }

    //�@���ꂼ��̃��x��������������
    void InitGame()
    {
        //  doingSetup��true�������A�v���C���[�������Ȃ��悤�ɂ���
        doingSetup = true;

        //  evelImage��"LevelImage"�̖��O�������đ������
        levelImage = GameObject.Find("LevelImage");

        //�@LevelText��"LevelText"��T����Text�R���|�[�l���g��������
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //  levelText��text��"day level"�̂悤�Ƀ��x������\������
        levelText.text = "Day " + level;

        
        
        //�@gameboard�̐ݒ蒆�̓��x���C���[�W��\��������
        levelImage.SetActive(true);

        //  HideLevelImage�֐���levelStartDelay�b��ɌĂяo��
        Invoke("HideLevelImage", levelStartDelay);

        //�@���X�g�ɓ����Ă�G�����ׂăN���A�[����
        enemies.Clear();

        //�@���݂̃��x������BoardManager�X�N���v�g��SetupScene�֐����Ă�
        boardScript.SetupScene(level);

            

    }


    //  HideLevelImage�֐��̒�`
    void HideLevelImage()
    {
        //�@levelImage�̃A�N�e�B�u��false�ɐݒ肷��
        levelImage.SetActive(false);

        //�@doingSetup��false�ɐݒ肷��
        doingSetup = false;
    }

    void Start()//  �X�^�[�g���ɌĂяo��
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void Update()// ���t���[���Ăяo��
    {


        //�@playersTurn�AenemiesMovin�AdoingSetup��true�Ȃ�
        if (playersTurn || enemiesMoving || doingSetup)

            //If any of these are true, return and do not start MoveEnemies.
            return;

        //�@moveEnemies�֐����Ăяo��
        StartCoroutine(MoveEnemies());

        

    }

    //  Enemy�^��script�ϐ��������Ɋ֐����`����
    public void AddEnemyToList(Enemy script)
    {
        //������script��n����enemies��add����
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
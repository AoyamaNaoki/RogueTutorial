using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{

    [System.Serializable]
    public class Count
    {
        public int minimum; //minimumをintで定義
        public int maximum; //maximumをintで定義

        public Count(int min, int max)  //minimum,maximumに数値を代入するメソッド
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8; //8列の定義
    public int rows = 8;    //8行の定義
    public Count wallCount = new Count(5, 9);   //	wallCountをminimum=5,maximum=9と定義
    public Count foodCount = new Count(1, 5);   //	foodCountをminimum=1,maximum=5と定義
    public GameObject exit;                     //	exitを定義
    public GameObject[] floorTiles;             //	floorTilesを定義			
    public GameObject[] wallTiles;              //	wallTilesを定義
    public GameObject[] foodTiles;              //	foodTilesを定義
    public GameObject[] enemyTiles;             //	enemyTilesを定義	
    public GameObject[] outerWallTiles;         //	outerWallTilesを定義

    private Transform boardHolder;              //	Transform型のboardHolderを定義
    private List<Vector3> gridPositions = new List<Vector3>();  //	Vector3型で動的な配列のgridPositionsを定義

    void InitialiseList()   //	InitialiseListメソッドを定義
    {
        gridPositions.Clear();  //	gridPositions(list)の要素を削除

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)  //	(1,1),(1,2)...とループさせたx,yをgridPositionsに要素を加える
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()   //	BoardSetupメソッドを定義
    {
        boardHolder = new GameObject("Board").transform;    //	Transform型のboardHolderにboardインスタンスの座標を代入

        for (int x = -1; x < columns + 1; x++)  //	xを列以下ならループさせる
        {
            for (int y = -1; y < rows + 1; y++) //	yを行以下ならをループさせる
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];  //	GameObject型のtoInstantiateにGameObject型で配列のfloorTilesの0~floorTilesの要素数でランダムな値を代入する
                if (x == -1 || x == columns || y == -1 || y == rows)    //	もしxが-1	または	xが列と同数　または	yが-1	または	yが行と同数なら
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];	//	toInstantiateにGameObject型で配列のouterWallTilesの0~outerWallTilesの要素数でランダムな値を代入する
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //	GameObject型のinstanceをtoInsntiateを座標(x,y)回転なしで生成しGameObject型にキャストする
                instance.transform.SetParent(boardHolder);  //	instanceの座標にboardHolderを親として親の座標を設定する?
            }
        }
    }

    Vector3 RandomPosition()    //	Vector3型のRandomPositionメソッドを定義
    {
        int randomIndex = Random.Range(0, gridPositions.Count); //	int型のrandomIndexに0~gridPositionsの要素数を取得する
        Vector3 randomPosition = gridPositions[randomIndex];    //	Vector3型のRandomPositioにList型のgridPositionsにrandomIndexの値を代入する
        gridPositions.RemoveAt(randomIndex);                    //	List型のgridPositionsからrandomindexの数値のインデックス数の項目を取り除く
        return randomPosition;                                  //	RandoPosition(L69)を戻り値に設定する
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) //引数にGameObject型のtileArray配列、int型のminimum,int型のmaximumを指定してLayoutObjectAtRandomメソッドを定義
    {
        int objectCount = Random.Range(minimum, maximum + 1);   //	int型のobjectCountにminimum~maximum+1のランダムな数値を代入

        for (int i = 0; i < objectCount; i++)   //	iをobjectCountまでループさせる
        {
            Vector3 randomPosition = RandomPosition();  //	Vector3型のrandomPositionにRandomPositionメソッドの戻り値を代入する
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];   //	GameObject型のtileChoiceにtileArray(第1引数)の0~要素数でランダムなインデックス番号のGameObjectを代入
            Instantiate(tileChoice, randomPosition, Quaternion.identity);   //	tileChoiceをrandomPositionで回転させずに生成する
        }
    }

    public void SetupScene(int level)   //	引数にint型のlevelを指定してSetupSceneメソッドを定義する
    {
        BoardSetup();                   //	BoardSetupメソッドを実行する
        InitialiseList();               //	InitialiseListメソッドを実行する(初期化)
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);  //	LayoutObjectAtRandomメソッドにwallTiles,wallCountのminimum,wallCountのmaximumを引数として実行
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);  //	
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready=0,
    Runing = 1,
    GameOver = 2,
}


public class Ryunm_GameManager : MonoBehaviour
{
    public static Ryunm_GameManager _instance;//静态的Public变量，可以直接通过类名Ryunm_GameManager调用

    public GameObject[] fruits;//声明出来的对象，要想使用，需要先实例化/绑定
    //public Transform startPos;
    //public int randomSeed = 42;
    public bool collectData = false;
    public TextMeshProUGUI time;
    public TextMeshProUGUI score;
    public TextMeshProUGUI game;
    public Transform cover;

    public GameState gameState = GameState.Ready;

    //UI
    //分数
    public float currentScore = 0;
    public TextMeshProUGUI currentScoreTxt;
    //public Text highestScoreTxt;
    public Button startBtn;

    public TextMeshProUGUI ID;

    //声音
    public AudioSource floorAudio;
    public AudioSource combineAudio;

    private GameObject fruit1;
    private GameObject fruit2;
    private GameObject fruit3;
    private GameObject fruit4;
    private GameObject fruit5;

    private int clickingNumber;
    private Telemetry.ClickData clickData;
    private float startTime;

    private int[] sequence = new int[5];
    private int index;

    public static int gameType = 0;
    public static int randomSeed = 42;

    private string type = "A";

    //会在start方法之前执行
    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {

        //foreach (var number in new int[] { 0, 0, 1, 1, 2 })
        //{
        //    Debug.Log(number + 1);
        //}

        if (gameType == 0)
        {
            cover.gameObject.SetActive(true);
            collectData = false;
            Random.InitState((int)Time.time);
            game.text = "Game 0";
        }
        else if (gameType == 1)
        {
            cover.gameObject.SetActive(true);
            collectData = true;
            type = "A";
            Random.InitState(42);
            game.text = "Game A";
        }
        else
        {
            cover.gameObject.SetActive(false);
            collectData = true;
            type = "B";
            Random.InitState(42);
            game.text = "Game B";
        }

        //Random.InitState(randomSeed);

        startTime = Time.time;
        clickData.startTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ID.text = "ID: \n" + clickData.startTime;

        //Step1:在上方随机生成待命的水果
        fruit1 = CreateNewFruit(0, new Vector3(0, 4.35f, 0));
        fruit2 = CreateNewFruit(0, new Vector3(4, 4f, 0));
        fruit3 = CreateNewFruit(1, new Vector3(5.3f, 4f, 0));
        fruit4 = CreateNewFruit(1, new Vector3(6.6f, 4f, 0));
        fruit5 = CreateNewFruit(2, new Vector3(7.9f, 4f, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (gameState != GameState.Runing) return;
        //
        if (fruit1 != null)
        {
            Vector3 mousePosition = Input.mousePosition;//鼠标的屏幕坐标
            Vector3 mouseUnityWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);//屏幕坐标转换成Unity世界坐标
            if (Input.GetMouseButton(0) && mouseUnityWorldPos.x <= 3 && mouseUnityWorldPos.x >= -3)
            {
                //鼠标X的位置变化，影响水果X的位置
                
                
                fruit1.transform.position = new Vector3(mouseUnityWorldPos.x, fruit1.transform.position.y, fruit1.transform.position.z);
            }
            if (Input.GetMouseButtonUp(0) && mouseUnityWorldPos.x <= 3 && mouseUnityWorldPos.x >= -3)
            {
                clickingNumber += 1;
                if (collectData)
                {
                    clickData.clickingNumber = clickingNumber;
                    clickData.coordinate = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
                    clickData.score = (int)currentScore;

                    StartCoroutine(Telemetry.SubitGoogleForm(clickData, type));
                }
                

                fruit1.GetComponent<Rigidbody2D>().gravityScale = 1;
                fruit1.GetComponent<Ryunm_Fruit>().fruitState = FruitState.Falling;
                fruit1 = null;


                Invoke("UpdateFruit", 1);//延迟一秒创建新的水果
            }
        }

        int nowTime = (int)(Time.time - startTime);
        string seconds = (nowTime % 60) < 10 ? "0" + (nowTime % 60).ToString() : (nowTime % 60).ToString();
        string minutes = (nowTime / 60) < 10 ? "0" + (nowTime / 60).ToString() : (nowTime / 60).ToString();
        time.text = minutes + ":" + seconds;
    }

    public void UpdateFruit()
    {
        fruit1 = fruit2;
        fruit1.transform.position = new Vector3(0, 4.35f, 0);
        fruit2 = fruit3;
        fruit2.transform.position = new Vector3(4, 4f, 0);
        fruit3 = fruit4;
        fruit3.transform.position = new Vector3(5.3f, 4f, 0);
        fruit4 = fruit5;
        fruit4.transform.position = new Vector3(6.6f, 4f, 0);


        int randomValue = Random.Range(0, 5);//0~5;取头不取尾；取随机数
        //Debug.Log(randomValue + 1);
        fruit5 = CreateNewFruit(randomValue, new Vector3(7.9f, 4f, 0));

        
    }
    public void GameOver()
    {
        //储存一个最高分
        //float historyHighestScore = PlayerPrefs.GetFloat("HCDXG_HighestScore");
        //if(currentScore> historyHighestScore)
        //{
        //    PlayerPrefs.SetFloat("HCDXG_HighestScore", currentScore);
        //}

        //if (startBtn)
        //{
        //    startBtn.gameObject.SetActive(true);
        //}

        ////延迟一秒调用ReloadScene
        //Invoke("ReloadScene",1);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene("hcdxg_0705");
    }

    GameObject CreateNewFruit(int index, Vector3 startPos)
    {
        gameState = GameState.Runing;

        GameObject fruitPrefabs = fruits[index];
        GameObject newFruit = Instantiate(fruitPrefabs, startPos, fruitPrefabs.transform.rotation);

        //设置重力
        Rigidbody2D rigid = newFruit.GetComponent<Rigidbody2D>();
        if (rigid)
        {
            rigid.gravityScale = 0;
        }

        newFruit.GetComponent<Ryunm_Fruit>().fruitState = FruitState.Standby;
        return newFruit;
    }

    public void CombineNewFruit(FruitType currentType,Vector2 pos)
    {
        //如果是最大的西瓜，那么不合成
        if (currentType == FruitType.Eleven) return;

        //合成新的水果
        int index = (int)currentType + 1;
        GameObject fruitObj = fruits[index];
        GameObject newFruit = Instantiate(fruitObj, pos, fruitObj.transform.rotation);

        //计算合成得分
        currentScore += (int)currentType+1;
        if (currentScoreTxt)
        {
            currentScoreTxt.text = currentScore.ToString();
            score.text = currentScoreTxt.text;
        }
        PlayCombineAudio();
    }

    public void PlayFloorAudio()
    {
        if (floorAudio)
        {
            floorAudio.Play();
        }
    }
    public void PlayCombineAudio()
    {
        if (combineAudio)
        {
            combineAudio.Play();
        }
    }

    public void GameB()
    {
        gameType = 2;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameA()
    {
        gameType = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Game0()
    {
        gameType = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}

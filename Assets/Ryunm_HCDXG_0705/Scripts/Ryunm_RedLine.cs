using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ryunm_RedLine : MonoBehaviour
{
    public bool isOver = false;
    public float speed = -2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOver)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            if (collision.gameObject.GetComponent<Ryunm_Fruit>().fruitState == FruitState.Collisioned)
            {
                //游戏结束
                Ryunm_GameManager._instance.gameState = GameState.GameOver;
                Invoke("SetOver",1);
            }
            //游戏结束的检测
            if (Ryunm_GameManager._instance.gameState == GameState.GameOver && isOver)
            {

                //最终奖励得分
                Ryunm_GameManager._instance.currentScore += (int)collision.gameObject.GetComponent<Ryunm_Fruit>().fruitType + 1;
                Ryunm_GameManager._instance.currentScoreTxt.text = Ryunm_GameManager._instance.currentScore.ToString();
                Ryunm_GameManager._instance.score.text = Ryunm_GameManager._instance.currentScoreTxt.text;

                Destroy(collision.gameObject);//销毁水果
            }
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            //所有的逻辑结束
            Ryunm_GameManager._instance.GameOver();
        }
    }
    void SetOver()
    {
        isOver = true;
    }
}

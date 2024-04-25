using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//声明水果类型的枚举
public enum FruitType
{
    One = 0,
    Two = 1,
    Three = 2,
    Four = 3,
    Five = 4,
    Six = 5,
    Seven = 6,
    Eight = 7,
    Nine = 8,
    Ten = 9,
    Eleven = 10,
}

public enum FruitState
{
    Defind=0,
    Standby=1,
    Falling = 2,
    Collisioned = 3
}

//继承基类
public class Ryunm_Fruit : MonoBehaviour
{
    //Unity的特性--可视化
    //在MonoBehaviour脚本种Public声明的变量，可以在Unity的检查器视图种查看，编辑
    public FruitType fruitType = FruitType.One;
    public float x_Limit = 2.5f;
    public FruitState fruitState = FruitState.Defind;

    // Start is called before the first frame update
    //Ryunm_Fruit脚本组件启用的时候会执行一次；
    void Start()
    {
        
    }

    // Update is called once per frame
    //更新函数，每帧执行一次
    void Update()
    {
        //if (this.gameObject.transform.position.x < -x_Limit)
        //{
        //    this.gameObject.transform.position = new Vector3(-x_Limit, transform.position.y, transform.position.z);
        //}
        //if (this.gameObject.transform.position.x > x_Limit)
        //{
        //    this.gameObject.transform.position = new Vector3(x_Limit, transform.position.y, transform.position.z);
        //}
    }

    //当前脚本组件挂载的游戏对象碰撞到collision碰撞体的时候，会执行一次
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //判断碰撞体collision是否为水果
        if (collision.gameObject.CompareTag("Fruit"))
        {
            //判断是否是一样的水果；自己的fuitType
            Ryunm_Fruit collisionFruit = collision.gameObject.GetComponent<Ryunm_Fruit>();
            if (collisionFruit)
            {
                if (collisionFruit.fruitType == fruitType)
                {
                    //合成大一号的水果：1、fruitType；2、合成的位置Position
                    //注意！！！！！两个水果都会进行碰撞检测；所以说需要限制执行合成逻辑的只能是其中一个
                    float pos_xy = this.transform.position.x + this.transform.position.y;
                    float collision_xy = collision.transform.position.x + collision.transform.position.y;
                    if (pos_xy> collision_xy)
                    {
                        Vector2 pos = (this.transform.position + collision.transform.position) / 2;
                        Ryunm_GameManager._instance.CombineNewFruit(fruitType, pos);
                    }

                    Destroy(this.gameObject);//销毁自身
                }
            }
        }

        //碰撞到任何碰撞体
        fruitState = FruitState.Collisioned;

        if (fruitState == FruitState.Collisioned)
        {
            if (collision.gameObject.CompareTag("Floor"))
            {
                Ryunm_GameManager._instance.PlayFloorAudio();
            }
        }
    }
}

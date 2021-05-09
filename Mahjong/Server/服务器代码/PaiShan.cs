using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mahjong_server
{
    class Paishan
    {
        //定义三个数组，用来组合麻将牌
        static int[] num = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        static String[] name = new String[] { "条", "筒", "万" };
        static String[] tablet = new String[] { "东风", "南风", "西风", "北风", "红中", "发财", "白板" };

        //用来存放牌
        public Hashtable HashTable1 = new Hashtable();
        //用来记录麻将存放个数，和次序
        List<int> number = new List<int>();
        //定义四个存放玩家牌的数组
        List<int> player1 = new List<int>();
        List<int> player2 = new List<int>();
        List<int> player3 = new List<int>();
        List<int> player4 = new List<int>();
        //用来存放底牌
        List<int> basedd = new List<int>();

        //记录摸牌的位置
        int n = 0;

        static List<int> Shuffle(List<int> arr)
        {
            List<int> oldArr = new List<int>(arr);
            //创建接收数组
            Random random = new Random();
            //for (int j = 0; j < 2; j++)
            //{//多次洗牌
            for (int i = 0; i < oldArr.Count; i++)
            {//洗牌操作：让两个随机下
             //标的元素交换位置，执行次数为数组中元素个数。
                int r1 = random.Next(0, oldArr.Count);
                int r2 = random.Next(0, oldArr.Count);
                int temp = (int)oldArr[r1];
                oldArr[r1] = oldArr[r2];
                oldArr[r2] = temp;
            }
            //}
            //使用新数组接收交换位置的数组。
            List<int> newArr = new List<int>(oldArr);
            //返回数据
            return newArr;
        }                   //shuffle打乱List<int>函数

        void Start()                                          //init
        {
            //记录牌的编号
            int k = 0;

            //准备牌,for循环添加条，筒，万
            for (int i = 0; i < name.Length; i++)
            {
                for (int j = 0; j < num.Length; j++)
                {
                    String s = num[j] + name[i];
                    for (int l = 0; l < 4; l++)
                    {
                        number.Add(k);
                        HashTable1.Add(k++, s);
                        //将牌的编号加入number中
                    }
                }
            }
            //for循环添加东南西北中白板发财
            for (int i = 0; i < tablet.Length; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    number.Add(k);
                    HashTable1.Add(k++, tablet[i]);
                }
                
            }
            Console.WriteLine(HashTable1.Count);
            /*Console.WriteLine("看牌: ");
           
            for (int i = 0; i < 136; i++)
            {
                Console.Write(HashTable1[number[i]] + "" + number[i] + " ");        //
                if (i % 4 == 3)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine();*/
            
        }

        void Licensing()                                      //发牌初始化
        {
            Console.WriteLine("将牌打乱顺序: ");
            //集合提供方法，将一个整形数组打乱
            number = Paishan.Shuffle(number);
            for (int i = 0; i < number.Count; i++)
            {
                Console.Write(HashTable1[number[i]] + " ");        //
            }

            //发牌
            //总共发三次，每一次每人摸四张牌,
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    player1.Add(number[n++]);
                    player2.Add(number[n++]);
                    player3.Add(number[n++]);
                    player4.Add(number[n++]);
                }
            }
            //庄家摸两次牌,其他人摸一张(玩家1是庄家)
            player1.Add(number[n++]);
            /*需要跳牌（庄家第一次和第二次摸的位置索引值差4）
            n = n + 4;
            player1.Add(number[n]);
            //回到庄家第一次摸牌的位置的下一个
            n = n - 3;*/
            player2.Add(number[n++]);
            player3.Add(number[n++]);
            player4.Add(number[n++]);
            n++;
            //剩下的底牌
            for (int i = n; i < number.Count; i++)
            {
                basedd.Add(number[i]);
            }
            //Console.WriteLine(n);

            //对玩家手里的牌进行排序,集合提供排序方法
            player1.Sort();
            player2.Sort();
            player3.Sort();
            player4.Sort();

            //看牌 ，  4个for循环遍历
            Console.WriteLine("看牌 :");
            Console.Write("player1 : ");
            for (int i = 0; i < player1.Count; i++)
            {
                Console.Write(HashTable1[player1[i]] + " ");        ////
            }
            Console.WriteLine();
            Console.Write("player2 : ");
            for (int i = 0; i < player2.Count; i++)
            {
                Console.Write(HashTable1[player2[i]] + " ");
            }
            Console.WriteLine();
            Console.Write("player3 : ");
            for (int i = 0; i < player3.Count; i++)
            {
                Console.Write(HashTable1[player3[i]] + " ");
            }
            Console.WriteLine();
            Console.Write("player4 : ");
            for (int i = 0; i < player4.Count; i++)
            {
                Console.Write(HashTable1[player4[i]] + " ");
            }
            Console.WriteLine();
            Console.Write("底牌 : ");
            for (int i = 0; i < basedd.Count; i++)
            {
                Console.Write(HashTable1[basedd[i]] + " ");
            }
            Console.WriteLine();
        }

        bool empty_hand()                                      //判断底牌是否为空
        {
            if (basedd.Count == 0)
            {
                return true;
            }
            else return false;
        }

        int size_hand()                                       //返回底牌数量
        {
            return basedd.Count;
        }

        int get_next()                                        //玩家摸牌
        {
            int temp = basedd[0];
            basedd.Remove(basedd[0]);
            return temp;
        }

        public void init()
        {
            /*初始化*/
            Start();
            Licensing();

        }
        public int getNext()
        {
            return get_next();
        }

        public bool empty()
        {
            return empty_hand();
        }

        public int size()
        {
            return size();
        }

        public List<int>[] getPlayerPai()
        {
            List<int>[] res = new List<int>[4];
            res[0] = player1;
            res[1] = player2;
            res[2] = player3;
            res[3] = player4;
            return res;
        }
    }
}

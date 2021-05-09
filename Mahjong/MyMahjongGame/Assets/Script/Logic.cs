using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mahjong
{
    class Hu
    {
        public static bool Eat(List<int> ma,int ID)                              //能否吃牌,有三种情况,别人的牌
        {
            if (ID >= 30 && ID <= 35) {                      //东南西北、中发白不存在吃行为
                return false;
            }

            if(ma.Contains(ID+1) && ma.Contains(ID+2)){
                return true;
            }
            else if(ma.Contains(ID - 1) && ma.Contains(ID - 2)){
                return true;
            }
            else if(ma.Contains(ID+1) && ma.Contains(ID - 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<int> Order_eat(List<int> ma,int ID)                       //进行吃牌操作
        {
            List<int> temp = new List<int>();
            if (ma.Contains(ID + 1) && ma.Contains(ID + 2))
            {
                temp.Add(ID + 1);
                temp.Add(ID + 2);
            }
            if (ma.Contains(ID - 1) && ma.Contains(ID - 2))
            {
                temp.Add(ID - 2);
                temp.Add(ID - 1);
            }
            if (ma.Contains(ID + 1) && ma.Contains(ID - 1))
            {
                temp.Add(ID - 1);
                temp.Add(ID + 1);
            }
            /*输出所有可以吃牌的选项*/
            /*for (int i=0;i<temp.Count/2;i++)                 
            {
                Console.WriteLine(temp[i*2] + " " + temp[i*2+1]);
            }*/
            return temp;
        }

        public static List<int> Make_eat(List<int> ma,int ID,List<int> temp,int order_num){
            /*需要进行选择*/
            Console.WriteLine("选择吃牌类型:");
            order_num = Convert.ToInt32(Console.ReadLine());
            //进行吃牌操作并添加到吃牌序列中);
            ma.Remove(temp[2 * order_num]);
            ma.Remove(temp[2 * order_num + 1]);
            return ma;
        }
        public static bool Touch(List<int> ma,int ID)                           //能否碰牌(判断三张相同),并添加到碰牌序列,别人的牌
        {

            List<int> ds = ma.FindAll(delegate (int d)
            {
                return d==ID;
            });
            if (ds.Count == 2)                              //如果相同牌存在两张以及以上
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<int> Order_touch(List<int> ma,int ID)                     //执行碰牌操作
        {
            //碰牌
            ma.Remove(ID);
            ma.Remove(ID);
            ma.Remove(ID);
            return ma;
        }

        public static bool Bar(List<int> ma,int ID)                             //判断杠牌,别人的牌或者自己的
        {
            List<int> ds = ma.FindAll(delegate (int d)
            {
                return d == ID;
            });
            if (ds.Count == 3)                              //如果相同牌存在两张以及以上
            {
                return true;
            }
            else
            {
                return false;
            }
        }                          //能否杠牌

        public static List<int> Order_Bar(List<int> ma, int ID)                       //执行杠牌操作
        {
            ma.Remove(ID);
            ma.Remove(ID);
            ma.Remove(ID);
            ma.Remove(ID);
            return ma;
            //从牌堆摸牌
        }
        /*
        public static bool Show(int ID)                            //存在中发白或者东南西北,自己的牌
        {
            List<int> temp = new List<int>(player1);
            temp.Add(ID);
            if (temp.Contains(31) && temp.Contains(32) && temp.Contains(33) && temp.Contains(30))
            {
                return true;
            }
            else if (temp.Contains(34) && temp.Contains(35) && temp.Contains(36))
            {
                return true;
            }
            else return false;
        }

        public static void Order_show(int ID)
        {
            List<int> temp = new List<int>(player1);
            player1.Add(ID);
            //存在东南西北
            if (player1.Contains(31) && player1.Contains(32) && player1.Contains(33) && player1.Contains(30))
            {
                Show_p.Add(30);Show_p.Add(31);
                Show_p.Add(32);Show_p.Add(33);
                //需要摸牌
            }
            if (player1.Contains(34) && player1.Contains(35) && player1.Contains(36))
            {
                Show_p.Add(34);Show_p.Add(35);Show_p.Add(36);
            }
            
        }
*/
        public static bool Seven_Hu(List<int> ma, int ID)                        //七小对胡,别人的牌或者自己的
        {
            List<int> temp = new List<int>(ma);
            temp.Add(ID);
            temp.Sort();
            if (temp.Count == 14)
            {
                for (int i = 0; i < temp.Count; i+=2)
                {
                    if (temp[i] != temp[i + 1])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static int Merge(List<int> m,int ID,List<int> ma)                //牌合并，看自己的牌，0为缺门，1为正常胡，2为清一色
        {
            List<int> temp_1 = new List<int>(m);
            temp_1.Add(ID);
            for(int i=0;i<ma.Count;i++){
                temp_1.Add(ma[i]);
            }
            temp_1.Sort();
            if (One_n(temp_1))
            {
                return Hu_setting(temp_1);
            }
            return 0;
        }

        public static bool One_n(List<int> m)                       //牌中是否存在1-9
        {
            int bet = m.Count;
            for (int i = 0; i < bet; i++)
            {
                if (m[i] % 10 == 0 || m[i] % 10 == 8 || (m[i] >= 30 && m[i] <= 36))
                {
                    return true;
                }
            }
            return false;
        }

        public static int Hu_setting(List<int> m)
        {
            int bet = m.Count;
            int flag1 = 0, flag2 = 0, flag3 = 0;
            if (m[0]+8>=m[bet-1])
            {
                return 2;
            }
            for (int i = 0; i < bet; i++)
            {
                if (m[i] / 10 == 0 ){
                    flag1 = 1;
                }
                if (m[i] / 10 == 1)
                {
                    flag2 = 1;
                }
                if (m[i] / 10 == 2)
                {
                    flag3 = 1;
                }
            }
            if (flag1 + flag2 + flag3 == 3){
                return 1;
            }
            return 0;
        }

        public static bool IsCanHU(List<int> ma,int ID,List<int> mmmm)     //正常胡牌方式,别人的牌或者自己的
        {
            Debug.Log("IF HuPai");
            List<int> pais = new List<int>(ma);

            if (Seven_Hu(ma,ID) == true){
                return true;
            }

            pais.Add(ID);
            //只有两张牌
            if (pais.Count == 2)                                    //剩余牌数为2张，判断是否为1对
            {
                //if (Merge(ma,ID,mmmm) != 0)
                //{
                    return pais[0] == pais[1];
                 // }
                //else return false;
            }

            //先排序
            pais.Sort();

            //依据牌的顺序从左到右依次分出1小对
            for (int i = 0; i < pais.Count; i++)
            {
                List<int> paiT = new List<int>(pais);
                List<int> ds = pais.FindAll(delegate (int d)
                {
                    return pais[i] == d;
                });

                //判断是否能做将牌
                if (ds.Count >= 2)
                {
                    //移除两张将牌
                    paiT.Remove(pais[i]);
                    paiT.Remove(pais[i]);

                    //避免重复运算 将光标移到其他牌上
                    i += ds.Count;

                    if (HuPaiPanDin(paiT))
                    {
                        //if (Merge(ma,ID,mmmm) != 0)
                        //{
                            return true;
                       // }
                        //else return false;
                    }
                }
            }
            return false;
        }

        public static bool HuPaiPanDin(List<int> mas)
        {
            if (mas.Count == 0)//数量为0则胡牌
            {
                return true;
            }

            List<int> fs = mas.FindAll(delegate (int a)
            {
                return mas[0] == a;
            });

            //组成碰
            if (fs.Count == 3)
            {
                mas.Remove(mas[0]);
                mas.Remove(mas[0]);
                mas.Remove(mas[0]);

                return HuPaiPanDin(mas);
            }
            else
            {  
                //存在东南西北且不构成碰
                if(mas[0]>=30&&mas[0]<=36)
                {
                    return false;
                }
                //组成顺子
                if (mas.Contains(mas[0] + 1) && mas.Contains(mas[0] + 2))
                {
                    //进行吃牌操作并添加到吃牌序列中
                    mas.Remove(mas[0] + 2);
                    mas.Remove(mas[0] + 1);
                    mas.Remove(mas[0]);
                    return HuPaiPanDin(mas);
                }
                return false;
            }
        }

        public static bool stop(List<int> ma,int ID,List<int> mmm)                          //听牌,自己的
        {
            for (int j = 0; j < ma.Count; j++)
            {
                List<int> temp = new List<int>(ma);
                temp.Add(ID);
                temp.Remove(temp[j]);                                 //剔除此牌可以使用户胡牌
                temp.Sort();
                for (int i = 0; i <= 36; i++)
                {
                    if (i % 10 != 9)
                    {
                        if (IsCanHU(temp,i,mmm) == true)                  //该牌可是使用户胡牌
                        {
                            Console.WriteLine(ma[j]);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int stop_Order(List<int> ma,int ID,List<int> mmm)                          //听牌,自己的
        {
            for (int j = 0; j < ma.Count; j++)
            {
                List<int> temp = new List<int>(ma);
                temp.Add(ID);
                temp.Remove(temp[j]);                                 //剔除此牌可以使用户胡牌
                temp.Sort();
                for (int i = 0; i <= 36; i++)
                {
                    if (i % 10 != 9)
                    {
                        if (IsCanHU(temp,i,mmm) == true)                  //该牌可是使用户胡牌
                        {
                            return ma[j];
                        }
                    }
                }
            }
            return -1;
        }
        
        /*public static void Main(String[] args)
        {
            player1 = new List<int>(){  2,3,4, 12, 12, 12, 12, 13,14,22,22,22,30 };
            int a =22;
            
            //判断能否七小对,测试
            if(Seven_Hu(a)==true)
            {
                Console.WriteLine("可以七小对胡牌");
            }
            
            //判断可以碰牌
            if(Hu.Touch(a)==true)
            {
                Console.WriteLine("可以碰牌");
                //控件选择
                Order_touch(a);
            }
            
            //判断杠牌
            if(Hu.Bar(a)==true)
            {
                Console.WriteLine("可以碰、杠牌");
                //控件选择
                Bar_choice(a);
            }
            
            //判断听牌
            if (Hu.stop(player1,a))
            {
                Console.WriteLine("可以听牌");
            }
            
            //判断能否吃牌
            if(Hu.Eat(a)==true)
            {
                Console.WriteLine("可以吃牌");
                //控件选择
                Order_eat(a);
            }
            
            //判断能否亮牌
            if (Hu.Show(a) == true)
            {
                Console.WriteLine("可以亮牌");
                //控件选择
                Order_show(a);
            }

            //吃牌序列
            for (int i=0;i<Eat_p.Count;i++)
            {
                Console.Write(Eat_p[i] + " ");
            }
            Console.WriteLine();
            //碰序列
            for (int i=0; i < Touch_p.Count; i++)
            {
                Console.Write(Touch_p[i] + " ");
            }
            Console.WriteLine();
            //玩家手牌序列
            for (int i=0; i < player1.Count; i++)
            {
                Console.Write(player1[i] + " ");
            }
            Console.WriteLine();
            //东南西北、中发白序列
            for (int i = 0; i < Show_p.Count; i++)
            {
                Console.Write(Show_p[i] + " ");
            }
            Console.WriteLine();
            for (int i = 0; i < Bar_Cards.Count; i++)
            {
                Console.Write(Bar_Cards[i] + " ");
            }
            Console.WriteLine();
        }*/

        public static List<int> Transform1(List<int> ma){                //由显示到胡牌   
            List <int> temp = new List<int>();
            for(int i = 0; i < ma.Count; i++){
                int aaa = ma[i]/4;
                if (aaa >= 0 && aaa <= 8) { }
                else if (aaa <= 17){
                    aaa += 1;
                }
                else if (aaa <= 26){
                    aaa += 2;
                }
                else aaa += 3;
                temp.Add(aaa);
            }
            return temp;
        }

        public static List<int> Transform2(List<int> ma){                 //由胡牌到显示
            List<int> temp = new List<int>();
            for (int i = 0; i < ma.Count; i++)
            {
                int aaa = ma[i];
                if (aaa >= 0 && aaa <= 8) { }
                else if (aaa <= 18)
                {
                    aaa -= 1;
                }
                else if (aaa <= 28)
                {
                    aaa -= 2;
                }
                else aaa -= 3;
                temp.Add(aaa);
            }
            return temp;
        }

        public  static int Transform3(int ma)
        {                //由显示到胡牌
                int aaa = ma / 4;
                if (aaa >= 0 && aaa <= 8) { }
                else if (aaa <= 17)
                {
                    aaa += 1;
                }
                else if (aaa <= 26)
                {
                    aaa += 2;
                }
                else aaa += 3;
            return aaa;
        }

        public static int Transform4(int ma)
        {                 //由胡牌到显示
                int aaa = ma;
                if (aaa >= 0 && aaa <= 8) { }
                else if (aaa <= 18)
                {
                    aaa -= 1;
                }
                else if (aaa <= 28)
                {
                    aaa -= 2;
                }
                else aaa -= 3;
            return aaa;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Mahjong_server
{
    class Game
    {
        protected Paishan paishan=new Paishan();
        protected int rounds = 0;
        protected List<int> playIDs;
        protected int now;
        public string[] trans = { "East", "South", "West", "North" };
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
        }

        bool sOpFlag = false;
        int opCnt = 0;
        public string[] sOp=new string[4];
        public string[] sOpPai = new string[4];
        public Dictionary<string, int> OpWin = new Dictionary<string, int> { { "OpHu", 1 }, { "OpGang", 2 }, { "OpPeng", 3 }, { "OpChi", 4 }, { "NoOp", 5 } };

        int lastWinID = -1;
        public Game(List<int> p)
        {
            p = Shuffle(p);
            playIDs = p;
            now = 0;
            lastWinID = -1;
            //玩家排序
            opClear();
            paishan = new Paishan();
            paishan.init();
        }
        public void nextGame()
        {
            /*下一把游戏*/
            if (lastWinID != playIDs[0])
            {
                now = 1;
                List<int> temp = new List<int>();
                for (int i = 0; i < playIDs.Count; i++)
                {
                    temp.Add(playIDs[(now + i) % playIDs.Count]);
                }
                playIDs = temp;
            }
            // ID 为东风
            now = 0;
            opClear();
            // 洗牌
            paishan = new Paishan();
            paishan.init();
        }


        public int getNextPai()
        {
            /*下一张牌*/
            return paishan.getNext();
        }

        public int size()
        {
            /*牌堆数量*/
            return paishan.size();
        }

        public bool empty()
        {
            /*牌堆是否为空*/
            return paishan.size() <= 12;
        }

        public List<int>[] getPlayerPai()
        {
            /*得到玩家的牌*/
            return paishan.getPlayerPai();
        }

        public List<int> getPlayerIDs()
        {
            /* 得到目前玩家的ID */
            List<int> res = new List<int>();
            for(int i = 0; i < playIDs.Count; i++)
            {
                res.Add(playIDs[(now+i)%playIDs.Count]);
            }
            return res;
        }
        
        public int getNowPlayerID()
        {
            /* 目前正在打牌玩家的ID */
            return playIDs[now];
        }

        public int nextPlayerID()
        {
            /* 切换到下一个玩家*/
            now = (now + 1) % (playIDs.Count);
            return playIDs[now];
        }

        public void setNowPlayerID(int ID)
        {
            /*设置出牌人*/
            for(int i = 0; i < playIDs.Count; i++)
            {
                if (ID == playIDs[i])
                {
                    now = i;
                    break;
                }
            }
        }

        public int getRounds()
        {
            /*得到目前的轮次*/
            return rounds;
        }

        public int nextRounds() {
            /* 下一轮次*/
            rounds++;
            return rounds;
        }

        public int getRand()
        {
            Random random = new Random();
            return random.Next(1, 7) + random.Next(1, 7);
        }

        public string getNowFeng()
        {
            /*得到当前的发牌的风*/
            return trans[now];
        }

        public void opClear()
        {
            sOpFlag = false;
            opCnt = 0;
            sOp = new string[4];
            sOpPai = new string[4];
        }

        public void setOpFlag()
        {
            /*特殊操作标识置位*/
            opClear();
            sOpFlag = true;
        }

        public bool sOpEnd()
        {
            return opCnt == 4;
        }

        public void addSOp(int ID,string op,string pai="")
        {
            /*添加一个特殊操作*/
            for(int i = 0; i < playIDs.Count; i++)
            {
                if (playIDs[i] == ID)
                {
                    if (sOp[i] == null)
                    {
                        Console.WriteLine("--- ID" + " " + op + " " + pai);
                        opCnt++;
                        sOp[i] = op;
                        sOpPai[i] = pai;
                    }
                    break;
                }
            }
        }

        public bool getSOpFlag()
        {
            return sOpFlag;
        }

        protected int getLastWinSOp()
        {
            /*返回特殊操作的最高优先级的位置*/
            int last = (now + 1) % playIDs.Count;
            for(int i = 1; i < playIDs.Count; i++)
            {
                if (OpWin[sOp[(now + 1 + i) % playIDs.Count]] < OpWin[sOp[last]]) 
                {
                    last = (now + 1 + i) % playIDs.Count;
                }
            }
            return last;
        }

        public void getSOpEnd(out int ID ,out string op,out string pai)
        {
            /*返回特殊操作的最后结果*/
            int last = getLastWinSOp();
            ID = playIDs[last];
            op = sOp[last];
            pai = sOpPai[last];
        }

        public string getPlayerFeng(int ID)
        {
            for(int i = 0; i < playIDs.Count; i++)
            {
                if (playIDs[i] == ID) {
                    return trans[i];
                }
            }
            throw new Exception();
        }

        public void over(int ID)
        {
            lastWinID = ID;
        }

        internal Paishan Paishan
        {
            get => default;
            set
            {
            }
        }
    }
}

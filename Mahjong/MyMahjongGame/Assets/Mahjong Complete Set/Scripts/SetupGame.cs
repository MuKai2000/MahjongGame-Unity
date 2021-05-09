using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupGame : MonoBehaviour
{
	private int state = 0;
	private int p1_dir;
	private string[] pai = new string[140];	// 牌
	public int east_player_seat = 1; //choose which seat (1~4) east player will sit at	东家
	public bool include_red_tiles = false;
	private List<int> seat1_tiles = new List<int>();	// 牌山
	private List<int> seat2_tiles = new List<int>();
	private List<int> seat3_tiles = new List<int>();
	private List<int> seat4_tiles = new List<int>();
	
	private int west_player_seat=0;
	private int north_player_seat=0;
	private int south_player_seat=0;
	
	private List<int> east_player_hand = new List<int>();	// 手牌
	private List<int> west_player_hand = new List<int>();
	private List<int> north_player_hand = new List<int>();
	private List<int> south_player_hand = new List<int>();
	
	private List<int> seat_order = new List<int>();
	private List<int> deal_order = new List<int>();
	private List<int> tile_stack = new List<int>();
	private HashSet<int> moved_tiles = new HashSet<int>();
	private int deal_count = 0;
	private int deal_cycle = 0;
	private bool chonchon_flag = false;
	private int dora_pai_index = 0;
	
	private float[] deal_tile_x_offset = new float[5];
	private float[] deal_tile_z_offset = new float[5];
	//private float deal_tile_x_offset_seat1=2f;
	//private float deal_tile_x_offset_seat2=2f;
	//private float deal_tile_x_offset_seat3=2f;
	//private float deal_tile_x_offset_seat4=2f;
	
	private int tile_stack_pointer=0;
	
	public int GetTileCurrentIndex(){
		return tile_stack_pointer;
	}
	
	public GameObject GetDoraTile(){
		return GameObject.Find(pai[dora_pai_index]);
	}
	
	public int GetDoraTileIndex(){
		return tile_stack.Count-6;
	}
	
	public GameObject GetTileGameObjectAtIndex(int i){
		return GameObject.Find(pai[i]);
	}
	
	public List<GameObject> GetTileDrawStack(){
		List<GameObject> tilesdstack = new List<GameObject>();
		//for(int i=tile_stack_pointer; i<pai.Length; i++){
		for(int i=0; i<pai.Length; i++){
			tilesdstack.Add(GameObject.Find(pai[i]));
		}
		return tilesdstack;
	}
	
	public List<GameObject> GetEastPlayerHand(){
		List<GameObject> hand = new List<GameObject>();
		for(int i=0; i<east_player_hand.Count; i++){
			hand.Add(GameObject.Find(pai[east_player_hand[i]]));
		}
		return hand;
	}

	public List<GameObject> GetWestPlayerHand(){
		List<GameObject> hand = new List<GameObject>();
		for(int i=0; i<west_player_hand.Count; i++){
			hand.Add(GameObject.Find(pai[west_player_hand[i]]));
		}
		return hand;
	}
	
	public List<GameObject> GetNorthPlayerHand(){
		List<GameObject> hand = new List<GameObject>();
		for(int i=0; i<north_player_hand.Count; i++){
			hand.Add(GameObject.Find(pai[north_player_hand[i]]));
		}
		return hand;
	}
	
	public List<GameObject> GetSouthPlayerHand(){
		List<GameObject> hand = new List<GameObject>();
		for(int i=0; i<south_player_hand.Count; i++){
			hand.Add(GameObject.Find(pai[south_player_hand[i]]));
		}
		return hand;
	}

	public GameObject GetTileAtIndex(int tile_index){
		return GameObject.Find(pai[tile_stack[tile_index]]);
	}
	
	public void IncTileIndex(){
		tile_stack_pointer++;
	}
	
	public void DecTileIndex(){
		tile_stack_pointer--;
	}
	
    // Start is called before the first frame update
    void Start()	// 开始 最开始执行
    {
		//souzu
		pai[0]="psouzu_5r";	// 红五
		pai[1]="psouzu_1";
		pai[2]="psouzu_2";
		pai[3]="psouzu_3";
		pai[4]="psouzu_4";
		pai[5]="psouzu_5";
		pai[6]="psouzu_6";
		pai[7]="psouzu_7";
		pai[8]="psouzu_8";
		pai[9]="psouzu_9";
		
		pai[10]="pjihai_chun";
		pai[11]="psouzu_1 (1)";
		pai[12]="psouzu_2 (1)";
		pai[13]="psouzu_3 (1)";
		pai[14]="psouzu_4 (1)";
		pai[15]="psouzu_5 (1)";
		pai[16]="psouzu_6 (1)";
		pai[17]="psouzu_7 (1)";
		pai[18]="psouzu_8 (1)";
		pai[19]="psouzu_9 (1)";
		
		pai[20]="pjihai_chun (1)";
		pai[21]="psouzu_1 (2)";
		pai[22]="psouzu_2 (2)";
		pai[23]="psouzu_3 (2)";
		pai[24]="psouzu_4 (2)";
		pai[25]="psouzu_5 (2)";
		pai[26]="psouzu_6 (2)";
		pai[27]="psouzu_7 (2)";
		pai[28]="psouzu_8 (2)";
		pai[29]="psouzu_9 (2)";
		
		pai[30]="pjihai_chun (2)";
		pai[31]="psouzu_1 (3)";
		pai[32]="psouzu_2 (3)";
		pai[33]="psouzu_3 (3)";
		pai[34]="psouzu_4 (3)";
		pai[35]="psouzu_5 (3)";
		pai[36]="psouzu_6 (3)";
		pai[37]="psouzu_7 (3)";
		pai[38]="psouzu_8 (3)";
		pai[39]="psouzu_9 (3)";
 
		//pinzu
 		pai[40]="ppinzu_5r";
		pai[41]="ppinzu_1";
		pai[42]="ppinzu_2";
		pai[43]="ppinzu_3";
		pai[44]="ppinzu_4";
		pai[45]="ppinzu_5";
		pai[46]="ppinzu_6";
		pai[47]="ppinzu_7";
		pai[48]="ppinzu_8";
		pai[49]="ppinzu_9";
		
 		pai[50]="ppinzu_5r (1)";
		pai[51]="ppinzu_1 (1)";
		pai[52]="ppinzu_2 (1)";
		pai[53]="ppinzu_3 (1)";
		pai[54]="ppinzu_4 (1)";
		pai[55]="ppinzu_5 (1)";
		pai[56]="ppinzu_6 (1)";
		pai[57]="ppinzu_7 (1)";
		pai[58]="ppinzu_8 (1)";
		pai[59]="ppinzu_9 (1)";

 		pai[60]="pjihai_chun (3)";
		pai[61]="ppinzu_1 (2)";
		pai[62]="ppinzu_2 (2)";
		pai[63]="ppinzu_3 (2)";
		pai[64]="ppinzu_4 (2)";
		pai[65]="ppinzu_5 (2)";
		pai[66]="ppinzu_6 (2)";
		pai[67]="ppinzu_7 (2)";
		pai[68]="ppinzu_8 (2)";
		pai[69]="ppinzu_9 (2)";

 		pai[70]="pjihai_hatsu";
		pai[71]="ppinzu_1 (3)";
		pai[72]="ppinzu_2 (3)";
		pai[73]="ppinzu_3 (3)";
		pai[74]="ppinzu_4 (3)";
		pai[75]="ppinzu_5 (3)";
		pai[76]="ppinzu_6 (3)";
		pai[77]="ppinzu_7 (3)";
		pai[78]="ppinzu_8 (3)";
		pai[79]="ppinzu_9 (3)";		

		//manzu
 		pai[80]="pmanzu_5r";
		pai[81]="pmanzu_1";
		pai[82]="pmanzu_2";
		pai[83]="pmanzu_3";
		pai[84]="pmanzu_4";
		pai[85]="pmanzu_5";
		pai[86]="pmanzu_6";
		pai[87]="pmanzu_7";
		pai[88]="pmanzu_8";
		pai[89]="pmanzu_9";		

 		pai[90]="pjihai_hatsu (1)";
		pai[91]="pmanzu_1 (1)";
		pai[92]="pmanzu_2 (1)";
		pai[93]="pmanzu_3 (1)";
		pai[94]="pmanzu_4 (1)";
		pai[95]="pmanzu_5 (1)";
		pai[96]="pmanzu_6 (1)";
		pai[97]="pmanzu_7 (1)";
		pai[98]="pmanzu_8 (1)";
		pai[99]="pmanzu_9 (1)";	

 		pai[100]="pjihai_hatsu (2)";
		pai[101]="pmanzu_1 (2)";
		pai[102]="pmanzu_2 (2)";
		pai[103]="pmanzu_3 (2)";
		pai[104]="pmanzu_4 (2)";
		pai[105]="pmanzu_5 (2)";
		pai[106]="pmanzu_6 (2)";
		pai[107]="pmanzu_7 (2)";
		pai[108]="pmanzu_8 (2)";
		pai[109]="pmanzu_9 (2)";

 		pai[110]="pjihai_hatsu (3)";
		pai[111]="pmanzu_1 (3)";
		pai[112]="pmanzu_2 (3)";
		pai[113]="pmanzu_3 (3)";
		pai[114]="pmanzu_4 (3)";
		pai[115]="pmanzu_5 (3)";
		pai[116]="pmanzu_6 (3)";
		pai[117]="pmanzu_7 (3)";
		pai[118]="pmanzu_8 (3)";
		pai[119]="pmanzu_9 (3)";

		pai[120]="pjihai_haku";
		pai[121]="pjihai_haku (1)";
		pai[122]="pjihai_haku (2)";
		pai[123]="pjihai_haku (3)";
		
		pai[124]="pjihai_pe";
		pai[125]="pjihai_pe (1)";
		pai[126]="pjihai_pe (2)";
		pai[127]="pjihai_pe (3)";

		pai[128]="pjihai_sha";
		pai[129]="pjihai_sha (1)";
		pai[130]="pjihai_sha (2)";
		pai[131]="pjihai_sha (3)";
		
		pai[132]="pjihai_nan";
		pai[133]="pjihai_nan (1)";
		pai[134]="pjihai_nan (2)";
		pai[135]="pjihai_nan (3)";
		
		pai[136]="pjihai_ton";
		pai[137]="pjihai_ton (1)";
		pai[138]="pjihai_ton (2)";
		pai[139]="pjihai_ton (3)";

		state = 1;
    }

	void DealTile(){
		float speed=4f;
		float x=0,y=0,z=0;
		int current_seat_deal=deal_order[deal_count];
		
		//decide where the tile arrives at each seat
		if(current_seat_deal==1){
			x=2f;
			y=0.25f;
			z=3.5f;
		}else if(current_seat_deal==2){
			x=-3.5f;
			y=0.25f;
			z=2f;
		}else if(current_seat_deal==3){
			x=3.5f;
			y=0.25f;
			z=-2f;				
		}else if(current_seat_deal==4){
			x=-2f;
			y=0.25f;
			z=-3.5f;					
		}
		
		GameObject.Find(pai[tile_stack[tile_stack_pointer]]).GetComponent<TileController>().SetSpeed(speed); //make speed of moving tile faster
		GameObject.Find(pai[tile_stack[tile_stack_pointer]]).GetComponent<TileController>().MoveTo(x + deal_tile_x_offset[current_seat_deal],y,z + deal_tile_z_offset[current_seat_deal]);
		
		if (current_seat_deal==1){
			GameObject.Find(pai[tile_stack[tile_stack_pointer]]).GetComponent<TileController>().SetYRot(0);
			deal_tile_x_offset[current_seat_deal]=deal_tile_x_offset[current_seat_deal]-0.3f;
		}else if (current_seat_deal==2){
			GameObject.Find(pai[tile_stack[tile_stack_pointer]]).GetComponent<TileController>().SetYRot(-90); //rotate tile to face seat player
			deal_tile_z_offset[current_seat_deal]=deal_tile_z_offset[current_seat_deal]-0.3f;
		}else if(current_seat_deal==3){
			GameObject.Find(pai[tile_stack[tile_stack_pointer]]).GetComponent<TileController>().SetYRot(90);
			deal_tile_z_offset[current_seat_deal]=deal_tile_z_offset[current_seat_deal]+0.3f;
		}else if(current_seat_deal==4){
			GameObject.Find(pai[tile_stack[tile_stack_pointer]]).GetComponent<TileController>().SetYRot(-180);
			deal_tile_x_offset[current_seat_deal]=deal_tile_x_offset[current_seat_deal]+0.3f;
		}
		
		int tindex=tile_stack[tile_stack_pointer];
		if(current_seat_deal==east_player_seat){
			east_player_hand.Add(tindex);
		}else if(current_seat_deal==west_player_seat){
			west_player_hand.Add(tindex);
		}else if(current_seat_deal==north_player_seat){
			north_player_hand.Add(tindex);
		}else if(current_seat_deal==south_player_seat){
			south_player_hand.Add(tindex);
		}
		//tile_stack[tile_stack_pointer] = i
		
		
		tile_stack_pointer++;		
	}
	
	bool IsTilesDoneMoving(){
			bool allIdle = true;
			for(int i=0; i<pai.Length; i++){
				if(GameObject.Find(pai[i]).GetComponent<TileController>().IsIdle()==false){
					moved_tiles.Add(i); //add moving tiles for later processing
					allIdle=false;
				}
			}
			return allIdle;
	}
	
	void RevealTileToSeat(int current_seat_deal, int i){
		GameObject.Find(pai[i]).GetComponent<TileController>().Flip();
		GameObject.Find(pai[i]).GetComponent<TileController>().SetY(0.2f);
		if (current_seat_deal==1){
			GameObject.Find(pai[i]).GetComponent<TileController>().SetXRot(90f);
			GameObject.Find(pai[i]).GetComponent<TileController>().SetZRot(0f);
		}else if(current_seat_deal==2){
			GameObject.Find(pai[i]).GetComponent<TileController>().SetXRot(90f);
			GameObject.Find(pai[i]).GetComponent<TileController>().SetZRot(90f);
		}else if(current_seat_deal==3){
			GameObject.Find(pai[i]).GetComponent<TileController>().SetXRot(90f);
			GameObject.Find(pai[i]).GetComponent<TileController>().SetZRot(-90f);
		}else if(current_seat_deal==4){
			GameObject.Find(pai[i]).GetComponent<TileController>().SetXRot(90f);
			GameObject.Find(pai[i]).GetComponent<TileController>().SetZRot(-180f);
		}		
	}

    // Update is called once per frame
    void Update()
    {	//
        if(state==1){ //initial state, flips tiles down and moves them to middle
			for(int i=0; i<pai.Length; i++){
				GameObject.Find(pai[i]).GetComponent<TileController>().Flip();	// 调用TileController的函数
				GameObject.Find(pai[i]).GetComponent<TileController>().MoveTo(0,0.25f,0);
			}
			
			//place marks
			GameObject cn = GameObject.Find("chicha N");
			GameObject ce = GameObject.Find("chicha E");
			GameObject cs = GameObject.Find("chicha S");
			GameObject cw = GameObject.Find("chicha W");
			
			Vector3 pos = ce.transform.position;
			if(east_player_seat==1){
				ce.transform.position = new Vector3 (0f,0f,1f);
				ce.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
				cw.transform.position = new Vector3 (0f,0f,-1f);
				cw.transform.localEulerAngles = new Vector3 (0f, 180f, 0f);
				cn.transform.position = new Vector3 (-1f,0f,0f);
				cn.transform.localEulerAngles = new Vector3 (0f, -90f, 0f);
				cs.transform.position = new Vector3 (1f,0f,0f);
				cs.transform.localEulerAngles = new Vector3 (0f, 90f, 0f);
				//east_player_seat=1;
				west_player_seat=4;
				north_player_seat=2;
				south_player_seat=3;
			}else if(east_player_seat==2){
				ce.transform.position = new Vector3 (-1f,0f,0f);
				ce.transform.localEulerAngles = new Vector3 (0f, -90f, 0f);
				cw.transform.position = new Vector3 (1f,0f,0f);
				cw.transform.localEulerAngles = new Vector3 (0f, 90f, 0f);
				cn.transform.position = new Vector3 (0f,0f,-1f);
				cn.transform.localEulerAngles = new Vector3 (0f, 180f, 0f);
				cs.transform.position = new Vector3 (0f,0f,1f);
				cs.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
				//east_player_seat=2;
				west_player_seat=3;
				north_player_seat=4;
				south_player_seat=1;				
			}else if(east_player_seat==3){
				ce.transform.position = new Vector3 (1f,0f,0f);
				ce.transform.localEulerAngles = new Vector3 (0f, 90f, 0f);
				cw.transform.position = new Vector3 (-1f,0f,0f);
				cw.transform.localEulerAngles = new Vector3 (0f, -90f, 0f);
				cn.transform.position = new Vector3 (0f,0f,1f);
				cn.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
				cs.transform.position = new Vector3 (0f,0f,-1f);
				cs.transform.localEulerAngles = new Vector3 (0f, 180f, 0f);
				//east_player_seat=3;
				west_player_seat=2;
				north_player_seat=1;
				south_player_seat=4;				
			}else if(east_player_seat==4){
				ce.transform.position = new Vector3 (0f,0f,-1f);
				ce.transform.localEulerAngles = new Vector3 (0f, 180f, 0f);				
				cw.transform.position = new Vector3 (0f,0f,1f);
				cw.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
				cn.transform.position = new Vector3 (1f,0f,0f);
				cn.transform.localEulerAngles = new Vector3 (0f, 90f, 0f);
				cs.transform.position = new Vector3 (-1f,0f,0f);
				cs.transform.localEulerAngles = new Vector3 (0f, -90f, 0f);
				//east_player_seat=4;
				west_player_seat=1;
				north_player_seat=3;
				south_player_seat=2;
			}
			
			//Vector3 pos = cn.transform.position;
			//cn.transform.position = new Vector3 (0,1,0);
			
			state = 2;
		}else if(state==2){ //wait for tiles to finish moving
			bool allIdle = true;
			for(int i=0; i<pai.Length; i++){
				if(GameObject.Find(pai[i]).GetComponent<TileController>().IsIdle()==false){
					allIdle=false;
				}
			}
			
			if (allIdle==true) state = 3; //goto next state when all tiles finished moving.			
		}else if(state ==3){ //shuffle and place tiles
			//add tiles to avaliable tiles list
			List<int> avail_tiles = new List<int>();
			for(int i=0; i<pai.Length; i++){
				
				if(include_red_tiles){
					if(i == 5 || i == 85 || i == 45 || i == 55){
						GameObject.Find(pai[i]).GetComponent<TileController>().Hide();
					}else{
						avail_tiles.Add(i);
					}					
				}else{
					if(i == 0 || i == 40 || i == 50 || i == 80){
						GameObject.Find(pai[i]).GetComponent<TileController>().Hide();
					}else{
						avail_tiles.Add(i);
					}
				}
			}
			//Debug.Log("Shuffled tiles");

			//place tiles seat 1
			float hoffset = 0f;
			float voffset = 0.25f;
			int hcount = 0;
			int vcount = 0;
			int stage = 1;
			float init_stackpos=3f;
			float startx=2.4f-0.3f;
			float startz=init_stackpos;
			float tileyrot=0f;
			
			//90
			while(true){
				if (avail_tiles.Count==0) break;
				int num = Random.Range(0,avail_tiles.Count);
				int tile_now = avail_tiles[num];
				GameObject.Find(pai[tile_now]).GetComponent<TileController>().SetYRot(tileyrot);
				
				if (stage==1 || stage==3){
					GameObject.Find(pai[tile_now]).GetComponent<TileController>().MoveTo(startx+hoffset,voffset,startz);
				}else{
					GameObject.Find(pai[tile_now]).GetComponent<TileController>().MoveTo(startx,voffset,startz+hoffset);
				}
				
				if (stage==1){
					seat1_tiles.Add(tile_now);
				}else if(stage == 2){
					seat2_tiles.Add(tile_now);
				}else if(stage == 4){
					seat3_tiles.Add(tile_now);
				}else if(stage == 3){
					seat4_tiles.Add(tile_now);
				}
				
				hcount++;
				if (hcount >= 17){
					if (vcount >=1){
						//return to 1st lvl
						hoffset = 0;
						hcount = 0;
						vcount=0;
						voffset=0.25f;
						stage++;
						if(stage==2){
							startx=-init_stackpos;
							startz=2.4f-0.3f; //2.4 - 0.3
							tileyrot=-90;
						}else if(stage==3){
							startx=-2.4f+0.3f;
							startz=-init_stackpos;
							tileyrot=180;
						}else if(stage==4){
							startx=init_stackpos;
							startz=-2.4f+0.3f;
							tileyrot=90f;							
						}
						
					}else{
						//go to second lvl
						hoffset = 0;
						hcount = 0;
						voffset=voffset+0.25f;
						vcount++;
					}

				}else{
					if(stage==1 || stage==2){
						hoffset=hoffset-0.3f;
					}else{
						hoffset=hoffset+0.3f;
					}
					
				}
				
				avail_tiles.RemoveAt(num);
			}
			
			//place tiles seat 2
			state = 4;
		}else if(state==4){ //wait for tiles to finish moving
			bool allIdle = true;
			for(int i=0; i<pai.Length; i++){
				if(GameObject.Find(pai[i]).GetComponent<TileController>().IsIdle()==false){
					allIdle=false;
				}
			}
			if (allIdle==true) state = 5; //goto next state when all tiles finished moving.	
		}else if(state==5){
			int dice1=Random.Range(1,6);
			int dice2=Random.Range(1,6);
			int dice_total=dice1+dice2;
			int tile_get_seat=0;
			if (dice_total == 3 || dice_total == 7 || dice_total == 11){	//front
				if (east_player_seat==1){
					tile_get_seat=4;
				}else if(east_player_seat==2){
					tile_get_seat=3;
				}else if(east_player_seat==3){
					tile_get_seat=2;
				}else if(east_player_seat==4){
					tile_get_seat=1;
				}
			}else if(dice_total == 4 || dice_total == 8 || dice_total == 12){ //left
				if (east_player_seat==1){
					tile_get_seat=3;
				}else if(east_player_seat==2){
					tile_get_seat=1;
				}else if(east_player_seat==3){
					tile_get_seat=4;
				}else if(east_player_seat==4){
					tile_get_seat=2;
				}				
			}else if(dice_total == 2 || dice_total == 6 || dice_total == 10){ //right
				if (east_player_seat==1){
					tile_get_seat=2;
				}else if(east_player_seat==2){
					tile_get_seat=4;
				}else if(east_player_seat==3){
					tile_get_seat=1;
				}else if(east_player_seat==4){
					tile_get_seat=3;
				}				
			}else if(dice_total == 5 || dice_total == 9){ //self
				if (east_player_seat==1){
					tile_get_seat=1;
				}else if(east_player_seat==2){
					tile_get_seat=2;
				}else if(east_player_seat==3){
					tile_get_seat=3;
				}else if(east_player_seat==4){
					tile_get_seat=4;
				}				
			}
			
			//decide to tile start point
			//Debug.Log("Get Tile Seat:" + tile_get_seat);
			//Debug.Log("Dice Total" + dice_total);
			//dice_total=12;
			//tile_get_seat=4;
			if(tile_get_seat == 1){
				seat_order.Add(1);
				seat_order.Add(3);
				seat_order.Add(4);
				seat_order.Add(2);
				seat_order.Add(1);			
			}else if(tile_get_seat == 2){
				seat_order.Add(2);
				seat_order.Add(1);
				seat_order.Add(3);
				seat_order.Add(4);
				seat_order.Add(2);	
			}else if(tile_get_seat == 3){
				seat_order.Add(3);
				seat_order.Add(4);
				seat_order.Add(2);
				seat_order.Add(1);
				seat_order.Add(3);	
			}else if(tile_get_seat == 4){
				seat_order.Add(4);
				seat_order.Add(2);
				seat_order.Add(1);
				seat_order.Add(3);
				seat_order.Add(4);	
			}
			
			//get next tiles from begin
			int seat=seat_order[0];
			if(seat==1){
				for(int i=16-dice_total; i>=0; i--){
					tile_stack.Add(seat1_tiles[17+i]);
					tile_stack.Add(seat1_tiles[i]);
				}				
			}else if(seat==2){
				for(int i=16-dice_total; i>=0; i--){
					tile_stack.Add(seat2_tiles[17+i]);
					tile_stack.Add(seat2_tiles[i]);
				}				
			}else if(seat==3){
				for(int i=16-dice_total; i>=0; i--){
					tile_stack.Add(seat3_tiles[17+i]);
					tile_stack.Add(seat3_tiles[i]);
				}				
			}else if(seat==4){
				for(int i=16-dice_total; i>=0; i--){
					tile_stack.Add(seat4_tiles[17+i]);
					tile_stack.Add(seat4_tiles[i]);
				}				
			}
			
			//get next tiles from middle
			for(int j=1; j<seat_order.Count-1; j++){
				seat=seat_order[j];
				if(seat==1){
					for(int i=16; i>=0; i--){
						tile_stack.Add(seat1_tiles[17+i]);
						tile_stack.Add(seat1_tiles[i]);
					}
				}else if(seat==2){
					for(int i=16; i>=0; i--){
						tile_stack.Add(seat2_tiles[17+i]);
						tile_stack.Add(seat2_tiles[i]);
					}					
				}else if(seat==3){
					for(int i=16; i>=0; i--){
						tile_stack.Add(seat3_tiles[17+i]);
						tile_stack.Add(seat3_tiles[i]);
					}					
				}else if(seat==4){
					for(int i=16; i>=0; i--){
						tile_stack.Add(seat4_tiles[17+i]);
						tile_stack.Add(seat4_tiles[i]);
					}					
				}
			}
			// get next tiles from last
			seat=seat_order[seat_order.Count-1];
			if(seat==1){
				for(int i=16; i>=16-dice_total+1; i--){
					tile_stack.Add(seat1_tiles[17+i]);
					tile_stack.Add(seat1_tiles[i]);
				}				
			}else if(seat==2){
				for(int i=16; i>=16-dice_total+1; i--){
					tile_stack.Add(seat2_tiles[17+i]);
					tile_stack.Add(seat2_tiles[i]);
				}				
			}else if(seat==3){
				for(int i=16; i>=16-dice_total+1; i--){
					tile_stack.Add(seat3_tiles[17+i]);
					tile_stack.Add(seat3_tiles[i]);
				}				
			}else if(seat==4){
				for(int i=16; i>=16-dice_total+1; i--){
					tile_stack.Add(seat4_tiles[17+i]);
					tile_stack.Add(seat4_tiles[i]);
				}				
			}
			
			//all tiles added to tile_stack list
			//set dora pai index
			dora_pai_index=tile_stack[tile_stack.Count-6]; //get the top tile from third stack from the end
			
			//init tile stack pointer
			tile_stack_pointer=0;
			
			//setup deal order
			if (east_player_seat==1){
				deal_order.Add(1);
				deal_order.Add(2);
				deal_order.Add(4);
				deal_order.Add(3);
			}else if(east_player_seat==2){
				deal_order.Add(2);
				deal_order.Add(4);
				deal_order.Add(3);
				deal_order.Add(1);				
			}else if(east_player_seat==3){
				deal_order.Add(3);
				deal_order.Add(1);
				deal_order.Add(2);
				deal_order.Add(4);				
			}else if(east_player_seat==4){
				deal_order.Add(4);
				deal_order.Add(3);
				deal_order.Add(1);
				deal_order.Add(2);				
			}
			
			//tile offsets saved in array
			deal_tile_x_offset[0]=0f;	
			deal_tile_x_offset[1]=0f;	
			deal_tile_x_offset[2]=0f;	
			deal_tile_x_offset[3]=0f;	
			deal_tile_x_offset[4]=0f;	
			
			deal_tile_z_offset[0]=0f;	
			deal_tile_z_offset[1]=0f;	
			deal_tile_z_offset[2]=0f;	
			deal_tile_z_offset[3]=0f;	
			deal_tile_z_offset[4]=0f;	
			
			deal_count=0;
			state=6;
		}else if(state==6){ //Deal Tiles
			if (tile_stack_pointer>=tile_stack.Count){
				//state=8;
			}else{				
				for(int i=1; i<=4; i++){	//deal 4 tiles at a time
					DealTile();				
				}
				state=7; //wait for tile to finish moving
			}
		}else if(state==7){ //wait for tile to finish moving
			if (IsTilesDoneMoving()){ //when tiles finished moving reveal to seat player
				int current_seat_deal=deal_order[deal_count];
				
				foreach (int i in moved_tiles){
					RevealTileToSeat(current_seat_deal,i);
				}//flip and stand the tiles that were just moved.
				moved_tiles.Clear();
				
				deal_count++; 	//inc to deal to next seat
				if (deal_count >3){	//max 4 seats
					deal_count=0;
					deal_cycle++;	//inc for dealing cycle
				}
				
				if (deal_cycle >=3){	//max 3 times
					deal_cycle=0;
					state = 8; //repeat deal to seats 3 times then go to haipai state (each take 1 tile)
				}else{
					state = 6; //continue dealing next 4 tiles
				}
				; 
			}
		}else if(state==8){ //haipai
				DealTile();	//deal 1 tile
				state=9;
		}else if(state==9){ //wait for tile to finish moving			
			if (IsTilesDoneMoving()){ //when tiles finished moving reveal to seat player
				int current_seat_deal=deal_order[deal_count];
				
				foreach (int i in moved_tiles){//flip and stand the tiles that were just moved.
					RevealTileToSeat(current_seat_deal,i);
				}
				moved_tiles.Clear();

				deal_count++; 
				if (chonchon_flag){
					chonchon_flag=false;
					state=10;
				}else if (deal_count >3 ){
					deal_count=0;
					state = 8; //when dealt one tile to all seats deal one last tile to chicha (chonchon)
					chonchon_flag=true;
					//Debug.Log("chon chon");
				}else{
					state = 8; //continue dealing
				}
			}
		}else if(state==10){ //flip dora tile
			GameObject.Find(pai[dora_pai_index]).GetComponent<TileController>().Flip(true);
			state=11;
		}
		else if(state==11){
			// for(int i=0; i<pai.Length; i++){
			// 	if(GameObject.Find(pai[i]).GetComponent<TileController>().IfClick())
			// 		GameObject.Find(pai[i]).GetComponent<TileController>().Hide();
			// }
		}
    }
}

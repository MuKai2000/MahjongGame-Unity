using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadmeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
		//--------------------------------------------------------------------------
		//************Showcase of functions included in SetupGame script************
		//--------------------------------------------------------------------------
		
       float fire1 = Input.GetAxis("Fire1"); //*click left mouse button to run the tests after game has setup!*;
		if (fire1 !=0){
			//Get component to call functions
			SetupGame mahjong_table = GameObject.Find("table").GetComponent<SetupGame>();
			
			//---------------------------------------------------------------------
			//	GetTileDrawStack()
			//returns a list of all the tiles in game as gameobjects in order of when they will be drawn 
			List<GameObject> tilesdstack = mahjong_table.GetTileDrawStack();
			Debug.Log("TILE COUNT:"+tilesdstack.Count);
			
			//---------------------------------------------------------------------
			//	GetEastPlayerHand()
			//	GetWestPlayerHand()
			//	GetNorthPlayerHand()
			//	GetSouthPlayerHand()
			//returns a list of tiles in east,west,north,south player's hand
			List<GameObject> east_player_hand = mahjong_table.GetEastPlayerHand();
			Debug.Log("EAST 0:"+east_player_hand[0].name);				//print the name of the first tile in east player's hand.
			List<GameObject> west_player_hand = mahjong_table.GetWestPlayerHand();
			Debug.Log("WEST 0:"+west_player_hand[0].name);
			List<GameObject> north_player_hand = mahjong_table.GetNorthPlayerHand();
			Debug.Log("NORTH 0:"+north_player_hand[0].name);
			List<GameObject> south_player_hand = mahjong_table.GetSouthPlayerHand();
			Debug.Log("SOUTH 0:"+south_player_hand[0].name);
			//---------------------------------------------------------------------
			//	GetTileCurrentIndex()
			//returns the index number of the next tile to be drawn
			int next_draw = mahjong_table.GetTileCurrentIndex();
			
			//---------------------------------------------------------------------	
			//	GetTileAtIndex(int tile_index)
			//returns tile as GameObject at index
			//print the name of the next tile to be drawn
			Debug.Log("NEXT TILE:"+mahjong_table.GetTileAtIndex(next_draw).name);
			//print the name of the tile that was just drawn
			Debug.Log("LAST TILE:"+mahjong_table.GetTileAtIndex(next_draw-1).name);
			
			//---------------------------------------------------------------------	
			//	IncTileIndex(); DecTileIndex();
			//increments and decrements tile index. Use only after tile is dealt.
			mahjong_table.IncTileIndex(); mahjong_table.DecTileIndex();
			
			//---------------------------------------------------------------------
			//	GetDoraTileIndex()
			//returns the tile index of the dora tile
			int dora_tile_index = mahjong_table.GetDoraTileIndex();
			Debug.Log("DORA TILE:"+mahjong_table.GetTileAtIndex(dora_tile_index).name);	//print the name of the dora tile
			
			//---------------------------------------------------------------------
			//	Moving/Rotating Mahjong Tiles
			//	Tiles contain some useful functions
			//---------------------------------------------------------------------
			//	MoveTo(float destx, float desty,float destz);
			// moves a mahjong tile gameobject to destx,desty,destz;
			mahjong_table.GetTileAtIndex(next_draw).GetComponent<TileController>().MoveTo(0, 1, 0);
			

			//---------------------------------------------------------------------
			//	SetXRot(float rotx);	SetYRot(float roty);	SetZRot(float rotz);
			// these functions rotate a mahjong tile gameobject;		
			mahjong_table.GetTileAtIndex(next_draw).GetComponent<TileController>().SetXRot(90f);
			mahjong_table.GetTileAtIndex(next_draw).GetComponent<TileController>().SetYRot(0f);
			mahjong_table.GetTileAtIndex(next_draw).GetComponent<TileController>().SetZRot(0f);
			
			
			//*WARNING*
			//The functions above provides the info of the tiles and players hand in the state after the game is setup.
			//The script does not provide functions to draw, move, or manage tiles.
			//These functions must be implemented by the user using the above game object information after the initial game setup.
		}
    }
}

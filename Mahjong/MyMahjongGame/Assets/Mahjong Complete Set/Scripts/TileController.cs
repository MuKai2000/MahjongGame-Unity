using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
	private Vector3 targetPosition;
	public float speed = 0.8F;
	private float startTime;
	private float journeyLength;
	private int state = 0;
	private float _minDistance = 0.01f;
	private bool face_down = false;

	
    // Start is called before the first frame update
    void Start(){
		state = 0; //idle state;
		//journeyLength = Vector3.Distance(move_start.position, move_end.position);
		//		Vector3 pos = this.gameObject.transform.position;
		//		this.gameObject.transform.position = new Vector3 (pos.x + 0.05f, pos.y, pos.z);	
		

    }

    // Update is called once per frame
    void Update(){
		
		if (state == 0){ //idle
			//Flip();
			//state= 3;
		}else if(state == 1){ //moving
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector3.Lerp(this.gameObject.transform.position, targetPosition, fracJourney);	
			if(Vector3.Distance(this.gameObject.transform.position, targetPosition) <= _minDistance){
				state = 2;
			}
		}else if(state == 2){ //finished moving
			state = 0 ;
			this.gameObject.transform.position = new Vector3(targetPosition.x , targetPosition.y, targetPosition.z); // set end position
		}else if(state == 3){
			
		}
		//GameObject.Find("オブジェクト名")

		// if (Input.GetMouseButtonDown(0)) this.Hide();
		
    }
	
	void FixedUpdate(){
		
	}
	
	public void MoveTo(float x, float y, float z){
		targetPosition = new Vector3(x,y,z);
		journeyLength = Vector3.Distance(this.gameObject.transform.position, targetPosition);
		startTime = Time.time;
		state = 1;
	}

	public void SetZRot(float rotz){
			Vector3 rot = this.gameObject.transform.localEulerAngles;
			this.gameObject.transform.localEulerAngles = new Vector3 (rot.x, rot.y, rotz);
		
	}

	public void SetYRot(float roty){
			Vector3 rot = this.gameObject.transform.localEulerAngles;
			this.gameObject.transform.localEulerAngles = new Vector3 (rot.x, roty, rot.z);
		
	}
	
	public void SetXRot(float rotx){
			Vector3 rot = this.gameObject.transform.localEulerAngles;
			this.gameObject.transform.localEulerAngles = new Vector3 (rotx, rot.y, rot.z);
		
	}
	
	public void SetY(float y){
		Vector3 pos = this.gameObject.transform.position;
		this.gameObject.transform.position = new Vector3 (pos.x, y, pos.z);		
	}
	
	public void Flip(bool keep_yrot=false){
		if(!face_down){	// 不是盖牌
//			Transform t = this.transform;
//			t.Rotate ( 0f, 0f, 180f );
			
			Vector3 rot = this.gameObject.transform.localEulerAngles;
			if(keep_yrot){
				this.gameObject.transform.localEulerAngles = new Vector3 (0f, rot.y, 180f);
			}else{
				this.gameObject.transform.localEulerAngles = new Vector3 (0f, 0f, 180f);
			}
			Vector3 pos = this.gameObject.transform.position;
			this.gameObject.transform.position = new Vector3 (pos.x, pos.y+0.25f, pos.z);
			
			
			face_down = true;
		}else{	// 盖牌
			Vector3 rot = this.gameObject.transform.localEulerAngles;
			if(keep_yrot){
				this.gameObject.transform.localEulerAngles = new Vector3 (0f, rot.y, 0f);
			}else{
				this.gameObject.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			}
			
			
			Vector3 pos = this.gameObject.transform.position;
			this.gameObject.transform.position = new Vector3 (pos.x, pos.y-0.25f, pos.z);

			face_down = false;
		}
	}
	
	public void SetSpeed(float sv){
		speed=sv;
	}
	
	public bool IsIdle(){
		if (state==0) return true;
		return false;
	}
	
	public void Hide(){
		this.gameObject.transform.position = new Vector3 (0,-9000,0);
	}

	// public void OnClickCubeItem(UnityEngine.EventSystems.BaseEventData data = null){
	// 	this.Hide();
	// }

	// public bool IfClick(){
	// 	return gameObject.GetMouseButtonDown(0);
	// }
}

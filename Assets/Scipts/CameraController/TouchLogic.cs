using UnityEngine;
using System.Collections;

public class TouchLogic : TouchEventControler {

	public static int currTouch = 0; // De cac scripts khac co the biet dc vi tri hien tai cua touch tren man hinh
	private Ray ray;	// Day la ray se dua ra bat dau tinh  tu luc touch vao man hinh
	private RaycastHit rayHitInfo = new RaycastHit();	// dua ve thong tin doi tuong da dc hit boi ray

	void OnTouchs()
	{
		// Kiem tra co phai co 1 touch tren man hinh
		if (Input.touches.Length <= 0) {
			// Neu khong co touch nao thi xu ly tiep
		} else {	
			// Neu day la 1 touch
			// loop tat ca cac touch cham tren man hinh ma chua bo ra
			for(int i = 0; i < Input.touchCount; i++)
			{
				Debug.Log (i); // kiem tra xem co may touch dang cham vao man hinh 
				currTouch = i;
				// chua biet dung lam gi GUITexture
				if(GetComponent<GUITexture>() != null && GetComponent<GUITexture>().HitTest(Input.GetTouch(i).position)){
					Debug.Log ("FUCK 111");
					if(Input.GetTouch(i).phase == TouchPhase.Began) {	// khi bat dau cham vao man hinh
						this.SendMessage("OnTouchBegan");
					}
					if(Input.GetTouch(i).phase == TouchPhase.Ended) {	// khi bat dau cham vao man hinh
						this.SendMessage("OnTouchEnded");
					}
					if(Input.GetTouch(i).phase == TouchPhase.Moved) {	// khi bat dau cham vao man hinh
						this.SendMessage("OnTouchMove");
					}
				}

				Debug.Log ("FUCK 222");
				// 
				if(Input.GetTouch(i).phase == TouchPhase.Began) {	// khi bat dau cham vao man hinh
					this.SendMessage("OnTouchBeganAnyWhere");
				}
				if(Input.GetTouch(i).phase == TouchPhase.Ended) {	// khi ket thuc cham vao man hinh
					this.SendMessage("OnTouchEndedAnyWhere");
				}
				if(Input.GetTouch(i).phase == TouchPhase.Moved) {	// khi cham vao man hinh va di chuyen
					this.SendMessage("OnTouchMoveAnyWhere");
				}
				if(Input.GetTouch(i).phase == TouchPhase.Stationary) {	// khi cham vao man hinh am ko di chuyen
					this.SendMessage("OnTouchStayAnyWhere");
				}
//
//				// Cho doi tuong 3d voi colliders
//				if(Input.GetTouch(i).phase == TouchPhase.Began) {	// khi bat dau cham vao man hinh
//					ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);	// khoi tao mot ray tu vi chi touch tren man hinh
//					if(Physics.Raycast(ray, out rayHitInfo)) {
//						rayHitInfo.transform.gameObject.SendMessage ("OnTouchBegan3D");
//					}
//				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Swing flag component
/// </summary>
public class SwingFlag : MonoBehaviour
{
	// インスタンスがロードされるときに呼び出されます。
	// Awakeはロード時のスレッド内で動作し、コルーチンは動かせません。
	void Awake()
	{
	}

	// 最初のUpdate()またはFixedUpdate()の直前に呼び出されます。
	// 主にゲームオブジェクトの初期化のために使用します。
	// Awake()と違いStart()はゲームスレッド内で動作します。
	void Start()
	{
		this.fromAx = base.transform.rotation.eulerAngles.x;
		this.toAx = 30;
		this.fromX = base.transform.localPosition.x;
		this.toX = -2;
		//base.transform.parent.Find( "Cloth" ).position = base.transform.position;
	}
	// ゲームオブジェクトが破棄されるときに呼び出されます。
	void OnDestroy()
	{
	}

	// Updateは1フレーム毎に呼び出されます。
	void Update()
	{
		if( 1 < this.time )
		{
			this.time = 0;
			this.fromAx = base.transform.rotation.eulerAngles.x;
			this.toAx *= -1;
			this.fromX = base.transform.localPosition.x;
			this.toX *= -1;
		}
		this.time += Time.deltaTime;
		var angle = base.transform.rotation.eulerAngles;
		angle.x = angle.z = Mathf.LerpAngle(this.fromAx, this.toAx, this.time);
		base.transform.rotation = Quaternion.Euler(angle);
		var pos = base.transform.rotation.eulerAngles;
		pos.x = Mathf.Lerp(this.fromX, this.toX, this.time);
		pos.z = Mathf.Lerp(this.fromX, this.toX, this.time*1.5f);
		base.transform.localPosition = pos;
	}

	// FixedUpdateは固定時間の1フレーム毎に呼び出されます。
	// RigidBodyに力を加えるなど物理処理はFixedUpdateでおこなうべきです。
	void FixedUpdate()
	{
	}

	// 全てのUpdate()が実行された後に呼び出されます。
	void LateUpdate()
	{
	}

	float fromX, toX;
	float fromAx, toAx;
	float time;
}

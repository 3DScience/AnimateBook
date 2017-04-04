using UnityEngine;
using System.Collections;

/// <summary>
/// Select flag component
/// </summary>
public class SelectFlag : MonoBehaviour
{
	/// <summary>
	/// Flag textures
	/// </summary>
	public Texture2D[] FlagImages;
	/// <summary>
	/// Selected flag ID 
	/// </summary>
	[HideInInspector]
	public int FlagID;

	// Use this for initialization
	void Start()
	{
		base.transform.Find( "Cloth" ).GetComponent<Renderer>().material.mainTexture = this.FlagImages[this.FlagID];
	}

	// Update is called once per frame
	void Update()
	{

	}
}

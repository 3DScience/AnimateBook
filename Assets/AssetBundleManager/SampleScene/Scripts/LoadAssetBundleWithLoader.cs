using UnityEngine;
using System.Collections;
using isotope;

/// <summary>
/// Load assetbundle sample2
/// </summary>
public class LoadAssetBundleWithLoader : MonoBehaviour
{
	/// <summary>
	/// Assetbundle path
	/// </summary>
	public string AssetBundlePath;
	// Use this for initialization
	IEnumerator Start()
	{
		var loader = base.gameObject.AddComponent<AssetBundleLoader>();
		loader.SetStreamingAssetsPath( this.AssetBundlePath );
		loader.AssetName = "flag2.png";
		loader.Load();

		while( loader.IsLoading )
			yield return 0;

		var texture = loader.Asset as Texture;
		base.GetComponent<Renderer>().material.mainTexture = texture;
		Destroy( loader );
	}

	// Update is called once per frame
	void Update()
	{
	}
}

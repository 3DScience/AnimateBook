using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using isotope;

/// <summary>
/// Display the progress of loading.
/// </summary>
public class ProgressText :MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		this.text = GetComponent<TextMesh>();
	}

	// Update is called once per frame
	void Update()
	{
		if(this.loader && this.text)
		{
			this.text.text = string.Format(this.format, this.loader.Progress * 100);
			if (this.loader.Progress < 1)
				Debug.Log("load:" + this.loader.Progress);
		}
	}
	[SerializeField]
	AssetBundleLoader loader = null;

	TextMesh text;
	[SerializeField]
	string format;
}

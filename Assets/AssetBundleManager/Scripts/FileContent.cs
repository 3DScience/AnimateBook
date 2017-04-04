using UnityEngine;
using System.Collections.Generic;

#if false
/// <summary>
/// One File
/// </summary>
//[System.Serializable]
public class FileContent : iContent
{
	/// <summary>Infomation</summary>
	public override string Name { get { return this._path; } }
	/// <summary>File path</summary>
	public string Path { get { return _path; } }

#if UNITY_EDITOR
	/// <summary>Get all content</summary>
	public override IEnumerable<string> GetContents()
	{
		yield return this.Name;
	}
#endif

	/// <summary>
	/// Initialize
	/// </summary>
	/// <param name="path">file path</param>
	public void Initialize(string path)
	{
		this._path = path;
	}
	[SerializeField]
	string _path;
}
#endif
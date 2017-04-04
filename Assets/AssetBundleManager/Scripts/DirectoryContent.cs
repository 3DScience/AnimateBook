using UnityEngine;
using System.Collections.Generic;

#if false
/// <summary>
/// Any files in directory
/// </summary>
//[System.Serializable]
public class DirectoryContent : iContent
{
	/// <summary>Content name</summary>
	public override string Name { get { return this._directory + "/" + this._pattern; } }
	/// <summary>Directory</summary>
	public string Directory { get { return _directory; } }
	/// <summary>Extension</summary>
	public string Pattern { get { return _pattern; } set { this._pattern = value; } }

#if UNITY_EDITOR
	/// <summary>Get all content</summary>
	public override IEnumerable<string> GetContents()
	{
		foreach (var f in System.IO.Directory.GetFiles(this.Directory, this.Pattern))
		{
			yield return f;
		}
	}
#endif

	/// <summary>
	/// Initialize
	/// </summary>
	/// <param name="directory">directory</param>
	/// <param name="pattern">Target file name pattern</param>
	public void Initialize(string directory, string pattern)
	{
		if (directory != null)
			this._directory = directory.TrimEnd('/');
		if (string.IsNullOrEmpty(pattern))
			pattern = "*.*";	// all files
		this._pattern = pattern;
	}

	[SerializeField]
	string _directory;
	[SerializeField]
	string _pattern;
}
#endif
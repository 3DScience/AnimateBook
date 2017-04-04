//--------------------------------------------------------------------------------
/*
 *	@file		AssetBundleManagerData
 *	@brief		
 *	@ingroup	
 *	@version	1.00
 *	@date		2014
 *	AssetBUndleManager data
 *	
 */
//--------------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

#if !DLL && UNITY_EDITOR
using UnityEditor;
#endif

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// AssetBundle management data
	/// </summary>
	public class AssetBundleManageData : ScriptableObject
	{
		/// <summary>AssetBundle List</summary>
		public List<AssetBundleData> assetbundles = new List<AssetBundleData>();
		/// <summary>If true, load asset from AssetDatabase</summary>
		public bool notUseBundleOnEditor;
		/// <summary>MultiPlatform</summary>
		public bool multiPlatform;
#if !DLL && UNITY_EDITOR
		/// <summary>BuildTarget list</summary>
		public List<BuildTarget> targets = new List<BuildTarget>();
#endif
	}
	/// <summary>
	/// AssetBundle data
	/// </summary>
	[System.Serializable]
	public class AssetBundleData
	{
		/// <summary>Constructor</summary>
		/// <param name="dir">directory name of assetbundle</param>
		/// <param name="file">file name of assetbundle</param>
		public AssetBundleData(string dir, string file)
		{
			this._directory = dir.Replace('\\', '/');
			this._file = file;
		}
		/// <summary>Target directory</summary>
		public string Directory { get { return this._directory; } protected set { this._directory = value; } }
		/// <summary>File Name</summary>
		public string File { get { return this._file; } protected set { this._file = value; } }
		/// <summary>Target path</summary>
		public string Path
		{
			get
			{
				if (string.IsNullOrEmpty(this.Directory))
					return this.File;
				else
					return this.Directory + '/' + this.File;
			}
		}
		/// <summary>Files make each assetbundle</summary>
		public bool Separated { get { return this._separated; } set { this._separated = value; } }
		/// <summary>Assetbundle for streamed scene</summary>
		public bool ForStreamedScene { get { return this._streamedScene; } set { this._streamedScene = value; } }
		/// <summary>Parent assetbundle. Use BuildPipeline.Push(Pop)AssetDependencies.</summary>
		public int ParentNo{ get { return this._parent; } set { this._parent = value; } }
		/// <summary>Create each platform folder.</summary>
		public bool PlatformFolder { get { return this._platformFolder; } set { this._platformFolder = value; } }
#if false//!DLL && UNITY_EDITOR
		/// <summary>BuildAssetBundleOption</summary>
		public BuildAssetBundleOptions Options { get { return this._options; } set { this._options = value; } }
#else
		/// <summary>BuildAssetBundleOption</summary>
		public int Options { get { return this._options; } set { this._options = value; } }
#endif
		/// <summary>changed.</summary>
		public bool Changed { get { return this._changed; } set { this._changed = value; } }

		/// <summary>
		/// SetPath
		/// </summary>
		/// <param name="path">AssetBundle Path</param>
		public void SetPath(string path)
		{
			path = path.Replace('\\', '/');
			int idx = path.LastIndexOf('/');
			if (0 < idx)
				this.Directory = path.Substring(0, idx);
			else
				this.Directory = "";
			this.File = path.Substring(idx + 1);
		}

		/// <summary>
		/// Get all contents.
		/// </summary>
		/// <returns>content information</returns>
		public List<iContent> GetAllContents()
		{
			return this._contents;
		}
		/// <summary>
		/// Get all contents path in assetbundle.
		/// </summary>
		/// <returns>content information</returns>
		public IEnumerable<string> GetAllContentsPath()
		{
			foreach (var c in this._contents)
			{
				foreach (var cc in c.GetContents())
					yield return cc;
			}
			yield break;
		}
		/// <summary>
		/// Get content information in assetbundle.
		/// </summary>
		/// <param name="idx">index</param>
		/// <returns>content information</returns>
		public iContent GetContent(int idx)
		{
			if (idx < 0 || this._contents.Count <= idx)
				return null;
			return this._contents[idx];
		}
		/// <summary>
		/// Add file with assetbundle
		/// </summary>
		/// <param name="path">File path</param>
		public void AddFile(string path)
		{
			//Debug.Log("Add file:" + path);
			var c = new iContent();
			c.Initialize(path);
			this._contents.Add(c);
		}
		/// <summary>
		/// Add directory with assetbundle
		/// </summary>
		/// <param name="directory">Directory</param>
		/// <param name="pattern">Target file name pattern</param>
		public void AddDirectory(string directory, string pattern)
		{
			//Debug.Log("Add directory:" + directory + pattern);
			var c = new iContent();
			c.Initialize(directory, pattern);
			this._contents.Add(c);
		}
		/// <summary>
		/// Delete content
		/// </summary>
		/// <param name="info">delete content information</param>
		public void Remove(string info)
		{
			var f = this._contents.Find(c => c.Name == info);
			if (f != null)
				this._contents.Remove(f);
		}

		[SerializeField]
		string _directory;
		[SerializeField]
		string _file;
		[SerializeField]
		bool _separated;
		[SerializeField]
		bool _streamedScene;
		[SerializeField]
		int _parent = -1;
		[SerializeField]
		bool _platformFolder;
#if false//!DLL && UNITY_EDITOR
		[SerializeField]
		BuildAssetBundleOptions _options;
#else
		[SerializeField]
		int _options;
#endif
		[SerializeField]
		List<iContent> _contents = new List<iContent>();
		[SerializeField]
		bool _changed = true;
	}
}

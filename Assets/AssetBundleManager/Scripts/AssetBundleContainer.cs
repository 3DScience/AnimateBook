//--------------------------------------------------------------------------------
/*
 *	@file		AssetBundleContainer
 *	@brief		AssetBundleManager
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	Container of AssetBundle
 */
//--------------------------------------------------------------------------------
using UnityEngine;

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// AssetBundle container
	/// </summary>
	public class AssetBundleContainer : MonoBehaviour
	{
		/// <summary>AssetBundle</summary>
		public AssetBundle AssetBundle { get; protected set; }
		/// <summary>Assetbundle address</summary>
		public string Name { get; set; }
		/// <summary>Asset list</summary>
		public BundleAssetInfo[] FileList { get; protected set; }

		/// <summary>If assetbundle is ready, return true.</summary>
		public bool IsReady { get; protected set; }
		/// <summary>If assetbundle is ready, return true.</summary>
		public bool IsError { get; protected set; }
		/// <summary>Error message.</summary>
		public string ErrorMsg{ get; protected set; }

		/// <summary>Progress of the loading.[0-1]</summary>
		public float Progress { get; internal set; }

		/// <summary>If the AssetBundle is a streamed scene AssetBundle</summary>
		public bool IsStreamedSceneAssetBundle { get { return this.AssetBundle && this.AssetBundle.isStreamedSceneAssetBundle; } }

		/// <summary>
		/// Initialize.
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="assetbundle">assetbundle</param>
		/// <param name="list">asset list</param>
		public void Initialize(string url, AssetBundle assetbundle, BundleAssetInfo[] list)
		{
			//foreach( var a in list )
			//	Debug.Log( a.Name );
			this.Name = url;
			this.AssetBundle = assetbundle;
			this.IsReady = true;
			this.Progress = 1;

			this.FileList = list;//this.AssetBundle.Load("list", typeof(AssetList)) as AssetList;
		}
		/// <summary>
		/// Set error.
		/// </summary>
		/// <param name="msg">error message</param>
		public void SetError(string msg)
		{
			this.IsError = true;
			this.ErrorMsg = msg;
		}
		/// <summary>
		/// Get asset
		/// </summary>
		/// <typeparam name="T">asset type</typeparam>
		/// <param name="name">asset name</param>
		/// <returns></returns>
		public T GetAsset<T>(string name)
			where T:UnityEngine.Object
		{
			//Debug.Log("GetAsset(" + name + "):");
			if( !this.IsReady )
			{
				Debug.LogError("AssetBundleManager:\n\t" + "Assetbundle don't ready.");
				return null;
			}
			if( this.AssetBundle )
#if UNITY_5
				return this.AssetBundle.LoadAsset<T>(name);
#else
				return this.AssetBundle.Load(name) as T;
#endif
#if UNITY_EDITOR
			else
			{
				// when check "Not load on assetbundle on editor" on Editor
				var loader = base.gameObject.AddComponent<AssetBundleLoader>();
				loader.SetURL(this.Name);
				loader.AssetBundle = this;
				loader.AssetName = name;
				loader.Load();
				var asset = loader.Asset;
				Destroy(loader);
				return asset as T;
			}
#else
			return null;
#endif
		}
		/// <summary>
		/// Get all assets.
		/// </summary>
		/// <returns>all assets</returns>
		public Object[] GetAllAsset()
		{
			return this.GetAllAsset<Object>();
		}
		/// <summary>
		/// Get all assets of type.
		/// </summary>
		/// <typeparam name="T">type</typeparam>
		/// <returns>all assets of type</returns>
		public T[] GetAllAsset<T>()
			where T:Object
		{
			//Debug.Log("GetAllAsset(" + name + "):");
			if( !this.IsReady )
			{
				Debug.LogError("AssetBundleManager:\n\t" + "Assetbundle don't ready.");
				return null;
			}
			if( this.AssetBundle )
			{
#if UNITY_5
				var all = this.AssetBundle.LoadAllAssets<T>();
#else
				var all = System.Array.ConvertAll( this.AssetBundle.LoadAll( typeof( T ) ), asset => asset as T );
#endif
				// remove list file
				System.Collections.Generic.List<T> list = new System.Collections.Generic.List<T>(all);
				list.RemoveAll(a => a.name == "list" || a.name == "BundleAssetList");
				return list.ToArray();
			}
#if UNITY_EDITOR
			else
			{
				// when check "Not load on assetbundle on editor" on Editor
				if( Application.isEditor )
				{
					var data = AssetBundleManager.GetManageData(true);
					if( data )
					{
						if( data.notUseBundleOnEditor )
						{
							// load asset from AssetDatabase
							string fname = this.Name.Substring(this.Name.LastIndexOf('/') + 1);
							var bundle = data.assetbundles.Find(ab => fname.CompareTo(ab.File) == 0);
							if( bundle != null )
							{
								if( !bundle.ForStreamedScene )
								{
									System.Collections.Generic.List<T> list = new System.Collections.Generic.List<T>();
									// Normal AssetBundle
									foreach( var s in bundle.GetAllContentsPath() )
									{
										//print("Assets:" + s);
										//this.Asset = UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(UnityEngine.Object));
										list.Add( UnityEditor.AssetDatabase.LoadAssetAtPath( s, typeof( T ) ) as T );
									}
									return list.ToArray();
								}
							}
							else
							{
								foreach( var ab in data.assetbundles )
								{
									if( ab.Separated )
									{
										foreach( var path in ab.GetAllContentsPath() )
										{
											if( fname.CompareTo(System.IO.Path.GetFileNameWithoutExtension(path) + ".unity3d") == 0 )
											{
												//Debug.Log("Find:" + path);
												return new T[] { UnityEditor.AssetDatabase.LoadAssetAtPath( path, typeof( T ) ) as T };
											}
										}
									}
								}
							}
						}
					}
				}
			}
#endif
			return null;
		}
		/// <summary>
		/// Get asset async.
		/// </summary>
		/// <param name="name">asset name</param>
		/// <returns>loader</returns>
		public AssetBundleLoader GetAssetAsync(string name)
		{
			//Debug.Log("GetAssetAsync(" + name + "):");
			if (!this.IsReady)
			{
				Debug.LogError("AssetBundleManager:\n\t" + "Assetbundle don't ready.");
				return null;
			}
			var loader = base.gameObject.AddComponent<AssetBundleLoader>();
			loader.SetURL(this.Name);
			loader.AssetBundle = this;
			loader.AssetName = name;
			loader.Load();
			return loader;
		}

		/// <summary>
		/// Load the StreamedScene in bundle.
		/// do nothing if the bundle is not StreamedSceneAssetBundle.
		/// </summary>
		/// <param name="name">Scene name</param>
		/// <returns>result</returns>
		public bool LoadStreamedScene(string name)
		{
			if(!this.IsReady)
			{
				Debug.LogError("AssetBundleManager:\n\t" + "Assetbundle don't ready.");
				return false;
			}
			else if(!this.IsStreamedSceneAssetBundle)
			{
				return false;
			}
			// remove ".unity"
			int idx = name.IndexOf(".unity");
			if(0 <= idx)
				name = name.Substring(0, idx);
			// load scene
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
			return true;
		}
	}
}

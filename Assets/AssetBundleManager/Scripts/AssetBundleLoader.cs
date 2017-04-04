//--------------------------------------------------------------------------------
/*
 *	@file		AppendFileLoader
 *	@brief		Assetbundle loader
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	Assetbundle loader
 *	
 */
//--------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// Assetbundle loader
	/// </summary>
	public class AssetBundleLoader : MonoBehaviour
	{
		/// <summary>AssetBundle URL</summary>
		public string URL
		{
			get
			{
				string path = this._path;
#if !NOT_USE_PLATFORM
				if (this.PlatformFolder && !string.IsNullOrEmpty(path))
				{
					path = path.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(Application.platform));
				}
#endif
				if (this._atStreamingAssets)
					return this.GetStreamingAssetsURL(path);
				else
					return path;
			}
		}
		/// <summary>
		/// AssetBundle Path
		/// </summary>
		public string Path{get { return this._path; }}
		/// <summary>AssetBundle</summary>
		public AssetBundleContainer AssetBundle { get { return this._assetbundle; } set { this._assetbundle = value; } }
		/// <summary>AssetBundleName</summary>
		public string AssetBundleName { get { return this._assetbundleName; } set { this._assetbundleName = value; } }
		/// <summary>AssetName</summary>
		public string AssetName { get { return this._assetName; } set { this._assetName = value; } }

		/// <summary>If true, add each platform folder to path.</summary>
		public bool PlatformFolder { get { return this._platformFolder; } set { this._platformFolder = value; } }

		/// <summary>If true, instantiate asset after load.</summary>
		public bool InstantiateAfterLoad { get { return this._instantiateAfterLoad; } set { this._instantiateAfterLoad = value; } }
		/// <summary>Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</summary>
		public int Version { get { return this._version; } set { this._version = value; } }
		/// <summary>An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</summary>
		public uint CRC
		{
			get
			{
				uint crc;
				if( uint.TryParse(this._crc, out crc) )
					return crc;
				return 0;
			}
			set { this._crc = value.ToString(); }
		}

		/// <summary>Return true while load assetbundle.</summary>
		public bool IsLoading { get; protected set; }
		/// <summary>Return true if finished to load assetbundle.</summary>
		public bool IsLoaded { get; protected set; }
		/// <summary>Return progress of loading assetbundle.</summary>
		public float Progress { get { return this.AssetBundle ? this.AssetBundle.Progress : 0; } }

		/// <summary>Loaded asset</summary>
		public Object Asset { get; protected set; }

		/// <summary>Return true while load assetbundle.</summary>
		public bool IsError
		{
			get
			{
				if( this.AssetBundle )
					return this.AssetBundle.IsError;
				return false;
			}
		}
		/// <summary>Error message.</summary>
		public string ErrorMsg
		{
			get
			{
				if( this.AssetBundle )
					return this.AssetBundle.ErrorMsg;
				return null;
			}
		}

		/// <summary>
		/// Load the StreamedScene named Assetname in this.
		/// do nothing if the bundle is not StreamedSceneAssetBundle.
		/// </summary>
		/// <returns>result</returns>
		public bool LoadStreamedScene()
		{
			return this.AssetBundle.LoadStreamedScene(this.AssetName);
		}

		/// <summary>
		/// Set assetbundle URL
		/// </summary>
		/// <param name="url">URL</param>
		public void SetURL(string url)
		{
			//Debug.Log("SetURL(" + url + ")");
			this._path = url;
			this._atStreamingAssets = false;
		}
		/// <summary>
		/// Set assetbundle path (belong StreamingAssets)
		/// </summary>
		/// <param name="path">path</param>
		public void SetStreamingAssetsPath(string path)
		{
			//Debug.Log("SetStreamingAssetsPath(" + path + ")");
			this._path = path.Replace('\\', '/');
			this._atStreamingAssets = true;
		}

		/// <summary>
		/// Load assetbundle
		/// </summary>
		public void Load()
		{
			if( this._path != null )
			{
				this.IsLoaded = false;
				this.IsLoading = true;
				StartCoroutine(this.LoadAssetBundle(this.URL, -1, 0));
			}
		}
		/// <summary>
		/// Loads an AssetBundle with the specified version number from the cache.
		/// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version)
		/// </summary>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
		/// <returns>Assetbundle</returns>
		public void LoadFromCacheOrDownload(int version)
		{
			this.LoadFromCacheOrDownload(version, 0);	// no check-crc 
		}
		/// <summary>
		/// Loads an AssetBundle with the specified version number from the cache.
		/// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version, crc)
		/// </summary>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>Assetbundle</returns>
		public void LoadFromCacheOrDownload(int version, uint crc)
		{
			if( this._path != null )
			{
				this.IsLoading = true;
				StartCoroutine(this.LoadAssetBundle(this.URL, version, crc));
			}
		}

		/// <summary>
		/// make compatible old version
		/// </summary>
		public void MakeCompatibleOldVersion()
		{
			// compatible old version
			if(this.PlatformFolder && !this.Path.Contains( AssetBundleManager.PlatformFolder ))
			{
				int idx = this.Path.LastIndexOf( '/' );
				var dir = this.Path.Substring( 0, idx + 1 );
				var file = this.Path.Substring( idx + 1 );
				this._path = dir + AssetBundleManager.PlatformFolder + "/" + file;
			}
		}

		void Awake()
		{
			this.MakeCompatibleOldVersion();
		}
		void Start()
		{
			if( !this.IsLoading && !this.IsLoaded )
				this.Load();
		}
		void OnDestroy()
		{
			//Debug.LogWarning("\t" + this.AssetBundle + "(" + (this.AssetBundle ? "="+this.AssetBundle.AssetBundle : "none") + ")" + ", " + this.useAssetbundle + ", " + (this.AssetBundle != null ? this.AssetBundle.Name : "null"));
			if( this.AssetBundle )
			{
#if UNITY_EDITOR
				if( this.useAssetbundle )
					AssetBundleManager.Instance.UnloadBundle(this.AssetBundle);
				else
				{
					if( this.AssetBundle.AssetBundle )
						this.AssetBundle.AssetBundle.Unload(false);
					Destroy(this.AssetBundle);
				}
#else
				AssetBundleManager.Instance.UnloadBundle( this.AssetBundle );
#endif
				this.AssetBundle = null;
#if UNITY_EDITOR
				foreach( var bundle in this.parentListOfStreamdSceneAssetBundleLoaded )
					AssetBundleManager.Instance.UnloadBundle(bundle);
#endif
			}
		}

		IEnumerator LoadAssetBundle(string url, int version, uint crc)
		{
			//Debug.Log("LoadAssetBundle " + url);
#if UNITY_EDITOR
			this.useAssetbundle = true;
#else
			bool useAssetbundle = true;
#endif

#if UNITY_EDITOR
			if (Application.isEditor)
			{
				var data = AssetBundleManager.GetManageData(true);
				if (data)
				{
					if( data.notUseBundleOnEditor )
					{
						// load asset from AssetDatabase
						useAssetbundle = false;

						string fname = url.Substring(url.LastIndexOf('/') + 1);
						var bundle = data.assetbundles.Find(ab => fname.CompareTo(ab.File) == 0);
						if( bundle == null )
						{
							bundle = data.assetbundles.Find(ab => {
								if( ab.Separated )
								{
									foreach( var path in ab.GetAllContentsPath() )
									{
										if( fname.CompareTo(System.IO.Path.GetFileNameWithoutExtension(path) + ".unity3d") == 0 )
										{
											//Debug.Log("Find:" + path);
											return true;
										}
									}
								}
								return false;
							});
						}
						if( bundle != null )
						{
							if( !bundle.ForStreamedScene )
							{
								// Normal AssetBundle
								if( !string.IsNullOrEmpty(this.AssetName) )
								{
									bool extension = this.AssetName.Contains(".");
									string assetBaseName = this.AssetName;
									if( extension )
										assetBaseName = assetBaseName.Substring(0, assetBaseName.LastIndexOf('.'));
									foreach( var s in bundle.GetAllContentsPath() )
									{
										//print("Assets:" + s);
										var file = s.Substring(s.LastIndexOf('/') + 1);
										if( (extension && this.AssetName.CompareTo(file) == 0) ||						// compare with extension
											assetBaseName.CompareTo(file.Substring(0, file.LastIndexOf('.'))) == 0		// compare without extension
										)
										{
											//this.Asset = UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(UnityEngine.Object));
											this.Asset = UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(UnityEngine.Object));
											break;
										}
									}
								}
							}
							else
							{
								// StreamedSceneAssetBundle
#if UNITY_EDITOR
#if !DLL
								// check if the scene is contained within "Scene in Builds" on BuildSettings.
								var scenes = UnityEditor.EditorBuildSettings.scenes;
								if( System.Array.Find(scenes, scene => scene.path == fname) == null )
								{
									// not be contained...
									useAssetbundle = true;
									// with follow way, scene is enable at next play.
									//System.Array.Resize(ref scenes, scenes.Length + 1);
									//scenes[scenes.Length - 1] = new UnityEditor.EditorBuildSettingsScene(s, true);
									//UnityEditor.EditorBuildSettings.scenes = scenes;
								}
#else
								useAssetbundle = true;
#endif
								if( useAssetbundle )
								{
									Debug.LogWarning(string.Format(@"AssetBundleManager:
		You can't LoadLevel the scene that is not contained within UnityEditor.EditorBuildSettings.scenes without assetbundle.
		So ""{0}"" is loaded from assetbundle.
		(ignored setting ""Not load on assetbundle on editor"")", System.IO.Path.GetFileNameWithoutExtension(fname)));
									// need to load parent assetbundle
									var bundlework = bundle;
									System.Collections.Generic.Stack<AssetBundleData> bundleStack = new System.Collections.Generic.Stack<AssetBundleData>();
									while( 0 <= bundlework.ParentNo )
									{
										bundlework = data.assetbundles[bundlework.ParentNo];
										bundleStack.Push(bundlework);
									}
									while( 0 < bundleStack.Count )
									{
										bundlework = bundleStack.Pop();
										Debug.Log("AssetBundleManager:\n\tLoad parent assetbundle(" + bundlework.Path + ") for loading " + url + ".");
										var loader = AssetBundleManager.Instance.LoadBundleAsync("file://" + Application.dataPath + "/../" + bundlework.Path);
										this.parentListOfStreamdSceneAssetBundleLoaded.Add(loader);
										while( !loader.IsReady )
											yield return 0;
									}
								}
#endif
							}
							if( !useAssetbundle )
							{
								// if not use assetbundle, create dammy AssetBundleContainer
								var prev = this.AssetBundle;
								this.AssetBundle = AssetBundleManager.Instance.LoadBundleAsync(null);	// dammy
								if( prev )
									AssetBundleManager.Instance.UnloadBundle(prev);
								this.AssetBundle.Initialize(url, null, null);
							}
						}
					}
				}
			}
#endif

			if( useAssetbundle )
			{
				// load from AssetBundle
				var prev = this.AssetBundle;
				this.AssetBundle = AssetBundleManager.Instance.LoadBundleFromCacheOrDownloadAsync(url, version, crc);
				if( prev )
					AssetBundleManager.Instance.UnloadBundle(prev);

				while( !this.AssetBundle.IsReady )
					yield return 0;

				if( !this.AssetBundle.IsError )
				{
					if(!string.IsNullOrEmpty(this.AssetName))
					{
						if(!this.AssetBundle.AssetBundle.isStreamedSceneAssetBundle)
						{
							// find asset type
							System.Type type = typeof(Object);
							// load async
							string assetname = this.AssetName;
							int extidx = assetname.LastIndexOf('.');
							if(0 < extidx)
							{
								type = GetType(assetname.Substring(extidx + 1));
								assetname = assetname.Substring(0, extidx);
							}
							if(UnityEngine.Application.HasProLicense())
							{
								// pro license
#if UNITY_5
								var request = this.AssetBundle.AssetBundle.LoadAssetAsync(assetname, type);
#else
								var request = this.AssetBundle.AssetBundle.LoadAsync(assetname, type);
#endif
								while(!request.isDone)
									yield return 0;
								this.Asset = request.asset; // If AssetName is .unity(Scene Asset), request takes no time and this.Asset is null. 
							}
							else
							{
								// free license
#if UNITY_5
								this.Asset = this.AssetBundle.AssetBundle.LoadAsset(assetname, type);
#else
								this.Asset = this.AssetBundle.AssetBundle.Load(assetname, type);
#endif
							}
						}
					}
				}
			}

			if( this.InstantiateAfterLoad )
			{
				if( this.Asset is GameObject )
				{
					// instantiate prefab
					var obj = Instantiate(this.Asset) as GameObject;
					Vector3 wpos = obj.transform.localPosition;
					Vector3 wscale = obj.transform.localScale;
					Quaternion wq = obj.transform.localRotation;
					obj.transform.parent = base.transform;
					obj.transform.localPosition = wpos;
					obj.transform.localScale = wscale;
					obj.transform.localRotation = wq;
				}
				else
				{
					if(this.AssetBundle.IsStreamedSceneAssetBundle)
					{
						this.AssetBundle.LoadStreamedScene(this.AssetName);
					}
				}
			}
			this.IsLoading = false;
			this.IsLoaded = true;
			yield break;
		}

		string GetStreamingAssetsURL(string path)
		{
			if( Application.platform == RuntimePlatform.Android )
			{
				// include "jar:file://" in Application.streamingAssetsPath
				return string.Format("{0}/{1}", Application.streamingAssetsPath, path);
			}
			return string.Format("file://{0}/{1}", Application.streamingAssetsPath, path);
		}
		static System.Type GetType( string ext )
		{
			switch( ext )
			{
			case "prefab":
				return typeof( GameObject );
			case "png":
			case "jpg":
			case "bmp":
			case "tga":
				return typeof( Texture );
			case "mat":
				return typeof( Material );
			case "txt":
				return typeof( TextAsset );
			}
			return typeof( Object );
		}

		[SerializeField]
		string _path;
		[SerializeField]
		string _assetbundleName;
		[SerializeField]
		string _assetName;
		[SerializeField]
		bool _atStreamingAssets;
		[SerializeField]
		bool _platformFolder;
		[SerializeField]
		bool _instantiateAfterLoad;
		[SerializeField, UnityEngine.Range(0, int.MaxValue)]
		int _version = -1;
		[SerializeField]
		string _crc;

		AssetBundleContainer _assetbundle;

#if UNITY_EDITOR
		// list of parent assetbundle loaded for StreamedSceneAssetBundle.
		System.Collections.Generic.List<AssetBundleContainer> parentListOfStreamdSceneAssetBundleLoaded = new System.Collections.Generic.List<AssetBundleContainer>();
		// if load from assetbundle, true
		bool useAssetbundle;
#endif
	}
}

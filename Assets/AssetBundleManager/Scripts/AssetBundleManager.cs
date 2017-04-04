//--------------------------------------------------------------------------------
/*
 *	@file		AppendFileManager
 *	@brief		AssetBundleManager
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	AssetBundleManager
 */
//--------------------------------------------------------------------------------

// if you don't need log message, comment-out follow line.
#define ASSETBUNDLE_LOG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if !DLL && UNITY_EDITOR
using UnityEditor;
#endif

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// Assetbundle Manager
	/// </summary>
	public class AssetBundleManager: MonoBehaviour
	{
		/// <summary>Platform folder</summary>
		public const string PlatformFolder = "$(Platform)";

		#region Singleton
		/// <summary>Singleton object</summary>
		public static AssetBundleManager Instance
		{
			get
			{
				if (_instance == null)
				{
					//Debug.LogError("Call CreateInstance() before use \"AssetBundleManager.I\".");
					var go = new GameObject("AssetBundleManager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<AssetBundleManager>();
				}
				return _instance;
			}

		}
		/// <summary>Destroy singleton object</summary>
		public static void DestroyInstance()
		{
			if (_instance != null)
			{
				var go = GameObject.Find("AssetBundleManager");
				if (go)
					DestroyImmediate(go);
				_instance = null;
			}
		}
		static AssetBundleManager _instance;
		#endregion

		/// <summary>
		/// Check if has loaded assetbundle on url.
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <returns>return true if assetbundle has been loaded</returns>
		public bool HasLoaded(string url)
		{
			return this._assetBundleDic.ContainsKey(url);
		}
		/// <summary>
		/// Loads an AssetBundle with the specified version number from the cache.
		/// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version)
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
		/// <returns>Assetbundle</returns>
		public AssetBundleContainer LoadBundleFromCacheOrDownloadAsync(string url, int version)
		{
			return this.LoadBundleFromCacheOrDownloadAsync(url, version, 0);	// no check-crc 
		}
		/// <summary>
		/// Loads an AssetBundle with the specified version number from the cache.
		/// refer to UnityEngine.WWW.LoadFromCacheOrDownload(url, version, crc)
		/// </summary>
		/// <param name="url">The URL to download the AssetBundle from, if it is not present in the cache. Must be '%' escaped.</param>
		/// <param name="version">Version of the AssetBundle. The file will only be loaded from the disk cache if it has previously been downloaded with the same version parameter. By incrementing the version number requested by your application, you can force Caching to download a new copy of the AssetBunlde from url.</param>
		/// <param name="crc">An optional CRC-32 Checksum of the uncompressed contents. If this is non-zero, then the content will be compared against the checksum before loading it, and give an error if it does not match. You can use this to avoid data corruption from bad downloads or users tampering with the cached files on disk. If the CRC does not match, Unity will try to redownload the data, and if the CRC on the server does not match it will fail with an error. Look at the error string returned to see the correct CRC value to use for an AssetBundle.</param>
		/// <returns>Assetbundle</returns>
		public AssetBundleContainer LoadBundleFromCacheOrDownloadAsync(string url, int version, uint crc)
		{
			//Debug.Log("LoadBundleAsync " + url);
			if( url != null )
				url = url.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(Application.platform));
			Value data;
			if ( string.IsNullOrEmpty(url) || !this._assetBundleDic.TryGetValue(url, out data))
			{
				// not find...
				data = new Value();
				data.AssetBundle = this.gameObject.AddComponent<AssetBundleContainer>();
				if( !string.IsNullOrEmpty(url) )
				{
					Debug.Log("AssetBundleManager:\n\t" + "LoadBundle " + url);
					this._assetBundleDic.Add(url, data);
					StartCoroutine(this.LoadAssetBundle(data.AssetBundle, url, version, crc));
				}
			}

			// add counter
			++data.Counter;
			return data.AssetBundle;
		}
		/// <summary>
		/// Load async assetbundle.
		/// </summary>
		/// <param name="url">Assetbundle URL</param>
		/// <returns>Assetbundle</returns>		
		public AssetBundleContainer LoadBundleAsync(string url)
		{
			return this.LoadBundleFromCacheOrDownloadAsync(url, -1, 0);
		}
#if false
		/// <summary>
		/// Load assetbundle.
		/// </summary>
		/// <param name="filename">Assetbundle URL</param>
		/// <returns>Assetbundle</returns>
		public AssetBundleContainer LoadBundle(string filename)
		{
			//Debug.Log("LoadBundle " + filename);
			Value data;
			if (!this._assetBundleDic.TryGetValue(filename, out data))
			{
				Debug.Log("LoadBundle " + data);
				// not find...
				data = new Value();
				data.AssetBundle = base.gameObject.AddComponent<AssetBundleContainer>();
				var bundle = AssetBundle.CreateFromFile(?);
				this._assetBundleDic.Add(filename, data);
				var list = www.assetBundle.Load("list", typeof(BundleAssetList)) as BundleAssetList;
				data.AssetBundle.Initialize(filename, bundle, list.Assets);
			}

			// add counter
			++data.Counter;
			return data.AssetBundle;
		}
#endif

		/// <summary>
		/// Unload assetbundle
		/// </summary>
		/// <param name="assetbundle">Assetbundle</param>
		public void UnloadBundle(AssetBundleContainer assetbundle)
		{
			if( assetbundle != null )
			{
				if( !string.IsNullOrEmpty(assetbundle.Name) )
					this.UnloadBundle(assetbundle.Name);
				else
				{
					// for debug
					if( assetbundle.AssetBundle )
						assetbundle.AssetBundle.Unload(false);
					Destroy(assetbundle);
				}
			}
		}
		/// <summary>
		/// Unload assetbundle
		/// </summary>
		/// <param name="filename">Assetbundle URL</param>
		public void UnloadBundle(string filename)
		{
			if (!string.IsNullOrEmpty(filename) && this._assetBundleDic.ContainsKey(filename))
			{
				// not find...
				Value data = this._assetBundleDic[filename];
				if( --data.Counter <= 0 )
				{
					Debug.Log("AssetBundleManager:\n\t" + "Unload " + filename);
					if( data.AssetBundle )
					{
						if( data.AssetBundle.AssetBundle )
							data.AssetBundle.AssetBundle.Unload(false);
						Destroy(data.AssetBundle);
					}
					this._assetBundleDic.Remove( filename );
				}
			}
		}

#if !NOT_USE_PLATFORM
		/// <summary>
		/// Get platform folder
		/// </summary>
		/// <param name="platform"></param>
		/// <returns>each platform folder</returns>
		public static string GetPlatformFolder(RuntimePlatform platform)
		{
			switch (platform)
			{
#if UNITY_EDITOR
#if !DLL
			case RuntimePlatform.OSXEditor:
				return GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget);
			case RuntimePlatform.WindowsEditor:
				return GetPlatformFolder( EditorUserBuildSettings.activeBuildTarget );
#else
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				{
					// use Reflection for UnityEditor
					var type = Types.GetType("isotope.GetPlatformClass", "Assembly-CSharp-Editor");
					var getPlatform = type.GetMethod("GetPlatform");
					return getPlatform.Invoke(null, null) as string;
				}
#endif
#endif
			case RuntimePlatform.OSXPlayer:
				return "Standalone";
			case RuntimePlatform.WindowsPlayer:
				return "Standalone";

#if UNITY_5_5_OR_NEWER
			case RuntimePlatform.WebGLPlayer:
#else
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
#endif
			case RuntimePlatform.OSXDashboardPlayer:
				return "WebPlayer";
			case RuntimePlatform.IPhonePlayer:
				return "iPhone";

#if !UNITY_5_5_OR_NEWER
			case RuntimePlatform.PS3:
				return "PS3";
			case RuntimePlatform.XBOX360:
				return "XBOX360";
#endif

			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.LinuxPlayer:
				return "Standalone";

#if !UNITY_5
			case RuntimePlatform.NaCl:
				break;
			case RuntimePlatform.FlashPlayer:
				return "Flash";
#endif
#if UNITY_5
			case RuntimePlatform.WSAPlayerX86:
			case RuntimePlatform.WSAPlayerX64:
			case RuntimePlatform.WSAPlayerARM:
#if !UNITY_5_3_OR_NEWER
			case RuntimePlatform.WP8Player:
			//case RuntimePlatform.BB10Player:
#endif
#else
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
#endif
				break;

#if UNITY_4_5 || UNITY_5
#if !UNITY_5_4_OR_NEWER
			case RuntimePlatform.BlackBerryPlayer:
#endif
			case RuntimePlatform.TizenPlayer:
			case RuntimePlatform.PSP2:
				break;
			case RuntimePlatform.PS4:
				return "PS4";
#if !UNITY_5
			case RuntimePlatform.PSMPlayer:
				return "PSM";
#endif
			case RuntimePlatform.XboxOne:
				return "XboxOne";
			case RuntimePlatform.SamsungTVPlayer:
				break;
#endif
			}
			return platform.ToString();
		}
#endif

#if UNITY_EDITOR && !DLL
		/// <summary>
		/// Get platform folder
		/// </summary>
		/// <param name="platform"></param>
		/// <returns>each platform folder</returns>
		public static string GetPlatformFolder(UnityEditor.BuildTarget platform)
		{
			switch (platform)
			{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
				return "Standalone";
			case BuildTarget.StandaloneWindows:
				return "Standalone";
#if UNITY_5
			case BuildTarget.WebGL:
#endif
#if !UNITY_5_5_OR_NEWER
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
#endif
				return "WebPlayer";
#if UNITY_5
			case BuildTarget.iOS:
#else
			case BuildTarget.iPhone:
#endif
				return "iPhone";
#if !UNITY_5_5_OR_NEWER
			case BuildTarget.PS3:
				return "PS3";
			case BuildTarget.XBOX360:
				return "XBOX360";
#endif
			case BuildTarget.Android:
				return "Android";
#if !UNITY_5
			case BuildTarget.StandaloneGLESEmu:
			case BuildTarget.NaCl:
				break;
			case BuildTarget.FlashPlayer:
				return "Flash";
#endif
			case BuildTarget.StandaloneLinux:
			case BuildTarget.StandaloneWindows64:
#if UNITY_5
			case BuildTarget.WSAPlayer:
#else
			case BuildTarget.MetroPlayer:
			case BuildTarget.WP8Player:
#endif
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
			case BuildTarget.StandaloneOSXIntel64:
				return "Standalone";
			
#if UNITY_4_5 || UNITY_5
#if !UNITY_5_4_OR_NEWER
			case BuildTarget.BlackBerry:
#endif
			//case BuildTarget.BB10:
			case BuildTarget.Tizen:
			case BuildTarget.PSP2:
			case BuildTarget.PS4:
				return "PS4";
			case BuildTarget.PSM:
				return "PSM";
			case BuildTarget.XboxOne:
				return "XboxOne";
			case BuildTarget.SamsungTV:
				break;
#endif
			}
			return platform.ToString();
		}
#endif
		/// <summary>
		/// Load manage data
		/// </summary>
		/// <param name="loadBackup">if exist, load backup file</param>
		/// <returns></returns>
		public static AssetBundleManageData GetManageData(bool loadBackup)
		{
			AssetBundleManageData abd = null;
#if UNITY_EDITOR
			string path = SettingDataPath;
			if (loadBackup)
			{
				var backupname = SettingBackupDataPath;
				if (File.Exists(Application.dataPath + "/" + backupname))
				{
					abd = AssetDatabase.LoadAssetAtPath("Assets/" + backupname, typeof(AssetBundleManageData)) as AssetBundleManageData;
				}
			}
			if (abd == null)
				abd = AssetDatabase.LoadAssetAtPath("Assets/" + path, typeof(AssetBundleManageData)) as AssetBundleManageData;
#endif
			return abd;
		}
#if UNITY_EDITOR
		/// <summary>設定データパス</summary>
		public static string SettingDataPath
		{
			get
			{
				var path = GetPrefsString(SettingDataKey);
				if (string.IsNullOrEmpty(path) || !File.Exists(Application.dataPath + "/" + path))
				{
					// find directory "AssetBundleManager"
					var find = FindDirectory(Application.dataPath, "AssetBundleManager");
					if (find == null)
						find = "Assets/";	// project root directory
					find = find.Replace('\\', '/');
					path = find.Substring(find.IndexOf("Assets/") + "Assets/".Length) + "/AssetBundleManagerSetting.asset";
					SetPrefsString(AssetBundleManager.SettingDataKey, path);
				}
				return path;
			}
		}
		/// <summary>設定バックアップデータパス</summary>
		public static string SettingBackupDataPath
		{
			get
			{
				if (!string.IsNullOrEmpty(SettingDataPath))
				{
					var path = SettingDataPath;
					return path.Substring(0, path.LastIndexOf('.')) + "_backup.asset";
				}
				return "";
			}
		}

		static string GetPrefsString(string name)
		{
#if !DLL
			return EditorPrefs.GetString(SettingDataKey);
#else
			return UnityEngine.PlayerPrefs.GetString(SettingDataKey);
#endif
		}
		static void SetPrefsString(string name, string value)
		{
#if !DLL
			EditorPrefs.SetString(SettingDataKey, value);
#else
			UnityEngine.PlayerPrefs.SetString(SettingDataKey, value);
#endif
		}

		const string SettingDataKey = "AssetBundleManagerDataDirectory";
#endif

		IEnumerator LoadAssetBundle(AssetBundleContainer bundle, string url, int version, uint crc)
		{
			WWW www = null;
			if( 0 <= version )
				www = WWW.LoadFromCacheOrDownload(url, version, crc);
			else
				www = new WWW(url);

			while (!www.isDone)
			{
				bundle.Progress = www.progress;
				yield return 9;
			}
			
			if (www.error != null)
			{
				Debug.LogError(string.Format("AssetBundleManager:\n\tload \"{0}\" error:{1}", url, www.error));
				bundle.Initialize(url, null, null);
				bundle.SetError(www.error);
			}
			else
			{
				BundleAssetInfo[] list = null;
				if (!www.assetBundle.isStreamedSceneAssetBundle)
				{
					if(Application.HasProLicense())
					{
#if UNITY_5
						var request = www.assetBundle.LoadAssetAsync<BundleAssetList>("list");
#else
						var request = www.assetBundle.LoadAsync( "list", typeof( BundleAssetList ) );
#endif
						yield return request;
						var bal = request.asset as BundleAssetList;
						list = bal.Assets;
					}
					else
					{
#if UNITY_5
						var bal = www.assetBundle.LoadAsset<BundleAssetList>("list");
#else
						var bal = www.assetBundle.Load( "list", typeof( BundleAssetList ) ) as BundleAssetList;
#endif
						list = bal.Assets;
					}
				}
				else
				{
					var paths = www.assetBundle.GetAllScenePaths();
					list = new BundleAssetInfo[paths.Length];
					for(int i = 0; i < paths.Length; ++i)
						list[i] = new BundleAssetInfo(paths[i]);
				}
				if ( list != null )
					bundle.Initialize(url, www.assetBundle, list);
				else
					//Debug.LogWarning( "There is no BundleAssetList in " + www.url );
					bundle.Initialize(url, www.assetBundle, null);
			}
			if( www != null )
				www.Dispose();
		}

		// Value for dictionary
		class Value
		{
			public AssetBundleContainer AssetBundle { get; set; }
			public int Counter { get; set; }
		}
		Dictionary<string, Value> _assetBundleDic = new Dictionary<string, Value>();

#if UNITY_EDITOR
		// Find directory from path
		static string FindDirectory(string path, string find)
		{
			var directories = Directory.GetDirectories(path);
			foreach (var f in directories)
			{
				if (Path.GetFileName(f) == find)
					return f;
			}
			foreach (var f in directories)
			{
				var ret = FindDirectory(f, find);
				if (ret != null)
					return ret;
			}
			return null;
		}	
#endif
	}

	namespace DebugUtility
	{
		class Debug
		{
			//[System.Diagnostics.Conditional("ASSETBUNDLE_LOG")]
			public static void Log(string format, params object[] args)
			{
#if ASSETBUNDLE_LOG
				UnityEngine.Debug.Log(string.Format(format, args));
#endif
			}
			//[System.Diagnostics.Conditional("ASSETBUNDLE_LOG")]
			public static void LogError(string format, params object[] args)
			{
#if ASSETBUNDLE_LOG
				UnityEngine.Debug.LogError(string.Format(format, args));
#endif
			}
			//[System.Diagnostics.Conditional("ASSETBUNDLE_LOG")]
			public static void LogWarning(string format, params object[] args)
			{
#if ASSETBUNDLE_LOG
				UnityEngine.Debug.LogWarning(string.Format(format, args));
#endif
			}
		}
	}
}

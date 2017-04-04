//--------------------------------------------------------------------------------
/*
 *	@file		AssetBundleUtility
 *	@brief		AssetBundleUtility
 *	@ingroup	AssetBundleManager
 *	@version	1.00
 *	@date		2013.09
 *	AssetBundleUtility
 *	
 */
//--------------------------------------------------------------------------------
#define UPDATE_CHECK

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// ignore warning of using Unity4 Assetbundle system
#pragma warning disable 618

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// AssetBundleUtility
	/// </summary>
	public static class AssetBundleUtility
	{
		/// <summary>AssetList of assetbundle that you build from now</summary>
		public static BundleAssetList AssetList { get; set; }

		/// <summary>
		/// Create assetbundle from a file
		/// </summary>
		/// <param name="file">asset file</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateBundle( string file, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateBundleProc(new string[] { file }, target, false, option, platform, rebuild);
		}
		/// <summary>
		/// Create assetbundle from files in directory.
		/// </summary>
		/// <param name="dir">directory</param>
		/// <param name="extension">target file extension</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateBundleFromDirectory( string dir, string extension, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateBundle( Directory.GetFiles( Application.dataPath + dir, "*." + extension ), target, option, platform, rebuild );
		}
		/// <summary>
		/// Create assetbundle from files in directories.
		/// </summary>
		/// <param name="dirs">directory list</param>
		/// <param name="extension">target file extension</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateBundleFromDirectory( string[] dirs, string extension, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateBundle( dirs.SelectMany( dir => Directory.GetFiles( Application.dataPath + dir, "*." + extension ) ).ToArray(), target, option, platform, rebuild );
		}
		/// <summary>
		/// Create assetbundle from a files
		/// </summary>
		/// <param name="files">asset file list</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateBundle( string[] files, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateBundleProc( files, target, false, option, platform, rebuild );
		}
		/// <summary>
		/// Create streamed scene assetbundle from a file
		/// </summary>
		/// <param name="file">asset file</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateSceneBundle( string file, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateBundleProc( new string[] { file }, target, true, option, platform, rebuild );
		}
		/// <summary>
		/// Create streamed scene assetbundle from files in directory.
		/// </summary>
		/// <param name="dir">directory</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateSceneBundleFromDirectory( string dir, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateSceneBundle( Directory.GetFiles( Application.dataPath + dir, "*.unity" ), target, option, platform, rebuild );
		}
		/// <summary>
		/// Create streamed scene assetbundle from a files
		/// </summary>
		/// <param name="files">asset file list</param>
		/// <param name="target">output assetbundle path</param>
		/// <param name="option">BuildAssetBundleOptions</param>
		/// <param name="platform">BuildTarget</param>
		/// <param name="rebuild">rebuild?</param>
		/// <returns>success or fail</returns>
		public static bool CreateSceneBundle( string[] files, string target, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			return CreateBundleProc( files.Select( s => s ).ToArray(), target, true, option, platform, rebuild );
		}
		// ファイルリストからアセットバンドル生成（更新日チェック、Streamingのチェック付き）
		static bool CreateBundleProc( string[] filelist, string target, bool isScene, BuildAssetBundleOptions option, BuildTarget platform, bool rebuild )
		{
			bool ret = false;
			// \ を / に変換
			var files = filelist.Select(f => f.Replace('\\', '/'));
			// Assetsから始める指定形式
			var filesAssets = files.Select(s => 0 <= s.IndexOf("Assets/") ? s.Substring(s.IndexOf("Assets/")) : "Assets/" + s);
			if( !isScene )
			{
				//Debug.Log( "rebuild:" + rebuild );
				//Debug.Log( "bundle:" + targetTime );
				//foreach( var f in dependFiles )
				//	Debug.Log( f + ":" + File.GetLastWriteTime( f ) );
				if( rebuild || CheckUpdateOfBundleAssets(filelist, target, option) )
				{
					IEnumerable<Object> objects = filesAssets.Select( s => AssetDatabase.LoadAssetAtPath( s.Substring( s.IndexOf( "Assets" ) ), typeof( Object ) ) );
					List<Object> list = new List<Object>( objects );
					if( AssetList )
					{
						list.Add( AssetList );
						var monos = MonoScript.FromScriptableObject( AssetList as ScriptableObject );	// BundleAssetList.cs
						list.Add( monos );
					}
					ret =
						BuildPipeline.BuildAssetBundle( null, list.ToArray(), target, option, platform );
				}
				else
				{
					Print( "Skip(because not update) {0}", target );
				}
			}
			else
			{
				//Print("Last:{0}", filesAssets.Max(f => File.GetLastWriteTime(f)));
				if( rebuild || CheckUpdateOfBundleAssets(filelist, target, option) )
				{
					string error = BuildPipeline.BuildStreamedSceneAssetBundle( filesAssets.ToArray(), target, platform );
					if( !string.IsNullOrEmpty(error) )
						Debug.LogError( error );
					ret = string.IsNullOrEmpty(error);
				}
				else
				{
					Print( "Skip(because not update) {0}", target );
				}
			}
			//if (ret)
			//	Print("Create {0}", target);
			return ret;
		}

		/// <summary>
		/// Check if assets of assetbundle is updated
		/// </summary>
		/// <returns>return true if assets of assetbundle is updated updated.</returns>
		public static bool CheckUpdateOfBundleAssets(string[] filelist, string bundlePath, BuildAssetBundleOptions option)
		{
			System.DateTime targetTime = File.GetLastWriteTime( bundlePath );
			
			// \ を / に変換
			var files = filelist.Select(f => f.Replace('\\', '/'));
			// Assetsから始める指定形式
			var filesAssets = files.Select(s => 0 <= s.IndexOf("Assets/") ? s.Substring(s.IndexOf("Assets/")) : "Assets/" + s);
			// 関連ファイル一覧
			string[] dependFiles;
			if( ( option & BuildAssetBundleOptions.CollectDependencies ) != 0 )
				dependFiles = AssetDatabase.GetDependencies( filesAssets.Select( s => s.Substring( s.IndexOf( "Assets/" ) ) ).ToArray() );
			else
				dependFiles = filesAssets.ToArray();

			return dependFiles.Any( f => targetTime < File.GetLastWriteTime( f ) || targetTime < File.GetLastWriteTime( f + ".meta" ) );
		}

		/// <summary>
		/// Convert from absolute path to relative path
		/// </summary>
		/// <param name="basePath">relative base path</param>
		/// <param name="filePath">absolute path</param>
		/// <returns>relative path</returns>
		public static string ChangeRelativePath(string basePath, string filePath)
		{
			//"%"を"%25"に変換しておく（デコード対策）
			basePath = basePath.Replace("%", "%25");
			filePath = filePath.Replace("%", "%25");

			//相対パスを取得する
			System.Uri baseUrl = new System.Uri(basePath);
			System.Uri fileUrl = new System.Uri(filePath);
			System.Uri relativeUri = baseUrl.MakeRelativeUri(fileUrl);
			string relativePath = relativeUri.ToString();

			//URLデコードする（エンコード対策）
			relativePath = System.Uri.UnescapeDataString(relativePath);
			//"%25"を"%"に戻す
			relativePath = relativePath.Replace("%25", "%");

			return relativePath;
		}

		static void Print(string format, params object[] args)
		{
#if UNITY_EDITOR
			Debug.Log(string.Format(format, args));
#endif
		}

#if false
		[MenuItem("Assets/AssetBundle/From Selection - Track dependencies")]
		static void ExportResource()
		{
			// Bring up save panel
			string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
			if (path.Length != 0)
			{
				// Build the resource file from the active selection.
				Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
				BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
					BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, EditorUserBuildSettings.activeBuildTarget);
				Selection.objects = selection;
			}
		}
		[MenuItem("Assets/AssetBundle/From Selection - No dependency tracking")]
		static void ExportResourceNoTrack()
		{
			// Bring up save panel
			string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
			if (path.Length != 0)
			{
				// Build the resource file from the active selection.
				BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path, (BuildAssetBundleOptions)0, EditorUserBuildSettings.activeBuildTarget);
			}
		}
#endif
	}
}

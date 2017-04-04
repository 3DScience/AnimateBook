using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace AssetBundles
{
	public class BuildScript
	{
		public static string overloadedDevelopmentServerURL = "";
	
		public static void BuildAssetBundles()
		{
			// Choose the output path according to the build target.
			string outputPath = Path.Combine(Utility.AssetBundlesOutputPath,  Utility.GetPlatformName());
			if (!Directory.Exists(outputPath) )
				Directory.CreateDirectory (outputPath);
            string[] str = AssetDatabase.GetAllAssetBundleNames();
            foreach (string item in str)
            {
                Debug.Log("Asset Name="+item+"");
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[3];

//			buildMap[0].assetBundleName = "universal";
//			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("universal");


//			buildMap[0].assetBundleName = "nearest_stars";
//			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("nearest_stars");
//			buildMap[1].assetBundleName = "universal";
//			buildMap[1].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("universal");

//            buildMap[0].assetBundleName = "q10k_01";
//			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("q10k_01");

//			buildMap[0].assetBundleName = "solar_system_book";
//			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("solar_system_book");

			buildMap[0].assetBundleName = "weather_maker";
			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("weather_maker");

//			buildMap[0].assetBundleName = "fishingman";
//			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("fishingman");
//			buildMap[1].assetBundleName = "weather_maker";
//			buildMap[1].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("weather_maker");


//			buildMap[0].assetBundleName = "littleredridinghood";
//			buildMap[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("littleredridinghood");

			BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            //AssetDatabase.
            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            //BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);

			string src = outputPath + "/" + EditorUserBuildSettings.activeBuildTarget.ToString();
			string des = outputPath + "/" + buildMap[0].assetBundleName + ".mf";
			if (File.Exists(des))
			{
				File.Delete(des);
			}

			Debug.Log("move file " + src+" ====> "+ des);
			System.IO.File.Move(src,des);

		}
	
		public static void WriteServerURL()
		{
			string downloadURL;
			if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
			{
				downloadURL = overloadedDevelopmentServerURL;
			}
			else
			{
				IPHostEntry host;
				string localIP = "";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIP = ip.ToString();
						break;
					}
				}
				downloadURL = "http://"+localIP+":7888/";
			}
			
			string assetBundleManagerResourcesDirectory = "Assets/AssetBundleManager/Resources";
			string assetBundleUrlPath = Path.Combine (assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
			Directory.CreateDirectory(assetBundleManagerResourcesDirectory);
			File.WriteAllText(assetBundleUrlPath, downloadURL);
			AssetDatabase.Refresh();
		}
	
		public static void BuildPlayer()
		{
			var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
			if (outputPath.Length == 0)
				return;
	
			string[] levels = GetLevelsFromBuildSettings();
			if (levels.Length == 0)
			{
				Debug.Log("Nothing to build.");
				return;
			}
	
			string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
			if (targetName == null)
				return;
	
			// Build and copy AssetBundles.
			BuildScript.BuildAssetBundles();
			WriteServerURL();
	
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
		}
		
		public static void BuildStandalonePlayer()
		{
			var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
			if (outputPath.Length == 0)
				return;
			
			string[] levels = GetLevelsFromBuildSettings();
			if (levels.Length == 0)
			{
				Debug.Log("Nothing to build.");
				return;
			}
			
			string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
			if (targetName == null)
				return;
			
			// Build and copy AssetBundles.
			BuildScript.BuildAssetBundles();
			BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundlesOutputPath) );
			AssetDatabase.Refresh();
			
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
		}
	
		public static string GetBuildTargetName(BuildTarget target)
		{
			switch(target)
			{
			case BuildTarget.Android :
				return "/test.apk";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "/test.exe";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "/test.app";
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
			case BuildTarget.WebGL:
				return "";
				// Add more build targets for your own.
			default:
				Debug.Log("Target not implemented.");
				return null;
			}
		}
	
		static void CopyAssetBundlesTo(string outputPath)
		{
			// Clear streaming assets folder.
			FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
			Directory.CreateDirectory(outputPath);
	
			string outputFolder = Utility.GetPlatformName();
	
			// Setup the source folder for assetbundles.
			var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, Utility.AssetBundlesOutputPath), outputFolder);
			if (!System.IO.Directory.Exists(source) )
				Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
	
			// Setup the destination folder for assetbundles.
			var destination = System.IO.Path.Combine(outputPath, outputFolder);
			if (System.IO.Directory.Exists(destination) )
				FileUtil.DeleteFileOrDirectory(destination);
			
			FileUtil.CopyFileOrDirectory(source, destination);
		}
	
		static string[] GetLevelsFromBuildSettings()
		{
			List<string> levels = new List<string>();
			for(int i = 0 ; i < EditorBuildSettings.scenes.Length; ++i)
			{
				if (EditorBuildSettings.scenes[i].enabled)
					levels.Add(EditorBuildSettings.scenes[i].path);
			}
	
			return levels.ToArray();
		}
	}
}
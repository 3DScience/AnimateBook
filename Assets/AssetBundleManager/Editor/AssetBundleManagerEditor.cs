//--------------------------------------------------------------------------------
/*
 *	@file		AssetFileManagerEditor
 *	@brief		AssetBundleManager
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	Editor of AssetBundleManager
 */
//--------------------------------------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

// ignore warning of using Unity4 Assetbundle system
#pragma warning disable 618

namespace isotope
{
    /// <summary>
    /// Editor window of AssetBundleManager
    /// </summary>
    public class AssetBundleManagerEditor : EditorWindow
	{
		/// <summary>
		/// Open editor window of AssetBundleManager.
		/// </summary>
		[MenuItem("Assets/AssetBundleManager/Setting")]
		public static void Open()
		{
			GetWindow<AssetBundleManagerEditor>(true, "AssetBundleManager");
		}

#if !UNITY_5
		// when you call BuildPipeline.BuildAssetBundle on Unity5, asset of setting file is missing ...
		// so remove this menu, sorry.
		/// <summary>
		/// Build all assetbundles.
		/// </summary>
		[MenuItem("Assets/AssetBundleManager/Build All")]
		public static void BuildAll()
		{
			// Create all assetbundles
			AssetBundleManageData abd = AssetBundleManager.GetManageData(true);
			if (abd)
			{
				CreateAssetBundleAll(abd, false);
				Save(abd);
				AssetDatabase.Refresh();
			}
		}
		/// <summary>
		/// Rebuild all assetbundles
		/// </summary>
		[MenuItem( "Assets/AssetBundleManager/Rebuild All" )]
		public static void RebuildALl()
		{
			// Create all assetbundles
			AssetBundleManageData abd = AssetBundleManager.GetManageData( true );
			if( abd )
			{
				CreateAssetBundleAll( abd, true );
				Save( abd );
				AssetDatabase.Refresh();
			}
		}
#endif

		/// <summary>Assetbundle path list</summary>
		internal GUIContent[] AssetBundlePathList { get { return this._assetBundlePathList; } }
		

#if false
		/// <summary>
		/// Get assetbundle data
		/// </summary>
		/// <param name="idx">no</param>
		/// <returns>assetbundle data</returns>
		public AssetBundleData GetAssetBundleData(int idx)
		{
			if (0 <= idx && idx < this._assetBundleData.assetbundles.Count)
				return this._assetBundleData.assetbundles[idx];
			return null;
		}
#endif
		/// <summary>
		/// Check circular reference with ParentNo
		/// </summary>
		/// <returns>result</returns>
		internal bool CheckCircularReferencek()
		{
			var bundles = this._assetBundleData.assetbundles;
			System.Collections.BitArray bitarray = new System.Collections.BitArray(bundles.Count);
			for (int i = 0; i < bundles.Count; ++i)
			{
				bitarray.SetAll(false);
				int idx = i;
				while (0 <= bundles[idx].ParentNo)
				{
					if (bitarray.Get(idx))
						return true;	// circular...
					bitarray.Set(idx, true);
					idx = bundles[idx].ParentNo;
				}
			}
			return false;
		}
		/// <summary>
		/// Check parent setting
		/// </summary>
		/// <returns>result[true:ok]</returns>
		internal bool CheckParentSetting( AssetBundleData data, bool child )
		{
			if( child )
			{
				// check as child
				if( 0 <= data.ParentNo && data.ParentNo < this._assetBundleData.assetbundles.Count )
					return !this._assetBundleData.assetbundles[data.ParentNo].Separated;
			}
			else
			{
				// check as parent
				if( data.Separated )
				{
					int no = this._assetBundleData.assetbundles.IndexOf( data );
					if( 0 <= no )
					{
						foreach( var b in this._assetBundleData.assetbundles )
						{
							if( b.ParentNo == no )
								return false;
						}
					}
				}
			}
			return true;
		}

		void Awake()
		{
			// if not exist setting data, create it.
			var path = AssetBundleManager.SettingDataPath;
			if (!File.Exists(Application.dataPath + "/" + path))
			{
				// if not exist, create setting data.
				if (!Directory.Exists(Application.dataPath + "/" + Path.GetDirectoryName(path)))
					Directory.CreateDirectory(Application.dataPath + "/" + Path.GetDirectoryName(path));
				var create = AssetBundleManageData.CreateInstance<AssetBundleManageData>();
				AssetDatabase.CreateAsset(create, "Assets/" + path);
				AssetDatabase.Refresh();
			}
			var backupname = AssetBundleManager.SettingBackupDataPath;
			AssetDatabase.DeleteAsset("Assets/" + backupname);
			if (AssetDatabase.CopyAsset("Assets/" + path, "Assets/" + backupname))
			{
				AssetDatabase.Refresh();
				//var abd = AssetDatabase.LoadAssetAtPath("Assets/" + backupname, typeof(AssetBundleManageData)) as AssetBundleManageData;
				//abd.hideFlags = HideFlags.HideAndDontSave;
			}
			// load setting data.
			AssetBundleManageData abd = AssetBundleManager.GetManageData(true);
			if( abd == null )
				return;
			this._settingDataFilePath = path;
			// edit copy data
			this._assetBundleData = abd;// AssetBundleManageData.Instantiate( abd ) as AssetBundleManageData;
			abd = null;
			// undo
			Undo.undoRedoPerformed += () =>
			{
				//Debug.Log("Undo:"+Undo.GetCurrentGroup());
				/*EditorUtility.SetDirty(me);
				Focus();
				this.RefreshAssetBundleList();*/
				this.Repaint();
				this._first = true;
			};
			this._undoObjects = new Object[] { this, this._assetBundleData };
			this._first = true;
		}

		void OnEnable()
		{
			this._first = true;
		}
		void OnDestroy()
		{
			this._assetBundleData = null;
			this.DeleteBackup();
			if( this._assetbundleWindow )
				this._assetbundleWindow.Close();
		}

		void OnGUI()
		{
			if (this._first)
			{
				this.RefreshAssetBundleList();
				this._first = false;
			}

			// Undo
			switch (Event.current.type)
			{
			case EventType.MouseUp:
			case EventType.MouseDown:
				if (Event.current.button == 0)	// Left Click
					Undo.RecordObjects(this._undoObjects, "AssetBundleData");
				break;
			case EventType.DragPerform:
				Undo.RecordObjects(this._undoObjects, "AssetBundleData");
				break;
			case EventType.KeyDown:
				Undo.RecordObjects(this._undoObjects, "AssetBundleData");
				break;
			}

			this._assetBundleData.notUseBundleOnEditor = EditorGUILayout.ToggleLeft(this._notUseBundleContent, this._assetBundleData.notUseBundleOnEditor);

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			EditorGUILayout.BeginVertical("Box");
			//EditorGUILayout.BeginHorizontal();
			//EditorGUILayout.PrefixLabel(this._editBundleContent);
			EditorGUI.BeginChangeCheck();
			// assetbundle list
			this._bundleListPos = EditorGUILayout.Popup(this._editBundleContent, this._bundleListPos, this._assetBundlePathList);
			if (EditorGUI.EndChangeCheck())
				this.SelectAssetBundle(this._bundleListPos);
			//EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Add", GUILayout.Width(100)))
			{
				var ret = EditorUtility.SaveFilePanel("Add AssetBundle", Application.dataPath, "NewAssetBundle", "unity3d");
				if (!string.IsNullOrEmpty(ret))
				{
					var path = AssetBundleUtility.ChangeRelativePath(Application.dataPath, ret);
					//FileInfo file = new FileInfo(path);
					var assetbundle = new AssetBundleData(Path.GetDirectoryName(path), Path.GetFileName(path));//file.DirectoryName, file.Name);
					this._assetBundleData.assetbundles.Add(assetbundle);
					this._bundleListPos = this._assetBundleData.assetbundles.Count - 1;
					this.RefreshAssetBundleList();
				}
			}
			EditorGUI.BeginDisabledGroup(this._assetBundleData.assetbundles.Count <= this._bundleListPos);
			if (GUILayout.Button("Delete", GUILayout.Width(100)))
			{
				// decrement parent no
				foreach (var assetbundle in this._assetBundleData.assetbundles)
				{
					if (this._bundleListPos == assetbundle.ParentNo)
						assetbundle.ParentNo = -1;
					if (this._bundleListPos < assetbundle.ParentNo)
						--assetbundle.ParentNo;
				}
				// remove
				this._assetBundleData.assetbundles.RemoveAt(this._bundleListPos);
				this.RefreshAssetBundleList();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

#if DLL
			EditorGUI.BeginDisabledGroup(true);
#endif
			// multi build
			this._assetBundleData.multiPlatform = EditorGUILayout.Toggle("Multi Platform", this._assetBundleData.multiPlatform);
			EditorGUI.BeginDisabledGroup(!this._assetBundleData.multiPlatform);
			// target list
			int targetflg = 0;
			var targets = (System.Enum.GetValues(typeof(BuildTarget))  as int[]).Distinct().Select(v=>(BuildTarget)v).ToArray();
#if !DLL
			for (int i = 0; i < targets.Length; ++i)
			{
				if (this._assetBundleData.targets.Contains(targets[i]))
					targetflg |= (1 << (int)i);
			}
#endif
			targetflg = EditorGUILayout.MaskField( "BuildTarget", targetflg,
				targets.Select( v => ( (BuildTarget)v ).ToString() ).ToArray() );
#if !DLL
			this._assetBundleData.targets.Clear();
			for( int i = 0; i < targets.Length; ++i )
			{
				if( ( targetflg & ( 1 << i ) ) != 0 )
					this._assetBundleData.targets.Add( targets[i] );
			}
#endif
				EditorGUI.EndDisabledGroup();
#if DLL
			EditorGUI.EndDisabledGroup();
#endif
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(this._assetBundleData.assetbundles.Count <= this._bundleListPos);
			if (GUILayout.Button("Build", GUILayout.Width(150), GUILayout.Height(25)))
			{
				// Create a selected assetbundle
				this._delayAction = () =>
				{
					this.CreateAssetBundleWithDependency(this._bundleListPos, false);
					this.Save();
				};
			}
			EditorGUI.EndDisabledGroup();
			if (GUILayout.Button("Build All", GUILayout.Width(150), GUILayout.Height(25)))
			{
				// Create all assetbundles
				this._delayAction = () =>
				{
					this.CreateAssetBundleAll(false);
					this.Save();
				};
			}
			EditorGUILayout.EndHorizontal();
			//EditorGUILayout.Space(); 
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup( this._assetBundleData.assetbundles.Count <= this._bundleListPos );
			if( GUILayout.Button( "Rebuild", GUILayout.Width( 150 ), GUILayout.Height( 25 ) ) )
			{
				// Create a selected assetbundle
				this._delayAction = () =>
				{
					this.CreateAssetBundleWithDependency( this._bundleListPos, true );
					this.Save();
				};
			}
			EditorGUI.EndDisabledGroup();
			if( GUILayout.Button( "Rebuild All", GUILayout.Width( 150 ), GUILayout.Height( 25 ) ) )
			{
				// Create all assetbundles
				this._delayAction = () =>
				{
					this.CreateAssetBundleAll( true );
					this.Save();
				};
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			//EditorGUILayout.LabelField("System");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (GUILayout.Button("Save"))
			{
				// Save
				this.Save();
			}
			if (GUILayout.Button("Exit"))
			{
				// Exit.
				this.Close();
			}
			EditorGUILayout.EndHorizontal();
		}
		void OnInspectorUpdate()
		{
			if (this._requestRefresh)
			{
				this._requestRefresh = false;
				AssetDatabase.Refresh();
			}
			if (this._delayAction != null)
			{
				this._delayAction();
				this._delayAction = null;
			}
		}

		void Save()
		{
			// save new setting data.
			string path = "Assets/" + this._settingDataFilePath;
			//AssetDatabase.DeleteAsset(path);
			string dir = Application.dataPath + "/" + Path.GetDirectoryName(this._settingDataFilePath);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			//EditorUtility.SetDirty(this._assetBundleData);
			var backup = Instantiate(this._assetBundleData);
			AssetDatabase.CreateAsset(backup, path);
			//AssetDatabase.CreateAsset(_assetBundleData, path);
			//AssetDatabase.Refresh();	// Begin-End 
			this._requestRefresh = true;
		}
		static void Save(AssetBundleManageData data)
		{
			// save new setting data.
			var path = AssetBundleManager.SettingDataPath;
			if( !string.IsNullOrEmpty( path ) )
			{
				//AssetDatabase.DeleteAsset(path);
				string dir = Application.dataPath + "/" + Path.GetDirectoryName( path );
				if( !Directory.Exists( dir ) )
					Directory.CreateDirectory( dir );
				path = "Assets/" + path;
				var backup = Instantiate( data );
				AssetDatabase.CreateAsset( backup, path );
			}
		}
		// Select assetBundle and create editor window.
		void SelectAssetBundle(int no)
		{
			if (no < this._assetBundleData.assetbundles.Count)
			{
				if (this._assetbundleWindow == null)
				{
					/*this._assetbundleWindow = AssetBundleWindow.CreateInstance<AssetBundleWindow>();
					this._assetbundleWindow.ShowUtility();
					this._assetbundleWindow.position =
						new Rect(base.position.x + base.position.width + 5, base.position.y, AssetBundleWindow.SIZE.x, AssetBundleWindow.SIZE.y);*/
					this._assetbundleWindow = EditorWindow.GetWindowWithRect<AssetBundleWindow>(new Rect(base.position.x + base.position.width + 5, base.position.y, AssetBundleWindow.SIZE.x, AssetBundleWindow.SIZE.y), true, "Editor");
					this._assetbundleWindow.ParentEditor = this;
				}
				this._assetbundleWindow.SetAssetBundle(this._assetBundleData.assetbundles[no]);
				this._assetbundleWindow.Repaint();
			}
			else
			{
				//this._assetbundleWindow.ShowUtility();// if window don't open, throw NullReferenceException with Close()...
				//this._assetbundleWindow.Close();
				if (this._assetbundleWindow)
				{
					this._assetbundleWindow.Close();
					this._assetbundleWindow = null;
				}
			}
		}
		// Refresh assetbundle list
		void RefreshAssetBundleList()
		{
			this._assetBundlePathList = this._assetBundleData.assetbundles.Select(d => new GUIContent(string.Format("{0} [{1}\\{0}]", d.File, d.Directory.Replace('/', '\\')))).ToArray();
			if (this._assetBundlePathList.Length <= this._bundleListPos)
				this._bundleListPos = Mathf.Max(this._assetBundlePathList.Length - 1, 0);
			this.SelectAssetBundle(this._bundleListPos);
		}
		// Create assetbundle
		void CreateAssetBundleWithDependency(int no, bool rebuild)
		{
			CreateAssetBundleWithDependency(this._assetBundleData, no, rebuild);
		}
		static void CreateAssetBundleWithDependency( AssetBundleManageData data,int no, bool rebuild )
		{
			var bundles = data.assetbundles;
			int wno = no;
			bool changed = bundles[wno].Changed;// || ((BuildAssetBundleOptions)bundles[wno].Options & BuildAssetBundleOptions.DeterministicAssetBundle) == 0;
			List<int> dependency = new List<int>();
			dependency.Add(wno);
			while (0 <= bundles[wno].ParentNo)
			{
				wno = bundles[wno].ParentNo;
				dependency.Add(wno);
				changed |= bundles[wno].Changed;
			}
			if( !rebuild && !changed && !CheckUpdateOfBundleAssets( data, bundles[wno] ) )
			{
				// no update
				Debug.Log( " Skip(because not update) " + bundles[no].File );
				return;
			}
			bool build = false;
			if (1 < dependency.Count)
			{
				if( !rebuild )
				{
					/*for( int i = dependency.Count - 1; 0 <= i; --i )
					{
						//Debug.Log(string.Format("{0}: changed:{1} option:{2}", bundles[dependency[i]].File, bundles[dependency[i]].Changed, bundles[dependency[i]].Options));
						if( !bundles[dependency[i]].Changed && ( (BuildAssetBundleOptions)bundles[dependency[i]].Options & BuildAssetBundleOptions.DeterministicAssetBundle ) != 0 )
							dependency.RemoveAt( i );	// child assetbundle can rebuild.
						else
							break;
					}*/
				}
				dependency.Reverse();
				//Debug.Log("Build: " + string.Join("->", dependency.Select(d => d.ToString()).ToArray()));
				nestString = "";
				foreach (var d in dependency)
				{
					BuildPipeline.PushAssetDependencies();
					CreateAssetBundle( data, bundles[d], true, true );	// if you build parent, must build child.
					nestString += "  ";
					build = true;
				}
				for(int i = 0; i < dependency.Count; ++i)
					BuildPipeline.PopAssetDependencies();
			}
			else
			{
				nestString = "";
				CreateAssetBundle(data, bundles[no], rebuild, false);
				build = true;
			}
			// send changing to child .
			if (build && ( (BuildAssetBundleOptions)bundles[no].Options & BuildAssetBundleOptions.DeterministicAssetBundle ) == 0 )
			{
				SendChangeToChild(data, no);
			}
			// delete all list.asset
			string listpath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(data)) + "/list";
			AssetDatabase.DeleteAsset(listpath);
		}
		static string nestString = "";
		void CreateAssetBundleAll(bool rebuild)
		{
			CreateAssetBundleAll(this._assetBundleData, rebuild);
		}
		static void CreateAssetBundleAll(AssetBundleManageData data, bool rebuild)
		{
			var bundles = data.assetbundles;
			// parent-child
			Dictionary<int, List<int>> family = new Dictionary<int, List<int>>();
			for (int i = 0; i < bundles.Count; ++i)
			{
				int parent = bundles[i].ParentNo;
				if (0 <= parent)
				{
					if (!family.ContainsKey(parent))
						family.Add(parent, new List<int>());
					family[parent].Add(i);
				}
			}
			for (int i = 0; i < bundles.Count; ++i)
			{
				int parent = bundles[i].ParentNo;
				if (parent < 0)
				{
					if (family.ContainsKey(i))
					{
						// family
						System.Action<int, bool> buildchild = null;
						nestString = "  ";
						buildchild = (no, parentbuild) =>
						{
							BuildPipeline.PushAssetDependencies();
							parentbuild |= CreateAssetBundle( data, bundles[no], rebuild | parentbuild, true );
							nestString += "  ";
							foreach (var child in family[no])
							{
								if (family.ContainsKey(child))
								{
									buildchild( child, parentbuild );
								}
								else
								{
									CreateAssetBundle( data, bundles[child], rebuild | parentbuild, true );
								}
							}
							BuildPipeline.PopAssetDependencies();
							nestString = nestString.Substring(0, nestString.Length - 2);
						};
						nestString = "";
						buildchild(i, false);
					}
					else
					{
						// single
						nestString = "";
						CreateAssetBundle( data, bundles[i], rebuild, false );
					}
				}
			}
			// delete all list.asset
			string listpath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(data)) + "/list";
			AssetDatabase.DeleteAsset( listpath );
		}
		static bool CreateAssetBundle(AssetBundleManageData data, AssetBundleData assetbundle, bool rebuild, bool dependency)
		{
			if (assetbundle.GetAllContents().Count <= 0)
				return false;
			// add all path to list
			List<string> files = new List<string>();
			foreach (var path in assetbundle.GetAllContentsPath())
			{
				if (File.Exists(path))
					files.Add(path);
			}
			// add special list asset for system
			BundleAssetList list = null;
			string listpath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(data)) + "/list/" + assetbundle.File + "/list.asset";//Application.temporaryCachePath + "/" + "list.asset";
			if( !assetbundle.ForStreamedScene )
			{
				list = CreateInstance<BundleAssetList>();
				list.Assets = files.Select(file => {
					FileInfo info = new FileInfo(file);
					BundleAssetInfo asset = new BundleAssetInfo(info.Name);
					return asset;
				}).ToArray();

				var directory = System.IO.Path.GetDirectoryName(listpath).Substring("Assets".Length);
				System.IO.Directory.CreateDirectory(Application.dataPath + directory);
				AssetDatabase.CreateAsset(list, listpath);
				//files.Add(listpath);
				AssetBundleUtility.AssetList = list;
				// BundleAssetList.cs(not exist file "BundleAssetList.cs" with DLL)
				//string bundleAssetsListPath = "Assets/" + Path.GetDirectoryName(AssetBundleManager.SettingDataPath) + "/Scripts/BundleAssetList.cs";
				//files.Add(bundleAssetsListPath);
			}
			var targets = new BuildTarget[] { EditorUserBuildSettings.activeBuildTarget };
#if !DLL
			if (assetbundle.PlatformFolder && data.multiPlatform && 0 < data.targets.Count)
				targets = data.targets.ToArray();
#endif

			bool ret = false;
			List<string> platformList = new List<string>();
			foreach( BuildTarget target in targets )
			{
				var path = assetbundle.Path.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(Application.platform));
#if !DLL
				if( assetbundle.PlatformFolder )
				{
					var platformFolder = AssetBundleManager.GetPlatformFolder( target );
					if( platformList.Contains( platformFolder ) )
						continue;
					platformList.Add( platformFolder );
					path = GetFullPath( assetbundle, target );
				}
#endif
				var dir = Path.GetDirectoryName(path);
				if( !string.IsNullOrEmpty(dir) && !Directory.Exists(dir) )
					Directory.CreateDirectory(dir);//AssetDatabase.CreateFolder(Path.GetDirectoryName(dir), Path.GetFileName(dir));
				if( !string.IsNullOrEmpty(dir) )
					dir = dir + "/";
				
				if( !assetbundle.ForStreamedScene )
				{
					if( !assetbundle.Separated )
					{
						Debug.Log(nestString + "Build " + assetbundle.File);
						if( AssetBundleUtility.CreateBundle(files.ToArray(), path, (BuildAssetBundleOptions)assetbundle.Options, target, rebuild | assetbundle.Changed) )
						{
							ret = true;
						}
					}
					else
					{
						var infos = new BundleAssetInfo[] { new BundleAssetInfo("") };
						list.Assets = infos;
						AssetBundleUtility.AssetList = list;
						foreach( var file in files )
						{
							infos[0].Name = file;
							string name = Path.GetFileNameWithoutExtension(file) + ".unity3d";
							Debug.Log(nestString + "Build " + name);
							if( AssetBundleUtility.CreateBundle(file, dir + name, (BuildAssetBundleOptions)assetbundle.Options, target, rebuild | assetbundle.Changed) )
							{
								ret = true;
							}
							BuildPipeline.PopAssetDependencies();
							BuildPipeline.PushAssetDependencies();
						}
					}
				}
				else
				{
					if( !assetbundle.Separated )
					{
						Debug.Log(nestString + "Build " + assetbundle.File);
						if( AssetBundleUtility.CreateSceneBundle(files.ToArray(), path, (BuildAssetBundleOptions)assetbundle.Options, target, rebuild | assetbundle.Changed) )
						{
							ret = true;
						}
					}
					else
					{
						foreach( var file in files )
						{
							string name = Path.GetFileNameWithoutExtension(file) + ".unity3d";
							Debug.Log(nestString + "Build " + name);
							if( AssetBundleUtility.CreateSceneBundle(file, dir + name, (BuildAssetBundleOptions)assetbundle.Options, target, rebuild | assetbundle.Changed) )
							{
								ret = true;
							}
							BuildPipeline.PopAssetDependencies();
							BuildPipeline.PushAssetDependencies();
						}
					}
				}
			}
			assetbundle.Changed = false;
			//AssetDatabase.DeleteAsset(listpath);	Child assetbundle require list.asset of parent assetbundle.
			//AssetDatabase.Refresh();
			return ret;
		}
		// Check if assets of assetbundle is updated
		static bool CheckUpdateOfBundleAssets( AssetBundleManageData data, AssetBundleData assetbundle )
		{
			if( assetbundle.GetAllContents().Count <= 0 )
				return false;
			List<string> files = new List<string>();
			foreach( var path in assetbundle.GetAllContentsPath() )
			{
				if( File.Exists( path ) )
					files.Add( path );
			}
			var targets = new BuildTarget[] { EditorUserBuildSettings.activeBuildTarget };
#if !DLL
			if (assetbundle.PlatformFolder && data.multiPlatform && 0 < data.targets.Count)
				targets = data.targets.ToArray();
#endif
			List<string> platformList = new List<string>(); 
			foreach( BuildTarget target in targets )
			{
				var path = assetbundle.Path;
#if !DLL
				if( assetbundle.PlatformFolder )
				{
					var platformFolder = AssetBundleManager.GetPlatformFolder( target );
					if( platformList.Contains( platformFolder ) )
						continue;
					platformList.Add( platformFolder );
					path = GetFullPath( assetbundle, target );
				}
#endif
				if( AssetBundleUtility.CheckUpdateOfBundleAssets( files.ToArray(), path, (BuildAssetBundleOptions)assetbundle.Options ) )
					return true;
			}
			return false;
		}
		// Send changing to child
		void SendChangeToChild(int no)
		{
			SendChangeToChild(this._assetBundleData, no);
		}
		static void SendChangeToChild(AssetBundleManageData data,int no)
		{
			var bundles = data.assetbundles;
			for (int i = 0; i < bundles.Count; ++i)
			{
				if (no == bundles[i].ParentNo)
				{
					//Debug.Log(string.Format("SendChangeToChild({0})->{1}", no, i));
					bundles[i].Changed = true;
					SendChangeToChild(data, i);
				}
			}
		}
		struct Bundle
		{
			public string Directory{get;set;}
			public string Name { get; set; }
			public string Path { get { return this.Directory + "/" + this.Name; } }
		}

		void DeleteBackup()
		{
			string backupname = AssetBundleManager.SettingBackupDataPath;
			if( !string.IsNullOrEmpty(backupname) )
			{
				AssetDatabase.DeleteAsset( "Assets/" + backupname );
				AssetDatabase.Refresh();
			}
		}


#if !DLL
		/// <summary>Target path</summary>
		static string GetFullPath(AssetBundleData data, UnityEditor.BuildTarget target)
		{
			return data.Directory.Replace(AssetBundleManager.PlatformFolder, AssetBundleManager.GetPlatformFolder(target)) + "/" + data.File;
		}
#endif

		//Vector2 _windowSize = new Vector2(400, 400);
		[SerializeField]
		string _settingDataFilePath;
		AssetBundleManageData _assetBundleData;
		GUIContent[] _assetBundlePathList;
		AssetBundleWindow _assetbundleWindow;
		Object[] _undoObjects;
		bool _first = false;
		bool _requestRefresh;
		System.Action _delayAction;

		[SerializeField]
		int _bundleListPos;

		// content
		GUIContent _notUseBundleContent = new GUIContent("Not load from assetbundle on editor", "If true, load asset from AssetDatabase on Editor.\nYou need not to build assetbundle for editor debug.");
		//GUIContent _settingFileContent = new GUIContent("Setting Data File");
		GUIContent _editBundleContent = new GUIContent("AssetBundle", "Select AssetBundle for edit on right window.");
	}
	/// <summary>
	/// AssetBundle Editor Window
	/// </summary>
	class AssetBundleWindow : EditorWindow
	{
		internal static Vector2 SIZE = new Vector2(400, 400);
		const float MARGIN = 2.5f;
		const float BUTTON_HEIGHT = 120f;

		/// <summary>Parent editor</summary>
		internal AssetBundleManagerEditor ParentEditor { get; set; }

		/// <summary>Type of assetbundle</summary>
		enum AssetBundleType
		{
			/// <summary>Normal assetbundle</summary>
			NormalAssetBundle,
			/// <summary>Streamed scene assetbundle</summary>
			StreamedSceneAssetBundle,
		}

		static string FOCUS_OUT_UID = "FocusDammy";
		internal void SetAssetBundle(AssetBundleData data)
		{
			this._data = data;
			this._listbox.Clear();
			if (data != null)
			{
				foreach (var c in data.GetAllContents())
				{
					this._listbox.AddEntry(c.Name);
				}
				base.title = this._data.File;
			}
			this._focusOut = true;
		}

		void Awake()
		{
			this.maxSize = SIZE;
			this._titleLabelStyle = new GUIStyle(GUI.skin.label);
			this._titleLabelStyle.fontStyle = FontStyle.Bold;
			this._boxStyle = new GUIStyle(GUI.skin.box);
			this._boxStyle.fontStyle = FontStyle.Bold;
			this._boxStyle.normal.textColor = Color.white;
			this._boxStyle.alignment = TextAnchor.MiddleCenter;
			this._listbox.ControlName = "List";
		}
		void OnEnable()
		{
			// list event
			this._listbox.OnSelectionChange += (list, prev) =>
			{
				// get focus from "Pattern TextField" for updating TextField.
				EditorGUI.FocusTextInControl("List");
				// Select asset at "Project window".
				var content = this._data.GetContent(list.SelectNo);
				if( content.Type == iContent.Types.File )
				{
					if( !Selection.activeObject || content.Name != Selection.activeObject.name )
						Selection.activeObject = AssetDatabase.LoadAssetAtPath( content.Name, typeof( Object ) );
				}
				else if( content.Type == iContent.Types.Directory )
				{
					var dc = content;// as DirectoryContent;
					var files = System.IO.Directory.GetFiles( dc.Directory, dc.Pattern );
					if( 0 < files.Length )
						Selection.objects = System.Array.ConvertAll( files, file => AssetDatabase.LoadAssetAtPath( file, typeof( Object ) ) );
					else
						Selection.activeObject = AssetDatabase.LoadAssetAtPath( dc.Directory, typeof( Object ) );
				}
			};
			this._listbox.OnDeleteItem += (list, no) =>
			{
				var c = this._data.GetContent(no);
				if (c != null)
				{
					this._data.Remove(c.Name);
					this.Refresh();
					//this.Repaint();
				}
			};
		}
		void OnGUI()
		{
			switch (Event.current.type)
			{
			case EventType.MouseUp:
			case EventType.MouseDown:
				if (Event.current.button == 0)
					Undo.RecordObject(this, "AssetBundleFile");
				break;
			case EventType.DragPerform:
				Undo.RecordObject(this, "AssetBundleFile");
				break;
			case EventType.KeyDown:
				Undo.RecordObject(this, "AssetBundleFile");
				break;
			}

			if (Event.current.isKey && Event.current.type == EventType.KeyDown)
			{
				switch (Event.current.keyCode)
				{
				case KeyCode.DownArrow:
					this._listbox.DownCursor();
					//EditorUtility.SetDirty(this);
					this.Repaint();
					break;
				case KeyCode.UpArrow:
					this._listbox.UpCursor();
					//EditorUtility.SetDirty(this);
					this.Repaint();
					break;
				case KeyCode.Delete:
				case KeyCode.Backspace:
					{
						//if (GUI.GetNameOfFocusedControl() == "List")
						var focus = GUI.GetNameOfFocusedControl();
						if(focus != ContentTypeName && focus != PathName)
						{
							var c = this._data.GetContent(this._listbox.SelectNo);
							if (c != null)
							{
								this._data.Remove(c.Name);
								//this._listbox.RemoveEntry(this._listbox.SelectNo);
								//EditorUtility.SetDirty(this);
								this.Refresh();
								this.Repaint();
							}
						}
					}
					break;
				}
			}

			var size = new Vector2(SIZE.x - MARGIN * 2, SIZE.y - MARGIN * 2);

			if( this._focusOut )
			{
				GUI.SetNextControlName(FOCUS_OUT_UID);
				GUI.TextField(new Rect(-100, -100, 1, 1), "");
				GUI.FocusControl(FOCUS_OUT_UID);
				this._focusOut = false;
			}

			EditorGUI.BeginChangeCheck();

			//EditorGUILayout.LabelField(this._data.File, this._titleLabelStyle);
			//EditorGUILayout.Separator(); 
			
			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			GUI.SetNextControlName(PathName);
			var path = EditorGUILayout.TextField("Path", this._data.Path);
			// if create each platform folders
			if (EditorGUI.EndChangeCheck())
			{
				this._data.SetPath(path);
			}
#if NOT_USE_PLATFORM
			EditorGUI.BeginDisabledGroup(true);
#endif
			bool prevPlatformFolder = this._data.PlatformFolder;
			this._data.PlatformFolder = EditorGUILayout.Toggle("Create platform folders", this._data.PlatformFolder);
			if( this._data.PlatformFolder != prevPlatformFolder )
			{
				// switch $(Platform)
				if( this._data.PlatformFolder )
				{
					if( !this._data.Directory.Contains(AssetBundleManager.PlatformFolder) )
						this._data.SetPath(this._data.Directory + "/" + AssetBundleManager.PlatformFolder + "/" + this._data.File);
				}
				else
				{
					if( this._data.Directory.Contains(AssetBundleManager.PlatformFolder) )
						this._data.SetPath(this._data.Directory.Replace(AssetBundleManager.PlatformFolder + "/", "").Replace("/" + AssetBundleManager.PlatformFolder, "") + "/" + this._data.File);
				}
			}
#if NOT_USE_PLATFORM
			EditorGUI.EndDisabledGroup();
#endif
			EditorGUILayout.Separator();
			// type
			var type = this._data.ForStreamedScene ? AssetBundleType.StreamedSceneAssetBundle : AssetBundleType.NormalAssetBundle;
			type = (AssetBundleType)EditorGUILayout.EnumPopup("Assetbundle type", type);
			this._data.ForStreamedScene = type == AssetBundleType.StreamedSceneAssetBundle;
			// parent assetbundle
			var list = new List<GUIContent>(this.ParentEditor.AssetBundlePathList);
			list.Insert(0, new GUIContent("none"));
			int idx = this._data.ParentNo + 1;
			int me = list.FindIndex(gc => gc.text.IndexOf(this._data.File) == 0);
			list.RemoveAll(gc => gc.text.IndexOf(this._data.File) == 0);
			if (me <= idx)
				--idx;
			EditorGUI.BeginChangeCheck();
			idx = EditorGUILayout.Popup(new GUIContent("Parent assetbundle"), idx, list.ToArray());
			if (EditorGUI.EndChangeCheck())
			{
				if (idx < me)
					--idx;
				int tmp = this._data.ParentNo;
				this._data.ParentNo = idx;
				if( 0 <= idx )
				{
					// Check if set separeted assetbundle as parent.
					if( !this.ParentEditor.CheckParentSetting( this._data, true ) )
					{
						EditorUtility.DisplayDialog( "Illegal setting", "Assetbundle checked \"Multi assetbundles\" must not set as parent", "OK" );
						this._data.ParentNo = tmp;	// back
					}
					//  Check "circular reference"
					if( this.ParentEditor.CheckCircularReferencek() )
					{
						EditorUtility.DisplayDialog( "Circular reference", "Circular reference on dependencies!\nCheck \"Parent AssetBundle\"", "OK" );
						this._data.ParentNo = tmp;	// back
					}
				}
			}
			// build option ( handle "EnumMaskField" bug? )
			var optionvalues = System.Enum.GetValues(typeof(BuildAssetBundleOptions));
			int optiontmp = 0;
			for (int i = 0; i < optionvalues.Length; ++i)
			{
				if (((int)this._data.Options & (int)optionvalues.GetValue(i)) != 0)
					optiontmp |= (1 << i);
			}
			var option = (BuildAssetBundleOptions)EditorGUILayout.EnumMaskField("BuildAssetBundleOptions", (BuildAssetBundleOptions)optiontmp);	// not value of BuildAssetBundleOptionsl (1, 2, 4, 8, ...)
			optiontmp = 0;
			for (int i = 0; i < optionvalues.Length; ++i)
			{
				if (((int)option & (1 << i)) != 0)
					optiontmp |= (int)optionvalues.GetValue(i);
			}
#if false//!DLL
			this._data.Options = (BuildAssetBundleOptions)optiontmp;
#else
			this._data.Options = optiontmp;
#endif
			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();
			// List ( if empty, draw help message. )
			if (0 < this._listbox.EntryNum)
				this._listbox.Draw("Contents in AssetBundle", size, 20);
			else
				GUILayout.Box("Drop asset or directory to add contents of the assetbundle.\nIf you want to delete content, you select and press \"Delete\"", this._boxStyle, GUILayout.MaxHeight(size.x), GUILayout.Width(size.y));

			DragAndDropUtility.DropProc(obj=>{
				//Debug.Log("Drag Object:" + AssetDatabase.GetAssetPath(obj));
				var objpath = AssetDatabase.GetAssetPath(obj);
				this._data.Remove(objpath);
				if (System.IO.Directory.Exists(Application.dataPath + objpath.Substring(6)))
				{
					// directory
					this._data.AddDirectory(objpath, null);
				}
				else
				{
					// file
					this._data.AddFile(objpath);
				}
				this._data.Changed = true;
				this.Refresh();
			});
			var content = this._data.GetContent(this._listbox.SelectNo);
			if( content != null )
			{
				EditorGUI.BeginDisabledGroup( !( content.Type == iContent.Types.Directory ) );
				GUI.SetNextControlName(ContentTypeName);
				if( content.Type == iContent.Types.Directory )
				{
					var dcontent = content;// as DirectoryContent;
					EditorGUI.BeginChangeCheck();
					dcontent.Pattern = EditorGUILayout.TextField( "Pattern", dcontent.Pattern );
					if( EditorGUI.EndChangeCheck() )
					{
						//this.Refresh();
						this._listbox.ChangeEntry(this._listbox.SelectNo, dcontent.Name);
					}
				}
				else
					EditorGUILayout.TextField( "Extension", "" );
				EditorGUI.EndDisabledGroup();
			}
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			//if( type == AssetBundleType.NormalAssetBundle )
			{
				bool tmp = this._data.Separated;
				this._data.Separated = EditorGUILayout.Toggle( new GUIContent( "Multi assetbundles", "Make assetbundles for each files" ), this._data.Separated );
				if( tmp != this._data.Separated )
				{
					// Check if set separeted assetbundle as parent.
					if( !this.ParentEditor.CheckParentSetting( this._data, false ) )
					{
						EditorUtility.DisplayDialog( "Illegal setting", "Assetbundle checked \"Multi assetbundles\" must not set as parent", "OK" );
						this._data.Separated = tmp;	// back
					}
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				//Debug.Log("Changed");
				this._data.Changed = true;
			}
		}
		// refresh list
		void Refresh()
		{
			this.SetAssetBundle(this._data);
		}

		[SerializeField]
		AssetBundleData _data;
		ListBox _listbox = new ListBox();

		GUIStyle _titleLabelStyle, _boxStyle;
		bool _focusOut;
		const string ContentTypeName = "ContentType";
		const string PathName = "Path";
	}
}
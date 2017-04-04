//--------------------------------------------------------------------------------
/*
 *	@file		AssetFileLoaderEditor
 *	@brief		AssetBundleManager
 *	@ingroup	AssetBundleManager
 *	@version	1.05
 *	@date		2015.02.28
 *	Editor of AssetBundleLoader
 */
//--------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace isotope
{
	using DebugUtility;
	[CustomEditor(typeof(AssetBundleLoader))]
	class AssetBundleLoaderEditor : Editor
	{
		void Awake()
		{
			Undo.undoRedoPerformed += () =>
			{
				//Debug.Log("Undo:"+Undo.GetCurrentGroup());
				/*EditorUtility.SetDirty(me);
				Focus();
				this.RefreshAssetBundleList();*/
				this.Repaint();
			};
			this._undoObjects = new Object[] { this, target};
		}
		void OnEnable()
		{
			AssetBundleLoader target = base.target as AssetBundleLoader;
			// compatible old version
			target.MakeCompatibleOldVersion();

			// recovery setting
			if (!string.IsNullOrEmpty(target.Path))
			{
				int didx = target.Path.LastIndexOf('/');
				string dir;
				if (0 <= didx)
				{
					dir = target.Path.Substring(0, didx);
				}
				else
				{
					dir = "";
				}
				if (0 <= target.URL.IndexOf(Application.streamingAssetsPath))
					this._source = Source.StreamingAssets;
				else
					this._source = Source.Network;
				this._path = dir;
			}
			if (!string.IsNullOrEmpty(target.AssetBundleName))
			{
				var data = AssetBundleManager.GetManageData(true);
				if (data)
				{
					this._assetbundleNo = data.assetbundles.FindIndex(ab => target.AssetBundleName.CompareTo(ab.File) == 0) + 1;
					this.SetupAssetbundle( data );
					if( this._assetList != null && !string.IsNullOrEmpty( target.AssetName ) )
						this._assetNo = System.Array.FindIndex( this._assetList, s => !string.IsNullOrEmpty( s ) && target.AssetName.CompareTo( s.Substring( s.LastIndexOf( '/' ) + 1 ) ) == 0 );
				}
			}
		}

		public override void OnInspectorGUI()
		{
			// Undo
			switch( Event.current.type )
			{
			case EventType.MouseUp:
			case EventType.MouseDown:
				if( Event.current.button == 0 )	// Left Click
					Undo.RecordObjects( this._undoObjects, "AssetBundleLoader" );
				break;
			case EventType.KeyDown:
				Undo.RecordObjects( this._undoObjects, "AssetBundleLoader" );
				break;
			}
			
			AssetBundleLoader target = base.target as AssetBundleLoader;
			// Select AssetBundle
			var data = AssetBundleManager.GetManageData(true);
			if (data != null)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("URL");
				string url = target.URL;
				if (!string.IsNullOrEmpty(url))
				{
					if (this._source == Source.StreamingAssets)
						url = url.Replace(Application.streamingAssetsPath, "$(StreamingAssetPath)");
#if !NOT_USE_PLATFORM
					if (target.PlatformFolder)
						url = url.Replace(AssetBundleManager.GetPlatformFolder(Application.platform), AssetBundleManager.PlatformFolder);
#endif
				}
				var wordwrapLabelStyel = EditorStyles.label;
				wordwrapLabelStyel.wordWrap = true;
				GUI.enabled = false;
				EditorGUILayout.LabelField( url, wordwrapLabelStyel );
				GUI.enabled = true;
				EditorGUILayout.EndHorizontal();

				EditorGUI.BeginChangeCheck();
				var tmp = this._source;
				this._source = (Source)EditorGUILayout.EnumPopup("Source", this._source);
				if( tmp != this._source )
				{
					// changed
					if( this._source == Source.Network )
					{
						if( string.IsNullOrEmpty( this._path ) )
							this._path = "http://";
					}
				}

				EditorGUILayout.BeginHorizontal();
				var wordwrapTextAreaStyel = EditorStyles.textArea;
				wordwrapTextAreaStyel.wordWrap = true;
				var prevPath = this._path;
				if (this._source == Source.StreamingAssets)
				{
					EditorGUILayout.PrefixLabel( "Directory" );
					this._path = EditorGUILayout.TextArea( this._path, wordwrapTextAreaStyel );
				}
				else
				{
					EditorGUILayout.PrefixLabel( "Base URL" );
					this._path = EditorGUILayout.TextArea( this._path, wordwrapTextAreaStyel );
				}
				if(prevPath != this._path)
				{
					if(target.PlatformFolder != this._path.Contains( AssetBundleManager.PlatformFolder ))
						target.PlatformFolder = this._path.Contains( AssetBundleManager.PlatformFolder );
				}
				EditorGUILayout.EndHorizontal();

#if NOT_USE_PLATFORM
				EditorGUI.BeginDisabledGroup(true);
#endif
				bool prevPlatformFolder = target.PlatformFolder;
				target.PlatformFolder = EditorGUILayout.Toggle("in platform folder", target.PlatformFolder);
				if( prevPlatformFolder != target.PlatformFolder )
					this.UpdatePlatformFolder(target.PlatformFolder);
#if NOT_USE_PLATFORM
				EditorGUI.EndDisabledGroup();
#endif
				if (EditorGUI.EndChangeCheck() && 0 < this._assetbundleNo)
					this.SetURL(target, data.assetbundles[this._assetbundleNo - 1]);

				if (0 < data.assetbundles.Count)
				{
					if( this._assetbundleList == null || this._assetbundleList.Length < data.assetbundles.Count + 1 )
					{
						this._assetbundleList = new string[data.assetbundles.Count + 1];
						this._assetbundleList[0] = " ";	// use "" to select nothing. 
					}
					else
						System.Array.Clear( this._assetbundleList, 1, this._assetbundleList.Length - 1 );
					int i = 1;
					foreach( var s in data.assetbundles.Select( n => string.Format( "{0} [{1}\\{0}]", n.File, n.Directory.Replace( '/', '\\' ) ) ) )
						this._assetbundleList[i++] = s;

					EditorGUI.BeginChangeCheck();
					this._assetbundleNo = EditorGUILayout.Popup( "AssetBundle", this._assetbundleNo, this._assetbundleList );

					if (EditorGUI.EndChangeCheck())
					{
						if (0 < this._assetbundleNo)
						{
							var assetbundle = data.assetbundles[this._assetbundleNo - 1];
							target.AssetBundleName = assetbundle.File;
							target.PlatformFolder = assetbundle.PlatformFolder;
							this.SetupAssetbundle( data );
							this.UpdatePlatformFolder(target.PlatformFolder);

							if (!assetbundle.Separated)
							{
								this.SetURL(target, assetbundle);
							}
							else
							{
								if (0 < this._assetNo)
									this.SetURL(target, assetbundle);
								else
									target.SetURL(null);
							}
						}
						else
						{
							target.SetURL(null);
							this._assetList = null;
						}
					}
					int num = 0;
					if(0 < this._assetbundleNo)
					{
						var assetbundle = data.assetbundles[this._assetbundleNo - 1];
						num = assetbundle.GetAllContentsPath().Count();
						if(num != this._contentNum)
						{
							this.SetupAssetbundle(data);
						}
					}
					if (this._assetList != null)
					{
						EditorGUI.BeginChangeCheck();
						this._assetNo = EditorGUILayout.Popup("Asset", this._assetNo, this._assetList);
						if (EditorGUI.EndChangeCheck())
						{
							// set asset
							if (0 < this._assetNo)
							{
								System.IO.FileInfo fileinfo = new System.IO.FileInfo(this._assetList[this._assetNo]);
								target.AssetName = fileinfo.Name;//.Substring(0, fileinfo.Name.Length - fileinfo.Extension.Length);
							}
							else
							{
								target.AssetName = null;
							}
							// on case "each assetbundle for asset", load different assetbundle for asset.
							var assetbundle = data.assetbundles[this._assetbundleNo - 1];
							if (assetbundle.Separated)
							{
								if (0 < this._assetNo)
									this.SetURL(target, assetbundle);
								else
									target.SetURL(null);
							}

						}
						EditorGUILayout.Separator();
						target.InstantiateAfterLoad = EditorGUILayout.Toggle("Instantiate After Load", target.InstantiateAfterLoad);
					}
					else if(this._assetbundleNo != 0)
					{
						EditorGUILayout.HelpBox("This AssetBundle has no asset.", MessageType.Warning);
					}
				}
				else
				{
					EditorGUILayout.HelpBox("There is no AssetBundle.", MessageType.Error);
				}

				bool useCache = EditorGUILayout.Toggle("Use cache", 0 <= target.Version);
				++EditorGUI.indentLevel;
				EditorGUI.BeginDisabledGroup(!useCache);
				if( useCache )
				{
					if( target.Version < 0 )
						target.Version = 0;
					target.Version = EditorGUILayout.IntField("Version", target.Version);
				}
				else
				{
					target.Version = -1;
				}
				EditorGUI.EndDisabledGroup();
				--EditorGUI.indentLevel;
			}
			else
			{
				EditorGUILayout.HelpBox("Setting isn't complete.", MessageType.Error);
			}
			EditorUtility.SetDirty(target);
		}

		void SetURL(AssetBundleLoader target, AssetBundleData bundle)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(this._path.Replace('\\', '/'));
			if (!string.IsNullOrEmpty(this._path) && sb[sb.Length - 1] != '/')
				sb.Append('/');
			if (!bundle.Separated)
				sb.Append(bundle.File);
			else
				sb.Append(Path.GetFileNameWithoutExtension(this._assetList[this._assetNo]) + ".unity3d");
			// source
			if (this._source == Source.StreamingAssets)
				target.SetStreamingAssetsPath(sb.ToString());
			else if (this._source == Source.Network)
				target.SetURL(sb.ToString());
			else
				target.SetURL(sb.ToString());
		}

		void SetupAssetbundle(AssetBundleManageData data)
		{
			if( 0 < this._assetbundleNo )
			{
				var assetbundle = data.assetbundles[this._assetbundleNo - 1];

				List<string> list = new List<string>();
				list.Add( " " );	// use "" to select the assetbundle. 
				var em = assetbundle.GetAllContentsPath();
				list.AddRange( em.Select( s =>
				{
					if( !string.IsNullOrEmpty( s ) && s.IndexOf( "Assets/" ) == 0 )
						return s.Substring( "Assets/".Length );
					return s;
				} ) );
				this._assetList = list.ToArray();
				this._contentNum = this._assetList.Length - 1;

				if( this._source == Source.StreamingAssets )
				{
					var streamingassets = "/StreamingAssets";
					int idx = assetbundle.Directory.IndexOf( streamingassets );
					if( 0 < idx && idx + streamingassets.Length + 1 < assetbundle.Directory.Length )
					{
						this._path = assetbundle.Directory.Substring( idx + streamingassets.Length + 1 );
					}
				}
			}
			else
			{
				this._assetList = null;
			}
		}
		// swithc Platform Folder setting.
		void UpdatePlatformFolder(bool platformFolder)
		{
			// switch $(Platform)
			if( platformFolder )
			{
				if( !this._path.Contains(AssetBundleManager.PlatformFolder) )
				{
					if( !this._path.EndsWith("/") )
						this._path += "/";
					this._path += AssetBundleManager.PlatformFolder;
				}
			}
			else
			{
				if( this._path.Contains(AssetBundleManager.PlatformFolder) )
					this._path = this._path.Replace(AssetBundleManager.PlatformFolder + "/", "").Replace("/" + AssetBundleManager.PlatformFolder, "");
			}
		}
		enum Source
		{
			StreamingAssets,
			Network,
			Etc
		}
		[SerializeField]
		Source _source;
		[SerializeField]
		string _path = "";
		[SerializeField]
		int _assetbundleNo;
		[SerializeField]
		string[] _assetbundleList;
		[SerializeField]
		string[] _assetList;
		[SerializeField]
		int _assetNo;
		[SerializeField]
		int _contentNum = 0;

		Object[] _undoObjects;
	}
}

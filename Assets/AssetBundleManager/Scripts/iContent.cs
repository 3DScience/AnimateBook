using UnityEngine;
using System.Collections.Generic;

namespace isotope
{
#if true
	using DebugUtility;
	/// <summary>
	/// AssetBundle content.
	/// </summary>
	[System.Serializable]
	public class iContent
	{
		/// <summary>
		/// content type
		/// </summary>
		public enum Types
		{
			/// <summary>File content</summary>
			File,
			/// <summary>Directory content</summary>
			Directory
		}
		/// <summary>Content type</summary>
		[SerializeField]
		public Types Type;

		/// <summary>Content name</summary>
		public string Name
		{
			get
			{
				if( this.Type == Types.Directory )
					return this._path + "/" + this._pattern;
				else
					return this._path;
			}
		}
		/// <summary>Directory</summary>
		public string Directory
		{
			get
			{
				if( this.Type == Types.Directory )
					return this._path;
				else
					return this._path.Substring( 0, this._path.LastIndexOf( '/' ) );
			}
		}
		/// <summary>Extension</summary>
		public string Pattern { get { return _pattern; } set { this._pattern = value; } }

		/// <summary>File path</summary>
		public string Path { get { return _path; } }

		/// <summary>Get all content</summary>
		public IEnumerable<string> GetContents()
		{
			if( this.Type == Types.Directory )
			{
				foreach( var f in System.IO.Directory.GetFiles( this.Directory, this.Pattern ) )
				{
					if( !f.Contains( ".meta" ) )
						yield return f.Replace( '\\', '/' );
				}
			}
			else
			{
				yield return this.Name;
			}
		}

		/// <summary>
		/// Initialize for directory
		/// </summary>
		/// <param name="directory">directory</param>
		/// <param name="pattern">Target file name pattern</param>
		public void Initialize( string directory, string pattern )
		{
			if( directory != null )
				this._path = directory.Replace( '\\', '/' ).TrimEnd( '/' );
			if( string.IsNullOrEmpty( pattern ) )
				pattern = "*.*";	// all files
			this._pattern = pattern;
			this.Type = Types.Directory;
		}

		/// <summary>
		/// Initialize for file
		/// </summary>
		/// <param name="path">file path</param>
		public void Initialize( string path )
		{
			this._path = path.Replace( '\\', '/' );
			this.Type = Types.File;
		}

		[SerializeField]
		string _pattern;

		[SerializeField]
		string _path;
	}
#else
	/// <summary>
	/// AssetBundle content.
	/// </summary>
	//[System.Serializable]
	public abstract class iContent : ScriptableObject
	{
		/// <summary>Content name</summary>
		public abstract string Name { get; }
#if UNITY_EDITOR
		/// <summary>Get all content</summary>
		public abstract IEnumerable<string> GetContents();
	#endif
	}
#endif
}
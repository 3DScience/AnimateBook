using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// ListBox
	/// </summary>
	internal class ListBox
	{
		/// <summary>Select No.</summary>
		public int SelectNo { get { return this._selected; } set { this._selected = Mathf.Clamp( value, 0, this.EntryNum ); } }
		/// <summary>Selected Entry.</summary>
		public Entry SelectedEntry { get; protected set; }
		/// <summary>Entry Number.</summary>
		public int EntryNum { get { return this.entryList.Count; } }

		/// <summary>ControlName of list</summary>
		public string ControlName { get; set; }

		/// <summary>Event on changing selected row</summary>
		public event System.Action<ListBox, int> OnSelectionChange;
		/// <summary>Event on changing selected row</summary>
		public event System.Action<ListBox, int> OnDeleteItem;

		public ListBox()
		{
		}

		//Public functions
		/// <summary>
		/// Move up cursor.
		/// </summary>
		public void UpCursor()
		{
			if( this.SelectNo < 0 )
				return;
			if( this.SelectNo == 0 )
				this.SelectNo = this.EntryNum - 1;
			else
				--this.SelectNo;
		}
		/// <summary>
		/// Move down cursor.
		/// </summary>
		public void DownCursor()
		{
			if( this.SelectNo < 0 )
				return;
			if( this.EntryNum - 1 <= this.SelectNo )
				this.SelectNo = 0;
			else
				++this.SelectNo;
		}
		/// <summary>
		/// Add entry in list
		/// </summary>
		/// <param name="name">entry name</param>
		public void AddEntry( string name )
		{
			this.entryList.Add( new Entry( name ) );
			this.sizeList.Add( Vector2.zero );
			this.contentList = null;
		}
		/// <summary>
		/// Change entry in list
		/// </summary>
		/// <param name="no">entry no</param>
		/// <param name="name">entry name</param>
		public void ChangeEntry(int no, string name)
		{
			this.entryList[no] = new Entry(name);
			this.contentList[no] = new GUIContent(name, name);
			// ïKóvÅHthis.sizeList[no] = Vector2.zero;
		}
		/// <summary>
		/// Remove entry in list
		/// </summary>
		/// <param name="name">entry name</param>
		public void RemoveEntry( Entry name )
		{
			this.RemoveEntry( this.entryList.IndexOf( name ) );
			this.contentList = null;
		}
		/// <summary>
		/// Remove entry in list
		/// </summary>
		/// <param name="idx">entry no</param>
		public void RemoveEntry( int idx )
		{
			if( 0 <= idx )
			{
				this.entryList.RemoveAt( idx );
				this.sizeList.RemoveAt( idx );
				this.contentList = null;
			}
		}
		/// <summary>
		/// Get entry in list
		/// </summary>
		/// <param name="no">entry no</param>
		/// <returns>entry</returns>
		public Entry GetEntry( int no )
		{
			if( 0 <= no )
				return this.entryList[no];
			return null;
		}
		/// <summary>
		/// Load entry list
		/// </summary>
		/// <param name="entryList">entry list</param>
		public void LoadList( List<Entry> entryList )
		{
			this.entryList = entryList;
			this.sizeList.Clear();
			this.contentList = null;
		}
		/// <summary>
		/// Clear entry in list
		/// </summary>
		public void Clear()
		{
			this.entryList.Clear();
			this.sizeList.Clear();
			this.contentList = null;
		}
		/// <summary>
		/// Draw list
		/// </summary>
		/// <param name="title">list title</param>
		/// <param name="size">list size</param>
		/// <param name="itemHeight">list item height</param>
		public void Draw( string title, Vector2 size, float itemHeight )
		{
			if( this._labelStyle == null )
			{
				this._labelStyle = new GUIStyle( GUI.skin.button );
				this._labelStyle.alignment = TextAnchor.MiddleLeft;
				// not look so much for Pro skin? So need to change color.
				//this._labelStyle.normal = GUI.skin.button.normal;
				this._labelStyle.onNormal.textColor = new Color(1 - this._labelStyle.normal.textColor.r, 1 - this._labelStyle.normal.textColor.g, 1 - this._labelStyle.normal.textColor.b);
			}
			if ( this.contentList == null )
				this.contentList = this.entryList.Select( e => new GUIContent( e.Name, e.Name ) ).ToArray();

			GUILayout.BeginHorizontal();
			GUILayout.Label( title );
			if( GUILayout.Button( "Delete list item" ) )
			{
				if( 0 <= this.SelectNo && this.OnDeleteItem != null )
					this.OnDeleteItem( this, this.SelectNo );
				return;
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical( "Box" );
			this._scroll = GUILayout.BeginScrollView( this._scroll, false, false, GUILayout.MaxHeight( size.x ), GUILayout.Width( size.y ) );
			int prev = this._selected;
#if UNITY_EDITOR
			UnityEditor.EditorGUI.BeginChangeCheck();
#else
			var tmp = this._selected;
#endif
			//GUI.SetNextControlName(this.ControlName);	// can't name SelectionGrid ?
			this._selected = GUILayout.SelectionGrid( this._selected, this.contentList, 1, this._labelStyle );
#if UNITY_EDITOR
			if( UnityEditor.EditorGUI.EndChangeCheck() )
#else
			if(tmp != this._selected)
#endif
			{
				// change
				if( this.OnSelectionChange != null )
					this.OnSelectionChange( this, prev );
			}
			GUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			//GUILayout.TextField("", GUILayout.MaxHeight(0), GUILayout.MaxWidth(0));	dammy content for focus
		}

		// Calculate entry label size
		private Vector2 CalcLabelSize()
		{
			Vector2 max = Vector2.zero;
			// calc size
			GUIContent content = new GUIContent();
			for( int i = 0; i < entryList.Count; i += 1 )
			{
				if( this.sizeList[i] == Vector2.zero )
				{
					content.text = this.entryList[i].Name;
					this.sizeList[i] = GUI.skin.label.CalcSize( content );
				}
				max.x = Mathf.Max( this.sizeList[i].x, max.x );
				max.y = Mathf.Max( this.sizeList[i].y, max.y );
			}
			return max;
		}

		private List<Entry> entryList = new List<Entry>();
		private List<Vector2> sizeList = new List<Vector2>();
		private GUIContent[] contentList;
		private int _selected = -1;
		private Vector2 _scroll;
		private GUIStyle _labelStyle;
	}
	internal class Entry
	{
		/// <summary>Entry name</summary>
		public string Name { get; protected set; }
		//
		public Entry( string name )
		{
			this.Name = name;
		}
	}
}
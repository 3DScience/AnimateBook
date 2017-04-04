using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof( SelectFlag ) )]
class SelectFlagEditor : Editor
{
	void Awake()
	{
		var target = base.target as SelectFlag;
		Undo.undoRedoPerformed += () =>
		{
			if( target.FlagID < target.FlagImages.Length )
				target.transform.Find( "Cloth" ).GetComponent<Renderer>().sharedMaterial.mainTexture = target.FlagImages[target.FlagID];
		};
		this._undoObjects = new Object[] { this, target };
	}

	void OnEnable()
	{
		var target = base.target as SelectFlag;
		this._flags = System.Array.ConvertAll( target.FlagImages, image => image ? image.name : "" );
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
		var target = base.target as SelectFlag;

		EditorGUI.BeginChangeCheck();
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if(EditorGUI.EndChangeCheck())
		{
			this._flags = System.Array.ConvertAll( target.FlagImages, image => image ? image.name : "" );
		}

		EditorGUI.BeginChangeCheck();
		target.FlagID = EditorGUILayout.Popup( "Flag Type", target.FlagID, this._flags );
		if( EditorGUI.EndChangeCheck() )
		{
			if( target.FlagID < target.FlagImages.Length )
				target.transform.Find( "Cloth" ).GetComponent<Renderer>().sharedMaterial.mainTexture = target.FlagImages[target.FlagID];
		}

		if( EditorGUI.EndChangeCheck() )
			EditorUtility.SetDirty( target );
	}
	[SerializeField]
	string[] _flags;
	Object[] _undoObjects;
}

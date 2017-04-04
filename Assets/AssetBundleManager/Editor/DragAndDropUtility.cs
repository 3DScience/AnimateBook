using UnityEngine;
using UnityEditor;

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// DragAndDropUtility
	/// </summary>
	static class DragAndDropUtility
	{
		/// <summary>
		/// DragAndDrop for LastRect
		/// </summary>
		/// <param name="OnDropped">Action for drop operation</param>
		internal static void DropProc(System.Action<Object> OnDropped)
		{
			var evt = Event.current;
	
			var dropArea = GUILayoutUtility.GetLastRect();
			int id = GUIUtility.GetControlID(FocusType.Passive);
			switch (evt.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!dropArea.Contains(evt.mousePosition)) break;
	
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				DragAndDrop.activeControlID = id;
	
				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
	
					foreach (var draggedObject in DragAndDrop.objectReferences)
					{
						//Debug.Log("Drag Object:" + AssetDatabase.GetAssetPath(draggedObject));
						if (OnDropped != null)
							OnDropped(draggedObject);
					}
					DragAndDrop.activeControlID = 0;
				}
				Event.current.Use();
				break;
			}
		}
	}
}


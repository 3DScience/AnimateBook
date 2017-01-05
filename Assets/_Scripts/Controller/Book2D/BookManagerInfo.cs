using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;

namespace Book.RTDatabase
{
	public class BookGetInfo
	{
		public int id;
		public string category;

		public List<BookGetInfoDetail> data = new List<BookGetInfoDetail> ();

		public class BookGetInfoDetail
		{
			public string name;
			public int status;
			public float price;
			public string picture_url;
			public string description;
			public string version;
			public string min_app_version;
			public string countBook;
			public string assetbundle;
			public string download_url;
		}

		public delegate void BookGetInfoDeleGate(List<BookGetInfoDetail> callback, string textObject, string imgObject, bool isLeftPage);

		public BookGetInfo(string category, int id) 
		{
			this.id = id;
			this.category = category;

		}


		public void getFromServer(DatabaseReference databaseReference, BookGetInfoDeleGate callback, string textObject, string imgObject, bool isLeftPage)
		{
			BookGetInfoDetail infoDetail = new BookGetInfoDetail ();
			data.Clear ();

			Debug.Log ("databaseReference : " + databaseReference);
			Debug.Log ("category : " + category);
			Debug.Log ("id.ToString () : " + id.ToString ());

			databaseReference.Child ("public").Child(GlobalVar.LANGUAGE).Child ("books").Child (id.ToString ()).ValueChanged += (object sender, ValueChangedEventArgs args) => {
				if (args.DatabaseError != null) {
					Debug.LogError (args.DatabaseError.Message);
					return;
				}
				if (args.Snapshot != null && args.Snapshot.ChildrenCount > 0) {

					infoDetail.name = args.Snapshot.Child ("name").Value.ToString ();
					infoDetail.status = int.Parse (args.Snapshot.Child ("status").Value.ToString ());
					infoDetail.price = int.Parse (args.Snapshot.Child ("price").Value.ToString ());
					infoDetail.picture_url = args.Snapshot.Child ("picture_url").Value.ToString ();
					infoDetail.description = args.Snapshot.Child ("description").Value.ToString ();
					infoDetail.version = args.Snapshot.Child ("version").Value.ToString ();
					infoDetail.min_app_version = args.Snapshot.Child ("min_app_version").Value.ToString ();
					infoDetail.assetbundle = args.Snapshot.Child ("assetbundle").Value.ToString ();
					infoDetail.download_url = args.Snapshot.Child ("picture_url_resized").Value.ToString ();

					data.Add (infoDetail);
					Debug.Log("getFromServer: " + data.Count );
					Debug.Log("getFromServer name: " + data[0].name );
					Debug.Log("getFromServer description: " + data[0].description );
					callback (data, textObject, imgObject, isLeftPage);
				}
			};
		}
	}
		
}

// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Book.RTDatabase;
using UnityEngine.SceneManagement;

// Handler for UI buttons on the scene.  Also performs some
// necessary setup (initializing the firebase app, etc) on
// startup.
public class UIHandler : MonoBehaviour
{
	private DatabaseReference _databaseReference;

//	ArrayList leaderBoard;
//	Vector2 scrollPosition = Vector2.zero;
	private Vector2 controlsScrollViewVector = Vector2.zero;

	public GUISkin fb_GUISkin;

//	private const int MaxScores = 5;
	private string logText = "";
//	private string email = "";
//	private int score = 100;

	//ducgv add
//	private string emailToDelete = "";
	//end of add

	private Vector2 scrollViewVector = Vector2.zero;
	bool UIEnabled = true;

	const int kMaxLogSize = 16382;
	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

	// When the app starts, check to make sure that we have
	// the required dependencies to use Firebase, and if not,
	// add them if possible.
	void Start ()
	{
		dependencyStatus = FirebaseApp.CheckDependencies ();
		if (dependencyStatus != DependencyStatus.Available) {
			FirebaseApp.FixDependenciesAsync ().ContinueWith (task => {
				dependencyStatus = FirebaseApp.CheckDependencies ();
				if (dependencyStatus == DependencyStatus.Available) {
					InitializeFirebase ();
				} else {
					// This should never happen if we're only using Firebase Analytics.
					// It does not rely on any external dependencies.
					Debug.LogError (
						"Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
		} else {
			InitializeFirebase ();
		}
	}

	// Initialize the Firebase database:
	void InitializeFirebase ()
	{
		
		FirebaseApp app = FirebaseApp.DefaultInstance;
//		app.SetEditorDatabaseUrl ("https://testunityfirebase-197e9.firebaseio.com/");
//		//app.SetEditorP12FileName ("TestUnityFirebase-04484f3999fc.p12");
//		app.SetEditorServiceAccountEmail ("testunityfirebase-197e9@appspot.gserviceaccount.com");
//		//app.SetEditorP12Password ("notasecret");

		app.SetEditorDatabaseUrl ("https://smallworld3d-2ac88.firebaseio.com/");
//		app.SetEditorP12FileName ("filebaseTest-2e653eef7319.p12");
		app.SetEditorServiceAccountEmail ("smallworld3d-2ac88@appspot.gserviceaccount.com");
//		app.SetEditorP12Password ("2e653eef7319ed39d40ed0a6370d9d222bbb555a");

		_databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

		//DebugLog ("_databaseReference 0000 " + _databaseReference);

//		leaderBoard = new ArrayList ();
		/*
		leaderBoard.Add ("Firebase Top " + MaxScores.ToString () + " Scores");
		FirebaseDatabase.DefaultInstance
      .GetReference ("Leaders").OrderByChild ("score")
      .ValueChanged += (object sender2, ValueChangedEventArgs e2) => {
			if (e2.DatabaseError != null) {
				Debug.LogError (e2.DatabaseError.Message);
				return;
			}
			string title = leaderBoard [0].ToString ();
			leaderBoard.Clear ();
			leaderBoard.Add (title);
			if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0) {
				foreach (var childSnapshot in e2.Snapshot.Children) {
					leaderBoard.Insert (1, childSnapshot.Child ("score").Value.ToString ()
					+ "  " + childSnapshot.Child ("email").Value.ToString ());
				}
			}
		};
		*/
	}

	void WriteNewQuestData(string description){
		/*
		var questData = new QuestData (description);
		string json = JsonUtility.ToJson (questData);

		// データベースにJson形式で書き込み
		_databaseReference
			.Child ("a")
			.Child("abc")
			//.Child (userId.ToString ())
			.SetRawJsonValueAsync (json);

*/
		// 値を直接書き込む場合はこっち
		//      _databaseReference
		//          .Child ("users")
		//          .Child (userId.ToString ())
		//          .Child (questId.ToString ())
		//          .SetValueAsync (isCompleted);
	}

	// Exit if escape (or back, on mobile) is pressed.
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}

	// Output text to the debug log text field, as well as the console.
	public void DebugLog (string s)
	{
		Debug.Log (s);
		logText += s + "\n";

		while (logText.Length > kMaxLogSize) {
			int index = logText.IndexOf ("\n");
			logText = logText.Substring (index + 1);
		}

		scrollViewVector.y = int.MaxValue;
	}

	// A realtime database transaction receives MutableData which can be modified
	// and returns a TransactionResult which is either TransactionResult.Success(data) with
	// modified data or TransactionResult.Abort() which stops the transaction with no changes.
	/*
	TransactionResult AddScoreTransaction (MutableData mutableData)
	{
		List<object> leaders = mutableData.Value as List<object>;

		if (leaders == null) {
			leaders = new List<object> ();
		} else if (mutableData.ChildrenCount >= MaxScores) {
			// If the current list of scores is greater or equal to our maximum allowed number,
			// we see if the new score should be added and remove the lowest existing score.
			long minScore = long.MaxValue;
			object minVal = null;
			foreach (var child in leaders) {
				if (!(child is Dictionary<string, object>))
					continue;
				long childScore = (long)((Dictionary<string, object>)child) ["score"];
				if (childScore < minScore) {
					minScore = childScore;
					minVal = child;
				}
			}
			// If the new score is lower than the current minimum, we abort.
			if (minScore > score) {
				return TransactionResult.Abort ();
			}
			// Otherwise, we remove the current lowest to be replaced with the new score.
			leaders.Remove (minVal);
		}

		// Now we add the new score as a new entry that contains the email address and score.
		Dictionary<string, object> newScoreMap = new Dictionary<string, object> ();
		newScoreMap ["score"] = score;
		newScoreMap ["email"] = email;
		leaders.Add (newScoreMap);

		// You must set the Value to indicate data at that location has changed.
		mutableData.Value = leaders;
		return TransactionResult.Success (mutableData);
	}

	*/

	TransactionResult AddNewDb (MutableData mutableData)
	{
		List<object> listItem = new List<object>();
		Dictionary<string, object> keyValue = new Dictionary<string, object> ();
		keyValue["description"] = "adsgasd";
		listItem.Add (keyValue);
		mutableData.Value = listItem;
		return TransactionResult.Success (mutableData);
	}

	public void pushBookInfo()
	{
		BookGeneralInfo bookInfo = new BookGeneralInfo ("fairytale",220001,"fishingman",1,1.0f, "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSEoOTTJ-xlSHTUCuDFISgZC05VzanwVY7LwKftRDegfwxgUHkZ","fishingman","1.0.0","1.0.0","","");
		bookInfo.pushToServer (_databaseReference);
	}

	public void pushBookProtectInfo()
	{
		BookProtectInfo bookInfo = new BookProtectInfo (100,100003,"1.0.2", DateTime.Now);
		bookInfo.pushToServer (_databaseReference);
	}

	public void getBookInfo()
	{
		Debug.Log ("getInfo from Data: start");
		BookGetInfo bookInfo = new BookGetInfo ("system",100001);
		bookInfo.getFromServer (_databaseReference,gettedData,null,null,false);

	}

	private void gettedData(List<BookGetInfo.BookGetInfoDetail> data, string textObject, string imgObject, bool isLeftPage)
	{
		DebugLog ("getInfo from Data: " + data.Count);
		DebugLog ("description: " + data[0].description);
		DebugLog ("min_app_version: " + data[0].min_app_version);
		DebugLog ("name: " + data[0].name);
		DebugLog ("price: " + data[0].price);
		DebugLog ("status: " + data[0].status);
	}

	public void AddScore ()
	{
		//WriteNewQuestData ("lkasdjgklas");
		//FirebaseDatabase.DefaultInstance.GetReference ("a").Push ();
		/*
		FirebaseDatabase.DefaultInstance.GetReference ("a").RunTransaction(AddNewDb).ContinueWith (task => {
			if (task.Exception != null) {
				DebugLog (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DebugLog ("AddNewDb complete.");
			}
		});;
		*/
		/*
		if (score == 0 || string.IsNullOrEmpty (email)) {
			DebugLog ("invalid score or email.");
			return;
		}
		DebugLog (String.Format ("Attempting to add score {0} {1}",
			email, score.ToString ()));
		
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference ("Leaders");

		DebugLog ("Running Transaction...");
		// Use a transaction to ensure that we do not encounter issues with
		// simultaneous updates that otherwise might create more than MaxScores top scores.
		reference.RunTransaction (AddScoreTransaction)
      .ContinueWith (task => {
			if (task.Exception != null) {
				DebugLog (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DebugLog ("Transaction complete.");
			}
		});
		*/
	}

	//ducgv add
	/*
	public void DeleteScore ()
	{

		DebugLog ("Deleting Score...");
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference ("Leaders");

		reference.OrderByChild("email").EqualTo(emailToDelete).ChildRemoved += (object sender, ChildChangedEventArgs args) => {
			if (args.DatabaseError != null) {
				Debug.LogError(args.DatabaseError.Message);
				return;
			}
			// Do something with the data in args.Snapshot
			//args.Snapshot.;
			//DebugLog ("DeleteScore: args.Snapshot.Value = "+args.Snapshot.Child().GetValue);
		};
	}
	*/
	//end of add

	// Render the log output in a scroll view.
	void GUIDisplayLog ()
	{
		scrollViewVector = GUILayout.BeginScrollView (scrollViewVector);
		GUILayout.Label (logText);
		GUILayout.EndScrollView ();
	}

	// Render the buttons and other controls.
	void GUIDisplayControls ()
	{
		if (UIEnabled) {
			controlsScrollViewVector =
          GUILayout.BeginScrollView (controlsScrollViewVector);
			GUILayout.BeginVertical ();
			/*
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Email:", GUILayout.Width (Screen.width * 0.20f));
			email = GUILayout.TextField (email);
			GUILayout.EndHorizontal ();

			GUILayout.Space (20);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Score:", GUILayout.Width (Screen.width * 0.20f));
			int.TryParse (GUILayout.TextField (score.ToString ()), out score);
			GUILayout.EndHorizontal ();

			GUILayout.Space (20);

			if (!String.IsNullOrEmpty (email) && GUILayout.Button ("Enter Score")) {
				AddScore ();
			}
			*/
			if (GUILayout.Button ("Home")) {
				SceneManager.LoadScene ("Home");
			}
			GUILayout.Space (20);

			if (GUILayout.Button ("Push BookProteect")) {
				pushBookProtectInfo ();
			}

			if (GUILayout.Button ("Push Book")) {
				pushBookInfo ();
			}

			if (GUILayout.Button ("Get Book")) {
				getBookInfo ();
			}

			/*
			//ducgv add
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Email to delete:", GUILayout.Width (Screen.width * 0.20f));
			emailToDelete = GUILayout.TextField (emailToDelete);
			GUILayout.EndHorizontal ();

			GUILayout.Space (20);

			if (!String.IsNullOrEmpty (emailToDelete) && GUILayout.Button ("Delete Score")) {
				DeleteScore ();
			}

			//end of add
			*/
			GUILayout.EndVertical ();
			GUILayout.EndScrollView ();
		}
	}
	/*
	void GUIDisplayLeaders ()
	{
		GUI.skin.box.fontSize = 36;
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, true);
		GUILayout.BeginVertical (GUI.skin.box);

		foreach (string item in leaderBoard) {
			GUILayout.Label (item, GUI.skin.box, GUILayout.ExpandWidth (true));
		}

		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}
*/
	// Render the GUI:
	void OnGUI ()
	{
		GUI.skin = fb_GUISkin;
		if (dependencyStatus != DependencyStatus.Available) {
			GUILayout.Label ("One or more Firebase dependencies are not present.");
			GUILayout.Label ("Current dependency status: " + dependencyStatus.ToString ());
			return;
		}
		Rect logArea, controlArea;

		if (Screen.width < Screen.height) {
			// Portrait mode
			controlArea = new Rect (0.0f, 0.0f, Screen.width, Screen.height * 0.5f);
//			leaderBoardArea = new Rect (0, Screen.height * 0.5f, Screen.width * 0.5f, Screen.height * 0.5f);
			logArea = new Rect (Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.5f, Screen.height * 0.5f);
		} else {
			// Landscape mode
			controlArea = new Rect (0.0f, 0.0f, Screen.width * 0.2f, Screen.height);
//			leaderBoardArea = new Rect (Screen.width * 0.5f, 0, Screen.width * 0.5f, Screen.height * 0.5f);
			logArea = new Rect (Screen.width * 0.5f, Screen.height * 0.5f, Screen.width * 0.5f, Screen.height * 0.5f);
		}
		/*
		GUILayout.BeginArea (leaderBoardArea);
		GUIDisplayLeaders ();
		GUILayout.EndArea ();
*/
		GUILayout.BeginArea (logArea);
		GUIDisplayLog ();
		GUILayout.EndArea ();

		GUILayout.BeginArea (controlArea);
		GUIDisplayControls ();
		GUILayout.EndArea ();
	}
}

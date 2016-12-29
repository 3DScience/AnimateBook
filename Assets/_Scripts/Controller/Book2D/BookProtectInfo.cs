using System;
using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Book.RTDatabase
{
	public class BookProtectInfo
	{
		public int user_id;
		public int book_id;

		public BookProtectInfoDetail infoDetail;

		public class BookProtectInfoDetail
		{
			public string book_version;
			public string paid_date;

			public BookProtectInfoDetail(string book_version, DateTime paid_date)
			{
				this.book_version = book_version;
				this.paid_date = paid_date.ToString();
			}
		}

		public BookProtectInfo (int user_id, int book_id, string book_version, DateTime paid_date)
		{
			this.user_id = user_id;
			this.book_id = book_id;
			infoDetail = new BookProtectInfoDetail(book_version, paid_date);
		}

		public void pushToServer(DatabaseReference databaseReference)
		{
			string json = JsonUtility.ToJson (infoDetail);
			databaseReference
				.Child ("protect")
				.Child(user_id.ToString())
				.Child("books")
				.Child(book_id.ToString())
				.SetRawJsonValueAsync (json);
		}
	}

}


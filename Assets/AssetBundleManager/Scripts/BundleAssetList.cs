//--------------------------------------------------------------------------------
/*
 *	@file		BundleAssetList
 *	@brief		Asset list in assetbundle
 *	@ingroup	AssetBundleManager
 *	@version	1.00
 *	@date		2014
 *	Asset list in assetbundle
 *	
 */
//--------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace isotope
{
	using DebugUtility;
	/// <summary>
	/// Asset list in bundle
	/// </summary>
	public class BundleAssetList : ScriptableObject
	{
		/// <summary>Assets list</summary>
		public BundleAssetInfo[] Assets;
	}
	/// <summary>
	/// Asset data in bundle
	/// </summary>
	[System.Serializable]
	public class BundleAssetInfo
	{
		/// <summary>Constructor</summary>
		/// <param name="name">Asset name</param>
		public BundleAssetInfo(string name)
		{
			this.Name = name;
		}
		/// <summary>Asset name</summary>
		public string Name;
	}
}
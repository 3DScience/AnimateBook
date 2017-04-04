==========================================
  Asset Bundle Manger
==========================================
	 AsstBundleManager is Unity asset for managing, creating and loading assetbundle.
	You can add the asset with drag and drop, and build assetsbundles at one touch.
	
	 Please refer to the "http://www.wcv.jp/~nerosatou/AssetBundleManager/Manual/index.html" for the latest document or detailed usage. 

	 Do support, bug report, the additional demand make a comment to 
	developer page "http://neros.blog31.fc2.com/blog-entry-2082.html" or
	to nerosatou@gmail.com, please.


------------------------------------------
Release note.
------------------------------------------	
	Version 1.0.10
		FEATURES
		-Added AssetBundleContainer.LoadStreamedScene() for loading the scene in assetbundle.
		-Added AssetBundleLoader.LoadStreamedScene() for loading the scene in assetbundle.
		 
		FIXES
		-Fixed a bug where can't load the StreamedScene in the StreamedSceneAssetBundle.
		 
	Version 1.0.9
		FEATURES
		-Support for Unity5.5.0f3
		-Added AssetBundleLoader.Progress for getting progress of loading of bundle.
		-Added AssetBundleContainer.Progress for getting progress of loading of bundle.

		INFORMATION
		-Sample scenes built in WebGL can't load bundles from our server "http://www.wcv.jp/~nerosatou/" because of security restrictions on accessing cross-domain resources.
		 You need to upload the bundles to your server if want to run sample scenes in WebGL.
		 Refer on "https://docs.unity3d.com/Manual/webgl-networking.html"

	Version 1.0.8
		FEATURES
		-Support for Unity5.3.1f
		(Can't use to some Expansion Menu:Assets->AssetBundleManager->"Build All" and "Rebuild All"）

	Version 1.0.7
		FIXES
		-Fixed a bug where can't clear the check box for "BuildTarget" on setting window
		
	Version 1.0.6
		FIXES
		-Changes metadata on AssetStore.
		
	Version 1.0.5
		FEATURES
		-Support for Unity5.
		(Can't use to some Expansion Menu:Assets->AssetBundleManager->"Build All" and "Rebuild All"）
		
		CHANGES
		-Added method "AssetBundleContainer.GetAllAsset"
		-Added "ASSETBUNDLE_LOG" macro. If you undefine it, log is no longer output.
		-Better AssetBundleLoader Inspector.

		FIXES
		-Fixed a bug where LoadAsset of AssetBundleContainer fail when "Not load from assetbundle on Editor" setting is enable.
		-Fix bundle resource leak on editor when "Not load from assetbundle on Editor" setting is enable.
		
	Version 1.0.4
		FIXES
		-Operation of asset list on setting window will not be disabled with switching to another application.
		-The Dependency between assetbundles will not be incorrectly, if it enables "Parent assetbundle" and "Multi assetbundles".
		
	Version 1.0.3
		FIXES
		-Fixed "AssetBundleManager API.chm"
		
	Version 1.0.2
		FEATURES
		-Added AssetBundleManager.LoadBundleFromCacheOrDownloadAsync() to load assetbundle from cache.
		 (Correspond to WWW.LoadFromCacheOrDownload)
		-Added support of loading assetbundle from cache with AssetBundleLoader.
		
		FIXES
		-Fixed building assetbundle with "Parent assetbundle" setting.
		-Fixed display on sub-window of "AssetBundleManager:Setting"
		-Make output-log easy to read.
		
	Version 1.0.1
		FEATURES
		-QuickGuide.pdf added
		
		FIXES
		-Fixed bug of StreamedSceneAssetBundle.

	Version 1.0.0 
		Initial release

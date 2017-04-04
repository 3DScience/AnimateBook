==========================================
  Asset Bundle Manger
==========================================
	AssetBundleManagerではアセットバンドルの管理・作成・読み込みを行うことが可能です。
	
	最新のドキュメントや詳しい使い方は"http://www.wcv.jp/~nerosatou/AssetBundleManager/Manual/index.html"を参照してください。
	サポート、バグ報告、追加要望などは開発者ページ"http://neros.blog31.fc2.com/blog-entry-2082.html"へのコメントか
	nerosatou@gmail.com　までお願いします。


------------------------------------------
変更履歴
------------------------------------------	
	Version 1.0.10
		特徴
		-AssetBundleContainer.LoadStreamedScene()を追加。バンドル内のシーンをロードする。
		-AssetBundleLoader.LoadStreamedScene()を追加。バンドル内のシーンをロードする。
		 
		修正
		-Unity5でIsStreamedSceneAssetBundle内のシーンのロードに失敗していた不具合を修正
		 
	Version 1.0.9
		特徴
		-Unity5.5.0f3に対応		  
		-AssetBundleLoader.Progressを追加。ロードの進捗度を取得する
		-AssetBundleContainer.Progressを追加。ロードの進捗度を取得する
		
		注意
		-WebGLでビルドしたサンプルシーンではクロスドメインというセキュリティ上の問題でいくつかの制限があるため、サーバー"http://www.wcv.jp/~nerosatou/"からアセットバンドルをロードできません。
		 WebGLでサンプルシーンを動かす場合は自前のサーバーにアセットバンドルをアップロードする必要があります。
		 WebGLのクロスドメイン問題については"https://docs.unity3d.com/ja/current/Manual/webgl-networking.html"を参照してください。


	Version 1.0.8
		特徴
		-Unity5.3.1fに対応
		（拡張Menu:Assets->AssetBundleManager->Build All 及び Rebuild All は非対応）

	Version 1.0.7
		修正
		-設定ウィンドウのBuildTargetで一部ターゲットのチェックを外せなかった不具合を修正
		
	Version 1.0.6
		修正
		-AssetStoreのメタデータを修正
		
	Version 1.0.5
		特徴
		-Unity5 に対応
		（拡張Menu:Assets->AssetBundleManager->Build All 及び Rebuild All は非対応）
		
		変更
		-AssetBundleContainer.GetAllAsset()を追加
		-AssetBundleLoaderのInspectorの使い勝手を改善
		-"ASSETBUNDLE_LOG"マクロの定義でログ出力の有無を切り替えられるようにした
		
		修正
		-"Not load from assetbundle on Editor"を設定してエディタで再生するとAssetBundleLoaderのContainerからアセットを読み込めない不具合を修正
		-"Not load from assetbundle on Editor"を設定してエディタで再生するとStreamedSceneAssetbundleをロードするとアンロードが正常に行われていなかった
	
	Version 1.0.4
		修正
		-設定ウィンドウを開いたまま他のアプリケーションに切り替えるとアセットリストの操作が効かなくなる問題を修正
		-"Parent assetbundle"と"Multi assetbundles"を同時に使用しているときにアセットバンドルの依存関係がおかしくなる問題を修正
		
	Version 1.0.3
		修正
		-AssetBundleManager API.chmを修正
		
	Version 1.0.2
		特徴
		-キャッシュからのロードに対応するAssetBundleManager.LoadBundleFromCacheOrDownloadAsync()を追加
		 (WWW.LoadFromCacheOrDownloadに相当する)
		-AssetBundleLoaderがキャッシュからのロードに対応.
		
		修正
		-"Parent assetbundle"を設定したアセットバンドルビルド不具合を修正
		-"AssetBundleManager:Setting"のサブウィンドウの表示を修正
		-出力されるログを見やすいように整形
		
	Version 1.0.1
		特徴
		QuickGuide.pdf を追加
		
		修正
		StreamedSceneAssetBundle のバグ修正

	Version 1.0.0 
		新規リリース

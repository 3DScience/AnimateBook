using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
namespace Fungus
{ 
   [CommandInfo("custom",
             "LoadSceneFromAssetBundle",
             "load a scene form assetbundle")]
    [AddComponentMenu("")]
    public class LoadSceneFromAssetBundle : Command
    {
        [TextArea(1, 10)]
        [SerializeField]
        protected string sceneName = "";
        [TextArea(1, 10)]
        [SerializeField]
        protected string assetBundleName = "";
        public override void OnEnter()
        {


 
            var flowchart = GetFlowchart();
            string assetBundleName_ = flowchart.SubstituteVariables(assetBundleName);
            string sceneName_ = flowchart.SubstituteVariables(sceneName);
            if (Debug.isDebugBuild)
                Debug.Log("[LoadSceneFromAssetBundle-OnEnter] assetBundleName=" + assetBundleName_ + ", sceneName=" + sceneName_);
            BookSceneLoader sceneLoader = GlobalVar.shareContext.gameObject.AddComponent<BookSceneLoader>();
            sceneLoader.assetBundleName = assetBundleName_;
            sceneLoader.sceneName = sceneName_;

            Continue();
        }

    }
}

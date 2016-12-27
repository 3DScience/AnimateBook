using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
namespace Fungus
{ 
   [CommandInfo("custom",
             "UnLoadScene",
             "load a scene form assetbundle")]
    [AddComponentMenu("")]
    public class UnLoadScene : Command
    {
        [TextArea(1, 10)]
        [SerializeField]
        protected string sceneName = "";
        public override void OnEnter()
        {
            var flowchart = GetFlowchart();
            string sceneName_ = flowchart.SubstituteVariables(sceneName);
            if (Debug.isDebugBuild)
                Debug.Log("[UnLoadScene-OnEnter] sceneName=" + sceneName_);
            SceneManager.UnloadScene(sceneName_);

            Continue();
        }

    }
}

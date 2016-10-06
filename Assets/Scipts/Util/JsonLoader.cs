using UnityEngine;
using System.Collections;
using Entities;
public class JsonLoader  {
    public SceneInfo loadSceneInfoFromJson(string json)
    {
        SceneInfo sceneInfo = JsonUtility.FromJson<SceneInfo>(json);
        return sceneInfo;
    }
}

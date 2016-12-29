using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Fungus
{
    public class LuaUtilsCustom : LuaUtils
    {
        public Camera getMainCamera()
        {
            Debug.Log("rx getMainCamera command");
            return Camera.main;

        }
        public Vector2 CreateVecter2(float x, float y)
        {
            return new Vector2(x, y);
        }
        public Type getType(String type)
        {
          //  Debug.Log("rx getType command");
            return Type.GetType(type);
        }
        public virtual GameObject Instantiate2(GameObject go,Transform parent)
        {
            return GameObject.Instantiate(go,parent);
        }
    }
}

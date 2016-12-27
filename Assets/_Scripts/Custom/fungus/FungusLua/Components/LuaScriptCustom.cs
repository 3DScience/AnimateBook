using UnityEngine;
using MoonSharp.Interpreter;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
namespace Fungus
{
    public class LuaScriptCustom : LuaScript
    {
        /// <summary>
        /// Text file containing Lua script to be executed.
        /// </summary>
        [Tooltip("Text file containing Lua script to be executed.")]
        [SerializeField]
        protected TextAsset[] luaFileList;
        /// <summary>
        /// Returns the Lua string to be executed. 
        /// This is the contents of the Lua script appended to the contents of the Lua file.
        /// </summary>
        /// <returns>The lua string.</returns>
        protected override string GetLuaString()
        {
            string s = base.GetLuaString();
            if (luaFileList != null && luaFileList.Length>0)
            {
                foreach (TextAsset lua in luaFileList)
                {
                    s = luaFile.text;
                }
            }

            return s;
        }

    }
}

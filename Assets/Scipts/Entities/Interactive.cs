using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Entities
{
    public enum INTERACTIVE_EVENT
    {
        TOUCH,
        DRAG,
        DROP
    }
    public enum INTERACTIVE_ACTION
    {
        SCALE,
        ANIMATION,
        MOVE,
        CHANGE_SCENE,
        SHOW_TEXT,
        NONE
    }
    [Serializable]
    public class Interactive
    {
        public bool isDefaultAnimation = true;
        public bool isScaling=false;
        public string eventName;
        public Action[] actions;

    }
}



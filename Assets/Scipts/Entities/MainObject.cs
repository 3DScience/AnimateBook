using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    [Serializable]
    public class MainObject
    {
        public string ObjectName;
        public string movePath;
        public string defaultAnimation;
        public TextContent[] texts;
        public Interactive[] interactives;
        public int currentTextIndex = 0;
    }
}

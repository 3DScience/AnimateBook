using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    [Serializable]
    public  class Action
    {
        private Dictionary<String, String> actionParams_;
        //public INTERACTIVE_ACTION action_;
        public string actionName;
        public ActionParam[] actionParams;

        // beacause jsonutility do not support Dictionary yet, so i walkaround 
        public Dictionary<String, String> getDictionaryActionParam()
        {
            if(actionParams_==null && actionParams != null)
            {
                actionParams_ = new Dictionary<string, string>();
                foreach (ActionParam p in actionParams)
                {
                    actionParams_.Add(p.paramName,p.paramValue);
                }
            }
            return actionParams_;
        }
    }
}

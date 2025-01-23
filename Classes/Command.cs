using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Temp.Classes
{
    public class Command
    {
        public string Name= "No-Name";
        public string definition = "No-Definition-Provided";
        public Action Method = null;
        public bool admin = false;
        public bool NSFW = false;
        public bool Case_Sensitive = false;
        public TriggerType Trigger = TriggerType.Is_Equal_To;
    }
}

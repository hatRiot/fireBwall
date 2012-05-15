using System;
using System.Collections.Generic;
using System.Text;

namespace FM
{
    public class ModuleMeta
    {
        public ModuleMeta()
        {
            Name = null;
            Version = null;
            Author = null;
            Description = null; 
            HelpString = null;
            Contact = null;
        }

        public string Name
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Contact
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string HelpString
        {
            get;
            set;
        }
    }
}

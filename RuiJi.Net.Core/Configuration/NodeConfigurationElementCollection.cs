using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RuiJi.Net.Core.Configuration
{
    public class NodeConfigurationElementCollection : ConfigurationElementCollection 
    {
        public NodeConfigurationElementCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {

        }

        new public NodeConfigurationElement this[string name]
        {
            get { return (NodeConfigurationElement)base.BaseGet(name); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new NodeConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NodeConfigurationElement)element).BaseUrl;
        }

        protected override string ElementName
        {
            get
            {
                return "add";
            }
        }
    }
}
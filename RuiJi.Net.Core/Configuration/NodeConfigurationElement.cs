using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Configuration
{
    public class NodeConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("baseUrl", IsRequired = true, IsKey = true)]
        public string BaseUrl
        {
            get
            {
                return this["baseUrl"] as string;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
        }

        [ConfigurationProperty("proxy", IsRequired = false)]
        public string Proxy
        {
            get
            {
                return this["proxy"] as string;
            }
        }

        //[ConfigurationProperty("zkServer", IsRequired = true)]
        //public string ZkServer
        //{
        //    get
        //    {
        //        return this["zkServer"] as string;
        //    }
        //}
    }
}

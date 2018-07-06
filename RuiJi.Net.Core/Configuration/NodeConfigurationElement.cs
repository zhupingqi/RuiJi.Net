using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Configuration
{
    /// <summary>
    /// node configuration to parse config
    /// </summary>
    public class NodeConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// node url
        /// </summary>
        [ConfigurationProperty("baseUrl", IsRequired = true, IsKey = true)]
        public string BaseUrl
        {
            get
            {
                return this["baseUrl"] as string;
            }
        }

        /// <summary>
        /// node type
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
        }

        /// <summary>
        /// node proxy url
        /// </summary>
        [ConfigurationProperty("proxy", IsRequired = false)]
        public string Proxy
        {
            get
            {
                return this["proxy"] as string;
            }
        }
    }
}

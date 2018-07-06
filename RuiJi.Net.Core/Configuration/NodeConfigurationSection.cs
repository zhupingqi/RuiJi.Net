using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Core.Configuration
{
    /// <summary>
    /// node configuration section
    /// </summary>
    public class NodeConfigurationSection : ConfigurationSection
    {
        private static NodeConfigurationElement[] settings;

        /// <summary>
        /// Standalone mode
        /// </summary>
        public static bool Standalone
        {
            get
            {
                return settings.Length == 0;
            }
        }

        static NodeConfigurationSection()
        {
            var section = ConfigurationManager.GetSection("nodeSettings") as NodeConfigurationSection;
            var sets = section.NodeSettings as NodeConfigurationElementCollection;
            settings = new NodeConfigurationElement[section.NodeSettings.Count];
            section.NodeSettings.CopyTo(settings, 0);
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public NodeConfigurationElementCollection NodeSettings
        {
            get
            {
                return (NodeConfigurationElementCollection)base[""];
            }
        }

        /// <summary>
        /// get node configuration from baseUrl
        /// </summary>
        /// <param name="baseUrl">node baseUrl</param>
        /// <returns>node configuration</returns>
        public static NodeConfigurationElement Get(string baseUrl)
        {
            return settings.SingleOrDefault(m => m.BaseUrl == baseUrl);
        }

        /// <summary>
        /// all node setting
        /// </summary>
        public static List<NodeConfigurationElement> Settings
        {
            get
            {
                return settings.ToList();
            }
        }
    }
}
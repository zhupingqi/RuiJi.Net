using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Configuration
{
    public class NodeConfigurationSection : ConfigurationSection
    {
        private static NodeConfigurationElement[] settings;

        static NodeConfigurationSection()
        {
            var section = ConfigurationManager.GetSection("nodeSettings") as NodeConfigurationSection;
            var sets = section.NodeSettings as NodeConfigurationElementCollection;
            settings = new NodeConfigurationElement[section.NodeSettings.Count];
            section.NodeSettings.CopyTo(settings, 0);
        }

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public  NodeConfigurationElementCollection NodeSettings
        {
            get
            {
                return (NodeConfigurationElementCollection)base[""];
            }
        }

        public static NodeConfigurationElement Get(string baseUrl)
        {
            return settings.SingleOrDefault(m => m.BaseUrl == baseUrl);
        }

        public static List<NodeConfigurationElement> Settings
        {
            get
            {
                return settings.ToList();
            }
        }
    }
}
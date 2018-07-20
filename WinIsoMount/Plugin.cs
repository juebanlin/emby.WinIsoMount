using System;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;
using WinIsoMount.Configuration;

namespace WinIsoMount
{
    public class Plugin : BasePlugin<PluginConfiguration>
    {
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
			Instance = this;
        }
        
        private Guid _id = new Guid("70D61B47-5D41-456C-BAEC-F982AEDB96BF");
        public override Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the name of the plugin
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get { return "WinIsoMount"; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Mount and stream ISO contents";
            }
        }
		
		/// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
		public static Plugin Instance { get; private set; }
    }
}

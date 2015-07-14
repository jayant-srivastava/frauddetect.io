using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace frauddetect.service.fraudwatcher
{
    [RunInstaller(true)]
    public partial class FWInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public FWInstaller()
        {
            InitializeComponent();
        }
    }
}

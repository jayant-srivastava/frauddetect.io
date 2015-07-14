using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.service.fraudwatcher
{
    public partial class FWService : ServiceBase
    {
        public FWService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            QueryUserDB db = new QueryUserDB();
        }

        protected override void OnStop()
        {
        }


    }
}

namespace frauddetect.service.fraudwatcher
{
    partial class FWInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.ServiceName = "FraudDetect";
            serviceInstaller.DisplayName = "Fraud Detect Watcher";
            serviceInstaller.Description = "Fraud Detect watches Splunk alerts";

            serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(new System.Configuration.Install.Installer[]
                { serviceProcessInstaller, serviceInstaller });
        }

        #endregion
    }
}
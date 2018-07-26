
namespace XAS.App.Services.Framework {

    public partial class WindowsServiceInstaller {

        // taken from: http://geekswithblogs.net/BlackRabbitCoder/archive/2011/03/01/c-toolbox-debug-able-self-installable-windows-service-template-redux.aspx
        // with modifications

        /// <summary>
        /// Required designer variable.
        /// </summary>
        /// 
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 
        protected override void Dispose(bool disposing) {

            if (disposing && (components != null)) {

                components.Dispose();

            }

            base.Dispose(disposing);

        }

    #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// 
        private void InitializeComponent() {

            components = new System.ComponentModel.Container();

        }

     #endregion

    }

}

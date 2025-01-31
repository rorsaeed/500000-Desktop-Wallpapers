﻿using DevExpress.XtraSplashScreen;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AiWallpapers
{
    public partial class SplashScreen1 : SplashScreen
    {
        public SplashScreen1()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 2023-" + DateTime.Now.Year.ToString();
            string[] args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                if (arg == "-a")
                {
                    Hide();
                    Opacity = 0;
                }
            }
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }
    }
}
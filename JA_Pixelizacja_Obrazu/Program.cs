﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 @Author: Bartosz Faruga
 @Title: Pixelization of an image

 @version: 0.1
 */


namespace JA_Pixelizacja_Obrazu
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Image_Processor_Form());
        }
    }
}

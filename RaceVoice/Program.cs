using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RaceVoice
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var csv = RaceVoiceLib.Parser.RaceVoiceCsv.LoadCsvFile(@"C:\Users\Lewis\Downloads\pittsburg-standalone-aer-111519.csv");
            var trace = csv.GetDataTrace();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new splash(0));
            Application.Run(new MainForm());
        }
    }
}
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Temperatur
{
    static class Program
    {
        /// <summary>
        /// Der Haupt-Einstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]  // Diese Annotation sorgt dafür, dass die Anwendung im Single-Threaded Apartment-Modus läuft, was für Windows Forms erforderlich ist.
        static void Main()
        {
            // Application.EnableVisualStyles() aktiviert die Windows-Stile für die Formulare.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


        }
    }
}
    


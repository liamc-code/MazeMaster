/* ControlPanel.cs
* MazeMaster
* Revision History
* Liam Conn, 2024.10.28: Created
* Liam Conn, 2020.01.03: Comments added
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeMaster
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
            Application.Run(new ControlPanelForm());
        }
    }
}

/* ControlPanel.cs
* MazeMaster
* Revision History
* Liam Conn, 2024.10.28: Created
* Liam Conn, 2020.10.28: Added code
* Liam Conn, 2024.11.02: Added Code
* Liam Conn, 2024.11.03: Debugging complete
* Liam Conn, 2020.01.03: Comments added
*
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeMaster
{
    /// <summary>
    /// Parent Class of ControlPanelForm
    /// Starting point for program
    /// </summary>
    public partial class ControlPanelForm : Form
    {
        public ControlPanelForm()
        {
            InitializeComponent();
        }

        private void btnDesign_Click(object sender, EventArgs e)
        {
            // opens DesignForm as new object
            DesignForm designForm = new DesignForm();
            designForm.Show();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            // opens PlayForm as new object
            PlayForm playForm = new PlayForm();
            playForm.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class first_window : Form
    {
        public first_window()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Data_in F = new Data_in();
            this.Hide();
            F.Show();
        }

        private void first_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

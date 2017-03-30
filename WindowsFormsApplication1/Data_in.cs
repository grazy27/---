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
    public partial class Data_in : Form
    {
        int[] B = new int[4];
        int[] A = new int[4];
        public Data_in()
        {
          
            InitializeComponent();
        }
        
        private void textBox27_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fill()) runKid(); 
        }
        private void runKid()
        {
            Form1 N = new Form1(A,B);
            this.Hide();
            N.Show();
        }
        private bool fill()
        {
            try
            {
                A[0] = int.Parse(textBox28.Text);
                A[1] = int.Parse(textBox27.Text);
                A[2] = int.Parse(textBox26.Text);
                A[3] = int.Parse(textBox25.Text);
                B[0] = int.Parse(textBox20.Text);
                B[1] = int.Parse(textBox19.Text);
                B[2] = int.Parse(textBox18.Text);
                B[3] = int.Parse(textBox17.Text);
                return true;
            }
            catch(Exception)
            {
                MessageBox.Show("Неверный формат ввода.");
                return false;
            }



        }

        private void Data_in_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
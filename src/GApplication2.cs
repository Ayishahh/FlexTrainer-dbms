using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DB_phase2_project
{
    public partial class GApplication2 : Form
    {
        public GApplication2()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var login = new LogIn();
            login.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var Gdash = new GymOwnerDashboard();
            Gdash.Show();
            this.Close();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

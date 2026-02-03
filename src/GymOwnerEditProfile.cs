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
    public partial class GymOwnerEditProfile : Form
    {
        public GymOwnerEditProfile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Changes saved successfully!");
            var gymDash = new GymOwnerDashboard();
            this.Close();
            gymDash.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

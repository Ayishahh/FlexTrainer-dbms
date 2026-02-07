using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexTrainer
{
    public partial class M_editprofile : Form
    {
        public M_editprofile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Changes saved successfully!");
            var gymDash = new MemberDashboard();
            this.Close();
            gymDash.Show();
        }
    }
}

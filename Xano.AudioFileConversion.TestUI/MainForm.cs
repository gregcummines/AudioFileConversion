using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xano.AudioFileConversion.TestUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnStartConversion_Click(object sender, EventArgs e)
        {
            if (txtInputDirectory.Text.Length == 0 || txtOutputDirectory.Text.Length == 0)
            {
                MessageBox.Show("Must pick both input and output directories before starting the conversion", "Audio File Conversion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(txtInputDirectory.Text))
            {
                MessageBox.Show("Input directory does not exist", "Audio File Conversion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(txtOutputDirectory.Text))
            {
                MessageBox.Show("Output directory does not exist", "Audio File Conversion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnInputDirectory_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtInputDirectory.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnOutputDirectory_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtOutputDirectory.Text = fbd.SelectedPath;
                }
            }
        }
    }
}

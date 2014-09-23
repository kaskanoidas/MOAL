using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Mixed Optimisation Algorithm TM Gludis 2014, Created by: Rolandas Rimkus
namespace Mixed_Optimisation_Algorithm_Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Global variables
        Form form = new Form();
        //Task task = new Task();
        Task task = new Task(new List<int>() { 3600, 5010, 3000, 7100, 9000 }, new List<List<int>>() 
            {
                new List<int>(){3,5,8,10,18},
                new List<int>(){7,1,9,11,10},
                new List<int>(){9,3,2,8,0},
                new List<int>(){15,8,7,7,3},
                new List<int>(){0,20,1,11,12},
            });

        // Combinatorics algorithm
        private void button1_Click(object sender, EventArgs e)
        {
            Combinatorics_Algorithm CA = new Combinatorics_Algorithm();
            string text = CA.Combinatorics_Algorithm_Start();
            Create_Form("Combinatorics algorithm", out form, text);
        }
        // Optimized Simplex algorithm 
        private void button2_Click(object sender, EventArgs e)
        {
            Optimized_Simplex_Algorithm OSA = new Optimized_Simplex_Algorithm();
            string text = OSA.Optimized_Simplex_Algorithm_Start(task);
            Create_Form("Optimized Simplex algorithm", out form, text);
        }
        // Genetic algorithm
        private void button3_Click(object sender, EventArgs e)
        {
            Genetic_Algorithm GA = new Genetic_Algorithm();
            string text = GA.Genetic_Algorithm_Start(task);
            Create_Form("Genetic algorithm", out form, text);
        }
        // Linking Simplex and Genetic algorithms
        private void button4_Click(object sender, EventArgs e)
        {
            Create_Form("Linking Simplex and Genetic algorithms", out form, "");
        }
        // Multi-Threading with Genetic algorithm
        private void button5_Click(object sender, EventArgs e)
        {
            Create_Form("Multi-Threading with Genetic algorithm", out form, "");
        }
        // Multi-Threading with Simplex algorithm
        private void button6_Click(object sender, EventArgs e)
        {
            Create_Form("Multi-Threading with Simplex algorithm", out form, "");
        }
        // Linking all algorithms together
        private void button7_Click(object sender, EventArgs e)
        {
            Create_Form("Linking all algorithms together", out form, "");
        }
        // Combinatorics algorithm rules handling
        private void button8_Click(object sender, EventArgs e)
        {
            Create_Form("Combinatorics algorithm rules handling", out form, "");
        }
        // Optimized Simplex algorithm rules handling
        private void button9_Click(object sender, EventArgs e)
        {
            Create_Form("Optimized Simplex algorithm rules handling", out form, "");
        }
        // Genetic algorithm rules handling
        private void button10_Click(object sender, EventArgs e)
        {
            Create_Form("Genetic algorithm rules handling", out form, "");
        }
        // Time, CPU and memory handling
        private void button11_Click(object sender, EventArgs e)
        {
            Create_Form("Time, CPU and memory handling", out form, "");
        }
        // UI form create
        private void Create_Form(string header, out Form MyForm, string text)
        {
            using (MyForm = new Form())
            {
                MyForm.Text = header;
                MyForm.ClientSize = new System.Drawing.Size(500, 500);
                MyForm.MinimumSize = new System.Drawing.Size(500, 500);
                MyForm.VerticalScroll.Enabled = true;
                RichTextBox textbox = new RichTextBox();
                textbox.Dock = DockStyle.Fill;
                textbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
                textbox.Location = new System.Drawing.Point(4, 4);
                textbox.ReadOnly = true;
                textbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
                textbox.Text = text;
                MyForm.Controls.Add(textbox);
                MyForm.ShowDialog();
            }
        }
    }
}

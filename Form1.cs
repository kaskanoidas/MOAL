using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


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
            BackgroundWorker Work_Thread = new BackgroundWorker();
            Work_Thread.DoWork += new DoWorkEventHandler(delegate(object sender2, DoWorkEventArgs e2)
            {
                Combinatorics_Algorithm CA = new Combinatorics_Algorithm();
                e2.Result = CA.Combinatorics_Algorithm_Start();
            });
            Work_Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender3, RunWorkerCompletedEventArgs e3)
            {
                Create_Form("Combinatorics algorithm", (string)e3.Result);
            });
            Work_Thread.RunWorkerAsync();
        }
        // Optimized Simplex algorithm 
        private void button2_Click(object sender, EventArgs e)
        {
            BackgroundWorker Work_Thread = new BackgroundWorker();
            Work_Thread.DoWork += new DoWorkEventHandler(delegate(object sender2, DoWorkEventArgs e2)
            {
                Optimized_Simplex_Algorithm OSA = new Optimized_Simplex_Algorithm();
                Tuple<string, List<Solution>> Results = OSA.Optimized_Simplex_Algorithm_Start(task, 1);
                e2.Result = Results.Item1;
            });
            Work_Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender3, RunWorkerCompletedEventArgs e3)
            {
                Create_Form("Optimized Simplex algorithm", (string)e3.Result);
            });
            Work_Thread.RunWorkerAsync();
        }
        // Genetic algorithm
        private void button3_Click(object sender, EventArgs e)
        {
            BackgroundWorker Work_Thread = new BackgroundWorker();
            Work_Thread.DoWork += new DoWorkEventHandler(delegate(object sender2, DoWorkEventArgs e2)
            {
                Genetic_Algorithm GA = new Genetic_Algorithm();
                e2.Result = GA.Genetic_Algorithm_Start(task, new List<Solution>() { }, 1);
            });
            Work_Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender3, RunWorkerCompletedEventArgs e3)
            {
                Create_Form("Genetic algorithm", (string)e3.Result);
            });
            Work_Thread.RunWorkerAsync();
        }
        // Linking Simplex and Genetic algorithms
        private void button4_Click(object sender, EventArgs e)
        {
            BackgroundWorker Work_Thread = new BackgroundWorker();
            Work_Thread.DoWork += new DoWorkEventHandler(delegate(object sender2, DoWorkEventArgs e2)
            {
                Optimized_Simplex_Algorithm OSA = new Optimized_Simplex_Algorithm();
                Tuple<string, List<Solution>> Results = OSA.Optimized_Simplex_Algorithm_Start(task, 1);
                Genetic_Algorithm GA = new Genetic_Algorithm();
                e2.Result = GA.Genetic_Algorithm_Start(task, new List<Solution>(Results.Item2), 1);
            });
            Work_Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender3, RunWorkerCompletedEventArgs e3)
            {
                Create_Form("Linking Simplex and Genetic algorithms", (string)e3.Result);
            });
            Work_Thread.RunWorkerAsync();
        }
        // Multi-Threading with Genetic algorithm
        private void button5_Click(object sender, EventArgs e)
        {
            BackgroundWorker Work_Thread = new BackgroundWorker();
            Work_Thread.DoWork += new DoWorkEventHandler(delegate(object sender2, DoWorkEventArgs e2)
            {
                Genetic_Algorithm GA = new Genetic_Algorithm();
                e2.Result = GA.Genetic_Algorithm_Start(task, new List<Solution>() { }, 10);
            });
            Work_Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender3, RunWorkerCompletedEventArgs e3)
            {
                Create_Form("Multi-Threading with Genetic algorithm", (string)e3.Result);
            });
            Work_Thread.RunWorkerAsync();
        }
        // Multi-Threading with Simplex algorithm
        private void button6_Click(object sender, EventArgs e)
        {
            BackgroundWorker Work_Thread = new BackgroundWorker();
            Work_Thread.DoWork += new DoWorkEventHandler(delegate(object sender2, DoWorkEventArgs e2)
            {
                Optimized_Simplex_Algorithm OSA = new Optimized_Simplex_Algorithm();
                Tuple<string, List<Solution>> Results = OSA.Optimized_Simplex_Algorithm_Start(task, 10);
                e2.Result = Results.Item1;
            });
            Work_Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender3, RunWorkerCompletedEventArgs e3)
            {
                Create_Form("Multi-Threading with Simplex algorithm", (string)e3.Result);
            });
            Work_Thread.RunWorkerAsync();
        }
        // UI form create
        private void Create_Form(string header, string text)
        {
            Thread MainThread = new Thread(delegate()
            {
                using (Form MyForm = new Form())
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
            });
            MainThread.Start();
        }
    }
}

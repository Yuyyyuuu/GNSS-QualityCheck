using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GNSS_QC
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        public int G_N;
        public int B_N;
        public double ep;
        public string[] s_PRN = new string[50];
        public int[] Gc_num = new int[50];
        public int[] Bc_num = new int[50];
        public double[] Gc_p = new double[50];
        public double[] Bc_p = new double[50];


        private void button1_Click(object sender, EventArgs e)
        {
           
            for (int i=0;i<G_N;i++)
            {
                Gc_p[i] = Gc_num[i]/ep;
                 //MessageBox.Show(Convert.ToString(Gc_p[i]));//检验所用
                //MessageBox.Show(Convert.ToString(Gc_num[i]));
            }
            // 设置表格的行数和列数
            int rowCount = 1+G_N;
            int columnCount = 4;
            dataGridView1.RowCount = rowCount;
            dataGridView1.ColumnCount = columnCount;

            // 给每一行和每一列的单元格赋值
                dataGridView1.Rows[0].Cells[0].Value = "PRN";
                dataGridView1.Rows[0].Cells[1].Value = "周跳数";
                dataGridView1.Rows[0].Cells[2].Value = "总历元数";
                dataGridView1.Rows[0].Cells[3].Value = "周跳比";
            
            for (int i = 1; i < rowCount; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = s_PRN[i-1];
            }
            for(int i=1;i < columnCount; i++)
            {
                for(int j=1;j < rowCount; j++)
                {
                    if(i==1)
                    {
                        dataGridView1.Rows[j].Cells[i].Value = Gc_num[j-1];
                    }
                    if (i == 2)
                    {
                        dataGridView1.Rows[j].Cells[i].Value = ep;
                    }
                    if (i == 3)
                    {
                        dataGridView1.Rows[j].Cells[i].Value = Gc_p[j-1];
                    }


                }

            }
            dataGridView1.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkBlue;
            dataGridView1.RowsDefaultCellStyle.SelectionBackColor = Color.Yellow;
            dataGridView1.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "P";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < B_N; i++)
            {
                Bc_p[i] = Bc_num[i] / ep;
            }

            // 设置表格的行数和列数
            int rowCount = 1 + B_N;
            int columnCount = 4;
            dataGridView2.RowCount = rowCount;
            dataGridView2.ColumnCount = columnCount;

            // 给每一行和每一列的单元格赋值
            dataGridView2.Rows[0].Cells[0].Value = "PRN";
            dataGridView2.Rows[0].Cells[1].Value = "周跳数";
            dataGridView2.Rows[0].Cells[2].Value = "总历元数";
            dataGridView2.Rows[0].Cells[3].Value = "周跳比";

            for (int i = 1; i < rowCount; i++)
            {
                dataGridView2.Rows[i].Cells[0].Value = s_PRN[i+ G_N - 1];
            }

            for (int i = 1; i < columnCount; i++)
            {
                for (int j = 1; j < rowCount; j++)
                {
                    if (i == 1)
                    {
                        dataGridView2.Rows[j].Cells[i].Value = Bc_num[j - 1];
                    }
                    if (i == 2)
                    {
                        dataGridView2.Rows[j].Cells[i].Value = ep;
                    }
                    if (i == 3)
                    {
                        dataGridView2.Rows[j].Cells[i].Value = Bc_p[j - 1];
                    }
                }
            }

            dataGridView2.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkBlue;
            dataGridView2.RowsDefaultCellStyle.SelectionBackColor = Color.Yellow;
            dataGridView2.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView2.Columns[3].DefaultCellStyle.Format = "P";
        }
    }
}

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
    public partial class Form2 : Form
    {

     

        public void SetData(int data1, int data2, double d3, double d4, double d5, double d6, double d7,
           double d8, double d9, double d10, double d11, double d12, double d13, double d14, double d15,
           double d16, double d17, double d18)
        {
            G_EP_SUM = data1;
            B_EP_SUM = data2;
            L1_EP = d3;
            L2_EP = d4;
            L5_EP = d5;
            GPS_EP = d6;
            B1_EP = d7;
            B2_EP = d8;
            B3_EP = d9;
            BDS_EP = d10;
            L1_INTE = d11;
            L2_INTE = d12;
            L5_INTE = d13;
            GPS_INTE = d14;
            B1_INTE = d15;
            B2_INTE = d16;
            B3_INTE = d17;
            BDS_INTE = d18;
        }

        public double L1_INTE, L2_INTE, L5_INTE, GPS_INTE, B1_INTE, B2_INTE, B3_INTE, BDS_INTE;//数据完整率

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

           
            // 设置控件的文本
            
                tableLayoutPanel1.Controls.Add(new Label() { Text = "GPS" }, 0, 0);
                tableLayoutPanel1.Controls.Add(new Label() { Text = "L1" }, 1, 0);
                tableLayoutPanel1.Controls.Add(new Label() { Text = "L2" }, 2, 0);
                tableLayoutPanel1.Controls.Add(new Label() { Text = "L5" }, 3, 0);
                tableLayoutPanel1.Controls.Add(new Label() { Text = "总系统" }, 4, 0);

                tableLayoutPanel1.Controls.Add(new Label() { Text = "实际历元数" }, 0, 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = L1_EP.ToString() }, 1, 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = L2_EP.ToString() }, 2, 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = L5_EP.ToString() }, 3, 1);
                tableLayoutPanel1.Controls.Add(new Label() { Text = GPS_EP.ToString() }, 4, 1);

                tableLayoutPanel1.Controls.Add(new Label() { Text = "理论历元数" }, 0, 2);
                tableLayoutPanel1.Controls.Add(new Label() { Text = G_EP_SUM.ToString() }, 1, 2);
                tableLayoutPanel1.Controls.Add(new Label() { Text = G_EP_SUM.ToString() }, 2, 2);
                tableLayoutPanel1.Controls.Add(new Label() { Text = G_EP_SUM.ToString() }, 3, 2);
                tableLayoutPanel1.Controls.Add(new Label() { Text = G_EP_SUM.ToString() }, 4, 2);

                tableLayoutPanel1.Controls.Add(new Label() { Text = "数据完整率" }, 0, 3);
                tableLayoutPanel1.Controls.Add(new Label() { Text = L1_INTE.ToString() }, 1, 3);
                tableLayoutPanel1.Controls.Add(new Label() { Text = L2_INTE.ToString() }, 2, 3);
                tableLayoutPanel1.Controls.Add(new Label() { Text = L5_INTE.ToString() }, 3, 3);
                tableLayoutPanel1.Controls.Add(new Label() { Text = GPS_INTE.ToString() }, 4, 3);
            // 设置 TableLayoutPanel 的样式和布局
                 tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                 tableLayoutPanel1.Padding = new Padding(10);
                 tableLayoutPanel1.Margin = new Padding(10);
            // 设置单元格的边框样式
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is Label)
                {
                    ((Label)control).BorderStyle = BorderStyle.FixedSingle;
                    ((Label)control).Padding = new Padding(1);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tableLayoutPanel2.Controls.Add(new Label() { Text = "BDS" }, 0, 0);
            tableLayoutPanel2.Controls.Add(new Label() { Text = "B1" }, 1, 0);
            tableLayoutPanel2.Controls.Add(new Label() { Text = "B2" }, 2, 0);
            tableLayoutPanel2.Controls.Add(new Label() { Text = "B3" }, 3, 0);
            tableLayoutPanel2.Controls.Add(new Label() { Text = "总系统" }, 4, 0);

            tableLayoutPanel2.Controls.Add(new Label() { Text = "实际历元数" }, 0, 1);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B1_EP.ToString() }, 1, 1);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B2_EP.ToString() }, 2, 1);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B3_EP.ToString() }, 3, 1);
            tableLayoutPanel2.Controls.Add(new Label() { Text = BDS_EP.ToString() }, 4, 1);

            tableLayoutPanel2.Controls.Add(new Label() { Text = "理论历元数" }, 0, 2);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B_EP_SUM.ToString() }, 1, 2);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B_EP_SUM.ToString() }, 2, 2);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B_EP_SUM.ToString() }, 3, 2);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B_EP_SUM.ToString() }, 4, 2);

            tableLayoutPanel2.Controls.Add(new Label() { Text = "数据完整率" }, 0, 3);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B1_INTE.ToString() }, 1, 3);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B2_INTE.ToString() }, 2, 3);
            tableLayoutPanel2.Controls.Add(new Label() { Text = B3_INTE.ToString() }, 3, 3);
            tableLayoutPanel2.Controls.Add(new Label() { Text = BDS_INTE.ToString() }, 4, 3);

            // 设置 TableLayoutPane2 的样式和布局
            tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel2.Padding = new Padding(10);
            tableLayoutPanel2.Margin = new Padding(10);
            // 设置单元格的边框样式
            foreach (Control control in tableLayoutPanel2.Controls)
            {
                if (control is Label)
                {
                    ((Label)control).BorderStyle = BorderStyle.FixedSingle;
                    ((Label)control).Padding = new Padding(1);
                }
            }
        }

      

        public double L1_EP, L2_EP, L5_EP, GPS_EP, B1_EP, B2_EP, B3_EP, BDS_EP;//实际历元数
        public int G_EP_SUM, B_EP_SUM;//理论历元数

        private void Form2_Load(object sender, EventArgs e)
        {

        }

       
        public Form2()
        {
            
            InitializeComponent();    
         

        }

        







    }
}

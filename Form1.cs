using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.WinForms;
using System.Windows.Forms.DataVisualization.Charting;
using Excel = Microsoft.Office.Interop.Excel;


namespace GNSS_QC
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //构建存储GPS系统数据的类
        public class GPS
        {
            public double C1 { get; set; }
            public double C2 { get; set; }
            public double C5 { get; set; }
            public double L1 { get; set; }
            public double L2 { get; set; }
            public double L5 { get; set; }
            public double S1 { get; set; }
            public double S2 { get; set; }
            public double S5 { get; set; }
            public string PRN { get; set; }
        }

        //构建存储BDS系统数据的类
        public class BDS
        {
            public double C2 { get; set; }
            public double C7 { get; set; }
            public double C6 { get; set; }
            public double L2 { get; set; }
            public double L7 { get; set; }
            public double L6 { get; set; }
            public double S2 { get; set; }
            public double S7 { get; set; }
            public double S6 { get; set; }
            public string PRN { get; set; }
        }

        //构建历元数据块
        public class EP
        {
            public double time;//自1980年的秒数
                               //建立GPS和BDS类型数组，存放每一历元的数据
            public GPS[] G = new GPS[50];
            public BDS[] B = new BDS[50];
        }

        //o文件地址
        public string filePath1 = "";

        //数据开始时间与结束时间
        public double ST, ET;
        //采样间隔
        public double T;
        //历元数
        public double epo;
        //GPS的卫星数
        public int NSatoG = 0;
        //BDS的卫星数
        public int NSatoB = 0;
        //GPS和BDS的卫星PRN号存储字符串，先GPS后BDS的顺序存放，取用时可以结合GPS和BDS卫星数进行精准取出
        public string[] str_PRN = new string[50];

        //历元块
        public EP[] E = new EP[9999];//这里设置元素个数多一些，防止不够用
        //历元计数器
        public int ei = 0;



        //读入o文件
        private void button1_Click(object sender, EventArgs e)
        {
            //初始化数据容器
            for (int i = 0; i < 9999; i++)
            {
                E[i] = new EP(); // 实例化每个 EP 对象
                E[i].time = 0;

                for (int j = 0; j < 50; j++)
                {

                    E[i].G[j] = new GPS(); // 实例化每个 GPS 对象
                    E[i].B[j] = new BDS(); // 实例化每个 BDS 对象
                    E[i].G[j].PRN = " ";
                    E[i].B[j].PRN = " ";
                    E[i].G[j].C1 = E[i].G[j].C2 = E[i].G[j].C5 = 0;
                    E[i].G[j].L1 = E[i].G[j].L2 = E[i].G[j].L5 = 0;
                    E[i].G[j].S1 = E[i].G[j].S2 = E[i].G[j].S5 = 0;
                    E[i].B[j].C2 = E[i].B[j].C6 = E[i].B[j].C7 = 0;
                    E[i].B[j].L2 = E[i].B[j].L6 = E[i].B[j].L7 = 0;
                    E[i].B[j].S2 = E[i].B[j].S6 = E[i].B[j].S7 = 0;
                }
            }
            for (int i = 0; i < 50; i++)
            {
                str_PRN[i] = " "; // 初始化每个元素
            }
            int Pnum = 0;//读取PRN值计数器
            //浏览文件
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;  //该值确定是否可以选择多个文件
            openFileDialog.Title = "请选择文件";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != "")
                {
                    filePath1 = openFileDialog.FileName; // 将所选文件的完整路径赋值给filePath变量
                    this.Search_text.Text = filePath1;

                    // 读取文件的每一行
                    string[] lines = File.ReadAllLines(filePath1);

                    foreach (string line in lines)
                    {
                        //读取采样间隔
                        if (line.Length >= 68 && line.Substring(60, 8) == "INTERVAL")
                        {
                            T = double.Parse(line.Substring(5, 5));

                            // 在消息框中显示字符串进行检查是否读取采样间隔正确
                            // MessageBox.Show(Convert.ToString(T));
                        }

                        //读取开始时间与结束时间
                        //都转化为以1980年整开始的秒数（因为GPS起点为1980年1月6日0h00m00s）
                        if (line.Length >= 77 && line.Substring(60, 17) == "TIME OF FIRST OBS")
                        {
                            int year, month, day, hour, min, second;
                            year = int.Parse(line.Substring(2, 4));
                            month = int.Parse(line.Substring(10, 2));
                            day = int.Parse(line.Substring(16, 2));
                            hour = int.Parse(line.Substring(22, 2));
                            min = int.Parse(line.Substring(28, 2));
                            second = int.Parse(line.Substring(33, 2));

                            DateTime givenTime = new DateTime(year, month, day, hour, min, second);
                            DateTime startTime = new DateTime(1980, 1, 1);

                            TimeSpan timeDifference = givenTime - startTime;
                            ST = timeDifference.TotalSeconds;

                            // 在消息框中显示字符串进行检查是否读取正确
                            // MessageBox.Show(Convert.ToString(ST));
                        }

                        if (line.Length >= 76 && line.Substring(60, 16) == "TIME OF LAST OBS")
                        {
                            int year, month, day, hour, min, second;
                            year = int.Parse(line.Substring(2, 4));
                            month = int.Parse(line.Substring(10, 2));
                            day = int.Parse(line.Substring(16, 2));
                            hour = int.Parse(line.Substring(22, 2));
                            min = int.Parse(line.Substring(28, 2));
                            second = int.Parse(line.Substring(33, 2));

                            DateTime givenTime = new DateTime(year, month, day, hour, min, second);
                            DateTime startTime = new DateTime(1980, 1, 1);

                            TimeSpan timeDifference = givenTime - startTime;
                            ET = timeDifference.TotalSeconds;

                            // 在消息框中显示字符串进行检查是否读取正确
                            //MessageBox.Show(Convert.ToString(ET));
                        }

                        //读取GPS和BDS的卫星个数和PRN号
                        if (line.Length >= 4 && line.Substring(3, 1) == "G")
                        {
                            NSatoG++;
                            str_PRN[Pnum] = "G" + (int.Parse(line.Substring(4, 2))).ToString();
                            Pnum++;

                        }
                        if (line.Substring(0, 1) == " " && line.Length >= 4 && line.Substring(3, 1) == "C")
                        {
                            NSatoB++;
                            str_PRN[Pnum] = "B" + (int.Parse(line.Substring(4, 2))).ToString();
                            Pnum++;

                        }

                        //读取每一历元的GPS和BDS各个卫星的观测数据
                        //历元时间

                        if (line.Substring(0, 1) == ">")
                        {
                            ei = ei + 1;
                            //MessageBox.Show(Convert.ToString(ei));
                            int year, month, day, hour, min, second;
                            year = int.Parse(line.Substring(2, 4));
                            month = int.Parse(line.Substring(7, 2));
                            day = int.Parse(line.Substring(10, 2));
                            hour = int.Parse(line.Substring(13, 2));
                            min = int.Parse(line.Substring(16, 2));
                            second = int.Parse(line.Substring(19, 2));

                            DateTime givenTime = new DateTime(year, month, day, hour, min, second);
                            DateTime startTime = new DateTime(1980, 1, 1);

                            TimeSpan timeDifference = givenTime - startTime;
                            E[ei].time = timeDifference.TotalSeconds;
                            //MessageBox.Show(Convert.ToString((E[ei].time)));
                        }

                        //观测数据
                        if (line.Substring(0, 1) == "G" && line.Substring(1, 1) != " ")
                        {
                            int nG = int.Parse(line.Substring(1, 2));
                            E[ei].G[nG].PRN = "G" + nG.ToString();
                            E[ei].G[nG].C1 = double.Parse(line.Substring(5, 12));
                            E[ei].G[nG].C2 = double.Parse(line.Substring(21, 12));
                            E[ei].G[nG].C5 = double.Parse(line.Substring(37, 12));
                            E[ei].G[nG].L1 = double.Parse(line.Substring(52, 15));
                            E[ei].G[nG].L2 = double.Parse(line.Substring(68, 15));
                            E[ei].G[nG].L5 = double.Parse(line.Substring(84, 15));
                            E[ei].G[nG].S1 = double.Parse(line.Substring(106, 9));
                            E[ei].G[nG].S2 = double.Parse(line.Substring(122, 9));
                            E[ei].G[nG].S5 = double.Parse(line.Substring(138, 9));
                        }

                        if (line.Substring(0, 1) == "C" && line.Substring(1, 1) != " " && line.Substring(1, 1) != "H")
                        {
                            int nB = int.Parse(line.Substring(1, 2));
                            E[ei].B[nB].PRN = "B" + nB.ToString(); ;
                            E[ei].B[nB].C2 = double.Parse(line.Substring(5, 12));
                            E[ei].B[nB].C7 = double.Parse(line.Substring(21, 12));
                            E[ei].B[nB].C6 = double.Parse(line.Substring(37, 12));
                            E[ei].B[nB].L2 = double.Parse(line.Substring(52, 15));
                            E[ei].B[nB].L7 = double.Parse(line.Substring(68, 15));
                            E[ei].B[nB].L6 = double.Parse(line.Substring(84, 15));
                            E[ei].B[nB].S2 = double.Parse(line.Substring(106, 9));
                            E[ei].B[nB].S7 = double.Parse(line.Substring(122, 9));
                            E[ei].B[nB].S6 = double.Parse(line.Substring(138, 9));
                        }


                    }

                    //计算总历元数
                    epo = (ET - ST) / T + 1;
                    //MessageBox.Show(Convert.ToString(epo));//检验所用
                    //MessageBox.Show(Convert.ToString(NSatoB));
                    //MessageBox.Show(Convert.ToString((E[1].B[8].C2)));
                }
            }
            //for (int i = 0; i < NSatoG + NSatoB; i++)
            // {
            //   MessageBox.Show(str_PRN[i]);//检验所用
            //  }        
            //MessageBox.Show(Convert.ToString(E[1].B[7].S2));
            // MessageBox.Show(Convert.ToString(E[1].B[7].S7));
            // MessageBox.Show(Convert.ToString(E[1].B[7].S6));
            MessageBox.Show("文件读取成功！");
        }





        //完整性检测
        public double L1_INTE, L2_INTE, L5_INTE, GPS_INTE, B1_INTE, B2_INTE, B3_INTE, BDS_INTE;//数据完整率


        public double L1_EP, L2_EP, L5_EP, GPS_EP, B1_EP, B2_EP, B3_EP, BDS_EP;//实际历元数

       

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        public int G_EP_SUM, B_EP_SUM;

        
        //完整性检测窗体
        private void button4_Click(object sender, EventArgs e)
        {
            //理论历元数
            G_EP_SUM = (int)epo * NSatoG;
            B_EP_SUM = (int)epo * NSatoB;
            //MessageBox.Show(Convert.ToString(B_EP_SUM));
            int[] GA1 = new int[50];
            int[] GA2 = new int[50];
            int[] GA5 = new int[50];
            int[] GC = new int[50];
            int[] BA1 = new int[50];
            int[] BA2 = new int[50];
            int[] BA3 = new int[50];
            int[] BC = new int[50];
            //赋初值
            for (int i = 0; i < 50; i++)
            {
                GA1[i] = GA2[i] = GA5[i] = GC[i] = 0;
            }
            for (int i = 0; i < 50; i++)
            {
                BA1[i] = BA2[i] = BA3[i] = BC[i] = 0;
            }
            L1_EP = L2_EP = L5_EP = GPS_EP = B1_EP = B2_EP = B3_EP = BDS_EP = 0;

            for (int i = 1; i < ((int)epo + 1); i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    if (E[i].G[j].C1 != 0 && E[i].G[j].L1 != 0)
                    {
                        GA1[j]++;
                    }
                    if (E[i].G[j].C2 != 0 && E[i].G[j].L2 != 0)
                    {
                        GA2[j]++;
                    }
                    if (E[i].G[j].C5 != 0 && E[i].G[j].L5 != 0)
                    {
                        GA5[j]++;
                    }
                    if (E[i].G[j].C1 != 0 && E[i].G[j].L1 != 0 && E[i].G[j].C2 != 0 && E[i].G[j].L2 != 0 && E[i].G[j].C5 != 0 && E[i].G[j].L5 != 0)
                    {
                        GC[j]++;
                    }
                    if (E[i].B[j].C2 != 0 && E[i].B[j].L2 != 0)
                    {
                        BA1[j]++;
                    }
                    if (E[i].B[j].C7 != 0 && E[i].B[j].L7 != 0)
                    {
                        BA2[j]++;
                    }
                    if (E[i].B[j].C6 != 0 && E[i].B[j].L6 != 0)
                    {
                        BA3[j]++;
                    }
                    if (E[i].B[j].C2 != 0 && E[i].B[j].L2 != 0 && E[i].B[j].C7 != 0 && E[i].B[j].L7 != 0 && E[i].B[j].C6 != 0 && E[i].B[j].L6 != 0)
                    {
                        BC[j]++;
                    }


                }


            }

            //计算实际历元数
            for (int i = 0; i < 50; i++)
            {
                L1_EP = L1_EP + GA1[i];
                L2_EP = L2_EP + GA2[i];
                L5_EP = L5_EP + GA5[i];
                GPS_EP = GPS_EP + GC[i];
            }
            for (int i = 0; i < 50; i++)
            {
                B1_EP = B1_EP + BA1[i];
                B2_EP = B2_EP + BA2[i];
                B3_EP = B3_EP + BA3[i];
                BDS_EP = BDS_EP + BC[i];
            }

            //计算数据完整率
            L1_INTE = L1_EP / G_EP_SUM;
            L2_INTE = L2_EP / G_EP_SUM;
            L5_INTE = L5_EP / G_EP_SUM;
            GPS_INTE = GPS_EP / G_EP_SUM;
            B1_INTE = B1_EP / B_EP_SUM;
            B2_INTE = B2_EP / B_EP_SUM;
            B3_INTE = B3_EP / B_EP_SUM;
            BDS_INTE = BDS_EP / B_EP_SUM;

            //MessageBox.Show(Convert.ToString(B1_INTE));//经检验，完整率数值计算合理

            Form2 f2 = new Form2();
            f2.SetData(G_EP_SUM, B_EP_SUM, L1_EP, L2_EP, L5_EP, GPS_EP, B1_EP, B2_EP, B3_EP, BDS_EP,
                L1_INTE, L2_INTE, L5_INTE, GPS_INTE, B1_INTE, B2_INTE, B3_INTE, BDS_INTE);
            f2.Show();     //打开新窗体

        }



        //新建GPS和BDS信噪比类
        public class G_SNR
        {
            public double S1 { get; set; }
            public double S2 { get; set; }
            public double S5 { get; set; }
            public string PRN { get; set; }
        }

       

        public class B_SNR
        {
            public double S1 { get; set; }
            public double S2 { get; set; }
            public double S3 { get; set; }
            public string PRN { get; set; }
        }

        //构建SNR历元数据块
        public class SNREP
        {
            public int tinum;
            public G_SNR[] Gsnr; // 定义 GSNR 数组
            public B_SNR[] Bsnr; // 定义 GSNR 数组
            // 构造函数
            public SNREP(int gLength, int bLength)
            {
                Gsnr = new G_SNR[gLength];
                Bsnr = new B_SNR[bLength];
            }
        }

        

        //画SNR时序图

        public SNREP[] snrep = new SNREP[9999];
        private void button3_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < (int)epo; i++)
            {
                snrep[i] = new SNREP(NSatoG, NSatoB);
                snrep[i].tinum = i + 1;

                // 初始化 Gsnr 数组
                for (int j = 0; j < NSatoG; j++)
                {
                    snrep[i].Gsnr[j] = new G_SNR();
                    snrep[i].Gsnr[j].PRN = str_PRN[j]; // 初始化 PRN 属性
                    snrep[i].Gsnr[j].S1 = 0.0; // 初始化 S1 属性
                    snrep[i].Gsnr[j].S2 = 0.0; // 初始化 S2 属性
                    snrep[i].Gsnr[j].S5 = 0.0; // 初始化 S5 属性
                }

                // 初始化 Bsnr 数组
                for (int j = 0; j < NSatoB; j++)
                {
                    snrep[i].Bsnr[j] = new B_SNR();
                    snrep[i].Bsnr[j].PRN = str_PRN[j + NSatoG]; // 初始化 PRN 属性
                    snrep[i].Bsnr[j].S1 = 0.0; // 初始化 S1 属性
                    snrep[i].Bsnr[j].S2 = 0.0; // 初始化 S2 属性
                    snrep[i].Bsnr[j].S3 = 0.0; // 初始化 S3 属性
                }
            }


            for (int i = 0; i < (int)epo; i++)
            {


                for (int j = 0; j < NSatoG; j++)
                {
                    //MessageBox.Show(snrep[i].Gsnr[j].PRN);//检验所用
                    for (int k = 0; k < 50; k++)
                    {
                        if (E[i + 1].G[k].PRN == snrep[i].Gsnr[j].PRN)
                        {
                            snrep[i].Gsnr[j].S1 = E[i + 1].G[k].S1;
                            snrep[i].Gsnr[j].S2 = E[i + 1].G[k].S2;
                            snrep[i].Gsnr[j].S5 = E[i + 1].G[k].S5;
                        }

                    }

                }


                for (int j = 0; j < NSatoB; j++)
                {
                    //MessageBox.Show(snrep[i].Bsnr[j].PRN);//检验所用
                    for (int k = 0; k < 50; k++)
                    {
                        if (E[i + 1].B[k].PRN == snrep[i].Bsnr[j].PRN)
                        {
                            snrep[i].Bsnr[j].S1 = E[i + 1].B[k].S2;
                            snrep[i].Bsnr[j].S2 = E[i + 1].B[k].S7;
                            snrep[i].Bsnr[j].S3 = E[i + 1].B[k].S6;
                        }

                    }
                }
            }

            MessageBox.Show("信噪比分析完毕！");

        }

        //下拉选项显示选中卫星系统的SNR时序图
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selectedOption = comboBox.SelectedItem.ToString();

            // 清空时序图数据
            chart.Series.Clear();
            // 取消显示主要水平格网线
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            // 取消显示主要垂直格网线
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            // 取消显示次要水平格网线
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            // 取消显示次要垂直格网线
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            // 根据选择的选项来显示相应的时序图
            if (selectedOption == "GPS")
            {
                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoG; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(snrep[1].Gsnr[i].PRN);
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoG - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY(snrep[j].tinum, ((snrep[j].Gsnr[i].S1 + snrep[j].Gsnr[i].S2 + snrep[j].Gsnr[i].S5) / 3));

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart.Series.Add(series);
                }

            }
            else if (selectedOption == "BDS")
            {

                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoB; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(snrep[1].Bsnr[i].PRN);
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoB - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY(snrep[j].tinum, ((snrep[j].Bsnr[i].S1 + snrep[j].Bsnr[i].S2 + snrep[j].Bsnr[i].S3) / 3));

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart.Series.Add(series);
                }

            }




        }

        


        private void chart1_Click_1(object sender, EventArgs e)
        {

        }


        //下面时周跳探测部分
        //设置卫星类
        public class G_Sat
        {
            public string PRN { get; set; }
            public int[] flag { get; set; }
            public int c_total { get; set; }
            public double[] C1 { get; set; }
            public double[] C2 { get; set; }
            public double[] L1 { get; set; }
            public double[] L2 { get; set; }
            public double[] GF { get; set; }
            public double[] MW { get; set; }

            public G_Sat(int length)
            {
                PRN = " ";
                flag = new int[length];
                c_total = 0;
                C1 = new double[length];
                C2 = new double[length];
                L1 = new double[length];
                L2 = new double[length];
                GF = new double[length];
                MW = new double[length];
            }
        }
        public class B_Sat
        {
            public string PRN { get; set; }
            public int[] flag { get; set; }
            public int c_total { get; set; }
            public double[] C2 { get; set; }
            public double[] C6 { get; set; }
            public double[] L2 { get; set; }
            public double[] L6 { get; set; }
            public double[] GF { get; set; }
            public double[] MW { get; set; }

            public B_Sat(int length)
            {
                PRN = " ";
                flag = new int[length];
                c_total = 0;
                C2 = new double[length];
                C6 = new double[length];
                L2 = new double[length];
                L6 = new double[length];
                GF = new double[length];
                MW = new double[length];
            }
        }
        //实例化GPS卫星
        G_Sat[] gSat = new G_Sat[50];
        // 实例化BDS卫星
        B_Sat[] bSat = new B_Sat[50];
        //周跳的数据处理button
        private void button2_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < 50; i++)
            {
                gSat[i] = new G_Sat(9999);

                gSat[i].PRN = " ";
                gSat[i].c_total = 0;

                for (int j = 0; j < 9999; j++)
                {
                    gSat[i].flag[j] = 0;
                    gSat[i].C1[j] = 0;
                    gSat[i].C2[j] = 0;
                    gSat[i].L1[j] = 0;
                    gSat[i].L2[j] = 0;
                    gSat[i].GF[j] = 0;
                    gSat[i].MW[j] = 0;
                }
            }

            //为GPS的各个元素赋正确的值
            for (int i = 0; i < NSatoG; i++)
            {
                gSat[i].PRN = str_PRN[i];
                //MessageBox.Show(gSat[i].PRN);//检验所用
            }


            for (int i = 0; i < (int)epo; i++)
            {

                for (int j = 0; j < NSatoG; j++)
                {

                    for (int k = 0; k < 50; k++)
                    {
                        if (E[i + 1].G[k].PRN == gSat[j].PRN)
                        {
                            gSat[j].C1[i] = E[i + 1].G[k].C1;
                            gSat[j].C2[i] = E[i + 1].G[k].C2;
                            gSat[j].L1[i] = E[i + 1].G[k].L1;
                            gSat[j].L2[i] = E[i + 1].G[k].L2;
                        }

                    }
                    //MessageBox.Show(Convert.ToString(gSat[j].C1[i]));//检验所用

                }

            }

            //GPS的两个频率
            double G_f1 = 1575.42e6;
            double G_f2 = 1227.60e6;
            //标准光速
            double c = 299792458;
            //计算GPS的GF和MW组合
            for (int i = 0; i < NSatoG; i++)
            {
                
                for (int j = 0; j < (int)epo; j++)
                {
                    gSat[i].GF[j] = gSat[i].L1[j]*(c/ G_f1) - gSat[i].L2[j] *( c / G_f2);
                    gSat[i].MW[j] = (gSat[i].L1[j]*c-  gSat[i].L2[j]*c)/(G_f1- G_f2)- (G_f1 * gSat[i].C1[j] + G_f2 * gSat[i].C2[j]) / (G_f1 + G_f2);
                    //MessageBox.Show(Convert.ToString(gSat[i].MW[j]));
                }
            }
            //计算周跳
            for (int i = 0; i < NSatoG; i++)
            {

                for (int j = 0; j < (int)epo; j++)
                {
                    if(
                        ((gSat[i].GF[j+1]- gSat[i].GF[j])>0.05) &&//GF历元差分阈值
                       ((gSat[i].GF[j + 1] - gSat[i].GF[j]) < 1)||
                        ((gSat[i].MW[j+1]- gSat[i].MW[j])>0.1)&&//MW历元差分阈值
                        ((gSat[i].MW[j + 1] - gSat[i].MW[j]) < 0.5)
                       )
                    {
                        gSat[i].flag[j] = 1;
                        gSat[i].c_total++;
                    }
                    //MessageBox.Show(Convert.ToString(gSat[i].MW[j]));
                }
                //MessageBox.Show(Convert.ToString(gSat[i].c_total));
            }

            
            for (int i = 0; i < 50; i++)
            {
                bSat[i] = new B_Sat(9999);

                bSat[i].PRN = " ";
                bSat[i].c_total = 0;

                for (int j = 0; j < 9999; j++)
                {
                    bSat[i].flag[j] = 0;
                    bSat[i].C2[j] = 0;
                    bSat[i].C6[j] = 0;
                    bSat[i].L2[j] = 0;
                    bSat[i].L6[j] = 0;
                    bSat[i].GF[j] = 0;
                    bSat[i].MW[j] = 0;
                }
            }

            // 为BDS的各个元素赋正确的值
            for (int i = 0; i < NSatoB; i++)
            {
                bSat[i].PRN = str_PRN[i+NSatoG];
                // MessageBox.Show(bSat[i].PRN); // 检验所用
            }
            for (int i = 0; i < (int)epo; i++)
            {
                for (int j = 0; j < NSatoB; j++)
                {
                    for (int k = 0; k < 50; k++)
                    {
                        if (E[i + 1].B[k].PRN == bSat[j].PRN)
                        {
                            bSat[j].C2[i] = E[i + 1].B[k].C2;
                            bSat[j].C6[i] = E[i + 1].B[k].C6;
                            bSat[j].L2[i] = E[i + 1].B[k].L2;
                            bSat[j].L6[i] = E[i + 1].B[k].L6;
                        }
                    }
                    // MessageBox.Show(Convert.ToString(bSat[j].C2[i])); // 检验所用
                }
            }

            //BDS的两个频率
            double B_f2 = 1561.098e6;
            double B_f6 = 1268.52e6; 
            // 计算BDS的GF和MW组合
            for (int i = 0; i < NSatoB; i++)
            {
                for (int j = 0; j < (int)epo; j++)
                {
                    bSat[i].GF[j] = bSat[i].L2[j] * (c / B_f2) - bSat[i].L6[j] * (c / B_f6);
                    bSat[i].MW[j] = (bSat[i].L2[j] * c - bSat[i].L6[j] * c) / (B_f2 - B_f6) - (B_f2 * bSat[i].C2[j] + B_f6 * bSat[i].C6[j]) / (B_f2 + B_f6);
                     //MessageBox.Show(Convert.ToString(bSat[i].GF[j]));
                }
            }

            // 计算周跳
            for (int i = 0; i < NSatoB; i++)
            {
                for (int j = 0; j < (int)epo; j++)
                {
                    if (((bSat[i].GF[j + 1] - bSat[i].GF[j]) > 0.05) && ((bSat[i].GF[j + 1] - bSat[i].GF[j]) < 1) ||
                        ((bSat[i].MW[j + 1] - bSat[i].MW[j]) > 0.1) && ((bSat[i].MW[j + 1] - bSat[i].MW[j]) < 0.5))
                    {
                        bSat[i].flag[j] = 1;
                        bSat[i].c_total++;
                    }
                   
                }
                 //MessageBox.Show(Convert.ToString(bSat[i].c_total));
            }

            MessageBox.Show("周跳探测完毕！");
        }

        //新建需要传递到子窗体的数组变量
        int[] G_cy_num = new int[50];
        int[] B_cy_num = new int[50];

        //打开另一窗口的button
        private void button5_Click(object sender, EventArgs e)
        {
            //赋初始值
            for (int i = 0; i < 50; i++)
            {
                G_cy_num[i] = 0; 
                B_cy_num[i] = 0; 
            }

            //赋所需值
            for (int i = 0; i < NSatoG; i++)
            {
                G_cy_num[i] = gSat[i].c_total;
                //MessageBox.Show(Convert.ToString(G_cy_num[i]));
            }
            for (int i = 0; i < NSatoB; i++)
            {
                B_cy_num[i] = bSat[i].c_total;
                //MessageBox.Show(Convert.ToString(B_cy_num[i]));
            }

            // 创建并打开另一个窗体（Form3）
            Form3 form3 = new Form3();
            form3.G_N = NSatoG;
            form3.B_N = NSatoB;
            form3.ep = epo;
            form3.s_PRN = str_PRN;
            form3.Gc_num = G_cy_num;
            form3.Bc_num = B_cy_num;
            form3.Show();







        }
        //保存周跳探测结果文件
        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("稍后会自动打开存放周跳探测结果的excel表格，若有需要您可以自行保存表格文件");
            MessageBox.Show("在打开excel表格后，由于数据量稍大，故您可能需要等待一会儿才能够看到数据显示");
            MessageBox.Show("经测试，大概需要1min30s的时间excel表格内的数据可以完全显示，您可以休息一会再来查看");
            MessageBox.Show("请不要在表格数据未加载完毕前关闭表格文件！否则您可能需要重新启动软件才能继续其他功能");
            // 创建 Excel 应用程序对象
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true ;
            

            // 添加新的工作簿
            Excel.Workbook workbook = excelApp.Workbooks.Add();

            // 获取第一个工作表
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // 设置表格的行数和列数
            int rowCount = (int)epo+1;
            int columnCount = NSatoB+NSatoG+1;

            // 给每一行和每一列的单元格赋值
            worksheet.Cells[1, 1] = "历元序号";
            for(int i=2;i<= columnCount;i++)
            {
                worksheet.Cells[1, i] = str_PRN[i-2];
            }

            for (int i = 2; i <= rowCount; i++)
            {
                worksheet.Cells[i,1] = i-1;
            }

            for(int i=2;i<=(NSatoG+1);i++)
            {
                for(int j=2;j<=rowCount;j++)
                {
                    if(gSat[i-2].flag[j-2]==0)
                    {
                        worksheet.Cells[j, i] = "正常";
                    }
                    else if (gSat[i - 2].flag[j - 2] == 1)
                    {
                        worksheet.Cells[j, i] = "周跳";
                    }
                }

            }
            for (int i = (NSatoG + 1+1); i <= columnCount; i++)
            {
                for (int j = 2; j <= rowCount; j++)
                {
                    if (bSat[i - 2- NSatoG].flag[j - 2] == 0)
                    {
                        worksheet.Cells[j, i] = "正常";
                    }
                    else if (bSat[i - 2 - NSatoG].flag[j - 2] == 1)
                    {
                        worksheet.Cells[j, i] = "周跳";
                    }
                }

            }




            MessageBox.Show("周跳结果文件生成成功！");

        }

        //下面开始多路径效应部分
        //设置合适的类
        public class GD
        {
            public string PRN { get; set; }
            public double[] C1 { get; set; }
            public double[] C2 { get; set; }
            public double[] C5 { get; set; }
            public double[] L1 { get; set; }
            public double[] L2 { get; set; }
            public double[] L5 { get; set; }
            public double[] TMP1 { get; set; }
            public double[] TMP2 { get; set; }
            public double[] TMP5 { get; set; }
            public double[] T_MP1 { get; set; }
            public double[] T_MP2 { get; set; }
            public double[] T_MP5 { get; set; }
            public double[] MP1 { get; set; }
            public double[] MP2 { get; set; }
            public double[] MP5 { get; set; }

            public GD()
            {
                int length = 9999;
                C1 = new double[length];
                C2 = new double[length];
                C5 = new double[length];
                L1 = new double[length];
                L2 = new double[length];
                L5 = new double[length];
                TMP1 = new double[length];
                TMP2 = new double[length];
                TMP5 = new double[length];
                T_MP1 = new double[length];
                T_MP2 = new double[length];
                T_MP5 = new double[length];
                MP1 = new double[length];
                MP2 = new double[length];
                MP5 = new double[length];
            }
        }
        GD[] gd = new GD[50];//实例化
        public class BD
        {
            public string PRN { get; set; }
            public double[] C2 { get; set; }
            public double[] C7 { get; set; }
            public double[] C6 { get; set; }
            public double[] L2 { get; set; }
            public double[] L7 { get; set; }
            public double[] L6 { get; set; }
            public double[] TMP2 { get; set; }
            public double[] TMP7 { get; set; }
            public double[] TMP6 { get; set; }
            public double[] T_MP2 { get; set; }
            public double[] T_MP7 { get; set; }
            public double[] T_MP6 { get; set; }
            public double[] MP2 { get; set; }
            public double[] MP7 { get; set; }
            public double[] MP6 { get; set; }

            public BD()
            {
                int length = 9999;
                C2 = new double[length];
                C7 = new double[length];
                C6 = new double[length];
                L2 = new double[length];
                L7 = new double[length];
                L6 = new double[length];
                TMP2 = new double[length];
                TMP7 = new double[length];
                TMP6 = new double[length];
                T_MP2 = new double[length];
                T_MP7 = new double[length];
                T_MP6 = new double[length];
                MP2 = new double[length];
                MP7 = new double[length];
                MP6 = new double[length];
            }
        }
        BD[] bd = new BD[50];//实例化
        //多路径效应计算button
        private void button7_Click(object sender, EventArgs e)
        {
            //初始化gd
            for (int i = 0; i < 50; i++)
            {
                gd[i] = new GD()
                {
                    PRN = " ",
                    C1 = new double[9999],
                    C2 = new double[9999],
                    C5 = new double[9999],
                    L1 = new double[9999],
                    L2 = new double[9999],
                    L5 = new double[9999],
                    TMP1 = new double[9999],
                    TMP2 = new double[9999],
                    TMP5 = new double[9999],
                    T_MP1 = new double[9999],
                    T_MP2 = new double[9999],
                    T_MP5 = new double[9999],
                    MP1 = new double[9999],
                    MP2 = new double[9999],
                    MP5 = new double[9999]
                };

                for (int j = 0; j < 9999; j++)
                {
                    gd[i].C1[j] = 0;
                    gd[i].C2[j] = 0;
                    gd[i].C5[j] = 0;
                    gd[i].L1[j] = 0;
                    gd[i].L2[j] = 0;
                    gd[i].L5[j] = 0;
                    gd[i].TMP1[j] = 0;
                    gd[i].TMP2[j] = 0;
                    gd[i].TMP5[j] = 0;
                    gd[i].T_MP1[j] = 0;
                    gd[i].T_MP2[j] = 0;
                    gd[i].T_MP5[j] = 0;
                    gd[i].MP1[j] = 0;
                    gd[i].MP2[j] = 0;
                    gd[i].MP5[j] = 0;
                }
            }
            //初始化bd
            for (int i = 0; i < 50; i++)
            {
                bd[i] = new BD()
                {
                    PRN = " ",
                    C2 = new double[9999],
                    C7 = new double[9999],
                    C6 = new double[9999],
                    L2 = new double[9999],
                    L7 = new double[9999],
                    L6 = new double[9999],
                    TMP2 = new double[9999],
                    TMP7 = new double[9999],
                    TMP6 = new double[9999],
                    T_MP2 = new double[9999],
                    T_MP7 = new double[9999],
                    T_MP6 = new double[9999],
                    MP2 = new double[9999],
                    MP7 = new double[9999],
                    MP6 = new double[9999]
                };

                for (int j = 0; j < 9999; j++)
                {
                    bd[i].C2[j] = 0;
                    bd[i].C7[j] = 0;
                    bd[i].C6[j] = 0;
                    bd[i].L2[j] = 0;
                    bd[i].L7[j] = 0;
                    bd[i].L6[j] = 0;
                    bd[i].TMP2[j] = 0;
                    bd[i].TMP7[j] = 0;
                    bd[i].TMP6[j] = 0;
                    bd[i].T_MP2[j] = 0;
                    bd[i].T_MP7[j] = 0;
                    bd[i].T_MP6[j] = 0;
                    bd[i].MP2[j] = 0;
                    bd[i].MP7[j] = 0;
                    bd[i].MP6[j] = 0;
                }
            }


            //赋予正确数据
            //PRN号
            for (int i = 0; i < NSatoG; i++)
            {
                gd[i].PRN = str_PRN[i];
                //MessageBox.Show(gd[i].PRN);//检验所用
            }
            for (int i = 0; i < NSatoB; i++)
            {
                bd[i].PRN = str_PRN[i+ NSatoG];
                //MessageBox.Show(bd[i].PRN);//检验所用
            }
            //观测数据
            for (int i = 0; i < (int)epo; i++)
            {

                for (int j = 0; j < NSatoG; j++)
                {

                    for (int k = 0; k < 50; k++)
                    {
                        if (E[i + 1].G[k].PRN == gd[j].PRN)
                        {
                            gd[j].C1[i] = E[i + 1].G[k].C1;
                            gd[j].C2[i] = E[i + 1].G[k].C2;
                            gd[j].C5[i] = E[i + 1].G[k].C5;
                            gd[j].L1[i] = E[i + 1].G[k].L1;
                            gd[j].L2[i] = E[i + 1].G[k].L2;
                            gd[j].L5[i] = E[i + 1].G[k].L5;
                        }

                    }
                    //MessageBox.Show(Convert.ToString(gd[j].C5[i]));//检验所用

                }

            }
            for (int i = 0; i < (int)epo; i++)
            {

                for (int j = 0; j < NSatoB; j++)
                {

                    for (int k = 0; k < 50; k++)
                    {
                        if (E[i + 1].B[k].PRN == bd[j].PRN)
                        {
                            bd[j].C2[i] = E[i + 1].B[k].C2;
                            bd[j].C7[i] = E[i + 1].B[k].C7;
                            bd[j].C6[i] = E[i + 1].B[k].C6;
                            bd[j].L2[i] = E[i + 1].B[k].L2;
                            bd[j].L7[i] = E[i + 1].B[k].L7;
                            bd[j].L6[i] = E[i + 1].B[k].L6;
                        }

                    }
                    //MessageBox.Show(Convert.ToString(bd[j].C2[i]));//检验所用

                }

            }
            //GPS的三个频率
            double G_f1 = 1575.42e6;
            double G_f2 = 1227.60e6;
            double G_f5 = 1176.45e6;
            //BDS的三个频率
            double B_f2 = 1561.098e6;
            double B_f7 = 1207.14e6;
            double B_f6 = 1268.52e6;
            //标准光速
            double c = 299792458;
            //计算多路径影响
            for (int i = 0; i < NSatoG; i++)
            {

                for (int j = 0; j < (int)epo; j++)
                {
                    if(gd[i].L1[j]!=0&& gd[i].L2[j]!=0&& gd[i].C1[j]!=0)
                    {
                        gd[i].TMP1[j] = -(c / G_f1) * gd[i].L1[j] * ((G_f1 * G_f1 + G_f2 * G_f2) / (G_f1 * G_f1 - G_f2 * G_f2)) + (c / G_f2) * gd[i].L2[j] * ((2 * G_f2 * G_f2) / (G_f1 * G_f1 - G_f2 * G_f2)) + gd[i].C1[j];
                    }
                    if (gd[i].L1[j] != 0 && gd[i].L2[j] != 0 && gd[i].C2[j] != 0)
                    {
                        gd[i].TMP2[j] = (c / G_f2) * gd[i].L2[j] * ((G_f1 * G_f1 + G_f2 * G_f2) / (G_f1 * G_f1 - G_f2 * G_f2)) - (c / G_f1) * gd[i].L1[j] * ((2 * G_f1 * G_f1) / (G_f1 * G_f1 - G_f2 * G_f2)) + gd[i].C2[j];
                    }
                    if (gd[i].L1[j] != 0 && gd[i].L2[j] != 0 && gd[i].C5[j] != 0)
                    {
                        gd[i].TMP5[j] = gd[i].C5[j] - (c / G_f1) * gd[i].L1[j] + ((G_f1 * G_f1 + G_f5 * G_f5) / (G_f1 * G_f1 - G_f2 * G_f2)) * ((G_f2 * G_f2) / (G_f5 * G_f5)) * ((c / G_f1) * gd[i].L1[j] - (c / G_f2) * gd[i].L2[j]);
                    }

                    //MessageBox.Show(Convert.ToString(gd[i].TMP1[j]));
                }
            }
            for (int i = 0; i < NSatoB; i++)
            {

                for (int j = 0; j < (int)epo; j++)
                {
                    if (bd[i].L2[j] != 0 && bd[i].L7[j] != 0 && bd[i].C2[j] != 0)
                    {
                        bd[i].TMP2[j] = -(c / B_f2) * bd[i].L2[j] * ((B_f2 * B_f2 + B_f7 * B_f7) / (B_f2 * B_f2 - B_f7 * B_f7)) + (c / B_f7) * bd[i].L7[j] * ((2 * B_f7 * B_f7) / (B_f2 * B_f2 - B_f7 * B_f7)) + bd[i].C2[j];
                    }
                    if (bd[i].L2[j] != 0 && bd[i].L7[j] != 0 && bd[i].C7[j] != 0)
                    {
                        bd[i].TMP7[j] = (c / B_f7) * bd[i].L7[j] * ((B_f2 * B_f2 + B_f7 * B_f7) / (B_f2 * B_f2 - B_f7 * B_f7)) - (c / B_f2) * bd[i].L2[j] * ((2 * B_f2 * B_f2) / (B_f2 * B_f2 - B_f7 * B_f7)) + bd[i].C7[j];
                    }
                    if (bd[i].L2[j] != 0 && bd[i].L7[j] != 0 && bd[i].C6[j] != 0)
                    {
                        bd[i].TMP6[j] = bd[i].C6[j] - (c / B_f2) * bd[i].L2[j] + ((B_f2 * B_f2 + B_f6 * B_f6) / (B_f2 * B_f2 - B_f7 * B_f7)) * ((B_f7 * B_f7) / (B_f6 * B_f6)) * ((c / B_f2) * bd[i].L2[j] - (c / B_f7) * bd[i].L7[j]);
                    }

                    //MessageBox.Show(Convert.ToString(bd[i].TMP6[j]));
                }
            }
            //计算多路径均值
            for (int i = 0; i < NSatoG; i++)
            {
                //求和器
                double[] Gsum1 = new double[(int)epo];
                double[] Gsum2 = new double[(int)epo];
                double[] Gsum5 = new double[(int)epo];
                //计数器
                int[] Gnu1 = new int[(int)epo];
                int[] Gnu2 = new int[(int)epo];
                int[] Gnu5 = new int[(int)epo];
                //初值置零
                Array.Clear(Gsum1, 0, Gsum1.Length);
                Array.Clear(Gsum2, 0, Gsum2.Length);
                Array.Clear(Gsum5, 0, Gsum5.Length);
                Array.Clear(Gnu1, 0, Gnu1.Length);
                Array.Clear(Gnu2, 0, Gnu2.Length);
                Array.Clear(Gnu5, 0, Gnu5.Length);

                for (int j = 0; j < (int)epo; j++)
                {
                   if(j<100)
                    {
                        for(int k=j;k<(50+j);k++)
                        {
                            if(gd[i].TMP1[k]!=0)
                            {
                                Gsum1[j]= Gsum1[j]+ gd[i].TMP1[k];
                                Gnu1[j]++;
                            }
                            if (gd[i].TMP2[k] != 0)
                            {
                                Gsum2[j] = Gsum2[j] + gd[i].TMP2[k];
                                Gnu2[j]++;
                            }
                            if (gd[i].TMP5[k] != 0)
                            {
                                Gsum5[j] = Gsum5[j] + gd[i].TMP5[k];
                                Gnu5[j]++;
                            }

                        }
                    }
                   if (j >= 100&&j<= ((int)epo-100))
                    {
                        for (int k = (j-25); k < (25 + j); k++)
                        {
                            if (gd[i].TMP1[k] != 0)
                            {
                                Gsum1[j] = Gsum1[j] + gd[i].TMP1[k];
                                Gnu1[j]++;
                            }
                            if (gd[i].TMP2[k] != 0)
                            {
                                Gsum2[j] = Gsum2[j] + gd[i].TMP2[k];
                                Gnu2[j]++;
                            }
                            if (gd[i].TMP5[k] != 0)
                            {
                                Gsum5[j] = Gsum5[j] + gd[i].TMP5[k];
                                Gnu5[j]++;
                            }
                        }
                    }
                   if (j >((int)epo - 100))
                    {
                        for (int k = j - 50; k < j; k++)
                        {
                            if (gd[i].TMP1[k] != 0)
                            {
                                Gsum1[j] = Gsum1[j] + gd[i].TMP1[k];
                                Gnu1[j]++;
                            }
                            if (gd[i].TMP2[k] != 0)
                            {
                                Gsum2[j] = Gsum2[j] + gd[i].TMP2[k];
                                Gnu2[j]++;
                            }
                            if (gd[i].TMP5[k] != 0)
                            {
                                Gsum5[j] = Gsum5[j] + gd[i].TMP5[k];
                                Gnu5[j]++;
                            }
                        }
                    }
                   if(gd[i].TMP1[j]!=0)
                    {
                        gd[i].T_MP1[j] = Gsum1[j] / Gnu1[j];
                    }
                    if (gd[i].TMP2[j] != 0)
                    {
                        gd[i].T_MP2[j] = Gsum2[j] / Gnu2[j];
                    }
                    if (gd[i].TMP5[j] != 0)
                    {
                        gd[i].T_MP5[j] = Gsum5[j] / Gnu5[j];
                    }
                    
                    //MessageBox.Show(Convert.ToString(gd[i].T_MP1[j]));

                }











            }
            for (int i = 0; i < NSatoB; i++)
            {
                //求和器
                double[] Gsum1 = new double[(int)epo];
                double[] Gsum2 = new double[(int)epo];
                double[] Gsum5 = new double[(int)epo];
                //计数器
                int[] Gnu1 = new int[(int)epo];
                int[] Gnu2 = new int[(int)epo];
                int[] Gnu5 = new int[(int)epo];
                //初值置零
                Array.Clear(Gsum1, 0, Gsum1.Length);
                Array.Clear(Gsum2, 0, Gsum2.Length);
                Array.Clear(Gsum5, 0, Gsum5.Length);
                Array.Clear(Gnu1, 0, Gnu1.Length);
                Array.Clear(Gnu2, 0, Gnu2.Length);
                Array.Clear(Gnu5, 0, Gnu5.Length);

                for (int j = 0; j < (int)epo; j++)
                {
                    if (j < 100)
                    {
                        for (int k = j; k < (50 + j); k++)
                        {
                            if (bd[i].TMP2[k] != 0)
                            {
                                Gsum1[j] = Gsum1[j] + bd[i].TMP2[k];
                                Gnu1[j]++;
                            }
                            if (bd[i].TMP7[k] != 0)
                            {
                                Gsum2[j] = Gsum2[j] + bd[i].TMP7[k];
                                Gnu2[j]++;
                            }
                            if (bd[i].TMP6[k] != 0)
                            {
                                Gsum5[j] = Gsum5[j] + bd[i].TMP6[k];
                                Gnu5[j]++;
                            }

                        }
                    }
                    if (j >= 100 && j <= ((int)epo - 100))
                    {
                        for (int k = (j - 25); k < (25 + j); k++)
                        {
                            if (bd[i].TMP2[k] != 0)
                            {
                                Gsum1[j] = Gsum1[j] + bd[i].TMP2[k];
                                Gnu1[j]++;
                            }
                            if (bd[i].TMP7[k] != 0)
                            {
                                Gsum2[j] = Gsum2[j] + bd[i].TMP7[k];
                                Gnu2[j]++;
                            }
                            if (bd[i].TMP6[k] != 0)
                            {
                                Gsum5[j] = Gsum5[j] + bd[i].TMP6[k];
                                Gnu5[j]++;
                            }
                        }
                    }
                    if (j > ((int)epo - 100))
                    {
                        for (int k = j - 50; k < j; k++)
                        {
                            if (bd[i].TMP2[k] != 0)
                            {
                                Gsum1[j] = Gsum1[j] + bd[i].TMP2[k];
                                Gnu1[j]++;
                            }
                            if (bd[i].TMP7[k] != 0)
                            {
                                Gsum2[j] = Gsum2[j] + bd[i].TMP7[k];
                                Gnu2[j]++;
                            }
                            if (bd[i].TMP6[k] != 0)
                            {
                                Gsum5[j] = Gsum5[j] + bd[i].TMP6[k];
                                Gnu5[j]++;
                            }
                        }
                    }
                    if (bd[i].TMP2[j] != 0)
                    {
                        bd[i].T_MP2[j] = Gsum1[j] / Gnu1[j];
                    }
                    if (bd[i].TMP7[j] != 0)
                    {
                        bd[i].T_MP7[j] = Gsum2[j] / Gnu2[j];
                    }
                    if (bd[i].TMP6[j] != 0)
                    {
                        bd[i].T_MP6[j] = Gsum5[j] / Gnu5[j];
                    }

                    //MessageBox.Show(Convert.ToString(bd[i].T_MP2[j]));

                }



            }
            //计算多路径误差
            for (int i = 0; i < NSatoG; i++)
            {
                for (int j = 0; j < (int)epo; j++)
                {
                    gd[i].MP1[j] = gd[i].TMP1[j] - gd[i].T_MP1[j];
                    gd[i].MP2[j] = gd[i].TMP2[j] - gd[i].T_MP2[j];
                    gd[i].MP5[j] = gd[i].TMP5[j] - gd[i].T_MP5[j];
                    //MessageBox.Show(Convert.ToString(gd[i].MP1[j]));

                }

            }
            for (int i = 0; i < NSatoB; i++)
            {
                for (int j = 0; j < (int)epo; j++)
                {
                    bd[i].MP2[j] = bd[i].TMP2[j] - bd[i].T_MP2[j];
                    bd[i].MP7[j] = bd[i].TMP7[j] - bd[i].T_MP7[j];
                    bd[i].MP6[j] = bd[i].TMP6[j] - bd[i].T_MP6[j];
                    //MessageBox.Show(Convert.ToString(bd[i].MP2[j]));

                }

            }

            MessageBox.Show("多路径效应计算完毕！");
        }

        //多路径效应的导出
        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("稍后会自动打开存放多路径误差计算结果的excel表格，若有需要您可以自行保存表格文件");
            MessageBox.Show("在打开excel表格后，由于数据量稍大，故您可能需要等待一会儿才能够看到数据显示");
            MessageBox.Show("经测试，大概需要3min25s的时间excel表格内的数据才可以完全显示，您可以休息一会再来查看");
            MessageBox.Show("tips：表格中的0并不是指多路径误差为0，而是因为缺少必要的观测数据而无法计算的量");
            MessageBox.Show("请不要在表格数据未加载完毕前关闭表格文件！否则您可能需要重新启动软件才能继续其他功能");
            // 创建 Excel 应用程序对象
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true;


            // 添加新的工作簿
            Excel.Workbook workbook = excelApp.Workbooks.Add();

            // 获取第一个工作表
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // 设置表格的行数和列数
            int rowCount = (int)epo + 1;
            int columnCount = (NSatoG*3+NSatoB*3) + 1;

            // 给每一行和每一列的单元格赋值
            worksheet.Cells[1, 1] = "历元序号";
            for (int i = 2; i <= NSatoG * 3 + 1; i=i+3)
            {
                worksheet.Cells[1, i] = (str_PRN[((i - 2)/3)]+"MP1");
                worksheet.Cells[1, i+1] = (str_PRN[((i - 2) / 3)] + "MP2");
                worksheet.Cells[1, i+2] = (str_PRN[((i - 2) / 3)] + "MP5");
            }
            for (int i = NSatoG * 3 + 2; i <=columnCount; i = i + 3)
            {
                worksheet.Cells[1, i] = (str_PRN[((i - 2) / 3)] + "MP2");
                worksheet.Cells[1, i + 1] = (str_PRN[((i - 2) / 3)] + "MP7");
                worksheet.Cells[1, i + 2] = (str_PRN[((i - 2) / 3)] + "MP6");
            }


            for (int i = 2; i <= rowCount; i++)
            {
                worksheet.Cells[i, 1] = i - 1;
            }

            for (int i = 2; i <= (3*NSatoG + 1); i=i+3)
            {
                for (int j = 2; j <= rowCount; j++)
                {
                   worksheet.Cells[j, i] = gd[((i-2)/3)].MP1[j-2];
                   worksheet.Cells[j, i+1] = gd[((i - 2) / 3)].MP2[j-2];
                   worksheet.Cells[j, i+2] = gd[((i - 2) / 3)].MP5[j-2];

                }

            }
            for (int i = NSatoG * 3 + 2; i <= columnCount; i = i + 3)
            {
                for (int j = 2; j <= rowCount; j++)
                {
                    worksheet.Cells[j, i] = bd[((i - 2) / 3)- NSatoG].MP2[j - 2];
                    worksheet.Cells[j, i + 1] = bd[((i - 2) / 3) - NSatoG].MP7[j - 2];
                    worksheet.Cells[j, i + 2] = bd[((i - 2) / 3) - NSatoG].MP6[j - 2];

                }
            }




            MessageBox.Show("多路径误差结果文件生成成功！");

        }

        //多路径效应的绘图
        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string selectedOption = comboBox1.SelectedItem.ToString();

            // 清空时序图数据
            chart1.Series.Clear();
            // 取消显示主要水平格网线
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            // 取消显示主要垂直格网线
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            // 取消显示次要水平格网线
            chart1.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            // 取消显示次要垂直格网线
            chart1.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
           
            // 根据选择的选项来显示相应的时序图
            if (selectedOption == "GPS/L1")
            {
                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoG; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(gd[i].PRN+ "MP1");
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoG - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY((j+1), gd[i].MP1[j]);

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart1.Series.Add(series);
                }

            }

            else if (selectedOption == "GPS/L2")
            {
                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoG; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(gd[i].PRN + "MP2");
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoG - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY((j + 1), gd[i].MP2[j]);

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart1.Series.Add(series);
                }

            }

            else if (selectedOption == "GPS/L5")
            {
                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoG; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(gd[i].PRN + "MP5");
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoG - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoG - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY((j + 1), gd[i].MP5[j]);

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart1.Series.Add(series);
                }

            }


            else if (selectedOption == "BDS/B1")
            {

                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoB; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(bd[i].PRN+"MP2");
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoB - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY(j+1, bd[i].MP2[j]);

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart1.Series.Add(series);
                }

            }

            else if (selectedOption == "BDS/B2")
            {

                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoB; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(bd[i].PRN + "MP7");
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoB - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY(j + 1, bd[i].MP7[j]);

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart1.Series.Add(series);
                }

            }

            else if (selectedOption == "BDS/B3")
            {

                // 创建一个集合来存储 Series 对象
                List<Series> seriesList = new List<Series>();

                // 循环创建多个 Series 对象
                for (int i = 0; i < NSatoB; i++)
                {
                    // 创建新的 Series 对象，并设置属性
                    Series series = new Series(bd[i].PRN + "MP6");
                    series.ChartType = SeriesChartType.Spline;
                    series.Color = Color.FromArgb((int)((i * 255) / (NSatoB - 1)), (int)((Math.Sin((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5), (int)((Math.Cos((i * Math.PI) / (NSatoB - 1)) + 1) * 127.5));
                    for (int j = 0; j < epo; j++)
                    {
                        series.Points.AddXY(j + 1, bd[i].MP6[j]);

                    }
                    // 添加 Series 对象到集合中
                    seriesList.Add(series);
                }

                // 将 Series 对象逐个添加到图表控件的 Series 集合中
                foreach (Series series in seriesList)
                {
                    chart1.Series.Add(series);
                }

            }







        }
    }
}




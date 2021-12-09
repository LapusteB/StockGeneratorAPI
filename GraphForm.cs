using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YahooFinanceApi;
using System.Windows.Forms.DataVisualization.Charting;

/*
 patterns done: Dragonfly Doji, marubuzo (BEARISH), marubuzo (BULLISH), neutral, Long Legged
 */
/// <summary>
/// This is the Graph form. This form can be instantiated infinitely many times based on any graph
/// module you chooose. this form directly uses members of the MainForm class to instantiate the start
/// date, end date, Tick selection, Company name Period of time: (Weeks, Months, Days). In this form
/// we create the graph dynamically with the Yahoo API data that is generated.
/// </summary>
namespace Project3_McLaughlin_Lapuste
{/// <summary>
/// This is class definition of the Graph Form
/// </summary>
    public partial class GraphForm : Form
    {
      
        DateTime start = MainForm.start;//start date
        DateTime end = MainForm.end;//End date from picker
        string tik = MainForm.tik;//tick selection
        string companyName;//company name/stock that is selected for the title ofthe graph
        YahooFinanceApi.Period p = MainForm.period;//Yahoo API object
        RectangleAnnotation rec = new RectangleAnnotation();//rec object for annotating the graph with rectangles
       
     /// <summary>
     /// Object constructor of the Graph Form That initalizes the
     /// combo box for selecting the pattern you want to view on the graph
     /// </summary>
        public GraphForm()
        {//Here the numbers represent the indencies of the combobox being selected
            InitializeComponent();
            comboBox1.Items.Add("Neutral (Spinning Top)");//0
            comboBox1.Items.Add("Long-Legged");//1
            comboBox1.Items.Add("gravestone");//2
            comboBox1.Items.Add("Dragonfly DOJI");//3
            comboBox1.Items.Add("Marubozu (BULLISH)");//4
            comboBox1.Items.Add("Marubozi (BEARISH)");//5
          
           
            
            chart1.Series.Clear();//clears the chart before hand for unlimited chart loadings
            chart1.Series.Add(tik);//adding the series name to the chart
            chart1.Series[tik].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;//make the chart type candlestick
            chart1.Name = companyName;
            // Set the style of the open-close marks
            chart1.Series[tik]["OpenCloseStyle"] = "Triangle";

            // Show both open and close marks
            chart1.Series[tik]["ShowOpenClose"] = "Both";

            // Set point width
            chart1.Series[tik]["PointWidth"] = "1.0";

            // Set colors bars
            chart1.Series[tik]["PriceUpColor"] = "Green"; // <<== use text indexer for series
            chart1.Series[tik]["PriceDownColor"] = "Red"; // <<== use text indexer for series

            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;//making the graph zoomable
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.MouseWheel += chart1_MouseWheel;//mouse wheel for graph




            rec.LineColor = Color.Blue;//set this annotation to blue
            rec.BackColor = Color.Transparent;//set color transparent
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = false;//adjust the graph


            

            var awaiter = getStockData(100, tik, start, end, p);//init the 
        }
        /// <summary>
        /// Load the Graph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphForm_Load(object sender, EventArgs e)
        {
            



        }
        /// <summary>
        /// This function is Asyncronous It takes in
        /// the selection for the combo box, symbol, start date
        /// end date for the graph, and a yahoo period object
        /// </summary>
        /// <param name="sel"></param>
        /// <param name="symbol"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task<int> getStockData(int sel, string symbol, DateTime startDate, DateTime endDate, YahooFinanceApi.Period period)
        {
           
            
            
            try
            {
               //Try block for getting the API data. Could fail if service is bad
                var historic_data = await Yahoo.GetHistoricalAsync(symbol, startDate, endDate, period);//create a variable for the data
                var security = await Yahoo.Symbols(symbol).Fields(Field.LongName).QueryAsync();
                var ticker = security[symbol];//ticker
                companyName = ticker[Field.LongName];
                Label2.Text = companyName;
               
                //loop through each data value
                for (int i = 0; i < historic_data.Count; i++)
                {
                    if (sel == 100)
                    {
                        //add each point to the x and y value of the graph
                        chart1.Series[tik].Points.AddXY(historic_data[i].DateTime.Month + "/" +
                            historic_data[i].DateTime.Day + "/" + historic_data[i].DateTime.Year, historic_data[i].High);
                        //Add the Y values of the candlestick
                        chart1.Series[tik].Points[i].YValues[1] = (double)historic_data[i].Low;
                        chart1.Series[tik].Points[i].YValues[2] = (double)historic_data[i].Open;
                        chart1.Series[tik].Points[i].YValues[3] = (double)historic_data[i].Close;
                    }
                    

                    decimal HightoLow = historic_data[i].High - historic_data[i].Low;
                    //Dividing the data 
                    decimal TopTHIRD = historic_data[i].High - HightoLow * (decimal)0.40;
                    decimal bottomTHIRD = historic_data[i].High - HightoLow * (decimal)0.60;
                    //splitting up data into thirds
                    decimal dragonfly = historic_data[i].High - HightoLow * (decimal)0.2;
                    decimal gravestone = historic_data[i].High - HightoLow * (decimal)0.80;
                    //getting values for dragonfly and gravestone
                    decimal neutralHigh = historic_data[i].High - HightoLow * (decimal)0.25;
                    decimal neutralLow = historic_data[i].High - HightoLow * (decimal)0.75;

                    //neutral
                    if (((historic_data[i].Open <= neutralHigh) && (historic_data[i].Close >= neutralLow) &&
                       (historic_data[i].Close <= neutralHigh) && (historic_data[i].Open >= neutralLow)) && sel == 0)
                    {
                       
                        RectangleAnnotation r = new RectangleAnnotation();
                        r.LineColor = Color.Blue;
                        r.BackColor = Color.Transparent;
                        r.Width = 5;
                        r.Width = 10;
                        r.Height = -10;
                        r.SetAnchor(chart1.Series[tik].Points[i]);
                        chart1.Annotations.Add(r);
                    }
                    

                  //long legged
                    if (((historic_data[i].Open <= TopTHIRD && historic_data[i].Close >= bottomTHIRD) &&
                       (historic_data[i].Close <= TopTHIRD && historic_data[i].Open >= bottomTHIRD)) && sel == 1)
                    {
                       
                        RectangleAnnotation r = new RectangleAnnotation();
                        r.LineColor = Color.Purple;
                        r.BackColor = Color.Transparent;
                        r.Width = 1;
                        r.Width = 10;
                        r.Height = -5;
                        r.SetAnchor(chart1.Series[tik].Points[i]);
                        chart1.Annotations.Add(r);
                    }

                    //Dragonfly Doji
                    if ((historic_data[i].Open >= dragonfly && historic_data[i].Close >= dragonfly) && sel == 3)
                    {

                        RectangleAnnotation r = new RectangleAnnotation();
                        r.LineColor = Color.Black;
                        r.BackColor = Color.Transparent;
                        r.Width = 1;
                        r.Width = 10;
                        r.Height = -5;
                        r.SetAnchor(chart1.Series[tik].Points[i]);
                        chart1.Annotations.Add(r);
                    }
                    //MORIBOZU BULLISH
                    if ((historic_data[i].High == historic_data[i].Close) && sel == 4)
                    {

                        RectangleAnnotation r = new RectangleAnnotation();
                        r.LineColor = Color.Green;
                        r.BackColor = Color.Transparent;
                        r.Width = 1;
                        r.Width = 10;
                        r.Height = -5;
                        r.SetAnchor(chart1.Series[tik].Points[i]);
                        chart1.Annotations.Add(r);
                    }

                    //MORIBOZU BEARISH
                    if ((historic_data[i].High == historic_data[i].Open) && sel == 5)
                    {

                        RectangleAnnotation r = new RectangleAnnotation();
                        r.LineColor = Color.Red;
                        r.BackColor = Color.Transparent;
                        r.Width = 1;
                        r.Width = 10;
                        r.Height = -5;
                        r.SetAnchor(chart1.Series[tik].Points[i]);
                        chart1.Annotations.Add(r);
                    }

                    //Gravestone
                    if ((historic_data[i].Open <= gravestone && historic_data[i].Close <= gravestone) && sel == 2)
                    {

                        RectangleAnnotation r = new RectangleAnnotation();
                        r.LineColor = Color.Yellow;
                        r.BackColor = Color.Transparent;
                        r.Width = 1;
                        r.Width = 10;
                        r.Height = -5;
                        r.SetAnchor(chart1.Series[tik].Points[i]);
                        chart1.Annotations.Add(r);
                    }
                  
                    Update();             
                }
                
            }
            catch
            {//Catch the error if it is thrown
                Console.WriteLine("Failed to get symbol: " + symbol);
            }
            return 1;
        }
        
        
        
        private void chart1_Click(object sender, EventArgs e)
        {
          
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

       
        /// <summary>
        /// This is the combo box selection Here you can select the pattern
        /// that shows the candlesticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sel;
            if (comboBox1.SelectedIndex == 0)
            {
                sel = 0;
                var awaiter = getStockData(sel, tik, start, end, p);
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                sel = 1;
                var awaiter = getStockData(sel, tik, start, end, p);
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                sel = 2;
                var awaiter = getStockData(sel, tik, start, end, p);
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                sel = 3;
                var awaiter = getStockData(sel, tik, start, end, p);
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                sel = 4;
                var awaiter = getStockData(sel, tik, start, end, p);
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                sel = 5;
                var awaiter = getStockData(sel, tik, start, end, p);
            }
           

        }





        /// <summary>
        /// This function allows for zooming into the graph
        /// for a better view of the candlestick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;
                    //changing the zooming strength
                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 2;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 2;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }
    }
}

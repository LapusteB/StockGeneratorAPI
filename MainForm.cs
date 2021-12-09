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


///Brian Lapuste U06279359
///Luke McLaughlin U20271491


/// <summary>
/// Main Namespace of the Project
/// </summary>
namespace Project3_McLaughlin_Lapuste
{/// <summary>
/// The main form that pops up
/// It allows the user to pick a start date, end date, stock tick and time period.
/// There is a go button that will indefinetely generate as many
/// graph forms as needed.
/// </summary>
    public partial class MainForm : Form
    {

        public static DateTime start;//start time
        public static DateTime end;//end time
        public static string tik;//stock type
        public static YahooFinanceApi.Period period;//period days, weeks, months
        
        /// <summary>
        /// Object that init's the stock ticks
        /// Start, end time and period. Go button
        /// </summary>
        public MainForm()
        {   //Init the combo box ticker
            InitializeComponent();
            Ticker.Items.Add("AFL");
            Ticker.Items.Add("MSFT");
            Ticker.Items.Add("ACN");
            Ticker.Items.Add("GOOGL");
            Ticker.Items.Add("AMZN");
            Ticker.Items.Add("AXP");
            Ticker.Items.Add("AAPL");
            Ticker.Items.Add("T");
            Ticker.Items.Add("BA");
            Ticker.Items.Add("CAT");
            Ticker.Items.Add("IBM");
            Ticker.Items.Add("RTX");
            Period.Items.Add(YahooFinanceApi.Period.Daily);
            Period.Items.Add(YahooFinanceApi.Period.Weekly);
            Period.Items.Add(YahooFinanceApi.Period.Monthly);
        }

       /// <summary>
       /// The go click button that starts the daily, weekly monthly and period
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void Go_Click(object sender, EventArgs e)
        {
            if (tik == null)
                tik = "AFL";

            //start = dateTimePicker1.Value;
            //end = dateTimePicker2.Value;
            //tik = Ticker.Text;
            if (Period.Text == "Daily")
                period = YahooFinanceApi.Period.Daily;
            else if (Period.Text == "Weekly")
                period = YahooFinanceApi.Period.Weekly;
            else if (Period.Text == "Monthly")
                period = YahooFinanceApi.Period.Monthly;
            else 
                period = YahooFinanceApi.Period.Daily;

            start = dateTimePicker1.Value;
            end = dateTimePicker2.Value;

            GraphForm GraphForm1 = new GraphForm();
            GraphForm1.Show();

        }
        /// <summary>
        /// Checks if value was changed for infinite form change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            start = dateTimePicker1.Value;
        }
        /// <summary>
        /// Checks if value was changed for infinite form change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            end = dateTimePicker2.Value;
        }
        /// <summary>
        /// Checks if value was changed for infinite form change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ticker_SelectedIndexChanged(object sender, EventArgs e)
        {
            tik = Ticker.Text;
        }
    }
}

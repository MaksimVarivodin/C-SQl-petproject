using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAB9
{
    public partial class Form1 : Form
    {
        private string[] select = {
        $"select * from Firma", 
        $"select * from Model", 
        $"select * from Cars",
        $"select * from SearchView"
        };
        private int[] keyCols =  {
        0, 0, -1 
        };
        private int[] keyColsEdit =  {
        0, 0, 0
        };
        private string[] Tables = {
        "Firma",
        "Model",
        "Cars",
        "SearchView"
        };
        private string[] ViewCols;
        // строка подключения
        private string DataSourcePathDB = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\ПК\\OneDrive - Нацiональний технiчний унiверситет Харкiвський полiтехнiчний iнститут\\3 курс\\Проектування БД та ІС\\Лаб9\\BDL9.mdf\";Integrated Security=True;Connect Timeout=30";
        // подключение
        private SqlConnection con = null;
        private SqlCommand bufferCmnd = null;
        private SqlDataReader bufferRdr = null;
        DataSet dtst = null;    
        public Form1()
        {
            InitializeComponent();
            con = new SqlConnection(DataSourcePathDB);// инициализация соединения
            foreach (string tab in Tables) {
                selTable.Items.Add(tab);
            }
            selTable.SelectedIndex= 0;
            Switcher(selTable.SelectedIndex);
            InitSearch();
            getFirmas();
            getModels();
            getViewCols();
        }
        private void Switcher(int index) { 
            con.Open();
            // 
            bufferCmnd = new SqlCommand(select[index], con);
            bufferRdr = bufferCmnd.ExecuteReader();

            dtst = new DataSet();
            dtst.Tables.Add(Tables[index]);
            dtst.Tables[0].Load(bufferRdr);
            preView.DataSource= dtst.Tables[0];

            bufferRdr.Close();
            con.Close();
        }
        private void InitSearch() {
            con.Open();
            // 
            bufferCmnd = new SqlCommand(select[3], con);
            bufferRdr = bufferCmnd.ExecuteReader();

            dtst = new DataSet();
            dtst.Tables.Add(Tables[3]);
            dtst.Tables[0].Load(bufferRdr);
            searchPreview.DataSource = dtst.Tables[0];

            bufferRdr.Close();
            con.Close();
        }
        private void changeTable(object sender, EventArgs e)
        {
            if (selTable.SelectedIndex != 3)
            {
                menu.Enabled = true;
            }
            else { 
                menu.Enabled = false;
            }
            Switcher(selTable.SelectedIndex);
        }

        private void openAdd(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2(Tables[selTable.SelectedIndex], select[selTable.SelectedIndex], keyCols[selTable.SelectedIndex]);

            fm2.Show();

            Switcher(selTable.SelectedIndex);
        }

        private void redact_Click(object sender, EventArgs e)
        {
            Form3 fm3 = new Form3(Tables[selTable.SelectedIndex], select[selTable.SelectedIndex], keyColsEdit[selTable.SelectedIndex]);
            fm3.Show();
            Switcher(selTable.SelectedIndex);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            Form4 fm4 = new Form4( Tables[selTable.SelectedIndex], select[selTable.SelectedIndex], keyColsEdit[selTable.SelectedIndex]);
            fm4.Show();
            Switcher(selTable.SelectedIndex);
        }
        private void getFirmas() {
            con.Open();
            // 
            bufferCmnd = new SqlCommand(select[0], con);
            bufferRdr = bufferCmnd.ExecuteReader();

            dtst = new DataSet();
            dtst.Tables.Add(Tables[0]);
            dtst.Tables[0].Load(bufferRdr);
            DataTable tab = dtst.Tables[0];
            foreach(DataRow row in tab.Rows) {
                comboBox1.Items.Add(row[1].ToString());
            }
            comboBox1.SelectedItem = -1;
            bufferRdr.Close();
            con.Close();

        }
        private void getModels()
        {
            con.Open();
            // 
            bufferCmnd = new SqlCommand(select[1], con);
            bufferRdr = bufferCmnd.ExecuteReader();

            dtst = new DataSet();
            dtst.Tables.Add(Tables[0]);
            dtst.Tables[0].Load(bufferRdr);
            DataTable tab = dtst.Tables[0];
            foreach (DataRow row in tab.Rows)
            {
                comboBox2.Items.Add(row[1].ToString());
            }
            comboBox2.SelectedItem = -1;
            bufferRdr.Close();
            con.Close();

        }
        private void getViewCols()
        {
            con.Open();
            // 
            bufferCmnd = new SqlCommand(select[3], con);
            bufferRdr = bufferCmnd.ExecuteReader();

            dtst = new DataSet();
            dtst.Tables.Add(Tables[3]);

            dtst.Tables[0].Load(bufferRdr);
            DataTable dt = dtst.Tables[0];
            ViewCols = new string[dt.Columns.Count];
            int i = 0;
            string[] tableNames = { "Firma.", "Model.", "Model.", "Model.", "Cars.", "Cars.", "Cars." };
            foreach (DataColumn column in dt.Columns)
            {
                ViewCols[i] = /*tableNames[i] +*/ column.ColumnName.ToString();
                i++;                
            }
            
            bufferRdr.Close();
            con.Close();
        }
        private void find(object sender, EventArgs e)
        {
            bool[] arr = { comboBox1.SelectedIndex != -1 ,
                           comboBox2.SelectedIndex != -1,
                           textBox2.Text.Length > 0,
                           textBox3.Text.Length > 0,
                           textBox1.Text.Length > 0,
                           (radioButton1.Checked || radioButton2.Checked),
                           textBox4.Text.Length > 0};
            string[] items = {" = N'" + comboBox1.SelectedItem + "'",
                " = N'" + comboBox2.SelectedItem + "'",
                " = " + textBox2.Text,
                " = " + textBox3.Text,
                " = N'" + textBox1.Text + "'",
                " = N'" + (radioButton1.Checked ? "benzin" : "dizel") + "'",
                " = N'" + textBox4.Text + "'"
            };
            bool AnythingSelected = false;
            int size = 0;
            foreach (bool o in arr)
            {
                if (o) {
                    AnythingSelected = true;
                    size++; 
                }
            }
            if (AnythingSelected)
            {

                string s = "select * from SearchView where (";
                
                
                int i = 0;
                for (int iter = 0; iter < items.Length; iter++)
                {
                    if (arr[iter])
                    {
                        s += ViewCols[iter] + items[iter];
                        if (i < size - 1)
                            s += " and ";
                        i++;
                    }
                }
                
                s += " )";
                richTextBox1.Text = s;
                con.Open();
                // 
                bufferCmnd = new SqlCommand(s, con);
                bufferRdr = bufferCmnd.ExecuteReader();

                dtst = new DataSet();
                dtst.Tables.Add(Tables[3]);
                dtst.Tables[0].Load(bufferRdr);
                searchPreview.DataSource = dtst.Tables[0];

                bufferRdr.Close();
                con.Close();

                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                textBox4.Text = "";

            }
            else {
                InitSearch();
                richTextBox1.Text = "";
            }
        }
    }
}

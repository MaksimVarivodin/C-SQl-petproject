using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LAB9
{
    public partial class Form2 : Form
    {
        public string table;
        public string comnd;
        private string[] Columns;
        private string[] values;
        private int KeyCol;
        // строка подключения
        private string DataSourcePathDB = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\ПК\\OneDrive - Нацiональний технiчний унiверситет Харкiвський полiтехнiчний iнститут\\3 курс\\Проектування БД та ІС\\Лаб9\\BDL9.mdf\";Integrated Security=True;Connect Timeout=30";
        private string insertString = null;
        // подключение
        private SqlConnection con = null;
        private SqlCommand bufferCmnd = null;
        private SqlDataReader bufferRdr = null;
        DataSet dtst = null;
        public Form2(string table, string comnd, int col)
        {            
            InitializeComponent();
            this.table = table;
            this.comnd = comnd;
            con = new SqlConnection(DataSourcePathDB);// инициализация соединения
            KeyCol = col;
            Switcher();
            foreach (string tab in Columns)
            {
                comboBox1.Items.Add(tab);
            }
            comboBox1.SelectedIndex = 0;
        }
        private void Switcher()
        {
            con.Open();
            // 
            bufferCmnd = new SqlCommand(comnd, con);
            bufferRdr = bufferCmnd.ExecuteReader();

            dtst = new DataSet();
            dtst.Tables.Add(table);
            if (Columns != null)
                Array.Clear(Columns, 0, Columns.Length);

            dtst.Tables[0].Load(bufferRdr);

            DataTable dt = dtst.Tables[0];
            Columns = new string[KeyCol>-1? dt.Columns.Count -1: dt.Columns.Count];
            values = new string[KeyCol > -1 ? dt.Columns.Count - 1 : dt.Columns.Count];


            int i = 0;
            int iter = 0;
            foreach (DataColumn column in dt.Columns)
            {
                if (iter == KeyCol) {
                    iter++;
                    continue; 
                }
                Columns[i] =  column.ColumnName.ToString();
                i++;
                iter++;
            }            
            
            addPreView.DataSource = dtst.Tables[0];
            


            bufferRdr.Close();
            con.Close();
        }
        private void FormInsert() {

            insertString = "insert into "+ table+ " (";
            for (int i = 0; i < Columns.Length; i++) {

                insertString+= Columns[i];
                insertString += i < Columns.Length - 1 ? ", " : "";
            }
            insertString += ") values (";
            for (int i = 0; i < Columns.Length; i++) {
                string val = "";
                if (values[i] != null)
                {
                    if (addPreView.Rows[0].Cells[KeyCol!= -1? i+ 1: i].Value.GetType().Name == "String")
                    {
                        val += values[i];
                        val = "N'" + val + "'";

                    }
                    else
                    {
                        val += values[i];
                    }
                }
                else
                    val = "NULL";
                

                insertString += val;
                insertString += i < Columns.Length - 1 ? ", " : "";
            }
            insertString += ");";

        
        }
        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            bufferCmnd = new SqlCommand(insertString, con);
            int k = bufferCmnd.ExecuteNonQuery();
            con.Close();
            Switcher();
            
        }

        private void Enter_Click(object sender, EventArgs e)
        {
            values[comboBox1.SelectedIndex] = textBox1.Text;
            FormInsert();
            richTextBox1.Text = insertString;
        }
    }
}

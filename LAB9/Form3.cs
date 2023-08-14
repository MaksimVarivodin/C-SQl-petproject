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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LAB9
{
    public partial class Form3 : Form
    {
        public string table;
        public string comnd;
        private string[] Columns;
        private int KeyCol;
        // строка подключения
        private string DataSourcePathDB = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\ПК\\OneDrive - Нацiональний технiчний унiверситет Харкiвський полiтехнiчний iнститут\\3 курс\\Проектування БД та ІС\\Лаб9\\BDL9.mdf\";Integrated Security=True;Connect Timeout=30";
        private string updateString = null;
        // подключение
        private SqlConnection con = null;
        private SqlCommand bufferCmnd = null;
        private SqlDataReader bufferRdr = null;
        private Type tp= null;
        private int updatedRow = -1;
        private int updatedCol = -1;

        DataSet dtst = null;
        public Form3(string table, string comnd, int col)
        {
            InitializeComponent();
            this.table = table;
            this.comnd = comnd;
            con = new SqlConnection(DataSourcePathDB);// инициализация соединения
            KeyCol = col;
            Switcher();

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
            Columns = new string[dt.Columns.Count];


            int i = 0;
            foreach (DataColumn column in dt.Columns)
            {

                Columns[i] = column.ColumnName.ToString();
                i++;
            }

            editPreView.DataSource = dtst.Tables[0];



            bufferRdr.Close();
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length != 0) {
                string specialVal = "";
                string specialKey = "";
                string specialUpd = "";

                if (tp.Name == "String")
                {
                    specialVal = "N'" + textBox1.Text + "'";
                    specialUpd = "N'" + textBox2.Text + "'";
                }
                else {
                    specialVal = textBox1.Text;
                    specialUpd = textBox2.Text;
                }
                    
                if (editPreView.Rows[updatedRow].Cells[KeyCol].Value.GetType().Name == "String")
                {
                    specialKey = "N'" + editPreView.Rows[updatedRow].Cells[KeyCol].Value.ToString() + "'";
                }
                else {
                    specialKey = editPreView.Rows[updatedRow].Cells[KeyCol].Value.ToString();
                }
                string s = $"update {table} set {Columns[updatedCol]} = {specialUpd} where {Columns[updatedCol]} = {specialVal} and {Columns[KeyCol]} = {specialKey}";
                richTextBox1.Text = s; 
                con.Open();

                bufferCmnd = new SqlCommand(s, con);
                int k = bufferCmnd.ExecuteNonQuery();
                con.Close();
                Switcher();
            }
        }

        private void getText(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != KeyCol) {
                string text = editPreView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                tp = editPreView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.GetType();
                textBox1.Text = text;
                updatedRow = e.RowIndex;
                updatedCol = e.ColumnIndex; 
            }
           
        }
    }
}

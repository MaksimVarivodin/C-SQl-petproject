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
    public partial class Form4 : Form
    {
       
        public string tableChosen;
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
        private Type tp = null;
        private int deletedRow = -1;


        DataSet dtst = null;
        public Form4( string tableChosen,  string comnd, int col)
        {
            InitializeComponent();
            
            this.tableChosen = tableChosen;
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
            dtst.Tables.Add(tableChosen);
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

            deletePreView.DataSource = dtst.Tables[0];



            bufferRdr.Close();
            con.Close();
        }

        private void chooseToDelete(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != KeyCol)
            {
                string text = deletePreView.Rows[e.RowIndex].Cells[KeyCol].Value.ToString();
                tp = deletePreView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.GetType();
                textBox1.Text = text;
                deletedRow = e.RowIndex;
            }
        }

        private void delete(object sender, EventArgs e)
        {
            if (deletedRow != -1)
            {

                string specialKey = "";               

                if (deletePreView.Rows[deletedRow].Cells[KeyCol].Value.GetType().Name == "String")
                {
                    specialKey = deletePreView.Rows[deletedRow].Cells[KeyCol].Value.ToString().TrimEnd();
                    specialKey = "N'" + specialKey + "'";
                }
                else
                {
                    specialKey = deletePreView.Rows[deletedRow].Cells[KeyCol].Value.ToString();
                }
                string s = "";
                con.Open();
                if (tableChosen == "Cars")
                {
                    s = $"delete from Cars where {tableChosen}.{Columns[KeyCol]} = {specialKey}";
                    bufferCmnd = new SqlCommand(s, con);
                    int k = bufferCmnd.ExecuteNonQuery();
                }
                else if (tableChosen == "Model")
                {
                    s = $"delete from Cars where Cars.mdlCode in(select Model.mdlCode from Model join Cars on Model.mdlCode = Cars.mdlCode  where Model.{Columns[KeyCol]} = {specialKey})";
                    bufferCmnd = new SqlCommand(s, con);
                    int k = bufferCmnd.ExecuteNonQuery();
                    s = $"delete from Model where {Columns[KeyCol]} = {specialKey}";
                    bufferCmnd = new SqlCommand(s, con);
                    k = bufferCmnd.ExecuteNonQuery();
                }
                else if (tableChosen == "Firma") {
                    s = $"delete from Cars where Cars.mdlCode in(select Model.mdlCode from Firma join Model on Firma.firmCode= Model.firmCode join Cars on Model.mdlCode = Cars.mdlCode  where Firma.{Columns[KeyCol]} = {specialKey})";
                    bufferCmnd = new SqlCommand(s, con);
                    int k = bufferCmnd.ExecuteNonQuery();
                    s = $"delete from Model where Model.firmCode in(select Model.firmCode from Firma join Model on Firma.firmCode= Model.firmCode  where Firma.{Columns[KeyCol]} = {specialKey})";
                    bufferCmnd = new SqlCommand(s, con);
                    k = bufferCmnd.ExecuteNonQuery();
                    s = $"delete from Firma where {Columns[KeyCol]} = {specialKey}";
                    bufferCmnd = new SqlCommand(s, con);
                    k = bufferCmnd.ExecuteNonQuery();

                }

                con.Close();
                Switcher();
                textBox1.Text = "";
                deletedRow = -1;
            }
        }
    }
}

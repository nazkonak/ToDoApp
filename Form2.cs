using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data.SqlClient;

namespace todoApp_proje
{
    public partial class Form2 : Form
    {
        string databasePath;
        string connectionString;
        DateTime defaultDate = DateTime.Today;
        public Form2()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            string projeKlasoru = Application.StartupPath;
            databasePath = Path.Combine(projeKlasoru, "todo.db");
            connectionString = $"Data Source = {databasePath};Version = 3;";

            CreateDatabase();
            CreateTables();
        }

        private void CreateDatabase()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
            }
        }

        private void CreateTables()
        {
            using (var SQLconnect = new SQLiteConnection(connectionString))
            {
                SQLconnect.Open();
                using (var SQLCommand = SQLconnect.CreateCommand())
                {
                    SQLCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS [users] (
                    [UserID] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [Name] TEXT NOT NULL,
                    [Surname] TEXT NOT NULL,
                    [Username] TEXT NOT NULL,
                    [Email] TEXT NOT NULL,
                    [Password] TEXT NOT NULL,
                    [Birthdate] TEXT NOT NULL,
                    [Gender] TEXT NULL
                );";
                    SQLCommand.ExecuteNonQuery();
                }
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = defaultDate;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Adınızı Giriniz");
                textBox1.Focus();

            }
            else if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Soyadınızı Giriniz");
                textBox2.Focus();
            }
            else if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Kullanıcı Adınızı Giriniz");
                textBox3.Focus();
            }
            else if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("E-mailinizi Giriniz");
                textBox4.Focus();

            }
            else if (string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("Şifrenizi Giriniz");
                textBox5.Focus();

            }

            else if (dateTimePicker1.Value == defaultDate)
            {
                MessageBox.Show("Doğum gününüzü giriniz!");
                dateTimePicker1.Focus();
                return;
            }



            else
            {
                string gender;

                if (radioButton1.Checked)
                    gender = radioButton1.Text;
                else if (radioButton2.Checked)
                    gender = radioButton2.Text;
                else
                    gender = null;  // hiçbirini seçmedi -> DB’ye NULL gider


                //veri tabanına kayıt işlemi 


                using (SQLiteConnection SQLconnect = new SQLiteConnection(connectionString))
                {
                    SQLconnect.Open();

                    string kontrolSql = "SELECT COUNT(*) FROM users WHERE Username = @Username";
                    using (SQLiteCommand kontrolCmd = new SQLiteCommand(kontrolSql, SQLconnect))
                    {
                        kontrolCmd.Parameters.AddWithValue("@Username", textBox3.Text.Trim());
                        if (Convert.ToInt32(kontrolCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Bu kullanıcı adını kullanamazsınız!");
                            return;
                        }
                    }

                    using (SQLiteCommand SQLcommand = SQLconnect.CreateCommand())
                    {
                        SQLcommand.CommandText = @"
            INSERT INTO users 
            (Name, Surname, Username, Email, Password, Birthdate, Gender)
            VALUES 
            (@Name, @Surname, @Username, @Email, @Password, @Birthdate, @Gender)";

                        SQLcommand.Parameters.AddWithValue("@Name", textBox1.Text);
                        SQLcommand.Parameters.AddWithValue("@Surname", textBox2.Text);

                        SQLcommand.Parameters.AddWithValue("@Username", textBox3.Text.Trim());
                        SQLcommand.Parameters.AddWithValue("@Email", textBox4.Text);
                        SQLcommand.Parameters.AddWithValue("@Password", textBox5.Text);
                        SQLcommand.Parameters.AddWithValue("@Birthdate", dateTimePicker1.Value.ToString("dd.MM.yyyy"));
                        SQLcommand.Parameters.AddWithValue("@Gender", gender);
                        SQLcommand.ExecuteNonQuery();
                        MessageBox.Show("Kayıt Başarılı! Giriş Yapabilirsiniz.");

                        this.Hide();
                        Form1 form1 = new Form1();
                        form1.Show();
                    }

                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}

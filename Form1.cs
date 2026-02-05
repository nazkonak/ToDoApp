using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace todoApp_proje
{
    public partial class Form1 : Form
    {
        
        string databasePath;
        string connectionString;

        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            string projeKlasoru = Application.StartupPath;
            databasePath = Path.Combine(projeKlasoru, "todo.db");
            connectionString = $"Data Source = {databasePath}; Version= 3;";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)){
                MessageBox.Show("Kullanıcı adınızı giriniz!");
                textBox1.Focus();
            }
            else if(string.IsNullOrEmpty(textBox2.Text)){
                MessageBox.Show("Şifrenizi giriniz");
                textBox2.Focus();
            }

            else
            {
                using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        baglanti.Open();

                        string sql = "SELECT UserID FROM users WHERE Username = @username AND Password = @password";

                        using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                        {
                            komut.Parameters.AddWithValue("@username", textBox1.Text);
                            komut.Parameters.AddWithValue("@password", textBox2.Text);

                            object sonuc = komut.ExecuteScalar();


                            if (sonuc != null)
                            {
                                int kullanici_id = Convert.ToInt32(sonuc);
                                MessageBox.Show("Giriş Başarılı");
                                Form3 form3 = new Form3(kullanici_id);
                                form3.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı adı veya şifre hatalı");
                                textBox1.Clear();
                                textBox2.Clear();
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veri Tabanına Bağlanılamadı! "+ ex.Message);
                    }

                }

            }
            

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();//Bu şekilde bulunduğumuz formu kapattık

            Form2 form2 = new Form2();
            form2.Show();// yeni formu açtık

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }


}



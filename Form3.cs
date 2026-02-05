using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace todoApp_proje
{
    public partial class Form3 : Form
    {

        int _kullanici_id;
        string databasePath;
        string connectionString;
        public Form3(int kullanici_id)
        {

            this.BackgroundImageLayout = ImageLayout.Tile;

            InitializeComponent();


            _kullanici_id = kullanici_id;
            string projeKlasoru = Application.StartupPath;
            databasePath = Path.Combine(projeKlasoru, "todo.db");
            connectionString = $"Data Source = {databasePath};Version = 3;";

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            timerTarih.Start();

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                string sql = "SELECT Name FROM users WHERE UserID = @kullanici_id";

                using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                {
                    komut.Parameters.AddWithValue("@kullanici_id", _kullanici_id);

                    object sonuc = komut.ExecuteScalar();

                    if (sonuc != null)
                    {
                        label3.Text = "Have A Nice Day, " + sonuc.ToString();
                    }
                    else
                    {
                        label3.Text = "Have A Nice Day!";
                    }
                }

                string sql2 = "SELECT PlanName FROM plan WHERE UserID = @kullanici_id AND PlanDate = @PlanDate AND State = @State";
                using (SQLiteCommand komut2 = new SQLiteCommand(sql2, baglanti))
                {
                    komut2.Parameters.AddWithValue("@kullanici_id", _kullanici_id);
                    komut2.Parameters.AddWithValue("@PlanDate", DateTime.Now.ToString("dd-MM-yyyy"));
                    komut2.Parameters.AddWithValue("@State", 0);
                    using (SQLiteDataReader reader = komut2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add(reader["PlanName"].ToString());

                        }
                    }
                }
            }
        }



        private void timerTarih_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("dd MMMM yyyy",
            new System.Globalization.CultureInfo("tr-TR"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Plan adı boş olamaz.");
                return;
            }

            listBox1.Items.Add(textBox1.Text);
            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                string sql = @"INSERT INTO plan (PlanName, PlanDate, UserID,State)
                               VALUES (@PlanName, @PlanDate, @UserID,@State);";


                using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                {
                    komut.Parameters.AddWithValue("@PlanName", textBox1.Text);
                    komut.Parameters.AddWithValue("@PlanDate", DateTime.Now.ToString("dd-MM-yyyy"));
                    komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                    komut.Parameters.AddWithValue("@State", 0);
                    komut.ExecuteNonQuery();

                }
            }
            ListeleriYenile();
            textBox1.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                var selectedItem = listBox1.SelectedItem;
                if (listBox1.Items.Count == 0)
                {
                    MessageBox.Show("Liste zaten boş.");
                    return;
                }

                else if (selectedItem != null)
                {
                    string planName = selectedItem.ToString();


                    string sql = "DELETE FROM plan WHERE PlanName = @PlanName AND UserID = @UserID";
                    using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                    {
                        komut.Parameters.AddWithValue("@PlanName", planName);
                        komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                        komut.ExecuteNonQuery();
                    }
                    listBox1.Items.Remove(listBox1.SelectedItem);
                }

                else
                {
                    MessageBox.Show("Silmek istediğiniz ögeyi seçiniz.");
                }

            }
            ListeleriYenile();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("Liste zaten boş.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Listedeki TÜM öğeler silinecek. Emin misiniz?",
                "Onay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
                {
                    baglanti.Open();

                    string sql = "DELETE FROM plan WHERE UserID = @UserID AND PlanDate = @PlanDate AND State = 0";
                    using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                    {

                        komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                        komut.Parameters.AddWithValue("@PlanDate", DateTime.Now.ToString("dd-MM-yyyy"));
                        komut.ExecuteNonQuery();
                    }
                    listBox1.Items.Clear();


                }
            }
            ListeleriYenile();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Plan adı boş olamaz.");
                return;
            }

            string planName = textBox2.Text;
            DateTime plan_date = monthCalendar1.SelectionStart;
            string planDateStr = plan_date.ToString("dd-MM-yyyy");



            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();
                string sql = @"
            INSERT INTO plan (PlanName, PlanDate, UserID, State)
            VALUES (@PlanName, @PlanDate, @UserID, @State);";

                using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                {
                    komut.Parameters.AddWithValue("@PlanName", planName);
                    komut.Parameters.AddWithValue("@PlanDate", planDateStr);
                    komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                    komut.Parameters.AddWithValue("@State", 0);
                    komut.ExecuteNonQuery();
                }
            }
            ListeleriYenile();
            textBox2.Clear();
        }



        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }




        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            listBox2.Items.Clear();

            DateTime plan_date = monthCalendar1.SelectionStart;
            string planDateStr = plan_date.ToString("dd-MM-yyyy");  // match insertion format


            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                string sql = "SELECT PlanName FROM plan WHERE UserID = @kullanici_id AND PlanDate = @PlanDate AND State= @State";

                using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                {
                    komut.Parameters.AddWithValue("@kullanici_id", _kullanici_id);
                    komut.Parameters.AddWithValue("@PlanDate", planDateStr);
                    komut.Parameters.AddWithValue("@State", 0);

                    bool found = false;
                    using (SQLiteDataReader reader = komut.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listBox2.Items.Add(reader["PlanName"].ToString());
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        
                        MessageBox.Show("Seçilen tarih için plan yok.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

            }
            ListeleriYenile();

        }



        void ListeleriYenile()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            string today = DateTime.Now.ToString("dd-MM-yyyy");
            string selectedDate = monthCalendar1.SelectionStart.ToString("dd-MM-yyyy");

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();


                string sql1 = @"SELECT PlanName FROM plan
                        WHERE UserID = @UserID
                        AND PlanDate = @PlanDate
                        AND State = 0";

                // listBox1 -> BUGÜN + yapılmayanlar
                using (SQLiteCommand cmd1 = new SQLiteCommand(sql1, baglanti))
                {
                    cmd1.Parameters.AddWithValue("@UserID", _kullanici_id);
                    cmd1.Parameters.AddWithValue("@PlanDate", today);

                    using (var r1 = cmd1.ExecuteReader())
                        while (r1.Read())
                            listBox1.Items.Add(r1["PlanName"].ToString());
                }

                // listBox2 -> SEÇİLEN TARİH + yapılmayanlar
                using (SQLiteCommand cmd2 = new SQLiteCommand(sql1, baglanti))
                {
                    cmd2.Parameters.AddWithValue("@UserID", _kullanici_id);
                    cmd2.Parameters.AddWithValue("@PlanDate", selectedDate);

                    using (var r2 = cmd2.ExecuteReader())
                        while (r2.Read())
                            listBox2.Items.Add(r2["PlanName"].ToString());
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir öge seçin.");
                return;
            }

            string planName = listBox1.SelectedItem.ToString();
            string today = DateTime.Now.ToString("dd-MM-yyyy");

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                string sql = @"UPDATE plan
                       SET State = 1
                       WHERE PlanName = @PlanName
                         AND PlanDate = @PlanDate
                         AND UserID = @UserID";

                using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                {
                    komut.Parameters.AddWithValue("@PlanName", planName);
                    komut.Parameters.AddWithValue("@PlanDate", today);
                    komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                    komut.ExecuteNonQuery();
                }
            }


            ListeleriYenile();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir öge seçin.");
                return;
            }

            string planName = listBox2.SelectedItem.ToString();
            string planDateStr = monthCalendar1.SelectionStart.ToString("dd-MM-yyyy");

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                string sql = @"UPDATE plan
                       SET State = 1
                       WHERE PlanName = @PlanName
                         AND PlanDate = @PlanDate
                         AND UserID = @UserID";

                using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                {
                    komut.Parameters.AddWithValue("@PlanName", planName);
                    komut.Parameters.AddWithValue("@PlanDate", planDateStr);
                    komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                    komut.ExecuteNonQuery();
                }
            }


            ListeleriYenile();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                var selectedItem = listBox2.SelectedItem;
                if (listBox2.Items.Count == 0)
                {
                    MessageBox.Show("Liste zaten boş.");
                    return;
                }

                else if (selectedItem != null)
                {
                    string planName = selectedItem.ToString();


                    string sql = "DELETE FROM plan WHERE PlanName = @PlanName AND UserID = @UserID";
                    using (SQLiteCommand komut = new SQLiteCommand(sql, baglanti))
                    {
                        komut.Parameters.AddWithValue("@PlanName", planName);
                        komut.Parameters.AddWithValue("@UserID", _kullanici_id);
                        komut.ExecuteNonQuery();
                    }
                    listBox2.Items.Remove(listBox2.SelectedItem);
                }

                else
                {
                    MessageBox.Show("Silmek istediğiniz ögeyi seçiniz.");
                }

            }
            ListeleriYenile();

        }


        void ChartHazirla()
        {
            chartPlans.Titles.Clear();

            chartPlans.Series.Clear();
            chartPlans.ChartAreas.Clear();

            chartPlans.ChartAreas.Add(new ChartArea());

            var area = chartPlans.ChartAreas[0];

            area.AxisY.Minimum = 0;
            area.AxisY.Interval = 1;

            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;

            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);

            Series yapilan = new Series("Yapılan")
            {
                ChartType = SeriesChartType.Column
            };

            Series yapilmayan = new Series("Yapılmayan")
            {
                ChartType = SeriesChartType.Column
            };

            chartPlans.Series.Add(yapilan);
            chartPlans.Series.Add(yapilmayan);
        }


        void HaftalikChart()
        {
            ChartHazirla();

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                for (int i = 6; i >= 0; i--)
                {
                    DateTime gun = DateTime.Now.AddDays(-i);
                    string tarih = gun.ToString("dd-MM-yyyy");
                    string etiket = gun.ToString("dd MMM");

                    int yapilan = SayGetir(baglanti, tarih, 1);
                    int yapilmayan = SayGetir(baglanti, tarih, 0);

                    chartPlans.Series["Yapılan"].Points.AddXY(etiket, yapilan);
                    chartPlans.Series["Yapılmayan"].Points.AddXY(etiket, yapilmayan);
                }
            }
        }
        void AylikChart()
        {
            ChartHazirla();

            int ay = DateTime.Now.Month;
            int yil = DateTime.Now.Year;


            string ayAdi = new DateTime(yil, ay, 1)
                               .ToString("MMMM yyyy", new CultureInfo("tr-TR"));
            chartPlans.Titles.Add(ayAdi);

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                for (int gun = 1; gun <= DateTime.DaysInMonth(yil, ay); gun++)
                {
                    DateTime tarihDT = new DateTime(yil, ay, gun);
                    string tarih = tarihDT.ToString("dd-MM-yyyy");

                    int yapilan = SayGetir(baglanti, tarih, 1);
                    int yapilmayan = SayGetir(baglanti, tarih, 0);

                    chartPlans.Series["Yapılan"].Points.AddXY(gun, yapilan);
                    chartPlans.Series["Yapılmayan"].Points.AddXY(gun, yapilmayan);
                }
            }
        }

        void YillikChart()
        {
            ChartHazirla();

            int yil = DateTime.Now.Year;

            using (SQLiteConnection baglanti = new SQLiteConnection(connectionString))
            {
                baglanti.Open();

                for (int ay = 1; ay <= 12; ay++)
                {
                    int yapilan = 0;
                    int yapilmayan = 0;

                    using (SQLiteCommand cmd = new SQLiteCommand(@"
                SELECT State, COUNT(*) as Adet
                FROM plan
                WHERE UserID=@UserID
                  AND substr(PlanDate, 4, 2)=@Ay
                  AND substr(PlanDate, 7, 4)=@Yil
                GROUP BY State", baglanti))
                    {
                        cmd.Parameters.AddWithValue("@UserID", _kullanici_id);
                        cmd.Parameters.AddWithValue("@Ay", ay.ToString("00"));
                        cmd.Parameters.AddWithValue("@Yil", yil.ToString());

                        using (SQLiteDataReader r = cmd.ExecuteReader())
                        {
                            int colState = r.GetOrdinal("State");
                            int colAdet = r.GetOrdinal("Adet");

                            while (r.Read())
                            {
                                int state = r.IsDBNull(colState) ? 0 : r.GetInt32(colState);
                                int adet = r.IsDBNull(colAdet) ? 0 : r.GetInt32(colAdet);

                                if (state == 1) yapilan += adet;
                                else yapilmayan += adet;
                            }
                        }
                    }

                    
                    string ayAdi = new DateTime(2000, ay, 1)
                                       .ToString("MMM", new CultureInfo("tr-TR"));

                    chartPlans.Series["Yapılan"].Points.AddXY(ayAdi, yapilan);
                    chartPlans.Series["Yapılmayan"].Points.AddXY(ayAdi, yapilmayan);
                }
            }
        }


        int SayGetir(SQLiteConnection baglanti, string tarih, int state)
        {
            string sql = @"SELECT COUNT(*) FROM plan
                   WHERE UserID=@UserID
                     AND PlanDate=@PlanDate
                     AND State=@State";

            using (SQLiteCommand cmd = new SQLiteCommand(sql, baglanti))
            {
                cmd.Parameters.AddWithValue("@UserID", _kullanici_id);
                cmd.Parameters.AddWithValue("@PlanDate", tarih);
                cmd.Parameters.AddWithValue("@State", state);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Haftalık":
                    HaftalikChart();
                    break;

                case "Aylık":
                    AylikChart();
                    break;

                case "Yıllık":
                    YillikChart();
                    break;
            }
        }


    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Data.SqlClient;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;
using System.Globalization;
using System.Data;
using Mono.Web;
using System.Diagnostics;
using CefSharp;
using CefSharp.Wpf;
using System.Windows.Input;
using DHelp.Properties;
using System.Windows.Controls.Primitives;
using DHelp.Controllers;

namespace DHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainController controller = new MainController();
        static string conectionString = @"Server = .\SQLEXPRESS; Database = DHelpEN; Integrated Security = True";
        static SqlConnection connection = new SqlConnection(conectionString);
        string city = string.Empty;
        private List<string> lines = new List<string>();
        string inputFilePath = @"..\..\..\..\..\..\input.txt";
        bool isButtonClick = false;
        double rate = 0;
        public MainWindow()
        {
            InitializeComponent();

            MenuCB.IsEnabled = true;
            MenuCB.Items.Add(new ComboBoxItem() { Content = "Home" });
            MenuCB.Items.Add(new ComboBoxItem() { Content = "Symptoms" });
            MenuCB.Items.Add(new ComboBoxItem() { Content = "Feedback" });
            gbMap.Visibility = Visibility.Hidden;
            mediaElementHeartbeat.Loaded += mediaElementHeartbeat_Loaded;//непрекъснато възпроизвеждане на видео
            //Color color = (Color)ColorConverter.ConvertFromString("#FF9C9CA4");//използва hex код и го превръща в цвят
            //rtbComment.Foreground = new SolidColorBrush(color);//сменя цвета на текста
            FindCity();
            if (File.Exists(inputFilePath))
            {
                lines = File.ReadAllLines(inputFilePath).ToList();
                if (lines.Count > 5)
                {
                    lines = lines.Skip(lines.Count - 5).ToList(); // Keep only last 5 lines
                }
            }
            foreach (string line in lines)
            {
                cbSearchHistory.Items.Add(line);
            }
        }
        //методи за върпоизвеждането на видеото
        private void mediaElementHeartbeat_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElementHeartbeat.Position = System.TimeSpan.Zero;
            mediaElementHeartbeat.Play();
        }
        private void mediaElementHeartbeat_Loaded(object sender, RoutedEventArgs e)
        {
            mediaElementHeartbeat.Play();
        }
        //методи за бутон изход
        private void btnExit_MouseMove(object sender, MouseEventArgs e)
        {
            btnExit.Visibility = Visibility.Hidden;
            btnExit1.Visibility = Visibility.Visible;
            this.Cursor = Cursors.Hand;
        }
        private void btnExit1_MouseLeave(object sender, MouseEventArgs e)
        {
            btnExit1.Visibility = Visibility.Hidden;
            btnExit.Visibility = Visibility.Visible;
            this.Cursor = Cursors.Arrow;
        }
        private void btnExit1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        //метод за сменяне на страниците
        private void MenuCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbHomePage.Visibility = Visibility.Visible;
            gbFeedback.Visibility = Visibility.Visible;
            gbSymptom.Visibility = Visibility.Visible;
            if (MenuCB.SelectedIndex == 0 || MenuCB.SelectedIndex < 0) //Home
            {
                noEmotions();
                //rtbCommentWriting.Document.Blocks.Clear();
                //rtbCommentWriting.Visibility = Visibility.Hidden;
                //rtbCommentWriting.Visibility = Visibility.Visible;
                gbHomePage.Visibility = Visibility.Visible;
                gbFeedback.Visibility = Visibility.Hidden;
                gbSymptom.Visibility = Visibility.Hidden;
                gbMap.Visibility = Visibility.Hidden;
                lstDoctors.Items.Clear();
                lstDoctors.Visibility = Visibility.Hidden;
                lblYourSymptom.Visibility = Visibility.Hidden;
                lblRecommendation.Visibility = Visibility.Hidden;
                connection.Close();
                isButtonClick = false;
                cbSearchHistory.Text = "--Search history--";
                noEmotions();
            }
            else if (MenuCB.SelectedIndex == 1) // Symptoms
            {
                gbHomePage.Visibility = Visibility.Hidden;
                gbFeedback.Visibility = Visibility.Hidden;
                gbMap.Visibility = Visibility.Hidden;
                gbSymptom.Visibility = Visibility.Visible;
                lstDoctors.Items.Clear();
                lstDoctors.Visibility = Visibility.Hidden;
                connection.Close();
                isButtonClick = false;
                noEmotions();
                cbSearchHistory.Text = "--Search history--";

            }
            else if (MenuCB.SelectedIndex == 2) // Feedback
            {
                gbHomePage.Visibility = Visibility.Hidden;
                gbSymptom.Visibility = Visibility.Hidden;
                gbFeedback.Visibility = Visibility.Visible;
                gbMap.Visibility = Visibility.Hidden;
                lstDoctors.Items.Clear();
                lstDoctors.Visibility = Visibility.Hidden;
                lblYourSymptom.Visibility = Visibility.Hidden;
                lblRecommendation.Visibility = Visibility.Hidden;
                connection.Close();
            }
        }
        //Смяна на изображението при кликване на емоджи за оценка
        private void btnReview1_Click(object sender, RoutedEventArgs e)
        {
            btnEmotions1();
            isButtonClick = true;
        }
        private void btnReview2_Click(object sender, RoutedEventArgs e)
        {
            btnEmotions2();
            isButtonClick = true;
        }
        private void btnReview3_Click(object sender, RoutedEventArgs e)
        {
            btnEmotions3();
            isButtonClick = true;
        }
        private void btnReview4_Click(object sender, RoutedEventArgs e)
        {
            btnEmotions4();
            isButtonClick = true;
        }
        private void btnReview5_Click(object sender, RoutedEventArgs e)
        {
            btnEmotions5();
            isButtonClick = true;
        }
        // Смяна на изображението при преминаване през емоджито
        private void btnReview1_Hover(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            if (!isButtonClick)
            {
                btnEmotions1();
            }
        }
        private void btnReview2_Hover(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            if (!isButtonClick)
            {
                btnEmotions2();
            }
        }
        private void btnReview3_Hover(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            if (!isButtonClick)
            {
                btnEmotions3();
            }
        }
        private void btnReview4_Hover(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            if (!isButtonClick)
            {
                btnEmotions4();
            }
        }
        private void btnReview5_Hover(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            if (!isButtonClick)
            {
                btnEmotions5();
            }
        }
        //Методи за смяна на изображенията
        public void btnEmotions1()
        {
            imageNoReview.Visibility = Visibility.Hidden;
            image1starReview.Visibility = Visibility.Visible;
            image2starReview.Visibility = Visibility.Hidden;
            image3starReview.Visibility = Visibility.Hidden;
            image4starReview.Visibility = Visibility.Hidden;
            image5starReview.Visibility = Visibility.Hidden;
        }
        public void btnEmotions2()
        {
            imageNoReview.Visibility = Visibility.Hidden;
            image1starReview.Visibility = Visibility.Hidden;
            image2starReview.Visibility = Visibility.Visible;
            image3starReview.Visibility = Visibility.Hidden;
            image4starReview.Visibility = Visibility.Hidden;
            image5starReview.Visibility = Visibility.Hidden;
        }
        public void btnEmotions3()
        {
            imageNoReview.Visibility = Visibility.Hidden;
            image1starReview.Visibility = Visibility.Hidden;
            image2starReview.Visibility = Visibility.Hidden;
            image3starReview.Visibility = Visibility.Visible;
            image4starReview.Visibility = Visibility.Hidden;
            image5starReview.Visibility = Visibility.Hidden;
        }
        public void btnEmotions4()
        {
            imageNoReview.Visibility = Visibility.Hidden;
            image1starReview.Visibility = Visibility.Hidden;
            image2starReview.Visibility = Visibility.Hidden;
            image3starReview.Visibility = Visibility.Hidden;
            image4starReview.Visibility = Visibility.Visible;
            image5starReview.Visibility = Visibility.Hidden;
        }
        public void btnEmotions5()
        {
            imageNoReview.Visibility = Visibility.Hidden;
            image1starReview.Visibility = Visibility.Hidden;
            image2starReview.Visibility = Visibility.Hidden;
            image3starReview.Visibility = Visibility.Hidden;
            image4starReview.Visibility = Visibility.Hidden;
            image5starReview.Visibility = Visibility.Visible;
        }
        public void noEmotions()
        {
            imageNoReview.Visibility = Visibility.Visible;
            image1starReview.Visibility = Visibility.Hidden;
            image2starReview.Visibility = Visibility.Hidden;
            image3starReview.Visibility = Visibility.Hidden;
            image4starReview.Visibility = Visibility.Hidden;
            image5starReview.Visibility = Visibility.Hidden;
        }
        public void buttonsEnabled()
        {
            btnReview1.IsEnabled = true;
        }
        //Смяна на изображението, когато курсора не е върху емоджито
        private void btnReview1_Leave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            if (!isButtonClick)
            {
                noEmotions();
            }
        }
        private void btnReview2_Leave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            if (!isButtonClick)
            {
                noEmotions();
            }
        }
        private void btnReview3_Leave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            if (!isButtonClick)
            {
                noEmotions();
            }
        }
        private void btnReview4_Leave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            if (!isButtonClick)
            {
                noEmotions();
            }
        }
        private void btnReview5_Leave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            if (!isButtonClick)
            {
                noEmotions();
            }
        }
        //Задаване на фокус върху GroupBox
        private void gbFeedback_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gbFeedback.Focus();
        }
        private void btnSendFeedback_Click(object sender, RoutedEventArgs e)
        {
            if (cbSearchHistory.SelectedIndex < 0)
            {
                MessageBox.Show("Choose a doctor you want to rate!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (isButtonClick == false)
            {
                MessageBox.Show("Please select a rating emoji!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            if (cbSearchHistory.SelectedIndex >= 0 && isButtonClick == true)
            {
                string doctor = cbSearchHistory.SelectedItem.ToString();
                if (image1starReview.Visibility == Visibility.Visible)
                {
                    rate = 1;
                }
                if (image2starReview.Visibility == Visibility.Visible)
                {
                    rate = 2;
                }
                if (image3starReview.Visibility == Visibility.Visible)
                {
                    rate = 3;
                }
                if (image4starReview.Visibility == Visibility.Visible)
                {
                    rate = 4;
                }
                if (image5starReview.Visibility == Visibility.Visible)
                {
                    rate = 5;
                }
                noEmotions();
                connection.Open();
                string queryFeedback = $"update DoctorsInformation set Sum_Rating = Sum_Rating + {(float)rate}, Count_Rating = Count_Rating + 1 where DoctorName = '{doctor}'";
                SqlCommand command = new SqlCommand(queryFeedback, connection);
                command.ExecuteNonQuery();
                connection.Close();
                //rtbCommentWriting.Document.Blocks.Clear();
                //rtbCommentWriting.Visibility = Visibility.Hidden;
                //rtbCommentWriting.Visibility = Visibility.Visible;
                cbSearchHistory.Text = " --Search history--";
                cbSearchHistory.SelectedIndex = -1;
                isButtonClick = false;
            }
        }
        //private void rtbComment_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    rtbComment.Visibility = Visibility.Hidden;
        //    rtbCommentWriting.Visibility = Visibility.Visible;
        //    rtbCommentWriting.Focus();
        //    //rtbComment.Document.Blocks.Clear();//изчиства текста
        //}
        //private void rtbCommentWriting_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (rtbCommentWriting.Document.Blocks.Count == 0 || string.IsNullOrWhiteSpace(new TextRange(rtbCommentWriting.Document.ContentStart, rtbCommentWriting.Document.ContentEnd).Text))
        //    {
        //        rtbComment.Visibility = Visibility.Visible;
        //        rtbCommentWriting.Visibility = Visibility.Hidden;
        //    }
        //}
        //Задаване на фокус върху друг обект, така се премахва фокуса от предишния елемент
        private void gbSymptom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gbSymptom.Focus();
        }
        //Отваряне на падащото меню при преминаване с мишката
        private void menuItemPain_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[0]; // Първия MenuItem (File)
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemFocus_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[1]; // Втори MenuItem (File)
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuStressAndAnxiety_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[2];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemEating_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[3];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemDiscomfort_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[4];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemSleeping_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[5];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemFatigue_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[6];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemSkin_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[7];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemHeart_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[8];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemWeight_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[9];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemEye_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[10];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemAllergy_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[11];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemInfection_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[12];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemStomach_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[13];
            fileMenu.IsSubmenuOpen = true;
        }
        private void menuItemOthers_MouseMove(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[14];
            fileMenu.IsSubmenuOpen = true;
        }
        //Затваряне на падащото меню, когато мишката не е върху него
        private void menuItemPain_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[0];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemFocus_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[1];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuStressAndAnxiety_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[2];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemEating_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[3];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemDiscomfort_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[4];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemSleeping_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[5];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemFatigue_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[6];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemSkin_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[7];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemHeart_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[8];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemWeight_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[9];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemEye_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[10];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemAllergy_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[11];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemInfection_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[12];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemStomach_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[13];
            fileMenu.IsSubmenuOpen = false;
        }
        private void menuItemOthers_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuItem fileMenu = (MenuItem)this.menuSymptoms.Items[14];
            fileMenu.IsSubmenuOpen = false;
        }
        //Заявки при натискане на симптом
        private string query(string table, string symptom)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            string doctor = null;
            lstDoctors.Visibility = Visibility.Visible;
            lblYourSymptom.Content = $"Your symptom: {Char.ToUpper(symptom[0])}{symptom.Substring(1).ToLower()}";
            lblYourSymptom.Visibility = Visibility.Visible;
            string q = $"Select specialist from doctors as d join {table} as t on t.SpecialistId = d.ID where t.{table} = '{symptom}';";
            SqlCommand cmd = new SqlCommand(q, connection);
            SqlDataReader reader1 = cmd.ExecuteReader();
            using (reader1)
            {
                if (reader1.Read())
                {
                    doctor = reader1[0].ToString();  // Вземаме името на специалиста
                    lblRecommendation.Content = $"Specialist: {Char.ToUpper(doctor[0])}{doctor.Substring(1).ToLower()}";
                }
            }
            lblRecommendation.Visibility = Visibility.Visible;
            return $"select DoctorName, Location, Address, PhoneNumber, ROUND((Sum_Rating/Count_Rating),1) as Rating from DoctorsInformation di join Doctors d on d.ID = di.SpecializationNumber where specializationNumber = (select specialistID from {table} as symptom join Doctors d on d.ID = symptom.SpecialistID where {table} = '{symptom}')order by Rating desc;";
        }
        private void LoadDoctors(string table, string symptom)
        {
            lstDoctors.Items.Clear();
            lstDoctors.Visibility = Visibility.Visible;

            var (doctors, specialist) = controller.GetDoctorsWithSpecialist(table, symptom, city);

            lblYourSymptom.Content = $"Your symptom: {Char.ToUpper(symptom[0])}{symptom.Substring(1).ToLower()}";
            lblYourSymptom.Visibility = Visibility.Visible;

            if (!string.IsNullOrWhiteSpace(specialist))
            {
                lblRecommendation.Content = $"Specialist: {Char.ToUpper(specialist[0])}{specialist.Substring(1).ToLower()}";
                lblRecommendation.Visibility = Visibility.Visible;
            }

            foreach (var doctor in doctors)
            {
                string row = $"{doctor.Name,-40} | {doctor.Location,-20} | {doctor.PhoneNumber,-25} | {doctor.Rating,-2}";
                lstDoctors.Items.Add(row);
            }
        }
        private void miBackPain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Back pain");
        }
        private void miBurningLimbs_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Burning sensation in limbs");
        }
        private void miKneePain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Knee pain");
        }
        private void miHeadache_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Headache");
        }
        private void miAbdominalPain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Abdominal pain");
        }
        private void miMenstrualCramps_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Menstrual cramps");
        }
        private void miChestPain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Chest pain");
        }
        private void miJointPain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Joint pain");
        }
        private void miNeckPain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Neck pain");
        }
        private void miFootPain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Foot pain");
        }
        private void miPainSurgery_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Pain after surgery");
        }
        private void miToothache_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Tootache");
        }
        private void miMusclePain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Muscle pain");
        }
        private void miPressurePain_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Pain on pressure");
        }
        private void miBurnSensSkin_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Pain", "Burning sensation on skin");
        }
        private void miForgetfulness_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("FocusAndMemory", "Forgetfulness");
        }
        private void miConcentratingDif_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("FocusAndMemory", "Difficulty concentrating");
        }
        private void miAbsentMind_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("FocusAndMemory", "Absent-mindedness");
        }
        private void miDepression_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("StressAndAnxiety", "Depression");
        }
        private void miAnxiety_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("StressAndAnxiety", "Anxiety");
        }
        private void miInferiority_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("StressAndAnxiety", "Inferiority complex");
        }
        private void miIncreasedAppetite_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("EatingDisorders", "Appetite-increased");
        }
        private void miLossAppetite_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("EatingDisorders", "Loss of appetite");
        }
        private void miSwallowingDif_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("EatingDisorders", "Difficulty swallowing");
        }
        private void miNausea_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Discomfort", "Nausea and vomiting");
        }
        private void miFaintness_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Discomfort", "Faintness");
        }
        private void miChills_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Discomfort", "Chills");
        }
        private void miDiarrhea_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Discomfort", "Diarrhea");
        }
        private void miConstipation_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Discomfort", "Constipation");
        }
        private void miInsomnia_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SleepDisorders", "Insomnia");
        }
        private void miSleepDisturbances_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SleepDisorders", "Sleep disturbances");
        }
        private void miSleepWalking_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SleepDisorders", "Sleepwalking");
        }
        private void miRaving_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SleepDisorders", "Raving");
        }
        private void miSleepiness_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("FatigueAndDrowsiness", "Excessive sleepiness");
        }
        private void miFatigue_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("FatigueAndDrowsiness", "Fatigue");
        }
        private void miFaintness2_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("FatigueAndDrowsiness", "Faintness");
        }
        private void miSkinRash_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SkinDiseases", "Skin rash");
        }
        private void miItchySkin_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SkinDiseases", "Itchy skin");
        }
        private void miBurnsScalds_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SkinDiseases", "Burns and scalds");
        }
        private void miAcne_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SkinDiseases", "Acne");
        }
        private void miPsoriasis_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SkinDiseases", "Psoriasis");
        }
        private void miEczema_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("SkinDiseases", "Eczema");
        }
        private void miPalpitations_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("CardiovascularDiseases", "Heart palpitations");
        }
        private void miHeartbeat_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("CardiovascularDiseases", "Heartbeat");
        }
        private void miShortnessOfBreath_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("CardiovascularDiseases", "Shortness of breath");
        }
        private void miWeightLoss_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("WeightFluctation", "Weight loss");
        }
        private void miFluidRetention_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("WeightFluctation", "Fluid retention");
        }
        private void miBlurredVision_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("EyeDIseases", "Blurred vision");
        }
        private void miDryEyes_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("EyeDIseases", "Dry eyes");
        }
        private void miEyeIrritation_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("EyeDIseases", "Eye irritation");
        }
        private void miRash_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Allergies", "Rash");
        }
        private void miItch_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Allergies", "Itch");
        }
        private void miAllergy_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Allergies", "Allergy");
        }
        private void miRecurrentInfections_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Infections", "Reccurent infections");
        }
        private void miBacterialInfections_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Infections", "Bactreial infections");
        }
        private void miVIralInfections_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Infections", "VIral infections");
        }
        private void miFungalInfections_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Infections", "Fungal infections");
        }
        private void miIndigestion_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("GastrointestinalDiseases", "Indigestion");
        }
        private void miDistendedAbdomen_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("GastrointestinalDiseases", "Distended abdomen");
        }
        private void miWeightLoss2_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("GastrointestinalDiseases", "Weight loss");
        }
        private void miMoodSwings_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Mood swings");
        }
        private void miMemoryProblems_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Memory problems");
        }
        private void miDryMouth_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Dry mouth");
        }
        private void miLimbNumbness_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Limb numbness");
        }
        private void miDizziness_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Dizziness");
        }
        private void miPoorConcentration_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Poor concentration");
        }
        private void miConstantFatigue_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Constant fatigue");
        }
        private void miCough_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Cough");
        }
        private void miFrequentHeadache_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Frequent headache");
        }
        private void miBalanceDisorders_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Balance disorders");
        }
        private void miDrainageWater_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Drainage water");
        }
        private void miWeakImmuneSystem_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Weak immune system");
        }
        private void miHairLoss_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Hair loss");
        }
        private void miSkinColor_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Skin discoloration or pigmentation");
        }
        private void miSensitiveTeeth_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Sensitive teeth");
        }
        private void miSneezing_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Sneezing");
        }
        private void miConstantThirst_Click(object sender, RoutedEventArgs e)
        {
            LoadDoctors("Others", "Constant thirst");
        }
        //Намиране на града, в който се намира потребителя чрез публичен IP адрес
        async void FindCity()
        {
            string ip = await GetPublicIP();
            var (latitude, longitude) = await GetLocationFromIP(ip);
            JObject data = null;
            string apiUrl = $"https://nominatim.openstreetmap.org/reverse?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&format=json";
            //MapDoctors.Address = $"https://www.google.com/maps/place/{latitude}+{longitude}";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyGeolocationApp/1.0)");
                string response = await client.GetStringAsync(apiUrl);
                data = JObject.Parse(response);
            }
            string text = data["address"]?["city"]?.ToString() ?? data["address"]?["town"]?.ToString() ??
                            data["address"]?["village"]?.ToString() ?? "City not found";
            city = "Blagoevgrad";
        }
        public async Task<string> GetPublicIP()
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync("https://api.ipify.org");
            }
        }
        public async Task<(double Latitude, double Longitude)> GetLocationFromIP(string ip)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"http://ip-api.com/json/{ip}";
                string response = await client.GetStringAsync(apiUrl);
                JObject data = JObject.Parse(response);
                double latitude = data["lat"]?.ToObject<double>() ?? 0.0;
                double longitude = data["lon"]?.ToObject<double>() ?? 0.0;
                return (latitude, longitude);
            }
        }
        //Превеждането на града от един език в друг
        public static async Task<string> TranslateText(string text, string sourceLang, string targetLang)
        {
            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={Mono.Web.HttpUtility.UrlEncode(text)}";

            using (WebClient webClient = new WebClient())
            {
                string result = webClient.DownloadString(url);
                return result.Split('"')[1]; // Extracts translated text
            }
        }
        //Взимане на геогравски кординати по зададен адрес
        public static (double Latitude, double Longitude) GetCoordinatesFromAddress(string town, string street, string number)
        {
            string query = Uri.EscapeDataString($"{number} {street}, {town}");
            string url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
                // Synchronous request using GetStringAsync().Result
                string responseData = client.GetStringAsync(url).Result;
                JArray data = JArray.Parse(responseData);
                if (data.Count > 0 && data[0]["lat"] != null && data[0]["lon"] != null)
                {
                    if (double.TryParse(data[0]["lat"].ToString(), out double lat) &&
                        double.TryParse(data[0]["lon"].ToString(), out double lon))
                    {
                        return (lat, lon);
                    }
                }
                return (0.0, 0.0);
            }
        }
        //Връщане отново в страницата за избиране на симптоми
        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            gbMap.Visibility = Visibility.Hidden;
            gbSymptom.Visibility = Visibility.Visible;
            MapDoctors.Address = null;
        }
        string url = "";
        //Показването на местоположението на лекаря и запазването му в история на търсенето
        private void lstDoctors_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            connection.Open();
            string selected = lstDoctors.SelectedItem.ToString().Split('|')[0].Trim();
            string query = $"select Location, Address from DoctorsInformation where DoctorName = '{selected}';";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (lines.Contains(selected.Trim()))
            {
                lines.Remove(selected.Trim());
            }
            lines.Insert(0, selected.Trim());
            if (lines.Count > 5)
            {
                lines.RemoveAt(lines.Count - 1);
            }
            File.WriteAllLines(inputFilePath, lines);
            if (File.Exists(inputFilePath))
            {
                lines = File.ReadAllLines(inputFilePath).ToList();
                if (lines.Count > 5)
                {
                    lines = lines.Skip(lines.Count - 5).ToList(); // Keep only last 5 lines
                }
            }
            cbSearchHistory.Items.Clear();
            foreach (string line in lines)
            {
                cbSearchHistory.Items.Add(line);
            }
            string city = string.Empty;
            string address = null;
            string num = null;
            string fAddress = null;
            using (reader)
            {
                while (reader.Read())
                {
                    city = reader[0].ToString().Trim();
                    address = reader[1].ToString().Trim().Split(new string[] { "St.", "Blvd." }, StringSplitOptions.None)[0]; // Взимане на адреса 
                    fAddress = reader[1].ToString().Trim();
                    Regex regex = new Regex(@"St\.\s*(\d+)");
                    Match match = regex.Match(fAddress);
                    num = match.Groups[1].Value;
                }
            }
            string[] searchingStr = fAddress.Split(' ');
            string[] searchingCity = city.Split(' ');
            string searching = "";
            foreach (string s in searchingStr)
            {
                searching += s + "%20";
            }
            foreach (string s in searchingCity)
            {
                searching += s + "%20";
            }
            connection.Close();
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");  // Изключване на GPU
            settings.CefCommandLineArgs.Add("disable-web-security", "1");  // Деактивира CORS
            settings.CefCommandLineArgs.Add("allow-running-insecure-content", "1"); // Позволява несигурно съдържание
            settings.CefCommandLineArgs.Add("ignore-certificate-errors", "1"); // Игнорира SSL грешки
            url = $"https://www.google.com/maps?q={searching}";
            MapDoctors.Address = $"https://www.google.com/maps?q={searching}";
            MapDoctors.UpdateLayout();
            gbMap.Visibility = Visibility.Visible;
            gbSymptom.Visibility = Visibility.Hidden;
            //var (latitudeDoc, longitudeDoc) = GetCoordinatesFromAddress(city, address, num);
            //MapDoctors.Load("https://www.google.com/maps?q={searching}");
            //MapDoctors.Load("https://www.google.com/maps?q={searching}");
            //MapDoctors.ExecuteScriptAsyncWhenPageLoaded("https://www.google.com/maps?q={searching}");
            //Process.Start("chrome.exe", $"https://www.google.com/maps?q={searching}");
            //MapDoctors.Load($"https://www.google.com/maps?q={searching}");  // Задава новия адрес в картата
            //settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            //settings.CefCommandLineArgs.Add("disable-software-rasterizer", "1");
            //Cef.Initialize(settings);
        }
        //private void MapDoctors_Initialized(object sender, EventArgs e)
        //{
        //    string query = $"select Location, Address from DoctorsInformation where DoctorName = '{selected}';";
        //    SqlCommand command = new SqlCommand(query, connection);
        //    SqlDataReader reader = command.ExecuteReader();
        //    string address = null;
        //    string num = null;
        //    string fAddress = null;
        //    using (reader)
        //    {
        //        while (reader.Read())
        //        {
        //            city = reader[0].ToString().Trim();
        //            address = reader[1].ToString().Trim().Split(new string[] { "St.", "Blvd." }, StringSplitOptions.None)[0];
        //            fAddress = reader[1].ToString().Trim();
        //            Regex regex = new Regex(@"St\.\s*(\d+)");
        //            Match match = regex.Match(fAddress);
        //            num = match.Groups[1].Value;
        //        }
        //    }
        //    string[] searchingStr = fAddress.Split(' ');
        //    string[] searchingCity = city.Split(' ');
        //    string searching = "";
        //    foreach (string s in searchingStr)
        //    {
        //        searching += s + "%20";
        //    }
        //    foreach (string s in searchingCity)
        //    {
        //        searching += s + "%20";
        //    }
        //    connection.Close();

        //    //var (latitudeDoc, longitudeDoc) = GetCoordinatesFromAddress(city, address, num);
        //    //MapDoctors.Load("https://www.google.com/maps?q={searching}");
        //    //MapDoctors.Load("https://www.google.com/maps?q={searching}");
        //    //MapDoctors.ExecuteScriptAsyncWhenPageLoaded("https://www.google.com/maps?q={searching}");
        //    //Process.Start("chrome.exe", $"https://www.google.com/maps?q={searching}");
        //    CefSettings settings = new CefSettings();
        //    settings.CefCommandLineArgs.Add("disable-gpu", "1");  // Изключване на GPU
        //    //settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
        //    //settings.CefCommandLineArgs.Add("disable-software-rasterizer", "1");
        //    //Cef.Initialize(settings);
        //    //MapDoctors.Address = $"https://www.google.com/maps?q={searching}"; string city = string.Empty;
        //    MapDoctors.Address = $"https://www.google.com/maps?q={searching}";
        //    //MapDoctors.Load($"https://www.google.com/maps?q={searching}");  // Задава новия адрес в картата
        //    gbMap.Visibility = Visibility.Visible;
        //    gbSymptom.Visibility = Visibility.Hidden;
        //}
    }
}
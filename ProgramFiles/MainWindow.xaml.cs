﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Drawing;


namespace Calendar
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // WebClient dla aktualnej pogody 
        public WebClient web = new WebClient();

        // Obiekt klasy obsługującej kolekcje wydarzeń oraz ich typów 
        public EventsCollections collections = new EventsCollections();

        /// <summary>
        /// Konstruktor klasy. Centruje okno, ustawia aktualną date w kalendarzu oraz dodaje do DataContextu colekcje danych
        /// </summary>
        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetProperlyDirectory();
            if (CalendarContol.SelectedDate == null)
                CalendarContol.SelectedDate = DateTime.Now;

            this.DataContext = this.collections ;
        }

        /// <summary>
        /// Jedyne co tu sie dzieje to pobranie rekordow(wszystkich=jednego)
        /// z BD przez entity po nacisnieciu przycisku
        /// </summary>

        private void GetProperlyDirectory()
        {
            string Path = Environment.CurrentDirectory;
            string[] appPath = Path.Split(new string[] { "bin" }, StringSplitOptions.None);
            AppDomain.CurrentDomain.SetData("DataDirectory", appPath[0]);

        }

        /// <summary>
        /// Przycisk do wyswietlania pogody 
        /// </summary>

        private void ShowWeather_Click(object sender, RoutedEventArgs e)
        {
            getWeather(web);
        }

        /// <summary>
        /// Pobieranie pogody za pomocą API ze strony OpenWeather, 
        /// użycie JsonConvert do konwersji między typami JSON, a .Net 
        /// </summary>

        void getWeather(WebClient web)
        {
            // Obiekt klasy handleWrongCitynames posiadający metodę do sprawdzania poprawności działania zapytania wysyłanego do serwera 
            handleWrongCitynames tmp = new handleWrongCitynames ();

            // CityName z TextBoxa , apiKey na stałe
            string cityName = cityTextBox.Text.ToString();
            string apiKey = "8b7c4545e3874014261a301b43f9d144";

            // Pełny link z apiKey oraz miastem
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?appid={0}&q={1}", apiKey, cityName);

            // Obsługa błędu 404 oraz innych wynikajacych ze źle wpisanego miasta lub jego braku wpisania 
            bool decision = tmp.checkUrl(url);

            if (decision == true)
            {
                MessageBox.Show("Prawdopodobnie pomyliłeś się przy wpisywaniu nazwy miasta ! \nSpróbuj ponownie");
            }

            else
            {
                    var json = web.DownloadString(url);

                    // Użycie JsonConvert
                    var result = JsonConvert.DeserializeObject<weatherinfo.root>(json);

                    weatherinfo.root outPut = result;

                    // Przypisanie zmiennym odpowiednich wartości 
                    string temperature = string.Format("{0}", Math.Round(outPut.main.temp - 273, 2));
                    string humidity = string.Format("{0}", outPut.main.humidity);
                    string pressure = string.Format("{0}", outPut.main.pressure);
                    string windSpeed = string.Format("{0}", outPut.wind.speed);

                    // Wyświetlanie danych o pogodzie w odpowiednich TextBoxach
                    temperatureTextBox.Text = temperature + " °C";
                    humidityTextBox.Text = humidity + " %";
                    pressureTextBox.Text = pressure + " hPa";
                    windSpeedTextBox.Text = windSpeed + " m/s";

                    // Wyświetlenie obrazka ze strony z pogodą
                    string imageId = string.Format("{0}", outPut.weather[0].icon);
                    string fullImageUrl = string.Format("https://openweathermap.org/img/wn/{0}@2x.png", imageId);

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullImageUrl);
                    bitmap.EndInit();

                    weatherLogo.Source = bitmap;
            }
        }

        /// <summary>
        /// City Text Box , niechcacy kliknałem , po usunieciu tego fragmentu nie kompiluje sie :-( , jakis problem z visualem ???
        /// mimo ze usuwam rowniez z xmla fragment kodu dotyczacy tego text boxa
        /// </summary>

        private void CityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /// <summary>
        /// Otwieranie nowego okna z prognozą pogody
        /// </summary>

        private void ShowForecast_Click(object sender, RoutedEventArgs e)
        {
            // Zmienna potrzebna do przekazania nazwy miasta do nowego okienka
            string cityName = cityTextBox.Text.ToString();

                // Tworzenie nowego okienka
                weatherForecast sW = new weatherForecast(cityName);
                sW.Show();      
        }

        /// <summary>
        /// Wczytanie aktualnych danych do kolekcji wydarzeń
        /// </summary>
        /// <param name="date"></param>

        public void refresh_Event_list(DateTime? date)
        {

            collections.ClearDailyEvents();
            BazaDanychEntities context = new BazaDanychEntities();




            var akuku = context.Events.Where(s => s.StartDate == date);
            collections.FillDailyEvents(akuku);
        }
        /// <summary>
        /// Reakcja na zmiane daty w kalendarzu. Powoduje wyświetlenie odpowiednich wydarzeń
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = sender as System.Windows.Controls.Calendar;

             refresh_Event_list(calendar.SelectedDate);

        }
        /// <summary>
        /// Uruchomienie okna dodania wydarzenia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Add_Event_Click(object sender, RoutedEventArgs e)
        {
            (new Window1(CalendarContol.SelectedDate, collections, CalendarContol, refresh_Event_list)).Show();
        }
        /// <summary>
        /// Uruchomienie okna usuwania wydarzenia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Event_Click(object sender, RoutedEventArgs e)
        {
            (new Delete_Ev()).Show();

        }
        /// <summary>
        /// Uruchomienie okna dodania typu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Type(object sender, RoutedEventArgs e)
        {
            (new Type_Add()).Show();
        }
        /// <summary>
        /// Uruchomienie okna usuwania typu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_Type(object sender, RoutedEventArgs e)
        {

            (new Del_Ev()).Show();
        }


    }
}
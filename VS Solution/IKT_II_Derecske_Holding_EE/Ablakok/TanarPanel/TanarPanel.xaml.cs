﻿using IKT_II_Derecske_Holding_EE.Ablakok.Stilus;
using IKT_II_Derecske_Holding_EE.Ablakok.TanarPanel;
using IKT_II_Derecske_Holding_EE.API_Data;
using IKT_II_Derecske_Holding_EE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace IKT_II_Derecske_Holding_EE.Ablakok.Tanar
{
    /// <summary>
    /// Interaction logic for TanarPanel.xaml
    /// </summary>
    public partial class TanarPanel : UserControl
    {
        TanarSzerverAdatok szerverAdatok;
        Dictionary<int,int> ujJegyek;
        Dictionary<int,Button> ujJegyekBtns;

        public TanarPanel()
        {
            InitializeComponent();
            ujJegyek = new();
            ujJegyekBtns = new();
            szerverAdatok = new();
            szerverAdatok.OsztalyokLekerdezve += () =>
            {
                OsztalyValasztoBox.ItemsSource = szerverAdatok.Osztalyok;
            };
            StatisztikaPanel.Content = new Statisztika();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
            OsztalyokBtn.Background = Szinek.ASPARAGUS;
        }

        private void AdatLekerdezes()
        {
            var osztaly = OsztalyValasztoBox.SelectedItem as Osztaly;
            string osztalyID = osztaly.ID;
            szerverAdatok = new(osztalyID);
            OsztalyTxt.Content = osztalyID;
            szerverAdatok.TanulokLekerdezve += () =>
            {
                TanuloAdatokGrid.ItemsSource = szerverAdatok.Tanulok;
            };
            szerverAdatok.OsztalyJegyekLekerdezve += () =>
            {
                JegyekGrid.ItemsSource = szerverAdatok.TesztJegy;
            };
            szerverAdatok.TantargyakLekerdezve += () =>
            {
                TantargyBox.ItemsSource = szerverAdatok.Tantargyak;
            };

        }

        private void OsztalyFulGomb(object sender, RoutedEventArgs e)
        {
            TabKonroll.SelectedIndex = 0;
            OsztalyokBtn.Background = Szinek.ASPARAGUS;
            OrarendBtn.Background = SystemColors.ControlBrush;
        }

        private void OrarendFulGomb(object sender, RoutedEventArgs e)
        {
            TabKonroll.SelectedIndex = 1;
            OrarendBtn.Background = Szinek.ASPARAGUS;
            OsztalyokBtn.Background = SystemColors.ControlBrush;
        }
        

        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("HH:mm:ss");
        }

        private void KilepesGomb(object sender, RoutedEventArgs e)
        {

        }

        private void UjJegy(object sender, RoutedEventArgs e)
        {
            UjJegyOszlop.Visibility = Visibility.Visible;
            UjJegyBtn.IsEnabled = false;
            MegseJegyBtn.Visibility = Visibility.Visible;
            JegyekMenteseBtn.Visibility = Visibility.Visible;
        }

        private void MegseJegy(object sender, RoutedEventArgs e)
        {
            UjJegyOszlop.Visibility = Visibility.Hidden;
            UjJegyBtn.IsEnabled = true;
            MegseJegyBtn.Visibility = Visibility.Hidden;
            JegyekMenteseBtn.Visibility = Visibility.Hidden;
        }

        private async void JegyekMentese(object sender, RoutedEventArgs e)
        {
            UjJegyOszlop.Visibility = Visibility.Hidden;
            UjJegyBtn.IsEnabled = true;
            MegseJegyBtn.Visibility = Visibility.Hidden;
            JegyekMenteseBtn.Visibility = Visibility.Hidden;
            adatPOST adatPOST = new adatPOST();
            int id = 54;
            foreach (var jegy in ujJegyek)
            {
                var tanulo = szerverAdatok.Tanulok.Where(x => x.ID == jegy.Key).FirstOrDefault();
                var tantargy = TantargyBox.SelectedItem as Tantargy;
                bool res = await adatPOST.JegyBevitel(new() { Datum = JegyDatum.DisplayDate, Jegy_Ertek = jegy.Value, Osztaly_ID = tanulo.Osztaly_ID, Tanar_ID = tantargy.Tanar_ID, Tantargy_ID = tantargy.ID, Tema = TemaTxt.Text, Tanulo_ID = jegy.Key, ID = id});
                id++;
                if (!res)
                {
                    MessageBox.Show("Sikertelen mentés!");
                    break;
                }
            }
            ujJegyek.Clear();
            AdatLekerdezes();
        }

        private void OsztalyValasztas(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex != -1) 
            { 
                AdatLekerdezes();
            }
        }

        private void Egyes_Jegy(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Szinek.RESEDA_GREEN;
            dynamic tanulo = JegyekGrid.SelectedValue;
            int id = (int)tanulo.id;
            if (ujJegyek.ContainsKey(id))
            {
                ujJegyekBtns[id].Background = SystemColors.ControlBrush;
                ujJegyekBtns.Remove(id);
                ujJegyek.Remove(id);
            }

            ujJegyekBtns.Add(id, btn);
            ujJegyek.Add(id, 1);
        }

        private void Kettes_Jegy(object sender, RoutedEventArgs e)
        {
            dynamic tanulo = JegyekGrid.SelectedValue;
            Button btn = sender as Button;
            btn.Background = Szinek.RESEDA_GREEN;
            int id = (int)tanulo.id;
            if (ujJegyek.ContainsKey(id)) 
            {
                ujJegyekBtns[id].Background = SystemColors.ControlBrush;
                ujJegyekBtns.Remove(id);
                ujJegyek.Remove(id);
            }

            ujJegyekBtns.Add(id, btn);
            ujJegyek.Add(id, 2);
        }

        private void Harmas_Jegy(object sender, RoutedEventArgs e)
        {
            dynamic tanulo = JegyekGrid.SelectedValue;
            Button btn = sender as Button;
            btn.Background = Szinek.RESEDA_GREEN;
            int id = (int)tanulo.id;
            if (ujJegyek.ContainsKey(id))
            {
                ujJegyekBtns[id].Background = SystemColors.ControlBrush;
                ujJegyekBtns.Remove(id);
                ujJegyek.Remove(id);
            }

            ujJegyekBtns.Add(id, btn);
            ujJegyek.Add(id, 3);
        }

        private void Negyes_Jegy(object sender, RoutedEventArgs e)
        {
            dynamic tanulo = JegyekGrid.SelectedValue;
            Button btn = sender as Button;
            btn.Background = Szinek.RESEDA_GREEN;
            int id = (int)tanulo.id;
            if (ujJegyek.ContainsKey(id))
            {
                ujJegyekBtns[id].Background = SystemColors.ControlBrush;
                ujJegyekBtns.Remove(id);
                ujJegyek.Remove(id);
            }

            ujJegyekBtns.Add(id, btn);
            ujJegyek.Add(id, 4);
        }

        private void Otos_Jegy(object sender, RoutedEventArgs e)
        {
            dynamic tanulo = JegyekGrid.SelectedValue;
            Button btn = sender as Button;
            btn.Background = Szinek.RESEDA_GREEN;
            int id = (int)tanulo.id;
            if (ujJegyek.ContainsKey(id))
            {
                ujJegyekBtns[id].Background = SystemColors.ControlBrush;
                ujJegyekBtns.Remove(id);
                ujJegyek.Remove(id);
            }

            ujJegyekBtns.Add(id, btn);
            ujJegyek.Add(id, 5);
        }

        private void NemIrt_Jegy(object sender, RoutedEventArgs e)
        {
            dynamic tanulo = JegyekGrid.SelectedValue;
            Button btn = sender as Button;
            btn.Background = Szinek.RESEDA_GREEN;
            int id = (int)tanulo.id;
            if (ujJegyek.ContainsKey(id))
            {
                ujJegyekBtns[id].Background = SystemColors.ControlBrush;
                ujJegyekBtns.Remove(id);
                ujJegyek.Remove(id);
            }

            ujJegyekBtns.Add(id, btn);
            ujJegyek.Add(id, -1);
        }
    }
}

﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using IKT_II_Derecske_Holding_EE.Models;
using IKT_II_Derecske_Holding_EE.API_Data;
using IKT_II_Derecske_Holding_EE.Ablakok.Stilus;
using System.Collections.ObjectModel;

namespace IKT_II_Derecske_Holding_EE.Ablakok.Login
{
    /// <summary>
    /// Interaction logic for LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : UserControl
    {
        SzerverAdatok SzerverAdatok;
        TanarSzerverAdatok TanarSzerverAdatok;
        public LoginPanel()
        {
            SzerverAdatok = new SzerverAdatok();
            TanarSzerverAdatok = new TanarSzerverAdatok("12.F");
            InitializeComponent();
            elsoBtn.Click += (s, e) => { tabs.SelectedIndex = 0; };
            masodikBtn.Click += (s, e) => { tabs.SelectedIndex = 1; };
            TanarSzerverAdatok.TanulokLekerdezve += () => { TestGrid.ItemsSource = TanarSzerverAdatok.Tanulok; };
            TanarSzerverAdatok.OsztalyJegyekLekerdezve += () =>
            {
                jegyekNew.ItemsSource = TanarSzerverAdatok.TesztJegy;
            };
        }

    }
}

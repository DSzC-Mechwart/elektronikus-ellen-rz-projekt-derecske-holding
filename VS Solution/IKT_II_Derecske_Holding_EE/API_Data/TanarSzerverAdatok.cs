﻿using IKT_II_Derecske_Holding_EE.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static IKT_II_Derecske_Holding_EE.API_Data.SzerverAdatok;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace IKT_II_Derecske_Holding_EE.API_Data
{
    public class TanarSzerverAdatok
    {
        public event AdatokLekerdezveD TanulokLekerdezve;
        public event AdatokLekerdezveD OsztalyJegyekLekerdezve;
        public event AdatokLekerdezveD OsztalyStatJegyekLekerdezve;
        public event AdatokLekerdezveD TantargyakLekerdezve;
        public event AdatokLekerdezveD OsztalyokLekerdezve;
        public event AdatokLekerdezveD SzakokLekerdezve;
        public event AdatokLekerdezveD MindenLekerdezve;

        /// <summary>
        /// A tanulók teljes listája.
        /// </summary>
        public ObservableCollection<Tanulo_Obj> Tanulok;
        /// <summary>
        /// Az összes osztály listája.
        /// </summary>
        public ObservableCollection<Osztaly> Osztalyok;
        /// <summary>
        /// A tantárgyak listája.
        /// </summary>
        public ObservableCollection<Tantargy> Tantargyak;

        public ObservableCollection<Szak> Szakok;

        public ObservableCollection<Orarend> Orarendek;
        public ObservableCollection<Jegy> OsztalyJegyek;

        public dynamic TesztJegy { get; set; }

        private int statAdatokLekerve;

        HttpClient client = new();

        public TanarSzerverAdatok(string osztalyID)
        {
            client.BaseAddress = new Uri("https://localhost:7181/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            Tantargyak = new();
            Orarendek = new();
            Tanulok = new();
            Osztalyok = new();
            OsztalyJegyek = new();
            GetTanulok(osztalyID);
            GetOsztalyJegyek(osztalyID);
            GetOsztalyJegyek2(osztalyID);
            GetTantargyak();
            GetOsztalyok();
            GetSzakok();
            statAdatokLekerve = 6;
        }

        public TanarSzerverAdatok()
        {
            client.BaseAddress = new Uri("https://localhost:7181/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );

            Tantargyak = new();
            Orarendek = new();
            Tanulok = new();
            Osztalyok = new();
            OsztalyJegyek = new();
            statAdatokLekerve = -1;
            GetOsztalyok();
            GetSzakok();
        }

        private void Lekerdezve()
        {
            if (statAdatokLekerve == 0)
            {
                MindenLekerdezve?.Invoke();
            }
        }


        private async void GetSzakok()
        {
            try
            {
                var response = await client.GetStringAsync("api/Szakok");
                var szakok = JsonConvert.DeserializeObject<ObservableCollection<Szak>>(response);
                Szakok = szakok??[];
                SzakokLekerdezve?.Invoke();
                statAdatokLekerve--;
                Lekerdezve();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver");
            }

        }

        private async void GetOsszesTanulok()
        {
            try
            {
                var response = await client.GetStringAsync("api/Tanulo");
                var tanulok = JsonConvert.DeserializeObject<ObservableCollection<Tanulo_Obj>>(response);
                Tanulok = tanulok ??[];
                TanulokLekerdezve.Invoke();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver");
            }

        }

        private async void GetTanulok(string id)
        {
            try
            {
                var response = await client.GetStringAsync($"api/Tanulo/{id}");
                var tanulok = JsonConvert.DeserializeObject<ObservableCollection<Tanulo_Obj>>(response);
                Tanulok = tanulok??[];
                TanulokLekerdezve?.Invoke();
                statAdatokLekerve--;
                Lekerdezve();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver");
            }

        }

        private async void GetOsztalyJegyek(string id)
        {
            try
            {
                OsztalyJegyek.Clear();
                var response = await client.GetStringAsync($"api/Jegyek/osztalyok/{id}");
                var jegyek = JsonConvert.DeserializeObject<ObservableCollection<Jegy>>(response);
                OsztalyJegyek = jegyek??[];
                OsztalyStatJegyekLekerdezve?.Invoke();
                statAdatokLekerve--;
                Lekerdezve();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver (Jegyek)");
                throw;
            }

        }

        private async void GetOsztalyJegyek2(string id)
        {
            try
            {
                OsztalyJegyek.Clear();
                var response = await client.GetStringAsync($"api/Jegyek/osztalyok/vmi/{id}");
                dynamic des = JsonConvert.DeserializeObject(response);
                foreach (dynamic tanulo in des)
                {
                    JObject honapLista = new ();
                    for (int i = 1; i < 13; i++)
                    {
                        if (i != 7 || i != 8)
                        {
                            string honap = "";
                            if (tanulo.jegyek[$"{i}"] != null)
                            {
                                foreach (dynamic jegy in tanulo.jegyek[$"{i}"])
                                {
                                    if ((int)jegy.jegy_Ertek > 0)
                                    {
                                        honap += $"{jegy.jegy_Ertek} ";
                                    }
                                    else
                                    {
                                        honap += $"- ";
                                    }
                                }
                            }
                            honapLista[$"{i}"] = honap;
                        }
                    }
                    tanulo.honapok = honapLista;
                }
                TesztJegy = des;
                statAdatokLekerve--;
                OsztalyJegyekLekerdezve?.Invoke();
                Lekerdezve();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver (Jegyek)");
            }

        }

        private async void GetOsztalyok()
        {
            try
            {
                var response = await client.GetStringAsync("api/Osztaly");
                var osztalyok = JsonConvert.DeserializeObject<ObservableCollection<Osztaly>>(response);
                Osztalyok = osztalyok??[];
                OsztalyokLekerdezve?.Invoke();
                statAdatokLekerve--;
                Lekerdezve();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver");
                throw ;
            }

        }

        private async void GetTantargyak()
        {
            try
            {
                var response = await client.GetStringAsync("api/Tantargyak");
                var tantargyak = JsonConvert.DeserializeObject<ObservableCollection<Tantargy>>(response);
                Tantargyak = tantargyak??[];
                TantargyakLekerdezve?.Invoke();
                statAdatokLekerve--;
                Lekerdezve();
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver");
            }

        }



        private async void GetOrarend()
        {
            try
            {
                var response = await client.GetStringAsync("api/Orarendek/osszes");
                //var orarendek = JsonConvert.DeserializeObject<ObservableCollection<Orarend>>(response);
                //Orarendek = orarendek;
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Nem található a szerver");
            }

        }

    }
}

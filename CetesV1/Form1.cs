using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace CetesV1
{

    public partial class Form1 : Form
    {
        public string today = DateTime.Now.Date.ToString(@"yyyy-MM-dd");
        public static DateTime lastWeekTime = DateTime.Now.AddDays(-8);
        public string lastWeek = lastWeekTime.ToString(@"yyyy-MM-dd");
        HttpClient httpClient;
        public Form1()
        {
            
            const string cetes28 = "SF43936";
            const string cetes91 = "SF43939";
            const string cetes182 = "SF43942";
            const string cetes365 = "SF43945";


            InitializeComponent();
            Cetes(cetes28);
            Cetes(cetes91);
            Cetes(cetes182);
            Cetes(cetes365);
            /*
            Cetes91();
            Cetes182();
            CetesYear();
            */
            this.ShowInTaskbar = false;
            this.SetDesktopLocation(0, 500);

        }

        private void SetHeaders(ref HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Bmx-Token", "59f68331def4d98402e513b26652dc7a18d81f6fca748180b05e729c58178a47");
        }

        private async Task<string> SendRequest(string codigo)
        {
            Uri apiBaseUrl = new Uri($"https://www.banxico.org.mx/SieAPIRest/service/v1/series/{codigo}/datos/{lastWeek}/{today}");
            httpClient = new HttpClient
            {
                BaseAddress = apiBaseUrl
            };
            SetHeaders(ref httpClient);
            HttpResponseMessage response = await httpClient.GetAsync(apiBaseUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private Dato GetTasa(dynamic output,int datoActual)
        {
            var data = output["bmx"]["series"][0]["datos"][datoActual];
            var nuevaTasaString = JsonConvert.SerializeObject(data["dato"]);
            nuevaTasaString = nuevaTasaString.Replace("\"", "");
            return new Dato
            {
                Fecha = JsonConvert.SerializeObject(data["fecha"]),
                Tasa = float.Parse(nuevaTasaString)
            };
        }

        private async Task<dynamic> ValidarTasasLength(string codigo)
        {
            string receive = await SendRequest(codigo);
            //string to json
            dynamic output = JsonConvert.DeserializeObject(receive);
            var length = output["bmx"]["series"][0]["datos"].Count;
            //string receive = "";
            //dynamic output;
            while (length < 2)
            {
                lastWeekTime=lastWeekTime.AddDays(-8);
                lastWeek = lastWeekTime.ToString(@"yyyy-MM-dd");
                receive = await SendRequest(codigo);
                output = JsonConvert.DeserializeObject(receive);
                length = output["bmx"]["series"][0]["datos"].Count;
            }
            return output;
        }

        private async void Cetes(string codigo)
        {
            Dato nuevaTasa;
            dynamic output=await ValidarTasasLength(codigo);
            Dato tasa = GetTasa(output,0);


            /*
            var data = output["bmx"]["series"][0]["datos"][0];
            string tasaString = JsonConvert.SerializeObject(data["dato"]);
            tasaString = tasaString.Replace("\"", "");
            float tasa = float.Parse(tasaString);
            mes.Text = tasaString;
            */

            switch (codigo)
            {
                case "SF43936":
                    mesPasada.Text = tasa.Tasa + "";
                    nuevaTasa = GetTasa(output,1);
                    mes.Text = nuevaTasa.Tasa+"";
                    fechaMes.Text = nuevaTasa.Fecha;
                    mes.BackColor = (tasa.Tasa > nuevaTasa.Tasa) ? Color.Red :  Color.Green;
                    break;
                case "SF43939":
                    tresMesesPasada.Text = tasa.Tasa + "";
                    nuevaTasa = GetTasa(output, 1);
                    tresMeses.Text = nuevaTasa.Tasa + "";
                    fechaTres.Text = nuevaTasa.Fecha;
                    tresMeses.BackColor = (tasa.Tasa > nuevaTasa.Tasa) ? Color.Red : Color.Green;
                    break;
                case "SF43942":
                    medioPasada.Text = tasa.Tasa + "";
                    nuevaTasa = GetTasa(output, 1);
                    medio.Text = nuevaTasa.Tasa + "";
                    fechaMedio.Text = nuevaTasa.Fecha;
                    medio.BackColor = (tasa.Tasa > nuevaTasa.Tasa) ? Color.Red : Color.Green;
                    break;
                case "SF43945":
                    yearPasada.Text = tasa.Tasa + "";
                    nuevaTasa = GetTasa(output, 1);
                    year.Text = nuevaTasa.Tasa + "";
                    fechaYear.Text = nuevaTasa.Fecha;
                    year.BackColor = (tasa.Tasa > nuevaTasa.Tasa) ? Color.Red : Color.Green;
                    break;
                default:
                    break;
            }


            


            //date.Text = JsonConvert.SerializeObject(data["fecha"]);
            
            /*
                "datos": [
                    {
                        "fecha": "03/11/2022",
                        "dato": "9.40"
                    },
                    {
                        "fecha": "10/11/2022",
                        "dato": "9.19"
                    }
                ] 
             * */

            /*
                lastWeek.ToString(@"yyyy-MM-dd");
            //string to json
            dynamic output = JsonConvert.DeserializeObject(receive);
            var data = output["bmx"]["series"][0]["datos"][0];
            date.Text = JsonConvert.SerializeObject(data["fecha"]);
            cetes28.Text = JsonConvert.SerializeObject(data["dato"]); 
            */
        }
        /*
        private async void Cetes182()
        {

            //6 meses
            Uri apiBaseUrl = new Uri($"https://www.banxico.org.mx/SieAPIRest/service/v1/series/{cetes182}/datos/oportuno");
            httpClient = new HttpClient
            {
                BaseAddress = apiBaseUrl
            };
            SetHeaders(ref httpClient);

            HttpResponseMessage response = await httpClient.GetAsync(apiBaseUrl);
            string receive = await response.Content.ReadAsStringAsync();

            //string to json
            dynamic output = JsonConvert.DeserializeObject(receive);
            var data = output["bmx"]["series"][0]["datos"][0];
            //date.Text = JsonConvert.SerializeObject(data["fecha"]);
            medio.Text = JsonConvert.SerializeObject(data["dato"]);
        }

        private async void Cetes91()
        {
            //28
            Uri apiBaseUrl = new Uri($"https://www.banxico.org.mx/SieAPIRest/service/v1/series/{cetes182}/datos/oportuno");
            httpClient = new HttpClient
            {
                BaseAddress = apiBaseUrl
            };
            SetHeaders(ref httpClient);

            HttpResponseMessage response = await httpClient.GetAsync(apiBaseUrl);
            string receive = await response.Content.ReadAsStringAsync();

            //string to json
            dynamic output = JsonConvert.DeserializeObject(receive);
            var data = output["bmx"]["series"][0]["datos"][0];
            tresMeses.Text = JsonConvert.SerializeObject(data["dato"]);
        }

        private async void CetesYear()
        {
            Uri apiBaseUrl = new Uri($"https://www.banxico.org.mx/SieAPIRest/service/v1/series/{cetes365}/datos/oportuno");
            httpClient = new HttpClient
            {
                BaseAddress = apiBaseUrl
            };
            SetHeaders(ref httpClient);

            HttpResponseMessage response = await httpClient.GetAsync(apiBaseUrl);
            string receive = await response.Content.ReadAsStringAsync();

            //string to json
            dynamic output = JsonConvert.DeserializeObject(receive);
            var data = output["bmx"]["series"][0]["datos"][0];
            year.Text = JsonConvert.SerializeObject(data["dato"]);
        }
        */
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}

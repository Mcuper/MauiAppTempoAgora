using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "0bb89bfb2e3c02a360ba44e4464f101d";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                                     $"q={cidade}&units=metric&appid={chave}&lang=pt_br";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JObject.Parse(json);

                    //DateTime time = new();
                    //DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    //DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    //t = new()
                    //{
                    //    lat = (double)rascunho["coord"]["lat"],
                    //    lon = (double)rascunho["coord"]["lon"],
                    //    description = (string)rascunho["weather"][0]["description"],
                    //    main = (string)rascunho["weather"][0]["main"],
                    //    temp_min = (double)rascunho["main"]["temp_min"],
                    //    temp_max = (double)rascunho["main"]["temp_max"],
                    //    speed = (double)rascunho["wind"]["speed"],
                    //    visibility = (int)rascunho["visibility"],
                    //    sunrise = sunrise.ToString(),
                    //    sunset = sunset.ToString(),
                    // 1. Correção das Datas (Unix Timestamp para DateTime)
                    long sunriseUnix = (long)rascunho["sys"]["sunrise"];
                    long sunsetUnix = (long)rascunho["sys"]["sunset"];

                    // Converte os segundos para o horário local (Brasília/SP)
                    DateTime sunrise = DateTimeOffset.FromUnixTimeSeconds(sunriseUnix).LocalDateTime;
                    DateTime sunset = DateTimeOffset.FromUnixTimeSeconds(sunsetUnix).LocalDateTime;

                    // 2. Preenchimento do Objeto Tempo com os novos campos
                    t = new Tempo()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],

                        // Dados solicitados na atividade:
                        description = (string)rascunho["weather"][0]["description"], // Descrição textual
                        speed = (double)rascunho["wind"]["speed"],                  // Velocidade do vento
                        visibility = (int)rascunho["visibility"],                  // Visibilidade

                        // Dados de temperatura e sol:
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        main = (string)rascunho["weather"][0]["main"],

                        // Formatando para exibir Dia/Mês/Ano e Hora:Minuto
                        sunrise = sunrise.ToString("dd/MM/yyyy HH:mm"),
                        sunset = sunset.ToString("dd/MM/yyyy HH:mm")
                    }; // Fecha obj do Tempo.
                } // Fecha if se o status do servidor foi de sucesso
                else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("Cidade não encontrada. Verifique o nome e tente novamente.");
                }  //fecha loop de cidade não encontrada
                else
                {
                    throw new Exception("Erro ao consultar o clima. Tente mais tarde.");
                } // fecha loop de internet não disponível
            } // fecha laço using

            return t;
        }
    }
}

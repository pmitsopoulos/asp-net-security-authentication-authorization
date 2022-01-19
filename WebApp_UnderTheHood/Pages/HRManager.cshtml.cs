using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "ManagerOfHumanResource")]
    public class HRManager : PageModel
    {
        private readonly ILogger<HRManager> _logger;
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; }

        public HRManager(ILogger<HRManager> logger, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            _logger = logger;
        }



        public async Task OnGetAsync()
        {
            /*


            var http = httpClientFactory.CreateClient("OurWebApi");

            //First we try to get the token from the session
            JwtToken token = null;

            var strTokenObj = HttpContext.Session.GetString("access_token");

            //checking if the token is available in the session
            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate(http);
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);
            }

            //checking if the token has expired
            if (token == null ||
                string.IsNullOrEmpty(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate(http);
            }





            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            weatherForecastItems = await http.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
            */
            weatherForecastItems = await InvokeEndopoint<List<WeatherForecastDTO>>("OurWebApi", "WeatherForecast");
        }

        private async Task<T> InvokeEndopoint<T>(string clientName, string endpoint)
        {

            var http = httpClientFactory.CreateClient(clientName);

            //First we try to get the token from the session
            JwtToken token = null;

            var strTokenObj = HttpContext.Session.GetString("access_token");

            //checking if the token is available in the session
            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate(http);
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);
            }

            //checking if the token has expired
            if (token == null ||
                string.IsNullOrEmpty(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate(http);
            }
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await http.GetFromJsonAsync<T>(endpoint);
        }


        private async Task<JwtToken> Authenticate(HttpClient http)
        {


            //Authenticate to auth endpoint of our WebApi Prroject
            var res = await http.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "pass" });

            //Vallidate that the authenticatiion was a success
            res.EnsureSuccessStatusCode();

            string sa = await res.Content.ReadAsStringAsync();

            //convert to c# object and return it
            return JsonConvert.DeserializeObject<JwtToken>(sa);

        }
    }
}
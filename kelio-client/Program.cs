using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kelio_client
{
  static class Program
  {
    /// <summary>
    /// Point d'entrée principal de l'application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Console.WriteLine("main");
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Console.WriteLine("main 2");
      // requests();
      Application.Run(new MainForm());
    }

    static async void requests()
    {
      Console.WriteLine("requests");
      Uri baseAddress = new Uri("***REMOVED***");
      CookieContainer cookieContainer = new CookieContainer();
      using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
      using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
      {
        // 1 - get login
        HttpResponseMessage response = await client.GetAsync("/open/login");
        string responseBody = await response.Content.ReadAsStringAsync();
        string csrfToken = new Regex(@"<input type=""hidden"" name=""_csrf_bodet"" value=""([a-z0-9\-]+)"" \/>")
                                .Match(responseBody).Groups[1].Value;

        // 2 - post login
        var postData = new Dictionary<string, string>();
        postData.Add("ACTION", "ACTION_VALIDER_LOGIN");
        postData.Add("username", "mcollomb");
        postData.Add("password", "***REMOVED***");
        postData.Add("_csrf_bodet", csrfToken);
        response = await client.PostAsync("/open/j_spring_security_check", new FormUrlEncodedContent(postData));
        responseBody = await response.Content.ReadAsStringAsync();

        // 3 - get tokens
        response = await client.GetAsync("/open/homepage?ACTION=intranet&asked=1&header=0");
        responseBody = await response.Content.ReadAsStringAsync();
        string token = new Regex(@"<input type=""hidden"" name=""JETON_INTRANET"" id=""JETON_INTRANET"" value=""([0-9]+)"" >")
                            .Match(responseBody).Groups[1].Value;
        csrfToken = new Regex(@"<input type=""hidden"" name=""_csrf_bodet"" value=""([a-z0-9\-]+)"" \/>")
                            .Match(responseBody).Groups[1].Value;
        Console.WriteLine("JETON_INTRANET : " + token);
        Console.WriteLine("_csrf_token : " + csrfToken);
      }
    }
  }
}

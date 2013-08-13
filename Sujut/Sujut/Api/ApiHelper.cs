using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using Sujut.Core;

namespace Sujut.Api
{
    public class ApiHelper
    {
        public static string BaseApiUrl = "https://sujut.apphb.com/";
        private const string CredentialsFolderName = "Credentials";
        private const string CredentialsFileName = "Credentials";

        public static bool UserIsLoggedIn()
        {
            return GetUserNameAndPassword() != null;
        }

        public static void SaveCredentials(string username, string password)
        {
            var stringToStore = username + ":" + password;

            // Obtain an isolated store for an application.
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists(CredentialsFolderName))
                    {
                        store.CreateDirectory(CredentialsFolderName);
                    }

                    var filePath = Path.Combine(CredentialsFolderName, CredentialsFileName);

                    if (store.FileExists(filePath))
                    {
                        // Can only have one logged-in user at a time (at this point)
                        store.DeleteFile(filePath);
                    }
                
                    try
                    {
                        using (var sw = new StreamWriter(store.CreateFile(filePath)))
                        {
                            sw.WriteLine(stringToStore);
                        }
                    }
                    catch (IsolatedStorageException ex)
                    {
                        // TODO: Handle that file could not be written to
                    }                 
                }
            }
            catch (IsolatedStorageException ex)
            {
                // TODO: Handle that store was unable to be accessed.
            }
        }

        public static Uri GetFullApiCallUri(string uri)
        {
            return new Uri(BaseApiUrl + uri, UriKind.Absolute);
        }

        public static WebClient AuthClient()
        {
            var usernameAndPassword = GetUserNameAndPassword();

            if (usernameAndPassword == null)
            {
                throw new Exception("Username and password not in Isolated storage.");
            }

            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.Authorization] = "Basic " + 
                Convert.ToBase64String(Encoding.UTF8.GetBytes(usernameAndPassword));

            return webClient;
        }

        public static Participant CurrentUser(DebtCalculation calculation)
        {
            var usernameAndPassword = GetUserNameAndPassword();

            if (usernameAndPassword == null)
            {
                throw new Exception("Username and password not in Isolated storage.");
            }

            var username = usernameAndPassword.Split(':')[0];

            var currentUser = calculation.Participants.Single(p => p.Email == username);

            return currentUser;
        }

        private static string GetUserNameAndPassword()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.DirectoryExists(CredentialsFolderName))
                    {
                        var filePath = Path.Combine(CredentialsFolderName, CredentialsFileName);

                        try
                        {
                            using (var reader = new StreamReader(store.OpenFile(filePath, FileMode.Open, FileAccess.Read)))
                            {
                                var contents = reader.ReadToEnd();

                                return contents.Trim();
                            }
                        }
                        catch (IsolatedStorageException ex)
                        {
                        }
                    }
                }
            }
            catch (IsolatedStorageException ex)
            {
                // TODO: Handle that store was unable to be accessed.
            }

            return null;
        }
    }
}

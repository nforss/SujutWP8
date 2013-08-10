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

namespace Sujut.Api
{
    public class ApiHelper
    {
        public static string BaseApiUrl = "";
        private const string CredentialsFolderName = "Credentials";
        private const string CredentialsFileName = "Credentials";

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

                                return contents;
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

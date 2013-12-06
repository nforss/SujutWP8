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
using Sujut.SujutApi;

namespace Sujut.Api
{
    public class ApiHelper
    {
        public static string BaseApiUrl = "https://sujut.apphb.com/";
        private const string CredentialsFolderName = "Credentials";
        private const string CredentialsFileName = "Credentials";
        private const string CurrentUserIdFolderName = "CurrentUserID";
        private const string CurrentUserIdFileName = "CurrentUserID";

        private static readonly Uri ApiUri = new Uri(BaseApiUrl + "api/");

        public static Container GetContainer()
        {
            var usernameAndPswd = GetUserNameAndPassword();

            var username = usernameAndPswd.Split(':').First();
            var password = usernameAndPswd.Split(':').Last();

            var container = new Container(ApiUri)
                {
                    Credentials = new NetworkCredential(username, password)
                };

            return container;
        }

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

        public static void SaveCurrentUserId(long id)
        {
            var stringToStore = id.ToString();

            // Obtain an isolated store for an application.
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists(CurrentUserIdFolderName))
                    {
                        store.CreateDirectory(CurrentUserIdFolderName);
                    }

                    var filePath = Path.Combine(CurrentUserIdFolderName, CurrentUserIdFileName);

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

        public static void Logout()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    var filePath = Path.Combine(CredentialsFolderName, CredentialsFileName);

                    if (store.FileExists(filePath))
                    {
                        store.DeleteFile(filePath);
                    }

                    filePath = Path.Combine(CurrentUserIdFolderName, CurrentUserIdFileName);

                    if (store.FileExists(filePath))
                    {
                        store.DeleteFile(filePath);
                    }
                }
            }
            catch (IsolatedStorageException ex)
            {
                // TODO: Handle that store was unable to be accessed.
            }
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

        public static long CurrentUserId()
        {
            return GetCurrentUserId();
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

        private static long GetCurrentUserId()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.DirectoryExists(CurrentUserIdFolderName))
                    {
                        var filePath = Path.Combine(CurrentUserIdFolderName, CurrentUserIdFileName);

                        try
                        {
                            using (var reader = new StreamReader(store.OpenFile(filePath, FileMode.Open, FileAccess.Read)))
                            {
                                var contents = reader.ReadToEnd();

                                return long.Parse(contents.Trim());
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

            return 0;
        }
    }
}

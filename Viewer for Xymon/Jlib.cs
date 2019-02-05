using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Media;

namespace Viewer_for_Xymon
{
    public static class Jlib
    {
        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            if (hex.Length == 6) hex = hex.Insert(0, "ff");
            if (hex.Length == 8)
            {
                byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
                SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
                return myBrush;
            }
            else
            {
                // Return gray if faulty input
                SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 127, 127, 127));
                return myBrush;
            }
        }

        public static async Task<string> getUserInfo(string p)
        {
            IReadOnlyList<User> users = await User.FindAllAsync();
            if (users.Count == 0) Debug.WriteLine("No users found");
            if (users.Count > 1) Debug.WriteLine("More than 1 user found");
            if (users.Count == 1)
            {
                string displayName = (string)await users[0].GetPropertyAsync(KnownUserProperties.DisplayName);
                string domainName = (string)await users[0].GetPropertyAsync(KnownUserProperties.DomainName);
                string login = domainName.Substring(domainName.IndexOf("\\") + 1);
                string email = (string)await users[0].GetPropertyAsync(KnownUserProperties.PrincipalName);

                Debug.WriteLine(displayName);
                Debug.WriteLine(domainName);
                Debug.WriteLine(login);
                Debug.WriteLine(email);

                switch (p)
                {
                    case "Display name": return displayName;
                    case "Domain name": return domainName;
                    case "Domain user": return login;
                    case "RFC 822": return email;
                    default: return "Parameter not found";
                }

            }
            return "getUserInfo failed";
        }

    }
}

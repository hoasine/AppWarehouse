using AppName;
using AppName.Model;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;


public class CheckConnectInternet
{
    public static string GetmethodName([CallerMemberName] string methodname = null)
    {
        return methodname;
    }

    public static bool IsConnectedNotClearCookie()
    {
        ILogger logger = DependencyService.Get<ILogManager>().GetLog();

        try
        {
            try
            {
                PopupNavigation.Instance.PopAllAsync();
            }
            catch (Exception)
            {
            }

            var isconnected = CrossConnectivity.Current.IsConnected;
            if (isconnected == false)
            {
                logger.Error("Please check the device has 3G/LTE or WIFI connection enabled.", "Error");

                var dialog = new NotificationWarrningPopup("CheckConnectInternet", "IsConnectedNotClearCookie") { Message = "Please check the device has 3G/LTE or WIFI connection enabled." };
                PopupNavigation.Instance.PushAsync(dialog);

                //Application.Current.Properties["IsLogin"] = false;
                //Application.Current.Properties["UserName"] = "";
                //Application.Current.Properties["RoleID"] = "";
                //Application.Current.Properties["Token"] = "";
                //Application.Current.Properties["DateExpires"] = "";

                //Application.Current.MainPage = new NavigationPage(new LoginFrm());

                return false;
            }
            else
            {
                string responsePingIP = "";
                var ipStr = RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault();

                if (ipStr == null)
                {
                    return false;
                }
                else
                {
                    var ipStrURL = ipStr.URLApi.ToUpper().Replace("HTTPS://", "").Replace("HTTP://", "").Split(":");

                    var canReach = ResourceHelper.PingWithResponse(10, ipStrURL[0], out responsePingIP);

                    //  canReach = true;

                    if (canReach == false)
                    {
                        logger.Error(responsePingIP, "Error");

                        var dialog = new NotificationWarrningPopup("CheckConnectInternet", "IsConnectedNotClearCookie") { Message = responsePingIP };
                        PopupNavigation.Instance.PushAsync(dialog);

                        //Application.Current.Properties["IsLogin"] = false;
                        //Application.Current.Properties["UserName"] = "";
                        //Application.Current.Properties["RoleID"] = "";
                        //Application.Current.Properties["Token"] = "";
                        //Application.Current.Properties["DateExpires"] = "";

                        //Application.Current.MainPage = new NavigationPage(new LoginFrm());

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

            var dialog = new NotificationWarrningPopup("CheckConnectInternet", "IsConnectedNotClearCookie") { Message = ex.Message.ToString() };
            PopupNavigation.Instance.PushAsync(dialog);

            return false;
        }
    }

    public static bool IsConnectedNotClearCookieNotLogin()
    {
        ILogger logger = DependencyService.Get<ILogManager>().GetLog();

        try
        {
            try
            {
                PopupNavigation.Instance.PopAllAsync();
            }
            catch (Exception)
            {
            }

            var isconnected = CrossConnectivity.Current.IsConnected;

            if (isconnected == false)
            {
                logger.Error("Please check the device has 3G/LTE or WIFI connection enabled.", "Error");

                var dialog = new NotificationWarrningPopup("CheckConnectInternet", CheckConnectInternet.GetmethodName()) { Message = "Please check the device has 3G/LTE or WIFI connection enabled." };
                PopupNavigation.Instance.PushAsync(dialog);

                return false;
            }
            else
            {
                string responsePingIP = "";
                var ipStr = RealmHelper.Instance.All<UserLSRetailConfig>().FirstOrDefault();

                if (ipStr == null)
                {
                    return false;
                }
                else
                {
                    var ipStrURL = ipStr.URLApi.ToUpper().Replace("HTTPS://", "").Replace("HTTP://", "").Split(":");

                    var canReach = ResourceHelper.PingWithResponse(10, ipStrURL[0], out responsePingIP);

                    // canReach = true;

                    if (canReach == false)
                    {
                        logger.Error(responsePingIP, "Error");

                        var dialog = new NotificationWarrningPopup("CheckConnectInternet", CheckConnectInternet.GetmethodName()) { Message = responsePingIP };
                        PopupNavigation.Instance.PushAsync(dialog);

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("Login Error: " + ex.Message.ToString() + ".", "Error");

            var dialog = new NotificationWarrningPopup("CheckConnectInternet", CheckConnectInternet.GetmethodName()) { Message = ex.Message.ToString() };
            PopupNavigation.Instance.PushAsync(dialog);

            return false;
        }
    }
}

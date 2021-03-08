using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheWebConnect
{
    public class TheWebConnectFactory
    {
        TheREST MyRest = new TheREST();
        public TheRequestData MyRequestData = null;

        string mPostMimeType = "application/x-www-form-urlencoded";
        string mTokenLocator;
        bool IsConnected;
        uint mRetryOnError;
        uint mReadyEvery;
        Uri mDeviceUrl;
        bool WasDisconnected = false;
        string mDevicePost=null;
        cdeConcurrentDictionary<string, string> mAddHeader;
        public Action<bool, TheRequestData> eventConnectionChanged;
        public Action<string, TheRequestData> eventMessage;
        public Action<string, TheRequestData> eventTokenReceived;
        public Action<string, TheRequestData> eventDeviceDataReceived;
        public bool WaitForService = false;

        public TheWebConnectFactory(uint ReadEvery=5000, uint RetryOnError=15000, string pMimeType=null)
        {
            if (!string.IsNullOrEmpty(pMimeType))
                mPostMimeType = pMimeType;
            mRetryOnError = RetryOnError;
            mReadyEvery = ReadEvery;
        }

        public void UpdateReadCycle(uint pIntervalInMs)
        {
            if (pIntervalInMs > 3000)
                mReadyEvery = pIntervalInMs;
        }

        public void ForceDisconnect()
        {
            IsConnected = false;
            WasDisconnected = true;
            eventConnectionChanged?.Invoke(IsConnected, MyRequestData);
        }

        /// <summary>
        /// Gets a token, header or just cookies from Target Website
        /// </summary>
        /// <param name="pTokenUrl">Url Of the token page</param>
        /// <param name="pTokenLocator">null for just cookies; HEADER: to fetch a specific header entry; otherwise a matchpattern for a token inside a page</param>
        /// <returns></returns>
        public bool FetchToken(Uri pTokenUrl, string pTokenLocator=null)
        {
            if (pTokenUrl == null) return false;
            mTokenLocator = pTokenLocator;
            var MyRequestData = new TheRequestData { RequestUri = pTokenUrl };// new Uri("https://ptdevices.com/login") };
            TheREST.GetRESTAsync(MyRequestData, sinkLoginFetchSuccess, sinkError);
            return true;
        }

        void sinkError(TheRequestData pData)
        {
            eventMessage?.Invoke(pData.ErrorDescription, pData);
            if (IsConnected && mRetryOnError>0)
            {
                TheCommonUtils.SleepOneEye(mRetryOnError, 100);
                GetDevices();
            }
        }

        void sinkLoginFetchSuccess(TheRequestData pdata)
        {
            var tRes = TheCommonUtils.CArray2UTF8String(pdata.ResponseBuffer);
            MyRequestData = pdata;
            if (!string.IsNullOrEmpty(mTokenLocator))
            {
                if (mTokenLocator.StartsWith("HEADER:"))
                {
                    var tHead = mTokenLocator.Substring("HEADER:".Length);
                    if (pdata.Header.TryGetValue(tHead, out string tHeader))
                    {
                        eventTokenReceived?.Invoke(tHeader, pdata);
                        return;
                    }
                }
                else
                {
                    var pos = tRes.IndexOf(mTokenLocator); // "name=\"_token\" value=\"");
                    if (pos > 0)
                    {
                        pos += mTokenLocator.Length; /// "name=\"_token\" value=\"".Length;
                        var pos2 = tRes.IndexOf("\"", pos);
                        var tToken = tRes.Substring(pos, pos2 - pos);
                        eventTokenReceived?.Invoke(tToken, pdata);
                        return;
                    }
                }
            }
            eventTokenReceived?.Invoke(null, pdata);
        }

        public void DoLogin(Uri pLoginUrl, string tPost, string MimeTypeOverride = null, cdeConcurrentDictionary<string, string> paddHeader = null)
        {
            if (MyRequestData == null)
                MyRequestData = new TheRequestData();
            MyRequestData.PostData = TheCommonUtils.CUTF8String2Array(tPost);
            MyRequestData.RequestUri = pLoginUrl;
            MyRequestData.HttpMethod = "POST";
            MyRequestData.Header = null;
            if (paddHeader != null)
            {
                MyRequestData.Header = new cdeConcurrentDictionary<string, string>();
                foreach (string key in paddHeader.Keys)
                    MyRequestData.Header.TryAdd(key, paddHeader[key]);
            }
            if (!string.IsNullOrEmpty(MimeTypeOverride))
                MyRequestData.ResponseMimeType = MimeTypeOverride; // "application/x-www-form-urlencoded; charset=UTF-8";
            else
                MyRequestData.ResponseMimeType = mPostMimeType; // "application/x-www-form-urlencoded; charset=UTF-8";
            MyRequestData.ResponseBuffer = null;
            MyRequestData.TimeOut = 15000;
            MyRequestData.DisableRedirect = true;
            MyRequestData.RequestCookies = MyRequestData?.SessionState?.StateCookies;
            MyRest.PostRESTAsync(MyRequestData, LoginSuccess, sinkError);
        }

        void LoginSuccess(TheRequestData pData)
        {
            MyRequestData = pData;
            IsConnected = true;
            eventConnectionChanged?.Invoke(IsConnected, pData);
        }

        public void GetDevices(Uri pDeviceURL=null, string tPost=null, cdeConcurrentDictionary<string,string> addHeader=null)
        {
            if (!IsConnected || !TheBaseAssets.MasterSwitch)
                return;
            if (pDeviceURL != null)
                mDeviceUrl = pDeviceURL;
            if (tPost != null)
                mDevicePost = tPost;
            if (addHeader != null)
                mAddHeader = addHeader;
            if (mDevicePost == null)
            {
                TheRequestData tRequest = new TheRequestData
                {
                    RequestUri = mDeviceUrl,
                    RequestCookies = MyRequestData?.SessionState?.StateCookies
                };
                if (mAddHeader!=null)
                {
                    tRequest.Header = new cdeConcurrentDictionary<string, string>();
                    foreach (string key in mAddHeader.Keys)
                        tRequest.Header.TryAdd(key, mAddHeader[key]);
                }
                TheREST.GetRESTAsync(tRequest, ParseDevices, sinkError);
            }
            else
            {
                MyRequestData.RequestUri = mDeviceUrl; // new Uri("https://ptdevices.com/device/all");
                //tPost = $"_token={WebSocketSharp.Ext.UrlEncode(mToken)}";
                MyRequestData.PostData = TheCommonUtils.CUTF8String2Array(mDevicePost);
                MyRequestData.HttpMethod = "POST";
                MyRequestData.Header = null;
                if (mAddHeader != null)
                {
                    MyRequestData.Header = new cdeConcurrentDictionary<string, string>();
                    foreach (string key in mAddHeader.Keys)
                        MyRequestData.Header.TryAdd(key, mAddHeader[key]);
                }

                MyRequestData.RequestCookies = MyRequestData?.SessionState?.StateCookies;
                MyRequestData.ResponseMimeType = mPostMimeType; // "application/x-www-form-urlencoded; charset=UTF-8";
                while (MyRest.IsPosting > 1 && IsConnected)
                {
                    TheCommonUtils.SleepOneEye(1000, 100);
                }
                if (IsConnected)
                    MyRest.PostRESTAsync(MyRequestData, ParseDevices, sinkError);
            }
        }


        public bool PostToWeb(Uri pUrl, string tPost, Action<TheRequestData> pCallback, string MimeTypeOverride=null, cdeConcurrentDictionary<string, string> paddHeader = null, string pMethodOveride="POST")
        {
            if (!IsConnected || !TheBaseAssets.MasterSwitch || MyRequestData == null || pUrl == null)
                return false;
            TheRequestData tD = MyRequestData;
            if (!string.IsNullOrEmpty(tPost))
                tD.PostData = TheCommonUtils.CUTF8String2Array(tPost);
            else
                tD.PostData = null;
            tD.RequestUri = pUrl; // new Uri($"https://ptdevices.com{Url}");
            tD.HttpMethod = pMethodOveride;
            tD.Header = null;
            if (paddHeader != null)
            {
                tD.Header = new cdeConcurrentDictionary<string, string>();
                foreach (string key in paddHeader.Keys)
                    tD.Header.TryAdd(key, paddHeader[key]);
            }
            tD.DisableRedirect = true;
            tD.RequestCookies = MyRequestData?.SessionState?.StateCookies;
            if (!string.IsNullOrEmpty(MimeTypeOverride))
                tD.ResponseMimeType = MimeTypeOverride; // "application/x-www-form-urlencoded; charset=UTF-8";
            else
                tD.ResponseMimeType = mPostMimeType; // "application/x-www-form-urlencoded; charset=UTF-8";
            while (MyRest.IsPosting > 1 && IsConnected)
            {
                TheCommonUtils.SleepOneEye(1000, 100);
            }
            if (IsConnected)
                MyRest.PostRESTAsync(tD, pCallback, sinkError);
            return true;
        }

        void ParseDevices(TheRequestData pdata)
        {
            MyRequestData = pdata;
            var tRes = TheCommonUtils.CArray2UTF8String(pdata.ResponseBuffer);
            eventDeviceDataReceived?.Invoke(tRes, pdata);

            if (IsConnected)
            {
                do
                {
                    TheCommonUtils.SleepOneEye(mReadyEvery, 100); //TheCommonUtils.CUInt(TheThing.GetSafePropertyNumber(MyBaseThing, "ReadEvery"))
                    if (WasDisconnected || !TheBaseAssets.MasterSwitch)
                    {
                        WasDisconnected = false;
                        return;
                    }
                } while (WaitForService);
                GetDevices();
            }
        }
    }
}

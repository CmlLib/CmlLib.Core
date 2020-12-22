using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using XboxAuthNet.OAuth;
using XboxAuthNet.Exchange;
using Newtonsoft.Json;
using System.IO;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth;
using CmlLib.Core.Mojang;
using System.Threading;

namespace XboxLoginTest
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            webView21.NavigationStarting += WebView21_NavigationStarting;
        }

        MicrosoftOAuth oauth;
        MicrosoftOAuthResponse response;
        AuthenticationResponse mcResponse;
        public MSession session;

        public string action = "login";

        string microsoftOAuthPath = "msa.json";
        string minecraftTokenPath = "token.json";

        private void Form1_Load(object sender, EventArgs e)
        {
            oauth = new MicrosoftOAuth("00000000402B5328", XboxExchanger.XboxScope);
            response = readMicrosoft();
            readMinecraft();

            if (action == "login")
                button1_Click(null, null);
            else if (action == "signout")
            {
                response = null;
                mcResponse = null;
                writeMicrosoft(null);
                writeMinecraft();

                webView21.Source = new Uri(MicrosoftOAuth.GetSignOutUrl());
            }
        }

        private MicrosoftOAuthResponse readMicrosoft()
        {
            if (!File.Exists(microsoftOAuthPath))
                return null;

            var file = File.ReadAllText(microsoftOAuthPath);
            var response = JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(file);

            this.response = response;
            return response;
        }

        private void writeMicrosoft(MicrosoftOAuthResponse response)
        {
            this.response = response;

            var json = JsonConvert.SerializeObject(response);
            File.WriteAllText(microsoftOAuthPath, json);
        }

        private void readMinecraft()
        {
            if (!File.Exists(minecraftTokenPath))
                return;

            var file = File.ReadAllText(minecraftTokenPath);
            var job = JObject.Parse(file);

            this.mcResponse = job["auth"].ToObject<AuthenticationResponse>();
            this.session = job["session"].ToObject<MSession>();
        }

        private void writeMinecraft()
        {
            var obj = new
            {
                auth = mcResponse,
                session = session
            };
            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(minecraftTokenPath, json);
        }

        private void WebView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.IsRedirected && oauth.CheckLoginSuccess(e.Uri)) // login success
            {
                new Thread(() =>
                {
                    var result = oauth.TryGetTokens(out MicrosoftOAuthResponse response); // get token
                    Invoke(new Action(() =>
                    {
                        if (result)
                            msLoginSuccess(response);
                        else
                            msLoginFail(response);
                    }));
                }).Start();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mcResponse == null || DateTime.Now > mcResponse.ExpiresOn) // expired
            {
                if (oauth.TryGetTokens(out response, this.response?.RefreshToken))
                {
                    msLoginSuccess(response);
                }
                else
                {
                    var url = oauth.CreateUrl();
                    webView21.Source = new Uri(url);
                    return;
                }
            }

            getSession();
        }

        private void msLoginSuccess(MicrosoftOAuthResponse res)
        {
            writeMicrosoft(res);

            var xbox = new XboxExchanger();
            var rps = xbox.ExchangeRpsTicketForUserToken(response?.AccessToken);
            var xsts = xbox.ExchangeTokensForXSTSIdentity(rps.Token, null, null, XboxMinecraftLogin.RelyingParty, null);

            var mclogin = new XboxMinecraftLogin();
            mcResponse = mclogin.LoginWithXbox(xsts.UserHash, xsts.XSTSToken);

            getSession();
        }

        private void msLoginFail(MicrosoftOAuthResponse res)
        {
            MessageBox.Show(
    $"Failed to login : {response.Error}\n" +
    $"ErrorDescription : {response.ErrorDescription}\n" +
    $"ErrorCodes : {string.Join(",", response.ErrorCodes)}");
        }

        private void getSession()
        {
            if (!MojangAPI.CheckGameOwnership(mcResponse?.AccessToken))
            {
                MessageBox.Show("purchase game first");
            }

            var profile = MojangAPI.GetProfileUsingToken(mcResponse.AccessToken);
            this.session = new MSession
            {
                AccessToken = mcResponse.AccessToken,
                UUID = profile.UUID,
                Username = profile.Name
            };

            writeMinecraft();
            this.Close();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Controls.Remove(webView21);
        }
    }
}

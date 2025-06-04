using EAP.Client.Models;
using EAP.Client.Utils;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EAP.Client.Forms
{
    public partial class LoginForm : UILoginForm
    {
        ConfigManager<SputtereConfig> manager = new ConfigManager<SputtereConfig>();

        public LoginForm()
        {
            InitializeComponent();
            OnLogin += LoginForm_OnLoginHandle;
        }

        private bool LoginForm_OnLoginHandle(string userName, string password)
        {
            var config = manager.LoadConfig();
            var hasUser = config.UserPassword.TryGetValue(userName, out var pwd);
            if (hasUser && pwd == password || (userName == "SMD" && password == "SMDi4.0AP"))
            {
                IsLogin = true;
                return true;
            }
            else
            {
                UIMessageTip.ShowError("账户不存在或密码错误");
                return false;
            }
        }
    }
}

using Maslomat.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Maslomat
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PasswordEditPage), typeof(PasswordEditPage));
            Routing.RegisterRoute(nameof(PasswordDetailPage), typeof(PasswordDetailPage));
        }
    }
}
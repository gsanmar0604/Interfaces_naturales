using Interfaces_naturales.View;
using Microsoft.Extensions.DependencyInjection;

namespace Interfaces_naturales
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

    }
}
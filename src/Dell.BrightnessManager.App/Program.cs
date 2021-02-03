using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace Dell.BrightnessManager.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();

            services
                .AddSingleton<TrayApplication,TrayApplication>()
                .AddLogging(builder =>
                {
                    builder.AddFile($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}"
                    + "\\DellBrightnessManager\\Logs\\log_{Date}.log", LogLevel.Information);
                });

            Application.Run(services.BuildServiceProvider().GetRequiredService<TrayApplication>());

        }
    }
}

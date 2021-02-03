using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SunRadiation.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dell.BrightnessManager.App
{
    class TrayApplication : ApplicationContext
    {
        private readonly ILogger<TrayApplication> logger;
        private readonly ILoggerFactory loggerFactory;

        private DellBrightnessManager brightnessManager;

        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayIconContextMenu;
        private ToolStripMenuItem CloseMenuItem;

        public TrayApplication(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<TrayApplication>();
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            InitializeComponent();
            TrayIcon.Visible = true;

            StartAsync(CancellationToken.None);
        }

        private void InitializeComponent()
        {
            TrayIcon = new NotifyIcon();

            TrayIcon.BalloonTipIcon = ToolTipIcon.Info;
            TrayIcon.BalloonTipTitle = "Hello there, fellow Dell user!";
            TrayIcon.BalloonTipText =
              "Do you want me to brighten up ypur day? ;)";
            TrayIcon.Text = "Dell Brightness Manager";

            TrayIcon.Icon = Icon.ExtractAssociatedIcon(Path.GetDirectoryName(typeof(TrayApplication).Assembly.Location) + "\\d_4464.ico");

            TrayIcon.DoubleClick += TrayIcon_DoubleClick;

            TrayIconContextMenu = new ContextMenuStrip();
            CloseMenuItem = new ToolStripMenuItem();
            TrayIconContextMenu.SuspendLayout();

            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] {
            this.CloseMenuItem});
            this.TrayIconContextMenu.Name = "TrayIconContextMenu";
            this.TrayIconContextMenu.Size = new Size(153, 70);

            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.Size = new Size(152, 22);
            this.CloseMenuItem.Text = "Close Dell Brightness Manager";
            this.CloseMenuItem.Click += new EventHandler(this.CloseMenuItem_Click);

            TrayIconContextMenu.ResumeLayout(false);
            TrayIcon.ContextMenuStrip = TrayIconContextMenu;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var localSettingsPath = System.IO.Path.GetDirectoryName(typeof(TrayApplication).Assembly.Location)
                + "\\settings.json";

            var settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\DellBrightnessManager\\settings.json";

            if (!File.Exists(settingsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
                File.Copy(localSettingsPath, settingsPath);
            }

            var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
            var sunRadiationRepository = new SunRadiationRepository(settings.ForecastURL);

            brightnessManager = new DellBrightnessManager(
                settings: settings,
                sunRadiationRepository: sunRadiationRepository,
                logger: loggerFactory.CreateLogger<DellBrightnessManager>());

            logger.LogInformation("Brightness Manager Initialized");

            return Task.CompletedTask;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            TrayIcon.Visible = false;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            //Here, you can do stuff if the tray icon is doubleclicked
            TrayIcon.ShowBalloonTip(10000);
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to close me?",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}

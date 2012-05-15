using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FM;

namespace PassThru
{
		public partial class MainWindow: Form 
        {
			public MainWindow() 
            {
				InitializeComponent();                
			}

			public void Exit() {
                try
                {
                    ac.Kill();
                    Application.Exit();
                }
                catch { }
			}
			
            private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) 
            {
				if (e.CloseReason == CloseReason.UserClosing)
				{
					this.Visible = false;
					e.Cancel = true;
                    //ac.Kill();
				}
			}

            //RuleEditor re;
            Tabs.LogDisplay log;
            AdapterControl ac;
            OptionsDisplay od;
            Help help;

			private void MainWindow_Load(object sender, EventArgs e) 
            {
                ColorScheme.SetColorScheme(this);
                //optionsTab.ItemSize = new Size((this.Width / 4) - 6, optionsTab.ItemSize.Height);
                System.Reflection.Assembly target = System.Reflection.Assembly.GetExecutingAssembly();
                this.Icon = new System.Drawing.Icon(target.GetManifestResourceStream("PassThru.Resources.newIcon.ico"));
                LogCenter.ti = new TrayIcon();
                Program.uc = new UpdateChecker();
                Program.uc.Updater();
                PassThru.Tabs.DownloadCenter.Instance.Width = 800;
                PassThru.Tabs.DownloadCenter.Instance.Height = 600;
                PassThru.Tabs.DownloadCenter.Instance.Show();
                PassThru.Tabs.DownloadCenter.Instance.Hide();
                // call the log purger
                LogCenter.cleanLogs();

                log = new Tabs.LogDisplay();
                log.Dock = DockStyle.Fill;
                ColorScheme.SetColorScheme(log);
                LogCenter.PushLogEvent += new LogCenter.NewLogEvent(log.Instance_PushLogEvent);
                splitContainer1.Panel2.Controls.Add(log);

                // load up the adapter control handler
				ac = new AdapterControl();
                ColorScheme.SetColorScheme(ac);
				ac.Dock = DockStyle.Fill;
				//tabPage3.Controls.Add(ac);

                // load up the options tab handler
                od = new OptionsDisplay();
                ColorScheme.SetColorScheme(od);
                od.Dock = DockStyle.Fill;
                //tabPage2.Controls.Add(od);

                // load up the options tab handler
                help = new Help();
                ColorScheme.SetColorScheme(help);
                help.Dock = DockStyle.Fill;
                //tabPage4.Controls.Add(help);

                ColorScheme.ThemeChanged += new System.Threading.ThreadStart(ColorScheme_ThemeChanged);

                switch (LanguageConfig.GetCurrentLanguage())
                {
                    case LanguageConfig.Language.NONE:
                    case LanguageConfig.Language.ENGLISH:
                        tabPage1.Text = "Log";
                        tabPage2.Text = "Options";
                        tabPage3.Text = "Adapters";
                        break;
                    case LanguageConfig.Language.CHINESE:
                        tabPage1.Text = "登录";
                        tabPage2.Text = "选项";
                        tabPage3.Text = "适配器";
                        break;
                    case LanguageConfig.Language.GERMAN:
                        tabPage1.Text = "Log";
                        tabPage2.Text = "Optionen";
                        tabPage3.Text = "Adapter";
                        break;
                    case LanguageConfig.Language.RUSSIAN:
                        tabPage1.Text = "журнал";
                        tabPage2.Text = "опции";
                        tabPage3.Text = "Адаптеры";
                        break;
                    case LanguageConfig.Language.SPANISH:
                        tabPage1.Text = "log";
                        tabPage2.Text = "opciones";
                        tabPage3.Text = "adaptadores";
                        break;
                    case LanguageConfig.Language.PORTUGUESE:
                        tabPage1.Text = "Entrar";
                        tabPage2.Text = "opções";
                        tabPage3.Text = "adaptadores";
                        break;
                }
                MainWindow_Resize(null, null);
                if (TrayIcon.StartMinimized)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
                ColorScheme_ThemeChanged();
			}

            void ColorScheme_ThemeChanged()
            {
                ColorScheme.SetColorScheme(this);
                ColorScheme.SetColorScheme(log);
                ColorScheme.SetColorScheme(ac);
                ColorScheme.SetColorScheme(od);
                ColorScheme.SetColorScheme(help);
                splitContainer1.Panel1.BackgroundImage = ColorScheme.GetCurrentBanner();
            }

            private void MainWindow_Resize(object sender, EventArgs e)
            {
                tabPage1.Location = new Point(2 * splitContainer1.Panel1.Width / 20 - tabPage1.Width / 2, (splitContainer1.Panel1.Height / 2) - (tabPage1.Height / 2) - 4);
                tabPage2.Location = new Point(14 * splitContainer1.Panel1.Width / 20 - tabPage2.Width / 2, (splitContainer1.Panel1.Height / 2) - (tabPage2.Height / 2) - 4);
                tabPage3.Location = new Point(6 * splitContainer1.Panel1.Width / 20 - tabPage3.Width / 2, (splitContainer1.Panel1.Height / 2) - (tabPage3.Height / 2) - 4);
                tabPage4.Location = new Point(18 * splitContainer1.Panel1.Width / 20 - tabPage4.Width / 2, (splitContainer1.Panel1.Height / 2) - (tabPage4.Height / 2) - 4);                
            }

            private void tabPage1_Click(object sender, EventArgs e)
            {
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(log);
            }

            private void tabPage2_Click(object sender, EventArgs e)
            {
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(od);
            }

            private void tabPage2_Click_1(object sender, EventArgs e)
            {
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(ac);
            }

            private void tabPage4_Click(object sender, EventArgs e)
            {
                splitContainer1.Panel2.Controls.Clear();
                splitContainer1.Panel2.Controls.Add(help);
            }

            private void tabPage1_MouseClick(object sender, MouseEventArgs e)
            {
                
            }

            private void tabPage3_Click(object sender, EventArgs e)
            {
                tabPage2_Click_1(sender, e);
            }

            private void tabPage1_Click_1(object sender, EventArgs e)
            {
                tabPage1_Click(sender, e);
            }

            private void tabPage2_Click_2(object sender, EventArgs e)
            {
                tabPage2_Click(sender, e);
            }

            private void tabPage4_Click_1(object sender, EventArgs e)
            {
                tabPage4_Click(sender, e);
            }

            private void button1_Click(object sender, EventArgs e)
            {
                tabPage1_Click_1(sender, e);
            }

            private void tabPage2_Click_3(object sender, EventArgs e)
            {
                tabPage2_Click_2(sender, e);
            }

            private void tabPage3_Click_1(object sender, EventArgs e)
            {
                tabPage3_Click(sender, e);
            }

            private void tabPage4_Click_2(object sender, EventArgs e)
            {
                tabPage4_Click_1(sender, e);
            }

            private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
            {
                MainWindow_Resize(null, null);
            }
		}
}

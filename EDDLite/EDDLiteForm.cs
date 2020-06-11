using EliteDangerousCore;
using EliteDangerousCore.DB;
using EliteDangerousCore.EDSM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace EDDLite
{
    public partial class EDDLiteForm : EDDLite.DraggableFormPos
    {
        EDDLiteController controller;
        Timer datetimetimer;

        public EDDLiteForm()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("App data " + EDDOptions.Instance.AppDataDirectory);

            UserDatabase.Instance.Start("UserDB");
            UserDatabase.Instance.Initialize();

            BaseUtils.Icons.IconSet.CreateSingleton();
            BaseUtils.Icons.IconSet.Instance.Add("Default", Properties.Resources.Logo);     // to satisfy the journal, add the backup Default in

            EDDConfig.Instance.Update();

            EDDLiteTheme.Instance.ApplyStd(this);

            RestoreFormPositionRegKey = "MainForm";

            extStatusStrip.Font =  this.Font;
            label_version.Text = EDDOptions.Instance.VersionDisplayString;
            labelGameDateTime.Text = "";
            labelInfoBoxTop.Text = "";
            extButtonEDSM.Enabled = extButtonEDSY.Enabled = extButtonCoriolis.Enabled = false;
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionBackColor = EDDLiteTheme.Instance.GridCellBack;    // hide selection
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionForeColor = EDDLiteTheme.Instance.GridCellText;

            controller = new EDDLiteController();
            controller.Start(this,a => BeginInvoke(a));

            if (!EDDOptions.Instance.DisableTimeDisplay)
            {
                datetimetimer = new Timer();
                datetimetimer.Interval = 1000;
                datetimetimer.Tick += (sv, ev) => { DateTime gameutc = DateTime.UtcNow.AddYears(1286); labelGameDateTime.Text = gameutc.ToShortDateString() + " " + gameutc.ToShortTimeString(); };
                datetimetimer.Start();

                timeToolStripMenuItem.Checked = labelGameDateTime.Visible = UserDatabase.Instance.GetSettingBool("TimeDisplay", true);

                this.timeToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.timeToolStripMenuItem_CheckStateChanged);
            }

            this.cmdrViewToolStripMenuItem.Checked = panelCmdrs.Visible = UserDatabase.Instance.GetSettingBool("CmdrView", true);
            this.cmdrViewToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.cmdrViewToolStripMenuItem_CheckStateChanged);

            splitContainerDataLogs.SplitterDistance(UserDatabase.Instance.GetSettingDouble("DataLogSplitter", 0.8));

            EliteDangerousCore.IGAU.IGAUClass.SoftwareName =
            EliteDangerousCore.EDDN.EDDNClass.SoftwareName =
            EliteDangerousCore.Inara.InaraClass.SoftwareName = 
            EDSMClass.SoftwareName = "EDDLite";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            controller.Stop();
            EDSMJournalSync.StopSync();
            base.OnClosing(e);
            UserDatabase.Instance.PutSettingDouble("DataLogSplitter", splitContainerDataLogs.GetSplitterDistance());
        }

        #region Controller feedback

        public void JournalReadProgress(string s )
        {
            toolStripStatus.Text = s;
        }

        public void ReadJournals()
        {
            LogLine("Journals fully read" + Environment.NewLine);

            dataGridViewCommanders.AutoGenerateColumns = false;             // BEFORE assigned to list..
            dataGridViewCommanders.DataSource = EDCommander.GetListCommanders();
        }

        HistoryEntry lasthe = null;

        public void HistoryEvent(HistoryEntry he, bool stored)
        {
            lasthe = he;
            he.journalEntry.FillInformation(out string info, out string detailed);

            LogLine(he.EventTimeUTC + " " + he.journalEntry.EventTypeStr + " " + info + Environment.NewLine);
            extButtonEDSM.Enabled = true;
            extButtonEDSY.Enabled = extButtonCoriolis.Enabled = he.ShipInformation != null;
            extButtonInaraStation.Enabled = he.IsDocked;
            extButtonInaraSystem.Enabled = true;
            labelCmdr.Text = he.Commander.Name + "/" + EDCommander.Current.Name;
            labelSystem.Text = he.System.Name;
            labelLocation.Text = he.WhereAmI;
            labelShip.Text = he.ShipInformation?.Name ?? "Unknown";
            labelData.Text = he.MaterialCommodity.DataCount.ToString();
            labelCargo.Text = he.MaterialCommodity.CargoCount.ToString();
            labelMaterials.Text = he.MaterialCommodity.MaterialsCount.ToString();

            if (!stored)
            {
                if (he.Commander.SyncToEdsm)
                {
                    EDSMJournalSync.SendEDSMEvents(LogLine, he);
                }

                if (he.Commander.SyncToIGAU)
                {
                   // EliteDangerousCore.IGAU.IGAUSync.NewEvent(LogLine, he);
                }

                if (EliteDangerousCore.EDDN.EDDNClass.IsEDDNMessage(he.EntryType, he.EventTimeUTC) && he.AgeOfEntry() < TimeSpan.FromDays(1.0) &&
                        he.Commander.SyncToEddn == true)
                {
                    EliteDangerousCore.EDDN.EDDNSync.SendEDDNEvents(LogLine, he);
                }

                if ( he.Commander.SyncToInara)
                {
                    EliteDangerousCore.Inara.InaraSync.NewEvent(LogLine,he);
                }

            }

        }

        public void UIEvent(UIEvent u)
        {
           // extRichTextBoxLog.AppendText( u.EventTimeUTC + " " + u.EventTypeStr + Environment.NewLine);
        }

        public void LogLine(string s)
        {
            if (Application.MessageLoop)
                extRichTextBoxLog.AppendText(s);
            else
                BeginInvoke((MethodInvoker) delegate { LogLine(s); });
        }

        #endregion

        #region Basic UI

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            OnCaptionMouseDown((Control)sender, e);
        }

        private void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            OnCaptionMouseDown((Control)sender, e);
        }

        private void panel_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel_eddiscovery_MouseClick(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region Commander editing

        private int rightclickrow = -1;
        private void dataGridViewCommanders_MouseDown(object sender, MouseEventArgs e)
        {
            dataGridViewCommanders.HandleClickOnDataGrid(e, out int unusedleftclickrow, out rightclickrow);
        }

        private void dataGridViewCommanders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                EditCmdr(e.RowIndex);
        }

        private void EditCmdr(int row)
        { 
            EDCommander cmdr = dataGridViewCommanders.Rows[row].DataBoundItem as EDCommander;

            EliteDangerousCore.Forms.CommanderForm cf = new EliteDangerousCore.Forms.CommanderForm();
            cf.Init(cmdr, false, true, true);

            if (cf.ShowDialog(FindForm()) == DialogResult.OK)
            {
                bool forceupdate = cf.Update(cmdr);
                List<EDCommander> edcommanders = (List<EDCommander>)dataGridViewCommanders.DataSource;
                EDCommander.Update(edcommanders, false);
                dataGridViewCommanders.Refresh();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rightclickrow >= 0)
                EditCmdr(rightclickrow);

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rightclickrow >= 0)
            {
                int row = dataGridViewCommanders.SelectedRows[0].Index;
                EDCommander cmdr = dataGridViewCommanders.Rows[row].DataBoundItem as EDCommander;

                var result = ExtendedControls.MessageBoxTheme.Show(FindForm(), "Do you wish to delete commander ".T(EDTx.UserControlSettings_DelCmdr) + cmdr.Name + "?", "Warning".T(EDTx.Warning), MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    EDCommander.Delete(cmdr);
                    UpdateCommandersListBox();
                }
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EliteDangerousCore.Forms.CommanderForm cf = new EliteDangerousCore.Forms.CommanderForm();
            cf.Init(true, true, true);

            if (cf.ShowDialog(FindForm()) == DialogResult.OK)
            {
                if (cf.Valid && !EDCommander.IsCommanderPresent(cf.CommanderName))
                {
                    EDCommander cmdr = new EDCommander();
                    cf.Update(cmdr);
                    EDCommander.Create(cmdr);
                    UpdateCommandersListBox();
                }
                else
                    ExtendedControls.MessageBoxTheme.Show(FindForm(), "Commander name is not valid or duplicate".T(EDTx.UserControlSettings_AddC), "Cannot create Commander".T(EDTx.UserControlSettings_AddT), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void UpdateCommandersListBox()
        {
            int selrow = dataGridViewCommanders.SelectedRows.Count > 0 ? dataGridViewCommanders.SelectedRows[0].Index : -1;
            dataGridViewCommanders.DataSource = null;
            List<EDCommander> cmdrs = EDCommander.GetListCommanders();
            dataGridViewCommanders.DataSource = cmdrs;
            if (selrow >= 0 && selrow < dataGridViewCommanders.RowCount)
                dataGridViewCommanders.Rows[selrow].Selected = true;
            dataGridViewCommanders.Update();
        }

        #endregion
        
        #region Form Buttons

        private void extButtonEDSM_Click(object sender, EventArgs e)
        {
            if (lasthe != null)
            {
                EDSMClass edsm = new EDSMClass();
                string url = edsm.GetUrlToEDSMSystem(lasthe.System.Name);

                if (url.Length > 0)         // may pass back empty string if not known, this solves another exception
                    System.Diagnostics.Process.Start(url);
                else
                    ExtendedControls.MessageBoxTheme.Show(FindForm(), "System unknown to EDSM");
            }

        }

        private void extButtonCoriolis_Click(object sender, EventArgs e)
        {
            if ( lasthe != null )
            {
                string errstr;
                string s = lasthe.ShipInformation.ToJSONCoriolis(out errstr);

                if (errstr.Length > 0)
                    ExtendedControls.MessageBoxTheme.Show(FindForm(), errstr + Environment.NewLine + "This is probably a new or powerplay module" + Environment.NewLine + "Report to EDD Team by Github giving the full text above", "Unknown Module Type");

                string uri = EDDConfig.Instance.CoriolisURL + "data=" + BaseUtils.HttpUriEncode.URIGZipBase64Escape(s) + "&bn=" + Uri.EscapeDataString(lasthe.ShipInformation.Name);

                if (!BaseUtils.BrowserInfo.LaunchBrowser(uri))
                {
                    ExtendedControls.InfoForm info = new ExtendedControls.InfoForm();
                    info.Info("Cannot launch browser, use this JSON for manual Coriolis import", FindForm().Icon, s);
                    info.ShowDialog(FindForm());
                }

            }
        }

        private void extButtonEDSY_Click(object sender, EventArgs e)
        {
            if ( lasthe != null )
            {
                Newtonsoft.Json.Linq.JObject jo = lasthe.ShipInformation.ToJSONLoadout();

                string loadoutjournalline = jo.ToString(Newtonsoft.Json.Formatting.Indented);

                //     File.WriteAllText(@"c:\code\loadoutout.txt", loadoutjournalline);

                string uri = EDDConfig.Instance.EDDShipyardURL + "#/I=" + BaseUtils.HttpUriEncode.URIGZipBase64Escape(loadoutjournalline);

                if (!BaseUtils.BrowserInfo.LaunchBrowser(uri))
                {
                    ExtendedControls.InfoForm info = new ExtendedControls.InfoForm();
                    info.Info("Cannot launch browser, use this JSON for manual ED Shipyard import", FindForm().Icon, loadoutjournalline);
                    info.ShowDialog(FindForm());
                }
            }
        }

        private void extButtonInaraSystem_Click(object sender, EventArgs e)
        {
            if (lasthe != null)
            {
                string uri = Properties.Resources.URLInaraStarSystem + HttpUtility.UrlEncode(lasthe.System.Name);
                BaseUtils.BrowserInfo.LaunchBrowser(uri);
            }

        }

        private void extButtonInaraStation_Click(object sender, EventArgs e)
        {
            if (lasthe != null)
            {
                if (lasthe.IsDocked)
                {
                    string uri = Properties.Resources.URLInaraStation + HttpUtility.UrlEncode(lasthe.System.Name + "[" + lasthe.WhereAmI +"]");
                    BaseUtils.BrowserInfo.LaunchBrowser(uri);
                }
            }

        }
        #endregion

        private void cmdrViewToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            panelCmdrs.Visible = cmdrViewToolStripMenuItem.Checked;
            UserDatabase.Instance.PutSettingBool("CmdrView", cmdrViewToolStripMenuItem.Checked);
        }

        private void timeToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            labelGameDateTime.Visible = timeToolStripMenuItem.Checked;
            UserDatabase.Instance.PutSettingBool("TimeDisplay", timeToolStripMenuItem.Checked);
        }
    }
}

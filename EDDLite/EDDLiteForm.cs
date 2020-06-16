/*
 * Copyright © 2020 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using EliteDangerousCore;
using EliteDangerousCore.DB;
using EliteDangerousCore.DLL;
using EliteDangerousCore.EDSM;
using EliteDangerousCore.ScreenShots;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Windows.Forms;

namespace EDDLite
{
    public partial class EDDLiteForm : EDDLite.Forms.DraggableFormPos
    {
        EDDLiteController controller;
        Timer datetimetimer;
        ScreenShotConverter screenshot;
        EDDDLLManager DLLManager;
        EDDDLLIF.EDDCallBacks DLLCallBacks;

        public EDDLiteForm()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("App data " + EDDOptions.Instance.AppDataDirectory);

            UserDatabase.Instance.Start("UserDB");
            UserDatabase.Instance.Initialize();

            BaseUtils.Icons.IconSet.CreateSingleton();
            BaseUtils.Icons.IconSet.Instance.Add("Default", Properties.Resources.Logo);     // to satisfy the journal, add the backup Default in

            EDDConfig.Instance.Update();

            EDDLiteTheme.Init();
            EDDLiteTheme.Instance.SetThemeByName(UserDatabase.Instance.GetSettingString("Theme", "EDSM"));
            ApplyTheme();

            RestoreFormPositionRegKey = "MainForm";

            extStatusStrip.Font = this.Font;
            label_version.Text = EDDOptions.Instance.VersionDisplayString;
            labelGameDateTime.Text = "";
            labelInfoBoxTop.Text = "";
            extButtonEDSM.Enabled = extButtonEDSY.Enabled = extButtonCoriolis.Enabled = false;
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionBackColor = EDDLiteTheme.Instance.GridCellBack;    // hide selection
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionForeColor = EDDLiteTheme.Instance.GridCellText;

            screenshot = new ScreenShotConverter();
            screenshotenableToolStripMenuItem.Checked = screenshot.AutoConvert;
            screenshotenableToolStripMenuItem.CheckedChanged += new System.EventHandler(this.enableToolStripMenuItem_CheckedChanged);

            controller = new EDDLiteController();
            controller.ProgressEvent += (s) => { toolStripStatus.Text = s; };
            controller.Refresh += ReadJournals;
            controller.NewEntry += HistoryEvent;

            controller.Start(a => BeginInvoke(a));

            if (!EDDOptions.Instance.DisableTimeDisplay)
            {
                datetimetimer = new Timer();
                datetimetimer.Interval = 1000;
                datetimetimer.Tick += (sv, ev) => { DateTime gameutc = DateTime.UtcNow.AddYears(1286); labelGameDateTime.Text = gameutc.ToShortDateString() + " " + gameutc.ToShortTimeString(); };
                datetimetimer.Start();

                timeToolStripMenuItem.Checked = labelGameDateTime.Visible = UserDatabase.Instance.GetSettingBool("TimeDisplay", true);

                this.timeToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.timeToolStripMenuItem_CheckStateChanged);
            }

            splitContainerCmdrDataLogs.SplitterDistance(UserDatabase.Instance.GetSettingDouble("CmdrDataLogSplitter", 0.1));
            splitContainerDataLogs.SplitterDistance(UserDatabase.Instance.GetSettingDouble("DataLogSplitter", 0.8));

            EliteDangerousCore.IGAU.IGAUClass.SoftwareName =
            EliteDangerousCore.EDDN.EDDNClass.SoftwareName =
            EliteDangerousCore.Inara.InaraClass.SoftwareName =
            EDSMClass.SoftwareName = "EDDLite";

            DLLManager = new EDDDLLManager();
            DLLCallBacks = new EDDDLLIF.EDDCallBacks(1, DLLRequestHistory, (s1, s2) => { return false; }, null);
        }



        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            screenshot.Start((a) => Invoke(a),
                             (b) => LogLine(b),
                             () =>
                             {
                                 if (lasthe != null)        // lasthe should have name and whereami, and an indication of commander
                                 {
                                     return new Tuple<string, string, string>(lasthe.System.Name, lasthe.WhereAmI, lasthe.Commander?.Name ?? "Unknown");
                                 }
                                 else
                                 {
                                     return new Tuple<string, string, string>("Unknown", "Unknown", "Unknown");
                                 }
                             }
                             );

            string alloweddlls = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingString("DLLAllowed", "");

            string verstring = EDDOptions.Instance.Version + ";EDLITE";

            Tuple<string, string, string> res = DLLManager.Load(EDDOptions.Instance.DLLAppDirectory(), verstring, EDDOptions.Instance.DLLAppDirectory(),
                                DLLCallBacks, alloweddlls);

            if (res.Item3.HasChars())
            {
                if (ExtendedControls.MessageBoxTheme.Show(this,
                                string.Format(("The following application extension DLLs have been found" + Environment.NewLine +
                                "Do you wish to allow these to be used?" + Environment.NewLine +
                                "{0} " + Environment.NewLine +
                                "If you do not, either remove the DLLs from the DLL folder in EDDLite Appdata"
                                ).T(EDTx.EDDiscoveryForm_DLLW), res.Item3),
                                "Warning".T(EDTx.Warning),
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    alloweddlls = alloweddlls.AppendPrePad(res.Item3, ",");
                    EliteDangerousCore.DB.UserDatabase.Instance.PutSettingString("DLLAllowed", alloweddlls);
                    DLLManager.UnLoad();
                    res = DLLManager.Load(EDDOptions.Instance.DLLAppDirectory(), verstring, EDDOptions.Instance.DLLAppDirectory(), DLLCallBacks, alloweddlls);
                }
            }

            if (res.Item1.HasChars())
                LogLine(string.Format("DLLs loaded: {0}".T(EDTx.EDDiscoveryForm_DLLL), res.Item1));
            if (res.Item2.HasChars())
                LogLine(string.Format("DLLs failed to load: {0}".T(EDTx.EDDiscoveryForm_DLLF), res.Item2));

            //EDDOptions.Instance.CheckRelease = true; // use this to force check for debugging

            Installer.CheckForNewInstallerAsync((rel) =>  // in thread
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    LogLine(string.Format("New EDDLite installer available: {0}".T(EDTx.EDDiscoveryForm_NI), rel.ReleaseName));
                    labelInfoBoxTop.Text = "New Release Available!".T(EDTx.EDDiscoveryForm_NRA);
                    if ( ExtendedControls.MessageBoxTheme.Show("New EDDLite Available, please upgrade!", "Warning".T(EDTx.Warning), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK )
                    {
                        System.Diagnostics.Process.Start(Properties.Resources.URLProjectReleases);
                    }
                });
            });

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            controller.Stop();
            EDSMJournalSync.StopSync();
            UserDatabase.Instance.PutSettingDouble("DataLogSplitter", splitContainerDataLogs.GetSplitterDistance());
            UserDatabase.Instance.PutSettingDouble("CmdrDataLogSplitter", splitContainerCmdrDataLogs.GetSplitterDistance());
            screenshot.Stop();
            screenshot.SaveSettings();
            DLLManager.UnLoad();
            base.OnClosing(e);
        }

        public bool DLLRequestHistory(long index, bool isjid, out EDDDLLIF.JournalEntry f)
        {
            f = new EDDDLLIF.JournalEntry();
            return false;
        }

        #region Controller feedback

        public void ReadJournals(HistoryEntry currenthe)
        {
            LogLine("Journals fully read");

            dataGridViewCommanders.AutoGenerateColumns = false;             // BEFORE assigned to list..
            dataGridViewCommanders.DataSource = EDCommander.GetListCommanders();

            if (currenthe != null)
            {
                DLLManager.Refresh(EDCommander.Current.Name, EDDDLLCallerHE.CreateFromHistoryEntry(currenthe));
                if (currenthe.Commander.SyncToInara)
                {
                    EliteDangerousCore.Inara.InaraSync.Refresh(LogLine, currenthe, currenthe.Commander);
                }
            }
        }

        HistoryEntry lasthe = null;

        public void HistoryEvent(HistoryEntry he, bool stored)
        {
            if ( lasthe == null || he.Commander.Name != lasthe.Commander.Name )
            {
                labelCmdr.Text = he.Commander.Name;
                if (EDCommander.Current.Name != he.Commander.Name)
                    labelCmdr.Text += " Report clash " + EDCommander.Current.Name;
            }

            if (lasthe == null || he.System.Name != lasthe.System.Name)
            {
                extButtonEDSM.Enabled = true;
                extButtonInaraSystem.Enabled = true;
                labelSystem.Text = he.System.Name;
            }

            if (lasthe == null || he.WhereAmI != lasthe.WhereAmI)
            {
                labelLocation.Text = he.WhereAmI;
            }

            extButtonEDSY.Enabled = extButtonCoriolis.Enabled = he.ShipInformation != null;

            if (he.ShipInformation != null && (lasthe == null || lasthe.ShipInformation == null || he.ShipInformation.ShipNameIdentType != lasthe.ShipInformation.ShipNameIdentType ))
            {
                labelShip.Text = he.ShipInformation.ShipNameIdentType ?? "Unknown";
            }

            if (lasthe == null || he.MaterialCommodity.DataCount != lasthe.MaterialCommodity.DataCount || he.MaterialCommodity.CargoCount != lasthe.MaterialCommodity.CargoCount
                                    || he.MaterialCommodity.MaterialsCount != lasthe.MaterialCommodity.MaterialsCount)
            {
                labelData.Text = he.MaterialCommodity.DataCount.ToString();
                labelCargo.Text = he.MaterialCommodity.CargoCount.ToString();
                labelMaterials.Text = he.MaterialCommodity.MaterialsCount.ToString();
            }

            if (lasthe == null || he.Credits != lasthe.Credits)
            {
                labelCredits.Text = he.Credits.ToString("N0");
            }

            he.journalEntry.FillInformation(out string info, out string detailed);
            LogLine(he.EventTimeUTC + " " + he.journalEntry.EventTypeStr + " " + info);

            extButtonInaraStation.Enabled = he.IsDocked;

            if (he.MissionList != null )
            {
                labelMissionCount.Text = he.MissionList.Missions.Count.ToString();
                string mtext = "";
                if (he.MissionList.Missions.Count > 0)
                {
                    var list = he.MissionList.GetAllCurrentMissions(DateTime.Now);
                    if (list.Count > 0)
                    {
                        var last = list[0];
                        mtext = BaseUtils.FieldBuilder.Build("", last.Mission.LocalisedName, "", last.Mission.Expiry, "", last.Mission.DestinationSystem, "", last.Mission.DestinationStation);
                    }
                }

                labelLatestMission.Text = mtext;
            }

            lasthe = he;

            if (!stored)
            {
                if (he.Commander.SyncToEdsm)
                {
                    EDSMJournalSync.SendEDSMEvents(LogLine, he);
                }

                if (he.Commander.SyncToIGAU)
                {
                    EliteDangerousCore.IGAU.IGAUSync.NewEvent(LogLine, he);
                }

                if (EliteDangerousCore.EDDN.EDDNClass.IsEDDNMessage(he.EntryType, he.EventTimeUTC) && he.AgeOfEntry() < TimeSpan.FromDays(1.0) &&
                        he.Commander.SyncToEddn == true)
                {
                  //  EliteDangerousCore.EDDN.EDDNSync.SendEDDNEvents(LogLine, he);
                }

                if (he.Commander.SyncToInara)
                {
                    EliteDangerousCore.Inara.InaraSync.NewEvent(LogLine, he);
                }

                if (DLLManager.Count > 0)       // if worth calling..
                    DLLManager.NewJournalEntry(EDDDLLCallerHE.CreateFromHistoryEntry(he));

                screenshot.NewJournalEntry(he.journalEntry);
            }
        }

        public void UIEvent(UIEvent u)
        {
           // extRichTextBoxLog.AppendText( u.EventTimeUTC + " " + u.EventTypeStr + Environment.NewLine);
        }

        public void LogLine(string s)       // can be called from other thread
        {
            if (Application.MessageLoop)
                extRichTextBoxLog.AppendText(s + Environment.NewLine);
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
            var frm = new Forms.AboutForm();
            frm.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel_eddiscovery_MouseClick(sender, null);
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
                controller.RequestRescan = true;
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
                    controller.RequestRescan = true;
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

        #region Menu UI

        private void timeToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            labelGameDateTime.Visible = timeToolStripMenuItem.Checked;
            UserDatabase.Instance.PutSettingBool("TimeDisplay", timeToolStripMenuItem.Checked);
        }

        #endregion

        private void enableToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            screenshot.AutoConvert = screenshotenableToolStripMenuItem.Checked;
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            screenshot.Configure(this);
        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = sender as ToolStripMenuItem;
            EDDLiteTheme.Instance.SetThemeByName(s.Text);
            UserDatabase.Instance.PutSettingString("Theme", s.Text);
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            EDDLiteTheme.Instance.ApplyStd(this);
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionBackColor = EDDLiteTheme.Instance.GridCellBack;    // hide selection
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionForeColor = EDDLiteTheme.Instance.GridCellText;
        }

    }
}

/*
 * Copyright © 2020-2022 EDDiscovery development team
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
 */

using EliteDangerousCore;
using EliteDangerousCore.DB;
using EliteDangerousCore.DLL;
using EliteDangerousCore.EDSM;
using EliteDangerousCore.ScreenShots;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Windows.Forms;

namespace EDDLite
{
    public partial class EDDLiteForm : EDDLite.Forms.DraggableFormPos
    {
        private EDDLiteController controller;
        private Timer datetimetimer;
        private ScreenShotConverter screenshot;
        private EDDDLLManager DLLManager;
        private EDDDLLInterfaces.EDDDLLIF.EDDCallBacks DLLCallBacks;
        private ExtendedControls.ThemeList ThemeList;

        private HistoryEntry queuedfsssd = null;

        int prevencodedcount, prevrawcount, prevmanucount, prevdatacount, previtemcount, prevconsumecount, prevcomponentcount, prevcargocount;

        #region Init

        public EDDLiteForm()
        {
            InitializeComponent();

            MaterialCommodityMicroResourceType.FillTable();     // lets statically fill the table way before anyone wants to access it

            // we need to add this in, even though we do not translate, so the forms using translation won't barfe.

            BaseUtils.Translator.Instance.AddExcludedControls(new Type[]
               {   typeof(ExtendedControls.ExtComboBox), typeof(ExtendedControls.NumberBoxDouble),typeof(ExtendedControls.NumberBoxFloat),typeof(ExtendedControls.NumberBoxLong),
                typeof(ExtendedControls.ExtScrollBar),typeof(ExtendedControls.ExtStatusStrip),typeof(ExtendedControls.ExtRichTextBox),typeof(ExtendedControls.ExtTextBox),
                typeof(ExtendedControls.ExtTextBoxAutoComplete),typeof(ExtendedControls.ExtDateTimePicker),typeof(ExtendedControls.ExtNumericUpDown) });

            string logpath = EDDOptions.Instance.LogAppDirectory();

            BaseUtils.LogClean.DeleteOldLogFiles(logpath, "*.hlog", 2, 256);        // Remove hlogs faster
            BaseUtils.LogClean.DeleteOldLogFiles(logpath, "*.log", 10, 256);

            BaseUtils.AppTicks.TickCountLap("MT", true);

            if (!System.Diagnostics.Debugger.IsAttached || EDDOptions.Instance.TraceLog != null)
            {
                BaseUtils.TraceLog.RedirectTrace(logpath, true, EDDOptions.Instance.TraceLog);
            }

            if (!System.Diagnostics.Debugger.IsAttached || EDDOptions.Instance.LogExceptions)
            {
                BaseUtils.ExceptionCatcher.RedirectExceptions(Properties.Resources.URLProjectFeedback);
            }

            if (EDDOptions.Instance.LogExceptions)
            {
                BaseUtils.FirstChanceExceptionCatcher.RegisterFirstChanceExceptionHandler();
            }

            BaseUtils.HttpCom.LogPath = logpath;

            // verify its first so its on top
            System.Diagnostics.Debug.Assert(extPanelScrollStatus.Controls[0] is ExtendedControls.ExtScrollBar);

            var appdata = EDDOptions.Instance.AppDataDirectory;     // FORCE ED options to Instance - do not remove.
            System.Diagnostics.Debug.WriteLine("App data " + appdata);

            UserDatabase.Instance.Name = "UserDB";
            UserDatabase.Instance.MinThreads = UserDatabase.Instance.MaxThreads = 2;        // set at 2 threads max/min
            UserDatabase.Instance.MultiThreaded = true;     // starts up the threads

            UserDatabase.Instance.Initialize();

            BaseUtils.Icons.IconSet.CreateSingleton();
            BaseUtils.Icons.IconSet.Instance.Add("Default", Properties.Resources.Logo);     // to satisfy the journal, add the backup Default in

            EDDConfig.Instance.Update();

            Bodies.Prepopulate();       // new! needed 

            BaseUtils.Icons.IconSet.Instance.DontReportMissingErrors = true;

            ThemeList = new ExtendedControls.ThemeList();
            ThemeList.LoadBaseThemes();                                         // default themes and ones on disk loaded

            string themename = UserDatabase.Instance.GetSettingString("Theme", "EDSM");
            highDPIToolStripMenuItem.Checked = themename.Contains("High DPI");
            this.highDPIToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.highDPIToolStripMenuItem_CheckStateChanged);
            SetTheme(themename);

            RestoreFormPositionRegKey = "MainForm";

            extStatusStrip.Font = this.Font;
            label_version.Text = EDDOptions.Instance.VersionDisplayString;
            labelGameDateTime.Text = "";
            labelInfoBoxTop.Text = "";

            extButtonEDSMSystem.Enabled = extButtonInaraSystem.Enabled = extButtonEDDBSystem.Enabled = false;
            extButtonInaraStation.Enabled = extButtonEDDBStation.Enabled = false;
            extButtonEDSY.Enabled = extButtonCoriolis.Enabled = false;

            useNotifyIconToolStripMenuItem.Checked = EDDConfig.Instance.UseNotifyIcon;
            useNotifyIconToolStripMenuItem.CheckedChanged += new System.EventHandler(this.useNotifyIconToolStripMenuItem_CheckedChanged);

            minimiseToNotificationAreaToolStripMenuItem.Checked = EDDConfig.Instance.MinimizeToNotifyIcon;
            minimiseToNotificationAreaToolStripMenuItem.CheckedChanged += new System.EventHandler(this.minimiseToNotificationAreaToolStripMenuItem_CheckedChanged);

            startMinimisedToolStripMenuItem.Checked = EDDConfig.Instance.StartMinimized;
            startMinimisedToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.startMinimizedToolStripMenuItem_CheckStateChanged);

            notifyIconEDD.Visible = EDDConfig.Instance.UseNotifyIcon;

            dataGridViewCommanders.RowsDefaultCellStyle.SelectionBackColor = ExtendedControls.Theme.Current.GridCellBack;    // hide selection
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionForeColor = ExtendedControls.Theme.Current.GridCellText;

            gameTimeToolStripMenuItem.Checked = false;
            utcToolStripMenuItem.Checked =
            localToolStripMenuItem.Checked = false;

            UpdateGameTimeTick();

            screenshot = new ScreenShotConverter();
            extButtonScreenshotDisabled.Visible = !screenshot.AutoConvert;

            controller = new EDDLiteController();
            controller.ProgressEvent += (s) => { toolStripStatus.Text = s; };
            controller.RefreshFinished += RefreshFinished;
            controller.NewEntry += HistoryEvent;
            controller.LogLine += LogLine;
            controller.NewUI += UIEvent;
            controller.Closed += ControllerClosed;

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
            splitContainerNamesButtonsScreenshot.SplitterDistance(UserDatabase.Instance.GetSettingDouble("NamesButtonsScreenshotSplitter", 0.8));
            EliteDangerousCore.IGAU.IGAUClass.SoftwareName =
            EliteDangerousCore.EDDN.EDDNClass.SoftwareName =
            EliteDangerousCore.Inara.InaraClass.SoftwareName =
            EliteDangerousCore.EDAstro.EDAstroClass.SoftwareName =
            EDSMClass.SoftwareName = "EDDLite";

         //   Bodies.Prepopulate();

            splitContainerNamesButtonsScreenshot.Panel2.Resize += SplitContainerNamesButtonsScreenshot_Resize;

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
                             },
                             8000
                             );

            screenshot.OnScreenshot += DisplayScreenshot;

            EDDDLLAssemblyFinder.AssemblyFindPaths.Add(EDDOptions.Instance.DLLAppDirectory());      // any needed assemblies from here
            AppDomain.CurrentDomain.AssemblyResolve += EDDDLLAssemblyFinder.AssemblyResolve;

            DLLManager = new EDDDLLManager();

            DLLCallBacks = new EDDDLLInterfaces.EDDDLLIF.EDDCallBacks();
            DLLCallBacks.ver = 2;       //explicit support
            DLLCallBacks.RequestHistory = DLLRequestHistory;
            DLLCallBacks.RunAction = (s1, s2) => { return false; };
            DLLCallBacks.GetShipLoadout = (s) => { return null; };

            string verstring = EDDOptions.Instance.Version;
            string[] options = new string[] { EDDDLLInterfaces.EDDDLLIF.FLAG_HOSTNAME + "EDDLITE",
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_JOURNALVERSION + EliteDangerousCore.DLL.EDDDLLCallerHE.JournalVersion.ToString(),
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_CALLBACKVERSION + DLLCallBacks.ver.ToString(),
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_CALLVERSION + EliteDangerousCore.DLL.EDDDLLCaller.DLLCallerVersion.ToStringInvariant(),
                                            };

            string alloweddlls = EDDConfig.Instance.DLLPermissions;

            Tuple<string, string, string,string> res = DLLManager.Load(new string[] { EDDOptions.Instance.DLLAppDirectory() }, new bool[] {false},
                                verstring,  options,
                                DLLCallBacks, ref alloweddlls,
                                 (name) => UserDatabase.Instance.GetSettingString("DLLConfig_" + name, ""), (name, set) => UserDatabase.Instance.PutSettingString("DLLConfig_" + name, set));

            if (res.Item3.HasChars())       // new DLLs
            {
                string[] list = res.Item3.Split(',');
                bool changed = false;
                foreach (var dll in list)
                {
                    if (ExtendedControls.MessageBoxTheme.Show(this,
                                    string.Format(("The following application extension DLL have been found" + Environment.NewLine +
                                    "Do you wish to allow these to be used?" + Environment.NewLine +
                                    "{0} " + Environment.NewLine
                                    ).T(EDTx.EDDiscoveryForm_DLLW), dll),
                                    "Warning".T(EDTx.Warning),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        alloweddlls = alloweddlls.AppendPrePad("+" + dll, ",");
                        changed = true;
                    }
                    else
                    {
                        alloweddlls = alloweddlls.AppendPrePad("-" + dll, ",");
                    }
                }

                EDDConfig.Instance.DLLPermissions = alloweddlls;

                if ( changed )
                {
                    DLLManager.UnLoad();
                    res = DLLManager.Load(new string[] { EDDOptions.Instance.DLLAppDirectory() }, new bool[] { false },
                                            verstring, options,
                                            DLLCallBacks, ref alloweddlls,
                                            (name) => UserDatabase.Instance.GetSettingString("DLLConfig_" + name, ""), (name, set) => UserDatabase.Instance.PutSettingString("DLLConfig_" + name, set));
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
                    if (ExtendedControls.MessageBoxTheme.Show("New EDDLite Available, please upgrade!", "Warning".T(EDTx.Warning), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        System.Diagnostics.Process.Start(Properties.Resources.URLProjectReleases);
                    }
                });
            });

            controller.Start(a => Invoke(a));       // synchronous, this means each controller action executes sync with the UI, so when controller is stopped, all controller actions stop

            if (EDDConfig.Instance.StartMinimized)
                WindowState = FormWindowState.Minimized;

            DLLManager.Shown();
        }

        protected void ControllerClosed()
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);

            EDSMJournalSync.StopSync();
            UserDatabase.Instance.PutSettingDouble("DataLogSplitter", splitContainerDataLogs.GetSplitterDistance());
            UserDatabase.Instance.PutSettingDouble("CmdrDataLogSplitter", splitContainerCmdrDataLogs.GetSplitterDistance());
            UserDatabase.Instance.PutSettingDouble("NamesButtonsScreenshotSplitter", splitContainerNamesButtonsScreenshot.GetSplitterDistance());
            screenshot.Stop();
            screenshot.SaveSettings();
            DLLManager.UnLoad();
            notifyIconEDD.Visible = false;
            notifyIconEDD.Dispose();
            cancelclosing = false;
            Close();
        }

        bool cancelclosing = true;

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if ( cancelclosing )
                controller.RequestStop();
            e.Cancel = cancelclosing;
        }

        public bool DLLRequestHistory(long index, bool isjid, out EDDDLLInterfaces.EDDDLLIF.JournalEntry f)
        {
            f = new EDDDLLInterfaces.EDDDLLIF.JournalEntry();
            return false;
        }

        #endregion

        #region Controller feedback

        public void RefreshFinished(HistoryEntry currenthe)
        {
            dataGridViewCommanders.AutoGenerateColumns = false;             // BEFORE assigned to list..
            dataGridViewCommanders.DataSource = EDCommander.GetListCommanders();

            if (currenthe != null)
            {
                var matlist = controller.GetMatList(currenthe);
                var missionlist = controller.GetCurrentMissionList(currenthe);
                DLLManager.Refresh(EDCommander.Current.Name, EDDDLLCallerHE.CreateFromHistoryEntry(currenthe, matlist, missionlist));

                if (currenthe.Commander.SyncToInara)
                {
                    EliteDangerousCore.Inara.InaraSync.Refresh(LogLine, currenthe, currenthe.Commander);
                }
            }
        }

        HistoryEntry lasthe = null;
        HistoryEntry lastuihe = null;       // last he that the ui was updated on.. used during store to try and prevent too much thrash

        public void HistoryEvent(HistoryEntry he, bool stored, bool recent)     // recent is true on stored for last few entries..
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);
            // System.Diagnostics.Debug.WriteLine($"Form HistoryEvent {he.EventTimeUTC} {he.EntryType} st {stored} rc {recent}");

            bool dontupdateui = (lasthe != null && stored && !recent);

            var matlist = controller.GetMatList(he);
            var missionlist = controller.GetCurrentMissionList(he);

            if (!dontupdateui)      // so, if we have displayed one, and we are in stored reply, and not a recent entry.. don't update UI
            {
                bool reposbut = false;

                if (lastuihe == null || !he.Commander.Name.Equals(lastuihe.Commander.Name))
                {
                    labelCmdr.Text = he.Commander.Name;
                    if (EDCommander.Current.Name != he.Commander.Name)
                        labelCmdr.Text += " Report clash " + EDCommander.Current.Name;
                }

                if (lastuihe == null || he.Credits != lastuihe.Credits)
                {
                    labelCredits.Text = he.Credits.ToString("N0");
                }

                if (!labelSystem.Text.Equals(he.System.Name))       // because of StartJump rewriting the previous entry, we can't detect system names changes using UI
                {                                                   // which ends up we never seeing lastui.system.name being different to he.system.name
                    extButtonEDSMSystem.Enabled = extButtonInaraSystem.Enabled = extButtonEDDBSystem.Enabled = true;
                    labelSystem.Text = he.System.Name;
                    reposbut = true;
                }

                if (lastuihe == null || !he.WhereAmI.Equals(lastuihe.WhereAmI))
                {
                    labelLocation.Text = he.WhereAmI;
                    reposbut = true;
                }

                bool hasmarketid = he?.MarketID.HasValue ?? false;
                bool hasbodyormarketid = hasmarketid || he.FullBodyID.HasValue;

                if (lastuihe == null || extButtonInaraStation.Enabled != hasmarketid)
                {
                    extButtonInaraStation.Enabled = extButtonEDDBStation.Enabled = hasmarketid;
                }

                if (lastuihe == null || extButtonSpanshStation.Enabled != hasbodyormarketid)
                {
                    extButtonSpanshStation.Enabled = hasbodyormarketid;
                }

                if ((he.ShipInformation != null) != extButtonEDSY.Enabled)      // enabled/visible causes effort, only do it if different
                {
                    extButtonEDSY.Enabled = extButtonCoriolis.Enabled = he.ShipInformation != null;
                }

                if (he.ShipInformation != null && (lastuihe == null || lastuihe.ShipInformation == null || he.ShipInformation.ShipNameIdentType != lastuihe.ShipInformation.ShipNameIdentType))
                {
                    labelShip.Text = he.ShipInformation.ShipNameIdentType ?? "Unknown";
                    reposbut = true;
                }

                if (reposbut)
                {
                    int maxx = Math.Max(labelSystem.Right, labelLocation.Right) + 2;

                    extButtonEDSMSystem.Left = maxx;
                    extButtonInaraSystem.Left = extButtonEDSMSystem.Right + 2;
                    extButtonEDDBSystem.Left = extButtonInaraSystem.Right + 2;
                    extButtonSpanshSystem.Left = extButtonEDDBSystem.Right + 2;

                    extButtonInaraStation.Left = maxx;
                    extButtonEDDBStation.Left = extButtonInaraStation.Right + 2;
                    extButtonSpanshStation.Left = extButtonEDDBStation.Right + 2;

                    extButtonCoriolis.Left = labelShip.Right + 2;
                    extButtonEDSY.Left = extButtonCoriolis.Right + 2;
                }

                var counts = MaterialCommoditiesMicroResourceList.Count(matlist);
                int encodedcount = counts[(int)MaterialCommodityMicroResourceType.CatType.Encoded];
                int rawcount = counts[(int)MaterialCommodityMicroResourceType.CatType.Raw];
                int manucount = counts[(int)MaterialCommodityMicroResourceType.CatType.Manufactured];
                int datacount = counts[(int)MaterialCommodityMicroResourceType.CatType.Data];
                int itemcount = counts[(int)MaterialCommodityMicroResourceType.CatType.Item];
                int consumecount = counts[(int)MaterialCommodityMicroResourceType.CatType.Consumable];
                int componentcount = counts[(int)MaterialCommodityMicroResourceType.CatType.Component];
                int cargocount = counts[(int)MaterialCommodityMicroResourceType.CatType.Commodity];

                if (encodedcount != prevencodedcount)
                    labelEncoded.Text = encodedcount.ToString();
                if (rawcount != prevrawcount)
                    labelRaw.Text = rawcount.ToString();
                if (manucount != prevmanucount)
                    labelManufactured.Text = manucount.ToString();

                if (datacount != prevdatacount)
                    labelData.Text = datacount.ToString();
                if (itemcount != previtemcount)
                    labelItems.Text = itemcount.ToString();
                if (consumecount != prevconsumecount)
                    labelConsumables.Text = consumecount.ToString();
                if (componentcount != prevcomponentcount)
                    labelComponents.Text = componentcount.ToString();

                if (cargocount != prevcargocount)
                    labelCargo.Text = cargocount.ToString();

                prevencodedcount = encodedcount;
                prevrawcount = rawcount;
                prevmanucount = manucount;
                prevdatacount = datacount;
                previtemcount = itemcount;
                prevconsumecount = consumecount;
                prevcomponentcount = componentcount;
                prevcargocount = cargocount;

                he.journalEntry.FillInformation(he.System,he.WhereAmI, out string info, out string detailed);
                LogLine( //BaseUtils.AppTicks.TickCountLap("MT") + " " +
                    EDDConfig.Instance.ConvertTimeToSelectedFromUTC(he.EventTimeUTC) + " " + he.journalEntry.SummaryName(he.System) + ": " + info);

                labelMissionCount.Text = missionlist.Count.ToString();
                string mtext = "";
                if (missionlist.Count > 0)
                {
                    var last = missionlist[0];
                    mtext = BaseUtils.FieldBuilder.Build("", last.Mission.LocalisedName, "", last.Mission.Expiry, "", last.Mission.DestinationSystem, "", last.Mission.DestinationStation);
                }

                labelLatestMission.Text = mtext;

                lastuihe = he;
                //System.Diagnostics.Debug.WriteLine("Set lastuihe to " + lastuihe.System.Name);
            }

            if (!stored)
            {
                if (he.Commander.SyncToEdsm && EDSMJournalSync.OkayToSend(he))
                {
                    EDSMJournalSync.SendEDSMEvents(LogLine, new List<HistoryEntry> { he }, he.journalEntry.GameVersion, he.journalEntry.Build);
                }

                if (he.Commander.SyncToIGAU)
                {
                    EliteDangerousCore.IGAU.IGAUSync.NewEvent(LogLine, he);
                }

                if (EDCommander.Current.SyncToEDAstro)
                {
                    EliteDangerousCore.EDAstro.EDAstroSync.SendEDAstroEvents(new List<HistoryEntry>() { he });
                }

                if (he.Commander.SyncToEddn == true)
                {
                    if (queuedfsssd != null && ((EliteDangerousCore.JournalEvents.JournalFSSSignalDiscovered)queuedfsssd.journalEntry).Signals[0].SystemAddress == he.System.SystemAddress)     // if queued, and we are now in its system
                    {
                        System.Diagnostics.Debug.WriteLine($"EDDN send of FSSSignalDiscovered is sent - now in system");
                        ((EliteDangerousCore.JournalEvents.JournalFSSSignalDiscovered)queuedfsssd.journalEntry).EDDNSystem = he.System; // override for EDDN purposes
                        EliteDangerousCore.EDDN.EDDNSync.SendEDDNEvents(LogLine, new List<HistoryEntry> { queuedfsssd });
                        queuedfsssd = null;
                    }

                    if (EliteDangerousCore.EDDN.EDDNClass.IsEDDNMessage(he.EntryType) && he.AgeOfEntry() < TimeSpan.FromDays(1.0))
                    {
                        // if FSS Signal discovered, but the system address is not of the current system we think we are in, then queue it until location/jump comes about
                        if (he.EntryType == JournalTypeEnum.FSSSignalDiscovered && ((EliteDangerousCore.JournalEvents.JournalFSSSignalDiscovered)he.journalEntry).Signals[0].SystemAddress != he.System.SystemAddress)
                        {
                            queuedfsssd = he;
                            System.Diagnostics.Debug.WriteLine($"EDDN send of FSSSignalDiscovered is queued due to SystemAddress not being Isystem address");
                        }
                        else
                            EliteDangerousCore.EDDN.EDDNSync.SendEDDNEvents(LogLine, new List<HistoryEntry> { he });
                    }
                }

                if (he.Commander.SyncToInara)
                {
                    var mcmrlist = controller.GetMatDict(he);
                    EliteDangerousCore.Inara.InaraSync.NewEvent(LogLine, he, mcmrlist);
                }

                screenshot.NewJournalEntry(he.journalEntry);
            }

            if (DLLManager.Count > 0)       // if worth calling..
            {
                var je = EDDDLLCallerHE.CreateFromHistoryEntry(he, matlist, missionlist, stored);
                DLLManager.NewUnfilteredJournalEntry(je,stored);
                DLLManager.NewJournalEntry(je, stored);
            }

            lasthe = he;
        }

        public void UIEvent(UIEvent uievent)
        {
            if (DLLManager.Count > 0)       // if worth calling..
            {
                string output = QuickJSON.JToken.FromObject(uievent, ignoreunserialisable: true,
                                                                                    ignored: new Type[] { typeof(Bitmap), typeof(Image) },
                                                                                    maxrecursiondepth: 3)?.ToString();
                if (output != null)
                    DLLManager.NewUIEvent(output);
                else
                    System.Diagnostics.Debug.WriteLine("**** ERROR Could not serialise " + uievent.EventTypeStr);
            }
        }

        public void LogLine(string s)     
        {
            if (!System.Windows.Forms.Application.MessageLoop)
                BeginInvoke((MethodInvoker)delegate { LogLine(s); });
            else
                extRichTextBoxLog.AppendText(s + Environment.NewLine);
        }

        private Size screenshotimagesize;

        public void DisplayScreenshot(string infile, string outfile, Size imagesize, EliteDangerousCore.JournalEvents.JournalScreenshot ss)
        {
            System.Diagnostics.Debug.WriteLine("Screen shot " + infile + " -> " + outfile + " " + imagesize);

            try
            {
                screenshotimagesize = imagesize;
                pictureBoxScreenshot.ImageLocation = outfile;                       // this could except, so protect..
                FitScreenshotToWindow();
            }
            catch
            {
            }
        }

        void FitScreenshotToWindow()
        {
            var boxsize = splitContainerNamesButtonsScreenshot.Panel2.ClientSize;
            double ratiopicture = (double)screenshotimagesize.Width / (double)screenshotimagesize.Height;

            int imagewidth = boxsize.Width;
            int imageheight = (int)((double)imagewidth / ratiopicture);

            if (imageheight > boxsize.Height)        // if width/ratio > available height, scale down width
            {
                double scaledownwidth = (double)imageheight / (double)boxsize.Height;
                imagewidth = (int)((double)imagewidth / scaledownwidth);
            }

            imageheight = (int)((double)imagewidth / ratiopicture);

            pictureBoxScreenshot.Location = new Point((boxsize.Width - imagewidth) / 2, (boxsize.Height - imageheight) / 2);
            pictureBoxScreenshot.Size = new Size(imagewidth, imageheight);
        }

        private void SplitContainerNamesButtonsScreenshot_Resize(object sender, EventArgs e)
        {
            if (pictureBoxScreenshot.ImageLocation != null)
                FitScreenshotToWindow();
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
                cmdr.Update();
                dataGridViewCommanders.Refresh();
                controller.RequestRescan = true;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewCommanders.RightClickRow >= 0)
                EditCmdr(dataGridViewCommanders.RightClickRow);

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewCommanders.RightClickRow >= 0)
            {
                int row = dataGridViewCommanders.RightClickRow;
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
                    EDCommander.Add(cmdr);
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
                string url = edsm.GetUrlToSystem(lasthe.System.Name);

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
                string loadoutjournalline = lasthe.ShipInformation.ToJSONLoadout();

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

        private void extButtonEDDBSystem_Click(object sender, EventArgs e)
        {
            if (lasthe != null)
                System.Diagnostics.Process.Start(Properties.Resources.URLEDDBSystemName + HttpUtility.UrlEncode(lasthe.System.Name));
        }

        private void extButtonEDDBStation_Click(object sender, EventArgs e)
        {
            if (lasthe != null && lasthe.MarketID != null)
                System.Diagnostics.Process.Start(Properties.Resources.URLEDDBStationMarketId + lasthe.MarketID.ToStringInvariant());

        }

        private void extButtonSpanshSystem_Click(object sender, EventArgs e)
        {
            if (lasthe != null && lasthe.System.SystemAddress.HasValue)
                System.Diagnostics.Process.Start(Properties.Resources.URLSpanshSystemSystemId + lasthe.System.SystemAddress.Value.ToStringInvariant());
        }

        private void extButtonSpanshStation_Click(object sender, EventArgs e)
        {
            if (lasthe != null)
            {
                if (lasthe.MarketID != null)
                  System.Diagnostics.Process.Start(Properties.Resources.URLSpanshStationMarketId + lasthe.MarketID.ToStringInvariant());
                else if (lasthe.FullBodyID.HasValue)
                    System.Diagnostics.Process.Start(Properties.Resources.URLSpanshBodyId + lasthe.FullBodyID.ToStringInvariant());

            }
        }

        #endregion

        #region Menu UI

        private void timeToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            labelGameDateTime.Visible = timeToolStripMenuItem.Checked;
            UserDatabase.Instance.PutSettingBool("TimeDisplay", timeToolStripMenuItem.Checked);
        }

        private void screenShotCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            screenshot.Configure(this);
            extButtonScreenshotDisabled.Visible = !screenshot.AutoConvert;
        }

        private void extButtonScreenshotDisabled_Click(object sender, EventArgs e)
        {
            screenShotCaptureToolStripMenuItem_Click(sender, e);
        }


        private void SetTheme(string theme)
        {
            theme = theme.Replace("High DPI", "").Trim();

            eDSMToolStripMenuItem.Checked = theme == "EDSM";
            eDSMEuroCapsToolStripMenuItem.Checked = theme == "EDSM EuroCaps";
            eDSMArialNarrowToolStripMenuItem.Checked = theme == "EDSM Arial Narrow";
            eliteVerdanaToolStripMenuItem.Checked = theme == "Elite Verdana";
            eliteCalistoToolStripMenuItem.Checked = theme == "Elite Calisto";
            eliteEurocapsToolStripMenuItem.Checked = theme == "Elite EuroCaps";
            materialDarkToolStripMenuItem.Checked = theme == "Material Dark";
            easyDarkToolStripMenuItem.Checked = theme == "Easy Dark";

            if (highDPIToolStripMenuItem.Checked)
                theme += " High DPI";

            ThemeList.SetThemeByName(theme);

            UserDatabase.Instance.PutSettingString("Theme", theme);
            ApplyTheme();
        }

        private void highDPIToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            SetTheme(UserDatabase.Instance.GetSettingString("Theme", "EDSM"));
        }

        private void themeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var s = sender as ToolStripMenuItem;
            SetTheme(s.Text);
        }

        private void ApplyTheme()
        {
            ExtendedControls.Theme.Current.ApplyStd(this);
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionBackColor = ExtendedControls.Theme.Current.GridCellBack;    // hide selection
            dataGridViewCommanders.RowsDefaultCellStyle.SelectionForeColor = ExtendedControls.Theme.Current.GridCellText;

            this.Refresh();                                             // force thru refresh to make sure its repainted
        }

        bool ingtchange = false;


        private void gameTimeToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!ingtchange)
            {
                ToolStripMenuItem mi = sender as ToolStripMenuItem;
                int index = ((string)mi.Tag).InvariantParseInt(0);
                EDDConfig.Instance.DisplayTimeIndex = index;
                UpdateGameTimeTick();
            }
        }

        private void UpdateGameTimeTick()
        {
            ingtchange = true;
            int index = EDDConfig.Instance.DisplayTimeIndex;
            gameTimeToolStripMenuItem.Checked = index == 2;
            utcToolStripMenuItem.Checked = index == 1;
            localToolStripMenuItem.Checked = index == 0;
            ingtchange = false;
        }
        private void removeDLLPermissionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ExtendedControls.MessageBoxTheme.Show(this, "Remove all DLL permissions, on next start, you will be asked per DLL if you wish to allow the DLL to run. Are you sure?".T(EDTx.EDDiscoveryForm_RemoveDLLPerms), "Warning".T(EDTx.Warning), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                EDDConfig.Instance.DLLPermissions = "";
            }
        }

        #endregion

        #region Tray

        private void EDDLiteForm_Resize(object sender, EventArgs e)
        {
            // if we are shown, and we are using minimize to notification..
            if (FormShownOnce && EDDConfig.Instance.UseNotifyIcon && EDDConfig.Instance.MinimizeToNotifyIcon)
            {
                if (FormWindowState.Minimized == WindowState)   // minized, hiding hides the taskbar icon
                    Hide();
                else if (!Visible)      // else make sure visible
                    Show();
            }
        }


        private void notifyIconEDD_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIconMenu_Open_Click(sender, e);

        }

        private void notifyIconMenu_Open_Click(object sender, EventArgs e)
        {
            // Toggle state
            if (FormWindowState.Minimized == WindowState)
            {
                if (EDDConfig.Instance.MinimizeToNotifyIcon)
                    Show();

                if (FormIsMaximised)
                    WindowState = FormWindowState.Maximized;
                else
                    WindowState = FormWindowState.Normal;
            }
            else
                WindowState = FormWindowState.Minimized;
        }

        private void notifyIconMenu_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void useNotifyIconToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            notifyIconEDD.Visible =
            EDDConfig.Instance.UseNotifyIcon = useNotifyIconToolStripMenuItem.Checked;
            if (!EDDConfig.Instance.UseNotifyIcon && !Visible)
                Show();
        }

        private void minimiseToNotificationAreaToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            EDDConfig.Instance.MinimizeToNotifyIcon = minimiseToNotificationAreaToolStripMenuItem.Checked;
        }


        private void startMinimizedToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            EDDConfig.Instance.StartMinimized = startMinimisedToolStripMenuItem.Checked;

        }



        #endregion

    }
}

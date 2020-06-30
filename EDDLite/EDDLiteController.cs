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
using System;
using System.Threading;

namespace EDDLite
{
    public class EDDLiteController
    {
        public bool RequestRescan { get; set; } = false;
        public Action<HistoryEntry> RefreshFinished { get; set; } = null;
        public Action<HistoryEntry,bool,bool> NewEntry { get; set; } = null;
        public Action<UIEvent> NewUI { get; set; } = null;
        public Action<string> ProgressEvent { get; set; } = null;
        public Action<string> LogLine { get; set; } = null;

        private int recentlimit = 50;       // entries marked as close to end as this are marked recent

        public void Start(Action<Action> invokeAsyncOnUiThread)
        {
            InvokeAsyncOnUiThread = invokeAsyncOnUiThread;
            controllerthread = new Thread(Controller);
            controllerthread.Start();
        }

        bool stopit = false;

        public void Stop()
        {
            stopit = true;
            controllerthread.Join();
        }

        private void Controller()
        {
            journalmonitor = new EDJournalUIScanner(InvokeAsyncOnUiThread);
            journalmonitor.OnNewJournalEntry += (je) => { Entry(je, false, true); };
            journalmonitor.OnNewUIEvent += (ui) => { InvokeAsyncOnUiThread(()=>NewUI?.Invoke(ui)); };

            LogLine?.Invoke("Reading Journals");
            Reset();
            journalmonitor.SetupWatchers();
            // order the reading of last 2 files (in case continue) and fire back the last two
            journalmonitor.ParseJournalFilesOnWatchers(UpdateWatcher, 2, (a,ji,jt,ei,et) => InvokeAsyncOnUiThread(() => {
                //System.Diagnostics.Debug.WriteLine("In FG {0} {1} {2} {3} {4} {5}", a.CommanderId, ji, jt, ei, et, a.EventTypeStr );
                Entry(a, true, ji==jt-1 && ei-et > -recentlimit); }), 2);

            InvokeAsyncOnUiThread(() => { RefreshFinished?.Invoke(currenthe); });

            LogLine?.Invoke("Finished reading Journals");

            journalmonitor.StartMonitor();

            while (!stopit)
            {
                if ( RequestRescan )
                {
                    RequestRescan = false;

                    LogLine?.Invoke("Re-reading Journals");
                    journalmonitor.StopMonitor();
                    Reset();
                    journalmonitor.SetupWatchers();
                    journalmonitor.ParseJournalFilesOnWatchers(UpdateWatcher, 2, 
                            (a,ji,jt,ei,et) => InvokeAsyncOnUiThread(() => { Entry(a, true, ji == jt - 1 && ei - et > -recentlimit); }), 2);
                    journalmonitor.StartMonitor();
                    InvokeAsyncOnUiThread(() => { RefreshFinished?.Invoke(currenthe); });
                    LogLine?.Invoke("Finished reading Journals");
                }

                Thread.Sleep(100);
            }

            journalmonitor.StopMonitor();
        }

        private void UpdateWatcher(int p, string s) // in thread
        {
            InvokeAsyncOnUiThread(() => { ProgressEvent?.Invoke(s); });
            System.Diagnostics.Debug.WriteLine("Update " + p + " " + s);
        }

        DateTime lastutc;

        private void Reset(bool full = true)
        {
            currenthe = null;
            outfitting = new OutfittingList();
            shipinformationlist = new ShipInformationList();
            matlist = new MaterialCommoditiesList();
            missionlistaccumulator = new MissionListAccumulator(); // and mission list..
            cashledger = new Ledger();

            if (full)
            {
                lastutc = DateTime.Now.AddYears(-100);
                currentcmdrnr = -1;
            }
        }

        public void Entry(JournalEntry je, bool stored, bool recent)        // on UI thread. hooked into journal monitor and receives new entries.. Also call if you programatically add an entry
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);

            if (je.EventTimeUTC >= lastutc)     // in case we get them fed in the wrong order, or during stored reply we have two playing, only take the latest one
            {
                System.Diagnostics.Debug.WriteLine("JE " + stored + ":" + recent + ":" + EDCommander.GetCommander(je.CommanderId).Name + ":" + je.EventTypeStr);

                if (je.CommanderId != currentcmdrnr)
                {
                    Reset(false);
                    currentcmdrnr = je.CommanderId;
                    EDCommander.CurrentCmdrID = currentcmdrnr;
                }

                HistoryEntry he = HistoryEntry.FromJournalEntry(je, currenthe, false, out bool unusedjournalupdate);
                he.UpdateMaterials( je, currenthe);

                cashledger.Process(je);
                he.Credits = cashledger.CashTotal;

                he.MissionList = missionlistaccumulator.Process(je, he.System, he.WhereAmI);

                currenthe = he;
                lastutc = je.EventTimeUTC;
                outfitting.Process(je);

                Tuple<ShipInformation, ModulesInStore> ret = shipinformationlist.Process(je, he.WhereAmI, he.System);
                he.ShipInformation = ret.Item1;
                he.StoredModules = ret.Item2;

                NewEntry?.Invoke(he, stored, recent);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Rejected older JE " + stored + ":" + recent + ":" + EDCommander.GetCommander(je.CommanderId).Name + " " + je.EventTypeStr);
            }
        }

        private HistoryEntry currenthe;
        private OutfittingList outfitting;
        private ShipInformationList shipinformationlist;
        private MaterialCommoditiesList matlist;
        private MissionListAccumulator missionlistaccumulator;
        private Ledger cashledger;
        private int currentcmdrnr;
        private Thread controllerthread;
        private EDJournalUIScanner journalmonitor;
        private Action<Action> InvokeAsyncOnUiThread;


    }
}
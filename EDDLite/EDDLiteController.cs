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
using System;
using System.Collections.Generic;
using System.Threading;

namespace EDDLite
{
    public class EDDLiteController
    {
        public bool RequestRescan { get; set; } = false;
        public Action<HistoryEntry> RefreshFinished { get; set; } = null;           // on UI thread
        public Action<HistoryEntry,bool,bool> NewEntry { get; set; } = null;        // on UI thread
        public Action<UIEvent> NewUI { get; set; } = null;                          // on UI thread
        public Action<string> ProgressEvent { get; set; } = null;                   // on UI thread
        public Action<string> LogLine { get; set; } = null;                         // on UI thread

        const int uirecentlimit = 50;         // entries marked as close to end as this are marked recent and shown to user
        const int journalstoreload = 4;   // how many journals back to read and replay to get info on

        public void Start(Action<Action> invokeAsyncOnUiThread)
        {
            InvokeAsyncOnUiThread = invokeAsyncOnUiThread;
            journalqueuedelaytimer = new Timer(DelayPlay, null, Timeout.Infinite, Timeout.Infinite);
            controllerthread = new Thread(Controller);
            controllerthread.Start();
        }

        bool stopit = false;

        public void Stop()
        {
            stopit = true;
            controllerthread.Join();
            journalqueuedelaytimer.Change(Timeout.Infinite, Timeout.Infinite);
            journalqueuedelaytimer.Dispose();
        }

        private void Controller()
        {
            journalmonitor = new EDJournalUIScanner(InvokeAsyncOnUiThread);
            journalmonitor.OnNewJournalEntry += (je, sr) => { Entry(je, false, true); };
            journalmonitor.OnNewUIEvent += (ui, sr) => { InvokeAsyncOnUiThread(() => NewUI?.Invoke(ui)); };

            StartWatchersAndReplayLastStoredEntries();

            while (!stopit)
            {
                if ( RequestRescan )
                {
                    RequestRescan = false;

                    InvokeAsyncOnUiThread(() => LogLine?.Invoke("Re-reading Journals") );
                    journalmonitor.StopMonitor();

                    StartWatchersAndReplayLastStoredEntries();
                }

                Thread.Sleep(100);
            }

            journalmonitor.StopMonitor();
        }

        private void StartWatchersAndReplayLastStoredEntries()
        {
            ResetStats();

            string stdfolder = EliteDangerousCore.FrontierFolder.FolderName();     // may be null

            journalmonitor.SetupWatchers(new string[] { stdfolder }, "Journal*.log", DateTime.MinValue);

            InvokeAsyncOnUiThread(() => LogLine?.Invoke("Reading Journals"));

            List<JournalEntry> toprocess = new List<JournalEntry>();        // we accumulate in a list of the journal entries from the previous N journals
            journalmonitor.ParseJournalFilesOnWatchers(UpdateWatcher, DateTime.MinValue, journalstoreload, toprocess);

            System.Diagnostics.Debug.WriteLine($"Play thru {toprocess.Count} to get state");

            InvokeAsyncOnUiThread(() =>
            {
                for (int i = 0; i < toprocess.Count; i++)           // fill up Entries with events
                {
                    Entry(toprocess[i], true, i >= toprocess.Count - uirecentlimit);
                    if ( i % 100 == 0 )
                        System.Windows.Forms.Application.DoEvents();        // just keep the message loop happy as we bombard the UI thread with stuff to do
                };

                LogLine?.Invoke($"Finished reading Journals");

                //InvokeAsyncOnUiThread(() => { RefreshFinished?.Invoke(currenthe); });
                RefreshFinished?.Invoke(currenthe);
            });

            journalmonitor.StartMonitor(false);
        }

        private void UpdateWatcher(int p, string s) // in thread
        {
            InvokeAsyncOnUiThread(() => { ProgressEvent?.Invoke(s); });
            System.Diagnostics.Debug.WriteLine("Watcher Update " + p + " " + s);
        }

        DateTime lastutc;

        private void ResetStats(bool full = true)
        {
            currenthe = null;
            outfitting = new OutfittingList();
            shipinformationlist = new ShipInformationList();
            matlist = new MaterialCommoditiesMicroResourceList();
            missionlistaccumulator = new MissionListAccumulator(); // and mission list..
            cashledger = new Ledger();

            if (full)
            {
                lastutc = DateTime.Now.AddYears(-100);
                currentcmdrnr = -1;
            }
        }

        private Queue<HistoryEntry> journalqueue = new Queue<HistoryEntry>();
        private System.Threading.Timer journalqueuedelaytimer;

        // on UI thread. hooked into journal monitor and receives new entries.. Also call if you programatically add an entry
        public void Entry(JournalEntry je, bool stored, bool recent)        
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);

            if (je.EventTimeUTC >= lastutc)     // in case we get them fed in the wrong order, or during stored reply we have two playing, only take the latest one
            {
               // System.Diagnostics.Debug.WriteLine("Controller Entry " + stored + ":" + recent + ":" + EDCommander.GetCommander(je.CommanderId).Name + ":" + je.EventTypeStr);

                if (je.CommanderId != currentcmdrnr)
                {
                    ResetStats(false);
                    currentcmdrnr = je.CommanderId;
                    EDCommander.CurrentCmdrID = currentcmdrnr;
                }

                HistoryEntry he = HistoryEntry.FromJournalEntry(je, currenthe);

                he.UpdateMaterialsCommodities( matlist.Process(je, currenthe?.journalEntry, he.Status.TravelState == HistoryEntryStatus.TravelStateType.SRV));

                cashledger.Process(je);
                he.Credits = cashledger.CashTotal;

                he.UpdateMissionList(missionlistaccumulator.Process(je, he.System, he.WhereAmI));

                currenthe = he;
                lastutc = je.EventTimeUTC;
                outfitting.Process(je);

                Tuple<ShipInformation, ModulesInStore> ret = shipinformationlist.Process(je, he.WhereAmI, he.System);
                he.UpdateShipInformation(ret.Item1);
                he.UpdateShipStoredModules(ret.Item2);

                int playdelay = (je.EventTypeID == JournalTypeEnum.FSSSignalDiscovered) ? 2000 : 0;     // have we got a delayable entry

                // if not stored entry, and merge says delaying to see if a companion event occurs. add it to list. Set timer so we pick it up

                if (!stored && playdelay > 0)  
                {
                    System.Diagnostics.Debug.WriteLine(Environment.TickCount + " Delay Play queue " + je.EventTypeID + " Delay for " + playdelay);
                    journalqueue.Enqueue(he);
                    journalqueuedelaytimer.Change(playdelay, Timeout.Infinite);
                }
                else
                {
                    journalqueuedelaytimer.Change(Timeout.Infinite, Timeout.Infinite);  // stop the timer, but if it occurs before this, not the end of the world
                    journalqueue.Enqueue(he);  // add it to the play list.
                    PlayJournalList(stored,recent);    // and play
                }
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Rejected older JE " + stored + ":" + recent + ":" + EDCommander.GetCommander(je.CommanderId).Name + " " + je.EventTypeStr);
            }
        }

        public void DelayPlay(Object s)             // timer thread timeout after play delay.. 
        {
            System.Diagnostics.Debug.WriteLine(Environment.TickCount + " Delay Play timer executed");
            journalqueuedelaytimer.Change(Timeout.Infinite, Timeout.Infinite);
            InvokeAsyncOnUiThread(() =>
            {
                PlayJournalList(false,true);
            });
        }

        void PlayJournalList(bool stored, bool recent)
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);

            while (journalqueue.Count > 0)      // dequeue
            {
                var current = journalqueue.Dequeue();
                //System.Diagnostics.Trace.WriteLine($"PlayJournalList {current.EventTimeUTC} {current.EntryType}");

                if (!stored)
                {
                    while (journalqueue.Count > 0)     // merge back 
                    {
                        var peek = journalqueue.Peek(); // is there a next one?

                        if (peek != null && current.EntryType == peek.EntryType)        // if same event again
                        {
                            if (current.EntryType == JournalTypeEnum.FSSSignalDiscovered) // if mergeable
                            {
                                var jdprev = current.journalEntry as EliteDangerousCore.JournalEvents.JournalFSSSignalDiscovered;
                                var jd = peek.journalEntry as EliteDangerousCore.JournalEvents.JournalFSSSignalDiscovered;

                                if (jdprev.Signals[0].SystemAddress == jd.Signals[0].SystemAddress)     // only if same system address
                                {
                                    jdprev.Add(jd);     // merge! and waste
                                    System.Diagnostics.Trace.WriteLine($"PlayJournalList Merge {current.EntryType}");
                                    journalqueue.Dequeue();                     // remove it
                                }
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                }

                NewEntry?.Invoke(current, stored, recent);      // dispatch possibly merged candidate
            }
        }


        public List<MaterialCommodityMicroResource> GetMatList(HistoryEntry he)
        {
            return matlist.Get(he.MaterialCommodity);
        }

        public Dictionary<string,MaterialCommodityMicroResource> GetMatDict(HistoryEntry he)
        {
            return matlist.GetDict(he.MaterialCommodity);
        }

        public List<MissionState> GetCurrentMissionList(HistoryEntry he)
        {
            return missionlistaccumulator.GetAllCurrentMissions(he.MissionList,he.EventTimeUTC);
        }

        private HistoryEntry currenthe;
        private OutfittingList outfitting;
        private ShipInformationList shipinformationlist;
        private MaterialCommoditiesMicroResourceList matlist;
        private MissionListAccumulator missionlistaccumulator;
        private Ledger cashledger;
        private int currentcmdrnr;
        private Thread controllerthread;
        private EDJournalUIScanner journalmonitor;
        private Action<Action> InvokeAsyncOnUiThread;


    }
}
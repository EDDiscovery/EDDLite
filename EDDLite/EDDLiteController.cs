using EliteDangerousCore;
using EliteDangerousCore.DB;
using EliteDangerousCore.JournalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDLite
{
    public class EDDLiteController
    {
        private Thread controllerthread;
        private EDJournalClass journalmonitor;
        private Action<Action> InvokeAsyncOnUiThread;
        EDDLiteForm UIForm;

        public void Start(EDDLiteForm frm, Action<Action> invokeAsyncOnUiThread)
        {
            InvokeAsyncOnUiThread = invokeAsyncOnUiThread;
            UIForm = frm;
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
            journalmonitor = new EDJournalClass(InvokeAsyncOnUiThread);
            journalmonitor.OnNewJournalEntry += NewEntry;
            journalmonitor.OnNewUIEvent += NewUIEvent;

            journalmonitor.SetupWatchers(true);
            journalmonitor.ParseJournalFilesOnWatchers(UpdateWatcher, false, forceLastReload: true, firebacknostore: (a) => InvokeAsyncOnUiThread(()=> { Entry(a,true); }));

            InvokeAsyncOnUiThread(() => { UIForm.ReadJournals(); });

            journalmonitor.StartMonitor();

            while (!stopit)
            {
                Thread.Sleep(100);
            }

            journalmonitor.StopMonitor();
        }

        private void UpdateWatcher(int p, string s) // in thread
        {
            InvokeAsyncOnUiThread(() => { UIForm.JournalReadProgress(s); });
            System.Diagnostics.Debug.WriteLine("Update " + p + " " + s);
        }

        DateTime lastutc = DateTime.Now.AddYears(-100);

        HistoryEntry currenthe;
        OutfittingList outfitting = new OutfittingList();
        ShipInformationList shipinformationlist = new ShipInformationList();
        MaterialCommoditiesList matlist = new MaterialCommoditiesList();
        int currentcmdrnr = -1;

        public void Entry(JournalEntry je, bool stored)        // on UI thread. hooked into journal monitor and receives new entries.. Also call if you programatically add an entry
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);
            System.Diagnostics.Debug.WriteLine("JE " + stored + " on UI thread " + je.EventTypeStr  );

            if (je.EventTimeUTC >= lastutc)     // in case we get them fed in the wrong order, or during stored reply we have two playing, only take the latest one
            {
                if (je.CommanderId != currentcmdrnr)
                {
                    outfitting = new OutfittingList();          // different commander, reset.
                    shipinformationlist = new ShipInformationList();
                    matlist = new MaterialCommoditiesList();
                    currenthe = null;
                    currentcmdrnr = je.CommanderId;
                    EDCommander.CurrentCmdrID = currentcmdrnr;
                }

                HistoryEntry he = HistoryEntry.FromJournalEntry(je, currenthe, false, out bool unusedjournalupdate);
                he.UpdateMaterials( je, currenthe);

                currenthe = he;
                lastutc = je.EventTimeUTC;
                outfitting.Process(je);

                Tuple<ShipInformation, ModulesInStore> ret = shipinformationlist.Process(je, he.WhereAmI, he.System);
                he.ShipInformation = ret.Item1;
                he.StoredModules = ret.Item2;

                UIForm.HistoryEvent(he,stored);
            }
            else
                System.Diagnostics.Debug.WriteLine("Reject due to older");
        }

        public void NewEntry(JournalEntry je)        // on UI thread. hooked into journal monitor and receives new entries.. Also call if you programatically add an entry
        {
            Entry(je, false);
        }

        void NewUIEvent(UIEvent u)                  // UI thread new event
        {
            System.Diagnostics.Debug.Assert(System.Windows.Forms.Application.MessageLoop);
            System.Diagnostics.Debug.WriteLine("UI on UI thread " + u.EventTypeStr);
            UIForm.UIEvent(u);
        }
    }
}
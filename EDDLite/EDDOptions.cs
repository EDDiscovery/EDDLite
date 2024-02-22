﻿/*
 * Copyright © 2018-2024 EDDiscovery development team
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

using EliteDangerousCore.EDSM;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EDDLite
{
    public class EDDOptions : EliteDangerousCore.IEliteOptions
    {
        #region Option processing

        private void ProcessOption(string optname, BaseUtils.CommandArgs ca, bool toeol)
        {
            optname = optname.ToLowerInvariant();
            //System.Diagnostics.Debug.WriteLine("     Option " + optname);

            if (optname == "-optionsfile" || optname == "-appfolder")
            {
                ca.Remove();   // waste it
            }
            else if (optname == "-translationfolder")
            {
                translationfolder = ca.NextEmpty();
                TranslatorDirectoryIncludeSearchUpDepth = ca.Int();
            }
            else if (optname == "-userdbpath")
            {
                UserDatabasePath = toeol ? ca.Rest() : ca.NextEmpty();
            }
            else if (optname == "-systemsdbpath")
            {
                SystemDatabasePath = toeol ? ca.Rest() : ca.NextEmpty();
            }
            else if (optname == "-tracelog")
            {
                TraceLog = toeol ? ca.Rest() : ca.NextEmpty();
            }
            else if (optname == "-defaultjournalfolder")
            {
                DefaultJournalFolder = toeol ? ca.Rest() : ca.NextEmpty();
            }
            else if (optname.StartsWith("-"))
            {
                string opt = optname.Substring(1);

                switch (opt)
                {
                    case "norepositionwindow": NoWindowReposition = true; break;
                    case "nrw": NoWindowReposition = true; break;
                    case "portable": StoreDataInProgramDirectory = true; break;
                    case "logexceptions": LogExceptions = true; break;
                    case "checkrelease": CheckRelease = true; break;
                    case "nocheckrelease": CheckRelease = false; break;
                    case "disablemerge": DisableMerge = true; break;
                    case "edsmbeta":
                        EDSMClass.ServerAddress = "http://beta.edsm.net:8080/";
                        break;
                    case "edsmnull":
                        EDSMClass.ServerAddress = "";
                        break;
                    case "disablebetacheck":
                        DisableBetaCommanderCheck = true;
                        break;
                    case "forcebeta":       // use to move logs to a beta commander for testing
                        ForceBetaOnCommander = true;
                        break;
                    case "tempdirindatadir": TempDirInDataDir = true; break;
                    case "notempdirindatadir": TempDirInDataDir = false; break;
                    case "lowpriority": LowPriority = true; break;
                    case "nolowpriority": LowPriority = false; break;
                    case "backgroundpriority": BackgroundPriority = true; break;
                    case "nobackgroundpriority": BackgroundPriority = false; break;
                    case "disabletimedisplay": DisableTimeDisplay = true; break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Unrecognized option -{opt}");
                        break;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Unrecognized non option {optname}");
            }
        }

        #endregion

        #region Variables

        public static bool Instanced { get { return options != null; } }

        public static EDDOptions Instance
        {
            get
            {
                if (options == null)
                    options = new EDDOptions();

                return options;
            }
        }

        static EDDOptions options = null;

        // only for EDD compatibility
        public string SystemDatabasePath { get; private set; }

        public string Version { get; private set; }
        public string VersionDisplayString { get; private set; }
        public string AppDataDirectory { get; private set; }
        public string UserDatabasePath { get; private set; }
        public bool NoWindowReposition { get; set; }
        public string TraceLog { get; private set; }        // null = auto file, or fixed name
        public bool LogExceptions { get; private set; }
        public bool DisableBetaCommanderCheck { get; private set; }
        public bool ForceBetaOnCommander { get; private set; }
        public bool CheckRelease { get; set; }
        public bool DisableMerge { get; set; }
        public bool TempDirInDataDir { get; set; }
        public bool LowPriority { get; set; }
        public bool BackgroundPriority { get; set; }
        public bool DisableTimeDisplay { get; set; }
        public string DefaultJournalFolder { get; private set; }        // default is null, use computed value

        public string SubAppDirectory(string subfolder)     // ensures its there.. name without \ slashes
        {
            string path = Path.Combine(AppDataDirectory, subfolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public string LogAppDirectory() { return SubAppDirectory("Log"); }
        public string DLLAppDirectory() { return SubAppDirectory("DLL"); }
        public string TranslatorDirectory() { return translationfolder; }
        public int TranslatorDirectoryIncludeSearchUpDepth { get; private set; }

        public string ScanCachePath => null;        // we don't implement scan cache saving

        static public string ExeDirectory() { return System.AppDomain.CurrentDomain.BaseDirectory;  }
        public string[] TranslatorFolders() { return new string[] { TranslatorDirectory(), ExeDirectory() }; }

        private string AppFolder;      // internal to use.. for -appfolder option
        private bool StoreDataInProgramDirectory;  // internal to us, to indicate portable
        private string translationfolder; // internal to us

        #endregion

        #region Implementation

        private EDDOptions()
        {
            Init();
        }

        public void ReRead()        // if you've changed the option file .opt then call reread
        {
            Init();
        }

        private void SetAppDataDirectory()
        {
            ProcessCommandLineOptions((optname, ca, toeol) =>              //FIRST pass thru command line options looking
            {                                                           //JUST for -appfolder
                if (optname == "-appfolder" && ca.More)
                {
                    AppFolder = ca.Next();
                    System.Diagnostics.Debug.WriteLine("App Folder to " + AppFolder);
                }
            });

            string appfolder = AppFolder;

            if (appfolder == null)  // if userdid not set it..
            {
                appfolder = (StoreDataInProgramDirectory ? "Data" : "EDDLite");
            }

            if (Path.IsPathRooted(appfolder))
            {
                AppDataDirectory = appfolder;
            }
            else if (StoreDataInProgramDirectory)
            {
                AppDataDirectory = Path.Combine(ExeDirectory(), appfolder);
            }
            else
            {
                AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appfolder);
            }

            if (!Directory.Exists(AppDataDirectory))        // make sure its there..
                Directory.CreateDirectory(AppDataDirectory);

            if (TempDirInDataDir == true)
            {
                var tempdir = Path.Combine(AppDataDirectory, "Temp");
                if (!Directory.Exists(tempdir))
                    Directory.CreateDirectory(tempdir);

                Environment.SetEnvironmentVariable("TMP", tempdir);
                Environment.SetEnvironmentVariable("TEMP", tempdir);
            }
        }

        private void SetVersionDisplayString()
        {
            Version = Assembly.GetExecutingAssembly().FullName.Split(',')[1].Split('=')[1];
            StringBuilder sb = new StringBuilder("Version " + Version);

            if (AppFolder != null)
            {
                sb.Append($" (Using {AppFolder})");
            }

            if (EDSMClass.ServerAddress.Length ==0)
                sb.Append(" (EDSM No server)");
            else if (EDSMClass.ServerAddress.IndexOf("Beta",StringComparison.InvariantCultureIgnoreCase)!=-1)
                sb.Append(" (EDSM Beta server)");

            if (DisableBetaCommanderCheck)
            {
                sb.Append(" (no BETA detect)");
            }
            if (ForceBetaOnCommander)
            {
                sb.Append(" (Force BETA)");
            }

            VersionDisplayString = sb.ToString();
        }

        private void ProcessCommandLineForOptionsFile(string basefolder, Action<string, BaseUtils.CommandArgs, bool> getopt)     // command line -optionsfile
        {
            //System.Diagnostics.Debug.WriteLine("OptionFile -optionsfile ");
            ProcessCommandLineOptions((optname, ca, toeol) =>              //FIRST pass thru command line options looking
            {                                                           //JUST for -optionsfile
                if (optname == "-optionsfile" && ca.More)
                {
                    string filepath = ca.Next();

                    if (!File.Exists(filepath) && !Path.IsPathRooted(filepath))  // if it does not exist on its own, may be relative to base folder ..
                        filepath = Path.Combine(basefolder, filepath);

                    if (File.Exists(filepath))
                        ProcessOptionFile(filepath, getopt);
                    else
                        System.Diagnostics.Debug.WriteLine("    No Option File " + filepath);
                }
            });
        }

        private void ProcessOptionFile(string filepath, Action<string, BaseUtils.CommandArgs, bool> getopt)       // read file and process options
        {
            //System.Diagnostics.Debug.WriteLine("Read File " + filepath);
            foreach (string line in File.ReadAllLines(filepath))
            {
                if (!line.IsEmpty())
                {
                    //string[] cmds = line.Split(new char[] { ' ' }, 2).Select(s => s.Trim()).ToArray();    // old version..
                    string[] cmds = BaseUtils.StringParser.ParseWordList(line, separ: ' ').ToArray();
                    BaseUtils.CommandArgs ca = new BaseUtils.CommandArgs(cmds);
                    getopt("-" + ca.Next().ToLowerInvariant(), ca, true);
                }
            }
        }

        private void ProcessCommandLineOptions(Action<string, BaseUtils.CommandArgs, bool> getopt)       // go thru command line..
        {
            string[] cmdlineopts = Environment.GetCommandLineArgs().ToArray();
            BaseUtils.CommandArgs ca = new BaseUtils.CommandArgs(cmdlineopts,1);
            //System.Diagnostics.Debug.WriteLine("Command Line:");
            while (ca.More)
            {
                getopt(ca.Next().ToLowerInvariant(), ca, false);
            }
        }

        private void ProcessConfigVariables()
        {
            var appsettings = System.Configuration.ConfigurationManager.AppSettings;

            if (appsettings["StoreDataInProgramDirectory"] == "true")
                StoreDataInProgramDirectory = true;

            UserDatabasePath = appsettings["UserDatabasePath"];
        }


        private void Init()
        {
#if !DEBUG
            CheckRelease = true;
#endif

            ProcessConfigVariables();
            ProcessCommandLineForOptionsFile(ExeDirectory(), ProcessOption);     // go thru the command line looking for -optionfile, use relative base dir

            string optval = Path.Combine(ExeDirectory(), "options.txt");      // options in the exe folder.
            if (File.Exists(optval))   // try options.txt in the base folder..
            {
                ProcessOptionFile(optval, (optname, ca, toeol) =>              //FIRST pass thru options.txt options looking
                {                                                           //JUST for -appfolder
                    if (optname == "-appfolder" && ca.More)
                    {
                        AppFolder = ca.Rest();
                        System.Diagnostics.Debug.WriteLine("App Folder to " + AppFolder);
                    }
                });
                ProcessOptionFile(optval, ProcessOption);
            }

            SetAppDataDirectory();      // set the app directory, now we have given base dir options to override appfolder, and we have given any -optionfiles on the command line

            translationfolder = Path.Combine(AppDataDirectory, "Translator");

            ProcessCommandLineForOptionsFile(AppDataDirectory, ProcessOption);     // go thru the command line looking for -optionfile relative to app folder, then read them

            optval = Path.Combine(AppDataDirectory, "options.txt");      // options in the base folder.
            if (File.Exists(optval))   // try options.txt in the base folder..
                ProcessOptionFile(optval, ProcessOption);

            // db move system option file will contain user and system db overrides
            optval = Path.Combine(AppDataDirectory, "dboptions.txt");   // look for this file in the app folder
            if (File.Exists(optval))
                ProcessOptionFile(optval, ProcessOption);

            ProcessCommandLineOptions(ProcessOption);       // do all of the command line except optionsfile and appfolder..

            SetVersionDisplayString();  // then set the version display string up dependent on options selected

            if (UserDatabasePath == null) UserDatabasePath = Path.Combine(AppDataDirectory, "EDDUser.sqlite");
            if (SystemDatabasePath == null) SystemDatabasePath = Path.Combine(AppDataDirectory, "EDDSystem.sqlite");

            EliteDangerousCore.EliteConfigInstance.InstanceOptions = this;
        }

    }

    #endregion
}

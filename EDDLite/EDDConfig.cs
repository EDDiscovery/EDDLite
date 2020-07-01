/*
 * Copyright © 2015 - 2017 EDDiscovery development team
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

using EDDLite;
using EliteDangerousCore.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace EDDLite
{
    public class EDDConfig : EliteDangerousCore.IEliteConfig
    {
        private static EDDConfig instance;

        private EDDConfig()
        {
        }

        public static EDDConfig Instance            // Singleton pattern
        {
            get
            {
                if (instance == null)
                {
                    instance = new EDDConfig();
                    EliteDangerousCore.EliteConfigInstance.InstanceConfig = instance;        // hook up so classes can see this which use this IF
                }
                return instance;
            }
        }

        #region Discrete Controls

        private bool useNotifyIcon = false;
        private bool orderrowsinverted = false;
        private bool minimizeToNotifyIcon = false;
        private bool keepOnTop = false; /**< Whether to keep the windows on top or not */
        private int displayTimeFormat = 0; //0=local,1=utc,2=elite time
        private string language = "Auto";
        private string coriolisURL = "";
        private string eddshipyardURL = "";

        /// <summary>
        /// Controls whether or not a system notification area (systray) icon will be shown.
        /// </summary>
        public bool UseNotifyIcon
        {
            get
            {
                return useNotifyIcon;
            }
            set
            {
                useNotifyIcon = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingBool("UseNotifyIcon", value);
            }
        }

        public bool OrderRowsInverted
        {
            get
            {
                return orderrowsinverted;
            }
            set
            {
                orderrowsinverted = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingBool("OrderRowsInverted", value);
            }
        }

        /// <summary>
        /// Controls whether or not the main window will be hidden to the
        /// system notification area icon (systray) when minimized.
        /// Has no effect if <see cref="UseNotifyIcon"/> is not enabled.
        /// </summary>
        public bool MinimizeToNotifyIcon
        {
            get
            {
                return minimizeToNotifyIcon;
            }
            set
            {
                minimizeToNotifyIcon = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingBool("MinimizeToNotifyIcon", value);
            }
        }

        public bool KeepOnTop
        {
            get
            {
                return keepOnTop;
            }
            set
            {
                keepOnTop = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingBool("KeepOnTop", value);
            }
        }

        public string GetTimeTitle()
        {
            if (displayTimeFormat == 2)
                return "Game Time".T(EDTx.GameTime);
            else if (displayTimeFormat == 1)
                return "UTC";
            else
                return "Time".T(EDTx.Time);
        }

        public DateTime ConvertTimeToSelectedFromUTC(DateTime t)        // from UTC->Display format
        {
            if (displayTimeFormat == 1)     // UTC
            {
                if (!DateTimeInRangeForGame(t))
                    t = DateTime.UtcNow;
                return t;
            }
            else if (displayTimeFormat == 2)    // Game time
            {
                t = t.AddYears(1286);  
                if (!DateTimeInRangeForGame(t))
                    t = DateTime.UtcNow.AddYears(1286);
                return t;
            }
            else                                 // local
            {
                if (!DateTimeInRangeForGame(t))
                    t = DateTime.Now;

                return t.ToLocalTime();
            }
        }

        public bool DateTimeInRangeForGame(DateTime t)
        {
            if (displayTimeFormat == 2)
                return t.Year >= 3300 && t.Year <= 3399;
            else
                return t.Year >= 2014 && t.Year <= 2114;
        }

        public DateTime EnsureTimeInRangeForGame(DateTime t)
        {
            if (!DateTimeInRangeForGame(t))
                t = ConvertTimeToSelectedFromUTC(DateTime.UtcNow);
            return t;
        }

        public DateTime ConvertTimeToSelectedNoKind(DateTime t)         // from a date time (no kind) -> Display format
        {
            if (displayTimeFormat == 2)
                return t.AddYears(1286);   // 2 is UTC+years
            else
                return t;
        }

        public DateTime ConvertTimeToUTCFromSelected(DateTime t)        // from selected format back to UTC
        {
            if (displayTimeFormat == 1)
                return t;
            else if (displayTimeFormat == 2)
                return t.AddYears(-1286);
            else
                return t.ToUniversalTime();
        }

        public bool DisplayTimeLocal
        {
            get
            {
                return displayTimeFormat == 0;
            }
        }

        public int DisplayTimeIndex //0= local, 1=UTC,2 = game time, backwards compatible with old DisplayUTC bool
        {
            get
            {
                return displayTimeFormat;
            }
            set
            {
                displayTimeFormat = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingInt("DisplayUTC", value);
            }
        }

        public string CoriolisURL
        {
            get
            {
                return coriolisURL;
            }
            set
            {
                coriolisURL = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingString("CorolisURL", value);
            }
        }

        public string EDDShipyardURL
        {
            get
            {
                return eddshipyardURL;
            }
            set
            {
                eddshipyardURL = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingString("EDDShipyardURL", value);
            }
        }

        public string Language         // as standard culture en-gb or en etc, or Auto
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
                EliteDangerousCore.DB.UserDatabase.Instance.PutSettingString("DefaultLanguage", value);
            }
        }

        public string EDSMFullSystemsURL   
        {
            get
            {
                return null;
            }
        }

        public string EDDBSystemsURL
        {
            get
            {
                return null;
            }
        }


        #endregion

        #region Update at start

       
        public void Update(bool write = true)     // call at start to populate above
        {
            try
            {
                useNotifyIcon = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingBool("UseNotifyIcon", false);
                orderrowsinverted = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingBool("OrderRowsInverted", false);
                minimizeToNotifyIcon = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingBool("MinimizeToNotifyIcon", false);
                keepOnTop = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingBool("KeepOnTop", false);
                displayTimeFormat = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingInt("DisplayUTC", 2);
                language = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingString("DefaultLanguage", "Auto");
                coriolisURL = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingString("CorolisURL", EDDLite.Properties.Resources.URLCoriolis);
                eddshipyardURL = EliteDangerousCore.DB.UserDatabase.Instance.GetSettingString("EDDShipyardURL", EDDLite.Properties.Resources.URLEDShipyard);

                if (eddshipyardURL == "http://www.edshipyard.com/")     // 30/jul/19 changed address
                    EDDShipyardURL = "http://edsy.org/";

                EliteDangerousCore.EDCommander.Load(write);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("EDDConfig.Update()" + ":" + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }

        }

        #endregion

    }
}

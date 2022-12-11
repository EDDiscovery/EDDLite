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

namespace EDDLite
{
    public enum EDTx
    {
        Warning, // Warning
        Today, // Today
        t24h, // 24h
        t7days, // 7 days
        All, // All
        OK, // OK
        Cancel, // Cancel
        Delete, // Delete
        Campaign, // Campaign
        NoScan, // No Scan
        Systemnotknown, // System not known
        Travel, // Travel
        GameTime, // Game Time
        Time, // Time
        NoData, // No Data
        None, // None
        on, // on
        Off, // Off
        Unknown, // Unknown
        Information, // Information
        NoPos, // No Pos

        EDDiscoveryForm_DLLW, // The following application extension DLLs have been found
        EDDiscoveryForm_DLLL, // DLLs loaded: {0}
        EDDiscoveryForm_DLLF, // DLLs failed to load: {0}
        EDDiscoveryForm_NI, // New EDDiscovery installer available: {0}
        EDDiscoveryForm_NRA, // New Release Available!

        EDDiscoveryForm_RemoveDLLPerms, // Confirm dll etc

        UserControlSettings_AddC, // Commander name is not valid or duplicate
        UserControlSettings_AddT, // Cannot create Commander
        UserControlSettings_DelCmdr, // delete commander
        
    }

    public static class EDTranslatorExtensions
    {
        static public string T(this string s, EDTx value)              // use the enum.
        {
            return BaseUtils.Translator.Instance.Translate(s, value.ToString().Replace("_", "."));
        }
    }
}

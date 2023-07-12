using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealState.Session
{
    public class Governorate
    {
        public List<string> get_nearest_govn(string name)
        {
            Dictionary<string, List<string>> Governorates = new Dictionary<string, List<string>>();

            Governorates.Add("Ad Daqahliyah", new List<string> { "Ash Sharqiyah", "Dumyat", "Al Buheira", "Al Qalyubiyah" });
            Governorates.Add("Al Bahr al Ahmar", new List<string> { "South Sinai", "Qina", "Luxor" });
            Governorates.Add("Al Buheira", new List<string> { "Kafr ash Shaykh", "Al Gharbiyah", "Ad Daqahliyah" });
            Governorates.Add("Al Fayyum", new List<string> { "Al Jizah", "Bani Suwayf", "Al Minya" });
            Governorates.Add("Al Gharbiyah", new List<string> { "Kafr ash Shaykh", "Dumyat", "Al Minufiyah", "Ad Daqahliyah" });

            Governorates.Add("Al Iskandariyah", new List<string> { "Al Buheira", "Matruh", "Kafr ash Shaykh" });
            Governorates.Add("Al Isma'iliyah", new List<string> { "Port Said", "Suez", "Ash Sharqiyah" });
            Governorates.Add("Al Jizah", new List<string> { "Cairo", "Faiyum", "Bani Suwayf" });
            Governorates.Add("Al Minufiyah", new List<string> { "Gharbia", "Ad Daqahliyah", "Al Qalyubiyah" });
            Governorates.Add("Al Minya", new List<string> { "Bani Suwayf", "Al Fayyum", "Asyut", "Suhaj" });

            Governorates.Add("Cairo", new List<string> { "Kafr ash Shaykh", "Dumyat", "Al Minufiyah", "Ad Daqahliyah" });
            Governorates.Add("Al Qalyubiyah", new List<string> { "Al Jizah", "Cairo", "Ash Sharqiyah" });
            Governorates.Add("l Wadi al AJadid", new List<string> { "Aswan", "Qina" });
            Governorates.Add("Ash Sharqiyah", new List<string> { "Ad Daqahliyah", "Al Isma'iliyah", "Port Said" });
            Governorates.Add("Aswan", new List<string> { "Luxor", "Al Bahr al Ahmar", "l Wadi al AJadid" });

            Governorates.Add("Asyut", new List<string> { "Al Minya", "Suhaj", "l Wadi al AJadid" });
            Governorates.Add("Bani Suwayf", new List<string> { "Al Fayyum", "Al Minya", "Al Jizah" });
            Governorates.Add("Port Said", new List<string> { "Al Isma'iliyah", "Suez", "Ad Daqahliyah" });
            Governorates.Add("South Sinai", new List<string> { "North Sinai", "Suez", "Al Bahr al Ahmar" });
            Governorates.Add("Kafr ash Shaykh", new List<string> { "Al Buheira", "Al Gharbiyah", "Ad Daqahliyah" });

            Governorates.Add("Matruh", new List<string> { "Al Iskandariyah", "Al Buheira", "Kafr El Sheikh" });
            Governorates.Add("Qina", new List<string> { "Luxor", "Aswan", "Al Bahr al Ahmar" });
            Governorates.Add("North Sinai", new List<string> { "Al Isma'iliyah", "Port Said", "Suez" });
            Governorates.Add("Suhaj", new List<string> { "Qina", "Asyut", "l Wadi al AJadid" });
            Governorates.Add("Dumyat", new List<string> { "Port Said", "Ash Sharqiyah", "Ad Daqahliyah" });

            List<string> list = Governorates[name];
            return list;
        }

        public List<string> Governorates_list()
        {
            List<string> Governorates = new List<string>();
            Governorates = new List<string>();
            Governorates.Add("None");
            Governorates.Add("Ad Daqahliyah");
            Governorates.Add("Al Bahr al Ahmar");
            Governorates.Add("Al Buheira");
            Governorates.Add("Al Fayyum");
            Governorates.Add("Al Gharbiyah");

            Governorates.Add("Al Iskandariyah");
            Governorates.Add("Al Isma'iliyah");
            Governorates.Add("Al Jizah");
            Governorates.Add("Al Minufiyah");
            Governorates.Add("Al Minya");

            Governorates.Add("Cairo");
            Governorates.Add("Al Qalyubiyah");
            Governorates.Add("l Wadi al AJadid");
            Governorates.Add("Ash Sharqiyah");
            Governorates.Add("Aswan");

            Governorates.Add("Asyut");
            Governorates.Add("Bani Suwayf");
            Governorates.Add("Port Said");
            Governorates.Add("South Sinai");
            Governorates.Add("Kafr ash Shaykh");

            Governorates.Add("Matruh");
            Governorates.Add("Qina");
            Governorates.Add("North Sinai");
            Governorates.Add("Suhaj");
            Governorates.Add("Dumyat");

            return Governorates;
        }
    }
}
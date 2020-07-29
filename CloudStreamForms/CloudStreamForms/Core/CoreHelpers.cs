﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CloudStreamForms.Core.CloudStreamCore;

namespace CloudStreamForms.Core
{
    public class BlotFreeProvider
    {
        public struct NonBloatSeasonData
        {
            public string name; // ID OF PROVIDER
            public bool subExists => subEpisodes.ContainsStuff();
            public bool dubExists => dubEpisodes.ContainsStuff();
            public List<string> subEpisodes;
            public List<string> dubEpisodes;
            public object extraData;
        }

        public class BloatFreeBaseAnimeProvider : BaseAnimeProvider
        {
            public BloatFreeBaseAnimeProvider(CloudStreamCore _core) : base(_core) { }

            public override int GetLinkCount(int currentSeason, bool isDub, TempThread? tempThred)
            {
                int count = 0;
                try {
                    for (int q = 0; q < activeMovie.title.MALData.seasonData[currentSeason].seasons.Count; q++) {
                        var list = activeMovie.title.MALData.seasonData[currentSeason].seasons[q].nonBloatSeasonData.Where(t => t.name == Name).ToList();
                        if (list.Count > 0) {
                            var ms = list[0];
                            if ((ms.dubExists && isDub) || (ms.subExists && !isDub)) {
                                count += (isDub ? ms.dubEpisodes.Count : ms.subEpisodes.Count);
                            }
                        }
                    }
                }
                catch (Exception) {
                }
                return count;
            }

            public NonBloatSeasonData GetData(MALSeason data, out bool suc)
            {
                var list = data.nonBloatSeasonData.Where(t => t.name == Name).ToList();
                suc = list.Count > 0;
                return suc ? list[0] : new NonBloatSeasonData();
            }

            public override void GetHasDubSub(MALSeason data, out bool dub, out bool sub)
            {
                var _data = GetData(data, out bool suc);
                if (suc) {
                    dub = _data.dubExists;
                    sub = _data.subExists;
                }
                else {
                    dub = false;
                    sub = false;
                }
            }

            public virtual NonBloatSeasonData GetSeasonData(MALSeason ms, TempThread tempThread, string year, object storedData)
            {
                throw new NotImplementedException();
            }

            public virtual object StoreData(string year, TempThread tempThred, MALData malData)
            {
                throw new NotImplementedException();
            }

            public override void FishMainLink(string year, TempThread tempThred, MALData malData)
            {
                object storedData = StoreData(year, tempThred, malData);
                for (int i = 0; i < activeMovie.title.MALData.seasonData.Count; i++) {
                    for (int q = 0; q < activeMovie.title.MALData.seasonData[i].seasons.Count; q++) {
                        try {
                            MALSeason ms;
                            lock (_lock) {
                                ms = activeMovie.title.MALData.seasonData[i].seasons[q];
                            }

                            NonBloatSeasonData data = GetSeasonData(ms, tempThred, year, storedData);
                            data.name = Name;

                            lock (_lock) {
                                ms = activeMovie.title.MALData.seasonData[i].seasons[q];
                                if (ms.nonBloatSeasonData == null) {
                                    ms.nonBloatSeasonData = new List<NonBloatSeasonData>();
                                }
                                ms.nonBloatSeasonData.Add(data);
                                activeMovie.title.MALData.seasonData[i].seasons[q] = ms;
                            }
                        }
                        catch (Exception _ex) {
                            print("FATAL EX IN Fish " + Name + " | " + _ex);
                        }
                    }
                }
            }

            public virtual void LoadLink(string episodeLink, int episode, int normalEpisode, TempThread tempThred, object extraData)
            {
                throw new NotImplementedException();
            }

            public override void LoadLinksTSync(int episode, int season, int normalEpisode, bool isDub, TempThread tempThred)
            {
                int currentep = 0;
                for (int q = 0; q < activeMovie.title.MALData.seasonData[season].seasons.Count; q++) {
                    var ms = GetData(activeMovie.title.MALData.seasonData[season].seasons[q], out bool suc);
                    if (suc) {
                        int subEp = episode - currentep;
                        currentep += isDub ? ms.dubEpisodes.Count : ms.subEpisodes.Count;
                        if (currentep > episode) {
                            try {
                                print("LOADING LINK FOR: " + Name);
                                LoadLink(isDub ? ms.dubEpisodes[subEp] : ms.subEpisodes[subEp], subEp, normalEpisode, tempThred, ms.extraData);
                            }
                            catch (Exception _ex) { print("FATAL EX IN Load: " + Name + " | " + _ex); }
                        }
                    }
                }
            }
        }
    }

    public static class CoreHelpers
    {
        public static bool ContainsStuff<T>(this IList<T> list)
        {
            if (list == null) return false;
            if (list.Count == 0) return false;
            return true;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsMovie(this MovieType mtype)
        {
            return mtype == MovieType.AnimeMovie || mtype == MovieType.Movie || mtype == MovieType.YouTube;
        }

        /// <summary>
        /// If is not null and is not ""
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsClean(this string s)
        {
            return s != null && s != "";
        }


        static readonly List<Type> types = new List<Type>() { typeof(decimal), typeof(int), typeof(string), typeof(bool), typeof(double), typeof(ushort), typeof(ulong), typeof(uint), typeof(short), typeof(short), typeof(char), typeof(long), typeof(float), };

        public static string FString(this object o, string _s = "")
        {
            return "";
#if RELEASE
            return "";
#endif
#if DEBUG
            if (o == null) {
                return "Null";
            }
            Type valueType = o.GetType();

            if (o is IList) {
                IList list = (o as IList);
                string s = valueType.Name + " {";
                for (int i = 0; i < list.Count; i++) {
                    s += "\n	" + _s + i + ". " + list[i].FString(_s + "	");
                }
                return s + "\n" + _s + "}";
            }


            if (!types.Contains(valueType) && !valueType.IsArray && !valueType.IsEnum) {
                string s = valueType.Name + " {";
                foreach (var field in valueType.GetFields()) {
                    s += ("\n	" + _s + field.Name + " => " + field.GetValue(o).FString(_s + "	"));
                }
                return s + "\n" + _s + "}";
            }
            else {
                if (valueType.IsArray) {
                    int _count = 0;
                    var enu = ((o) as IEnumerable).GetEnumerator();
                    string s = valueType.Name + " {";
                    while (enu.MoveNext()) {
                        s += "\n	" + _count + ". " + enu.Current.FString(_s + "	");
                        _count++;
                    }
                    return s + "\n" + _s + "}";
                }
                else if (valueType.IsEnum) {
                    return valueType.GetEnumName(o);
                }
                else {
                    return o.ToString();
                }
            }
#endif

        }

        public static string RString(this object o)
        {
            string s = "VALUE OF: ";
            foreach (var field in o.GetType().GetFields()) {
                s += ("\n" + field.Name + " => " + field.GetValue(o).ToString());
            }
            return s;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

// About RIOT_API ====================================================================
// RIOT_API From -> https://developer.riotgames.com/

public class RIOT_API : SingletonObj<RIOT_API>
{
    /* ==== Base Methods ==== */
    private IEnumerator GetRequest_Base(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            //Debug.Log("Try to get request by \"" + uri + "\"");

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Has Error On getting request \"" + uri + "\" :: ERROR : " + webRequest.error);
                yield break;
            }
            yield return webRequest.downloadHandler.text;
        }
    }
    private IEnumerator GetRequestTexture_Base(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri))
        {
            //Debug.Log("Try to get request by \"" + uri + "\"");

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Has Error On getting request \"" + uri + "\" :: ERROR : " + webRequest.error);
                yield break;
            }
            yield return DownloadHandlerTexture.GetContent(webRequest);
        }
    }


    /* ==== Required Reference(like using #define) ==== */
    // About API_KEY
    private readonly string MY_API_KEY = "<your-rgapi-key>";
    private string API_KEY_TO_PARAM => "?api_key=" + MY_API_KEY;

    // About Version
    private List<string> versions;
    private string CURR_VERSION => versions[0];

    // About Platform
    private string CURR_PLATFORM; // get from UI on info scene

    // About Links - API ref
    // => base URI
    private string API_BASE_URI => "https://" + CURR_PLATFORM + ".api.riotgames.com/";
    // => others
    private string API_SPECTATOR_URI => API_BASE_URI + "lol/spectator/v4/";
    private string API_SUMMONER_URI => API_BASE_URI + "lol/summoner/v4/";

    // About Links - DataDragon ref
    // => base URI
    private readonly string DATA_DRAGON_BASE_URI = "https://ddragon.leagueoflegends.com/";
    private string DATA_DRAGON_IMG_BASE_URI => DATA_DRAGON_BASE_URI + "cdn/" + CURR_VERSION + "/img/";
    // => img URI
    private string DATA_DRAGON_IMG_PORTRAIT_URI(string imageFilename)
                    => DATA_DRAGON_IMG_BASE_URI + "champion/" + imageFilename;
    private string DATA_DRAGON_IMG_SPELL_URI(string spellFilename)
                    => DATA_DRAGON_IMG_BASE_URI + "spell/" + spellFilename;
    private string DATA_DRAGON_IMG_PASSIVE_URI(string passiveFilename)
                    => DATA_DRAGON_IMG_BASE_URI + "passive/" + passiveFilename;
    // => others
    private string DATA_DRAGON_SPLASH_URI(string championId, int index)
                    => DATA_DRAGON_BASE_URI + "cdn/img/champion/splash/" + championId + "_" + index + ".jpg";
    private string DATA_DRAGON_CHAMPION_URI(string championId)
                    => DATA_DRAGON_BASE_URI + "cdn/" + CURR_VERSION + "/data/ko_KR/champion/" + championId + ".json";


    /* ==== SUMMONER ==== */
    // Initial Definition
    [Serializable]
    public class SUMMONER    // ver. SUMMONER-V4
    {
        // variable for return
        public string accountId;
        public int profileIconId;
        public long revisionDate;
        public string name;
        public string id;
        public string puuid;
        public long summonerLevel;


        public bool IsNull => accountId == null;

        // Only for Debugging
        public void Print()
        {
            Debug.Log(
                "accountId = " + accountId + "\n" +
                "profileIconId = " + profileIconId + "\n" +
                "revisionDate = " + revisionDate + "\n" +
                "name = " + name + "\n" +
                "id = " + id + "\n" +
                "puuid = " + puuid + "\n" +
                "summonerLevel = " + summonerLevel
            );
        }
    }
    public SUMMONER Summoner { get; private set; }

    // Get SUMMONER Func
    ///<summary>
    /// A function that allows the resultant value of function 'GetSummoner'
    /// to be substituted for variable 'Summoner' of type SUMMONER predefined
    ///</summary>
    public IEnumerator GetSummonerByAccount(string encryptedAccountId)
    {
        // Set Summoner to New one by account
        yield return GetSummoner(API_SUMMONER_URI +
                                 "summoners/by-account/" +
                                 encryptedAccountId +
                                 API_KEY_TO_PARAM);
    }
    ///<summary>
    /// A function that allows the resultant value of function 'GetSummoner'
    /// to be substituted for variable 'Summoner' of type SUMMONER predefined
    ///</summary>
    public IEnumerator GetSummonerByName(string summonerName)
    {
        // Set Summoner to New one by name
        yield return GetSummoner(API_SUMMONER_URI +
                                 "summoners/by-name/" +
                                 summonerName +
                                 API_KEY_TO_PARAM);
    }
    ///<summary>
    /// A function that allows the resultant value of function 'GetSummoner'
    /// to be substituted for variable 'Summoner' of type SUMMONER predefined
    ///</summary>
    public IEnumerator GetSummonerByPuuid(string encryptedPUUID)
    {
        // Set Summoner to New one by puuid
        yield return GetSummoner(API_SUMMONER_URI +
                                 "summoners/by-puuid/" +
                                 encryptedPUUID +
                                 API_KEY_TO_PARAM);
    }

    ///<summary>  Get SUMMONER Information from URI and Apply to Summoner </summary>
    ///<param name="uri"> Get Request through this </param>
    private IEnumerator GetSummoner(string uri)
    {
        GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.GetSummoner; // because of networking Sequence

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequest_Base(uri));  // return : webRequest.downloadHandler.text
        yield return getRequestWithData.coroutine;

        Summoner = JsonUtility.FromJson<SUMMONER>((string)getRequestWithData.result);
        //Summoner.Print();

        yield return GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.None;  // give a signal when work's done
    }


    /* ==== SPECTATOR ==== */
    // Initial Definition
    [Serializable]
    public class SPECTATOR    // ver. SPECTATOR-V4
    {
        // modified class definition for JsonUtility.FromJson
        [Serializable] public class BannedChampion{
            public int pickTurn = 0;
            public long championId = 0;
            public long teamId = 0;

            public override string ToString() => "{" +
                "pickTurn = " + pickTurn + ", " +
                "championId = " + championId + ", " +
                "teamId = " + teamId + 
            "}";
        }
        [Serializable] public class Observer
        {
            public string encryptionKey = "";

            public override string ToString() => "{" + 
                "encryptionKey = " + encryptionKey +
            "}";
        }
        [Serializable] public class CurrentGameParticipant
        {
            [Serializable] public class Perks
            {
                public List<long> perkIds = new List<long>();
                public long perkStyle = 0;
                public long perkSubStyle = 0;

                public override string ToString() => "{" +
                    "perkIds = [" + string.Join(", ", perkIds) + "], " +
                    "perkStyle = " + perkStyle + ", " +
                    "perkSubStyle = " + perkSubStyle +
                "}";
            }
            [Serializable] public class GameCustomizationObject
            {
                public string category = "";
                public string content = "";

                public override string ToString() => "{" +
                    "category = " + category + ", " +
                    "content = " + content +
                "}";
            }

            public long championId = 0;
            public Perks perks = new Perks();
            public long profileIconId = 0;
            public bool bot = false;
            public long teamId = 0;
            public string summonerName = "";
            public string summonerId = "";
            public long spell1Id = 0;
            public long spell2Id = 0;
            public List<GameCustomizationObject> gameCustomizationObjects = new List<GameCustomizationObject>();

            public override string ToString() => "{" +
                "championId = " + championId + ", " +
                "perks = " + perks.ToString() + ", " +
                "profileIconId = " + profileIconId + ", " +
                "bot = " + bot + ", " +
                "teamId = " + teamId + ", " +
                "summonerName = " + summonerName + ", " +
                "summonerId = " + summonerId + ", " +
                "spell1Id = " + spell1Id + ", " +
                "spell2Id = " + spell2Id + ", " +
                "gameCustomizationObjects = " + string.Join(", ", gameCustomizationObjects) +
            "}";

            // ==== custom define ====
            public List<SUMMONER_SPELLS_DATA.Data> GetSummonerSpellsInfoOfMember()
            {
                List<SUMMONER_SPELLS_DATA.Data> ret = new List<SUMMONER_SPELLS_DATA.Data>
                {
                    Instance.GetDataAbout_A_SummonerSpell(spell1Id),
                    Instance.GetDataAbout_A_SummonerSpell(spell2Id)
                };

                return ret;
            }
        }

        // variable for return
        public long gameId = 0;
        public string gameType = "";
        public long gameStartTime = 0;
        public long mapId = 0;
        public long gameLength = 0;
        public string platformId = "";
        public string gameMode = "";
        public List<BannedChampion> bannedChampions = new List<BannedChampion>();
        public long gameQueueConfigId = 0;
        public Observer observers = new Observer();
        public List<CurrentGameParticipant> participants = new List<CurrentGameParticipant>();


        // ==== custom define ====
        // To determine if the SPECTATOR is null
        public bool IsNull => gameId == 0;

        // To increase usability
        public class Member
        {
            public CurrentGameParticipant userInfo;
            public CHAMPIONS_DATA.Data usersChampionInfo;

            public override string ToString() => "{" +
                "userInfo = " + userInfo.ToString() + ", " +
                "usersChampionInfo = " + usersChampionInfo.ToString() +
            "}";
        }
        public List<Member> GetTeamMembersOfParticipants(int teamNum)
        {
            List<Member> ret = new List<Member>();

            long targetTeam = teamNum * 100;

            foreach(CurrentGameParticipant item in participants)
            {
                if(item.teamId == targetTeam)
                {
                    Member tmp = new Member
                    {
                        userInfo = item,
                        usersChampionInfo = Instance.GetDataAbout_A_Champion(item.championId)
                    };
                    ret.Add(tmp);
                }
            }

            return ret;
        }
        public void SwapMembersInList(ref List<Member> target, int aIndex, int bIndex)
        {
            Member tempA = target[aIndex];
            Member tempB = target[bIndex];

            target.RemoveAt(aIndex);
            target.Insert(aIndex, tempB);
            target.RemoveAt(bIndex);
            target.Insert(bIndex, tempA);
        }


        // Only for Debugging
        public void Print()
        {
            Debug.Log(
                "gameId = " + gameId + "\n" +
                "gameType = " + gameType + "\n" +
                "gameStartTime = " + gameStartTime + "\n" +
                "mapId = " + mapId + "\n" +
                "gameLength = " + gameLength + "\n" +
                "platformId = " + platformId + "\n" +
                "gameMode = " + gameMode + "\n" +
                "gameLength = " + gameLength + "\n" +
                "bannedChampions = " + string.Join(", ", bannedChampions) + "\n" +
                "gameQueueConfigId = " + gameQueueConfigId + "\n" +
                "observers = " + observers.ToString() + "\n" +
                "participants = " + string.Join(", ", participants)
            );
        }
    }
    public SPECTATOR Spectator { get; private set; }

    // Get SPECTATOR Func
    ///<summary>
    /// A function that allows the resultant value of function 'GetSpectator'
    /// to be substituted for variable 'Spectator' of type SPECTATOR predefined
    ///</summary>
    public IEnumerator GetSpectatorBySummoner(string encryptedSummonerId)
    {
        yield return GetSpectator(API_SPECTATOR_URI +
                                  "active-games/by-summoner/" +
                                  encryptedSummonerId +
                                  API_KEY_TO_PARAM);
    }

    ///<summary>  Get SPECTATOR Information from URI and Apply to Spectator </summary>
    ///<param name="uri"> Get Request through this </param>
    private IEnumerator GetSpectator(string uri)
    {
        GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.GetSpectator; // because of networking Sequence

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequest_Base(uri));  // return : webRequest.downloadHandler.text
        yield return getRequestWithData.coroutine;

        Spectator = JsonUtility.FromJson<SPECTATOR>((string)getRequestWithData.result);
        //Spectator.Print();

        yield return GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.None;  // give a signal when work's done
    }


    /* ==== CHAMPION DATA ==== */
    // initial definition
    [Serializable]
    public class CHAMPIONS_DATA
    {
        [Serializable] public class Data
        {
            [Serializable] public class Info
            {
                public int attack = 0;
                public int defense = 0;
                public int magic = 0;
                public int difficulty = 0;

                public override string ToString() => "{" +
                    "attack = " + attack + ", " +
                    "defense = " + defense + ", " +
                    "magic = " + magic + ", " +
                    "difficulty = " + difficulty +
                "}";
            }
            [Serializable] public class Image
            {
                public string full = "";
                public string sprite = "";
                public string group = "";
                public long x = 0;
                public long y = 0;
                public long w = 0;
                public long h = 0;

                public override string ToString() => "{" +
                    "full = " + full + ", " +
                    "sprite = " + sprite + ", " +
                    "group = " + group + ", " +
                    "x = " + x + ", " +
                    "y = " + y + ", " +
                    "w = " + w + ", " +
                    "h = " + h +
                "}";


                // ==== custom define ====
                public Texture texture;
            }
            [Serializable] public class Stats
            {
                public long hp = 0;
                public long hpperlevel = 0;
                public long mp = 0;
                public long mpperlevel = 0;
                public long movespeed = 0;
                public double armor = 0;
                public double armorperlevel = 0;
                public double spellblock = 0;
                public double spellblockperlevel = 0;
                public long attackrange = 0;
                public double hpregen = 0;
                public double hpregenperlevel = 0;
                public double mpregen = 0;
                public double mpregenperlevel = 0;
                public long crit = 0;
                public long critperlevel = 0;
                public double attackdamage = 0;
                public double attackdamageperlevel = 0;
                public double attackspeedperlevel = 0;
                public double attackspeed = 0;

                public override string ToString() => "{" +
                    "hp = " + hp + ", " +
                    "hpperlevel = " + hpperlevel + ", " +
                    "mp = " + mp + ", " +
                    "mpperlevel = " + mpperlevel + ", " +
                    "movespeed = " + movespeed + ", " +
                    "armor = " + armor + ", " +
                    "armorperlevel = " + armorperlevel + ", " +
                    "spellblock = " + spellblock + ", " +
                    "spellblockperlevel = " + spellblockperlevel + ", " +
                    "attackrange = " + attackrange + ", " +
                    "hpregen = " + hpregen + ", " +
                    "hpregenperlevel = " + hpregenperlevel + ", " +
                    "mpregen = " + mpregen + ", " +
                    "mpregenperlevel = " + mpregenperlevel + ", " +
                    "crit = " + crit + ", " +
                    "critperlevel = " + critperlevel + ", " +
                    "attackdamage = " + attackdamage + ", " +
                    "attackdamageperlevel = " + attackdamageperlevel + ", " +
                    "attackspeedpervel = " + attackspeedperlevel + ", " +
                    "attackspeed = " + attackspeed +
                "}";
            }

            public string version = "";
            public string id = "";
            public string key = "";
            public string name = "";
            public string title = "";
            public string blurb = "";
            public Info info = new Info();
            public Stats stats = new Stats();
            public Image image = new Image();
            public List<string> tags = new List<string>();
            public string partype = "";
            
            public override string ToString() => "{" +
                "version = " + version + ", " +
                "id = " + id + ", " +
                "key = " + key + ", " +
                "name = " + name + ", " +
                "title = " + title + ", " +
                "blurb = " + SummarizeTooLongSentence(blurb, 6) + ", " +
                "info = " + info.ToString() + ", " +
                "stats = " + stats.ToString() + ", " +
                "image = " + image.ToString() + ", " +
                "tags = " + string.Join(", ", tags) + ", " +
                "partype = " + partype + ", " +
                "advancedData = " + advancedData.ToString() +
            "}";

            // ==== custom define ====
            [Serializable]
            public class AdvancedData
            {
                // To Deserialize
                [Serializable] public class Skin
                {
                    public string id = "";
                    public long num = 0;
                    public string name = "";
                    public bool chromas = false;

                    public override string ToString() => "{" +
                        "id = " + id + ", " +
                        "num = " + num + ", " +
                        "name = " + name + ", " +
                        "chromas = " + chromas +
                    "}";
                }
                [Serializable] public class Spell
                {
                    [Serializable] public class Leveltip
                    {
                        public List<string> label = new List<string>();
                        public List<string> effect = new List<string>();

                        public override string ToString() => "{" +
                            "label = " + string.Join(", ", label) + ", " +
                            "effect = " + string.Join(", ", effect) +
                        "}";
                    }
                    [Serializable] public class Datavalues
                    {


                        public override string ToString() => "{" +
                        "}";
                    }
                    [Serializable] public class Var
                    {
                        public string link = "";
                        //public List<double> coeff = new List<double>(); // TODO : solve this prob(Sometimes 'List<double>', sometimes just 'double')
                        public string key = "";

                        public override string ToString() => "{" +
                            "link = " + link + ", " +
                            //"coeff = " + string.Join(", ", coeff) + ", " +
                            "key = " + key +
                        "}";
                    }

                    public string id = "";
                    public string name = "";
                    public string description = "";
                    public string tooltip = "";
                    public Leveltip leveltip = new Leveltip();
                    public int maxrank = 0;
                    public List<double> cooldown = new List<double>();
                    public string cooldownBurn = "";
                    public List<int> cost = new List<int>();
                    public string costBurn = "";
                    public Datavalues datavalues = new Datavalues();
                    public List<List<long>> effect = new List<List<long>>();
                    public List<string> effectBurn = new List<string>();
                    public List<Var> vars = new List<Var>();
                    public string costType = "";
                    public string maxammo = "";
                    public List<long> range = new List<long>();
                    public string rangeBurn = "";
                    public Image image = new Image();
                    public string resource = "";

                    public override string ToString() => "{" +
                        "id = " + id + ", " +
                        "name = " + name + ", " +
                        "description = " + SummarizeTooLongSentence(description, 6) + ", " +
                        "tooltip = " + SummarizeTooLongSentence(tooltip, 6) + ", " +
                        "leveltip = " + leveltip.ToString() + ", " +
                        "maxrank = " + maxrank + ", " +
                        "cooldown = " + string.Join(", ", cooldown) + ", " +
                        "cooldownBurn = " + cooldownBurn + ", " +
                        "cost = " + string.Join(", ", cost) + ", " +
                        "costBurn = " + costBurn + ", " +
                        "datavalues = " + datavalues.ToString() + ", " +
                        "effect = " + ToStringListOfList(effect) + ", " +
                        "effectBurn = " + string.Join(", ", effectBurn) + ", " +
                        "vars = " + string.Join(", ", vars) + ", " +
                        "costType = " + costType + ", " +
                        "maxammo = " + maxammo + ", " +
                        "range = " + string.Join(", ", range) + ", " +
                        "rangeBurn = " + rangeBurn + ", " +
                        "image = " + image.ToString() + ", " +
                        "resource = " + resource +
                    "}";

                    private string ToStringListOfList<T>(List<List<T>> target)
                    {
                        string ret = "";

                        ret += "[";
                        if (target != null)
                        {
                            for (int i = 0; i < target.Count; i++)
                            {
                                ret += "[";

                                if (target[i] != null)
                                {
                                    for (int c = 0; c < target[i].Count; c++)
                                    {
                                        ret += target[i][c].ToString();

                                        if (c < target[i].Count - 1)
                                        {
                                            ret += ", ";
                                        }
                                    }
                                }
                                ret += "]";

                                if (i < target.Count - 1)
                                {
                                    ret += ", ";
                                }
                            }
                        }
                        ret += "]";

                        return ret;
                    }
                }
                [Serializable] public class Passive
                {
                    public string name = "";
                    public string description = "";
                    public Image image = new Image();

                    public override string ToString() => "{" +
                        "name = " + name + ", " +
                        "description = " + SummarizeTooLongSentence(description, 6) + ", " +
                        "image = " + image.ToString() +
                    "}";
                }

                public List<Skin> skins = new List<Skin>();
                public string lore = "";
                public List<string> allytips = new List<string>();
                public List<string> enemytips = new List<string>();
                public List<Spell> spells = new List<Spell>();
                public Passive passive = new Passive();

                public override string ToString() => "{" +
                    "skins = " + string.Join(", ", skins) + ", " +
                    "lore = " + SummarizeTooLongSentence(lore, 6) + ", " +
                    "allytips = " + string.Join(", ", allytips) + ", " +
                    "enemytips = " + string.Join(", ", enemytips) + ", " +
                    "spells = " + string.Join(", ", spells) + ", " +
                    "passive = " + passive.ToString() +
                "}";


                // only for JSON deserialize
                public Dictionary<string, AdvancedData> data = new Dictionary<string, AdvancedData>();
            }
            public AdvancedData advancedData = new AdvancedData();

            public Texture defaultSplashImage;

            // TODO : there is no need for summoner spell to be in champion data(need for spectator)
            public Texture dSpellImage;
            public Texture fSpellImage;
        }

        public string type = "";
        public string format = "";
        public string version = "";
        public Dictionary<string, Data> data = new Dictionary<string, Data>();

        public void Print()
        {
            Debug.Log(
                "type = " + type + "\n" +
                "format = " + format + "\n" +
                "version = " + version + "\n" +
                "data = " + string.Join(", ", data)
            );
        }
    }
    public CHAMPIONS_DATA ChampionsData { get; private set; }

    // Utility to find champion by "Key".
    // The reason why this utility is needed is because "Key" is the only information that can be obtained about Champion
    private SortedList<long, string> championListSortedByKey; // long = key, string = championName
    private void MakeChampionListSortedByKey()
    {
        championListSortedByKey = new SortedList<long, string>(ChampionsData.data.Count);

        foreach (KeyValuePair<string, CHAMPIONS_DATA.Data> pair in ChampionsData.data)
        {
            long keyTemp = Convert.ToInt64(pair.Value.key);
            string championTemp = pair.Key;
            championListSortedByKey.Add(keyTemp, championTemp);
        }
        //foreach(KeyValuePair<string, string> pair in championListSortedByKey) { Debug.Log("KEY = " + pair.Key + ", CHAMPION = " + pair.Value); }
    }

    // Get CHAMPIONS_DATA Func
    ///<summary>  Get CHAMPIONS_DATA Information from URI and Apply to ChampionsData </summary>
    public IEnumerator GetChampionsData()
    {
        GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.GetChampionsData;

        string uri = DATA_DRAGON_BASE_URI + "cdn/" + CURR_VERSION + "/data/ko_KR/champion.json";

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequest_Base(uri));
        yield return getRequestWithData.coroutine;

        ChampionsData = JsonConvert.DeserializeObject<CHAMPIONS_DATA>((string)getRequestWithData.result);
        //ChampionsData.Print();

        MakeChampionListSortedByKey();

        yield return GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.None;
    }

    ///<summary>  Get advanced data of champion and then Apply datas to that champion's data  </summary>
    ///<param name="championId"> champion id to which you wanna get advanced data </param>
    public IEnumerator GiveAdvancedChampionData(string championId)
    {
        string uri = DATA_DRAGON_CHAMPION_URI(championId);

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequest_Base(uri));
        yield return getRequestWithData.coroutine;

        ChampionsData.data[championId].advancedData =
            JsonConvert.DeserializeObject<CHAMPIONS_DATA.Data.AdvancedData>
                ((string)getRequestWithData.result).data[championId];
    }
    ///<summary>  Get indexed texture of champion and then Apply a Texture to that champion's data  </summary>
    ///<param name="championId"> champion id to which you wanna get advanced data </param>
    ///<param name="index">
    ///which 'index' of splashes you want
    ///<para>- '0' : default splash index</para>
    ///</param>
    public IEnumerator GiveChampionSpalsh(string championId, int index = 0)
    {
        string uri = DATA_DRAGON_SPLASH_URI(championId, index);

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequestTexture_Base(uri));
        yield return getRequestWithData.coroutine;

        ChampionsData.data[championId].defaultSplashImage = (Texture)getRequestWithData.result;
    }
    ///<summary>  Get portrait image of champion and then Apply a Texture to that champion's data  </summary>
    ///<param name="championId"> champion id to which you wanna get advanced data </param>
    public IEnumerator GiveChampionPortraitImage(string championId)
    {
        string imageFullname = ChampionsData.data[championId].image.full;
        string uri = DATA_DRAGON_IMG_PORTRAIT_URI(imageFullname);

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequestTexture_Base(uri));
        yield return getRequestWithData.coroutine;

        ChampionsData.data[championId].image.texture = (Texture)getRequestWithData.result;
    }
    ///<summary>  Get spells[Q,W,E,R] image of champion and then Apply a Texture to that champion's data  </summary>
    ///<param name="championId"> champion id to which you wanna get advanced data </param>
    public IEnumerator GiveChampionSpellsImage(string championId)
    {
        string uri;
        CoroutineWithData getRequestWithData;
        string spellFullname;
        
        var spells = ChampionsData.data[championId].advancedData.spells;

        for(int i = 0; i < spells.Count; i++)
        {
            spellFullname = spells[i].image.full;
            uri = DATA_DRAGON_IMG_SPELL_URI(spellFullname);

            getRequestWithData = new CoroutineWithData(this, GetRequestTexture_Base(uri));
            yield return getRequestWithData.coroutine;

            ChampionsData.data[championId].advancedData.spells[i].image.texture = (Texture)getRequestWithData.result;
        }
    }
    ///<summary>  Get passive image of champion and then Apply a Texture to that champion's data  </summary>
    ///<param name="championId"> champion id to which you wanna get advanced data </param>
    public IEnumerator GiveChampionPassiveImage(string championId)
    {
        string passiveFullname = ChampionsData.data[championId].advancedData.passive.image.full;
        string uri = DATA_DRAGON_IMG_PASSIVE_URI(passiveFullname);

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequestTexture_Base(uri));
        yield return getRequestWithData.coroutine;

        ChampionsData.data[championId].advancedData.passive.image.texture = (Texture)getRequestWithData.result;
    }

    ///<summary>  Get data that is type Data about only one champion By champion key  </summary>
    ///<param name="championKey"> key for the champion to which you want to get data </param>
    ///<returns> Target champion's data </returns>
    public CHAMPIONS_DATA.Data GetDataAbout_A_Champion(long championKey)
    {
        return ChampionsData.data[championListSortedByKey[championKey]];
    }
    ///<summary>  Get data that is type Data about only one champion By champion id  </summary>
    ///<param name="championId"> id for the champion to which you want to get data </param>
    ///<returns> Target champion's data </returns>
    public CHAMPIONS_DATA.Data GetDataAbout_A_Champion(string championId)
    {
        return ChampionsData.data[championId];
    }


    /* ==== SUMMONER SPELL DATA ==== */
    // initial definition
    [Serializable]
    public class SUMMONER_SPELLS_DATA
    {
        [Serializable] public class Data
        {

            [Serializable] public class Datavalues
            {


                public override string ToString() => "{" +
                "}";
            }
            [Serializable] public class Var
            {
                public string link = "";
                //public List<double> coeff = new List<double>(); // TODO : solve this prob(Sometimes 'List<double>', sometimes just 'double')
                public string key = "";

                public override string ToString() => "{" +
                    "link = " + link + ", " +
                    //"coeff = " + string.Join(", ", coeff) + ", " +
                    "key = " + key +
                "}";
            }
            [Serializable] public class Image
            {
                public string full = "";
                public string sprite = "";
                public string group = "";
                public long x = 0;
                public long y = 0;
                public long w = 0;
                public long h = 0;

                public override string ToString() => "{" +
                    "full = " + full + ", " +
                    "sprite = " + sprite + ", " +
                    "group = " + group + ", " +
                    "x = " + x + ", " +
                    "y = " + y + ", " +
                    "w = " + w + ", " +
                    "h = " + h +
                "}";


                // ==== custom define ====
                public Texture texture;
            }

            public string id = "";
            public string name = "";
            public string description = "";
            public int maxrank = 0;
            public List<double> cooldown = new List<double>();
            public string cooldownBurn = "";
            public List<int> cost = new List<int>();
            public string costBurn = "";
            public Datavalues datavalues = new Datavalues();
            public List<List<long>> effect = new List<List<long>>();
            public List<string> effectBurn = new List<string>();
            public List<Var> vars = new List<Var>();
            public string key = "";
            public long summonerLevel = 0;
            public List<string> modes = new List<string>();
            public string costType = "";
            public string maxammo = "";
            public List<long> range = new List<long>();
            public string rangeBurn = "";
            public Image image = new Image();
            public string resource = "";

            public override string ToString() => "{" +
                "id = " + id + ", " +
                "name = " + name + ", " +
                "description = " + SummarizeTooLongSentence(description, 6) + ", " +
                "maxrank = " + maxrank + ", " +
                "cooldown = " + string.Join(", ", cooldown) + ", " +
                "cooldownBurn = " + cooldownBurn + ", " +
                "cost = " + string.Join(", ", cost) + ", " +
                "costBurn = " + costBurn + ", " +
                "datavalues = " + datavalues.ToString() + ", " +
                "effect = " + ToStringListOfList(effect) + ", " +
                "effectBurn = " + string.Join(", ", effectBurn) + ", " +
                "vars = " + string.Join(", ", vars) + ", " +
                "key = " + key + ", " +
                "summonerLevel = " + summonerLevel + ", " +
                "modes = " + string.Join(", ", modes) + ", " +
                "maxammo = " + maxammo + ", " +
                "range = " + string.Join(", ", range) + ", " +
                "rangeBurn = " + rangeBurn + ", " +
                "image = " + image.ToString() + ", " +
                "resource = " + resource +
            "}";

            private string ToStringListOfList<T>(List<List<T>> target)
            {
                string ret = "";

                ret += "[";
                if (target != null)
                {
                    for (int i = 0; i < target.Count; i++)
                    {
                        ret += "[";

                        if (target[i] != null)
                        {
                            for (int c = 0; c < target[i].Count; c++)
                            {
                                ret += target[i][c].ToString();

                                if (c < target[i].Count - 1)
                                {
                                    ret += ", ";
                                }
                            }
                        }
                        ret += "]";

                        if (i < target.Count - 1)
                        {
                            ret += ", ";
                        }
                    }
                }
                ret += "]";

                return ret;
            }
        }

        public string type = "";
        public string version = "";
        public Dictionary<string, Data> data = new Dictionary<string, Data>();

        public void Print()
        {
            Debug.Log(
                "type = " + type + "\n" +
                "version = " + version + "\n" +
                "data = " + string.Join(", ", data)
            );
        }
    }
    public SUMMONER_SPELLS_DATA SummonerSpellsData { private set; get; }

    // Utility to find summoner spell by "Key".
    // The reason why this utility is needed is because "Key" is the only information that can be obtained about SummonerSpell
    private SortedList<long, string> summonerSpellListSortedByKey; // long = key, string = championName
    private void MakeSummonerSpellListSortedByKey()
    {
        summonerSpellListSortedByKey = new SortedList<long, string>(SummonerSpellsData.data.Count);

        foreach(KeyValuePair<string, SUMMONER_SPELLS_DATA.Data> pair in SummonerSpellsData.data)
        {
            long keyTemp = Convert.ToInt64(pair.Value.key);
            string summonerSpellTemp = pair.Key;
            summonerSpellListSortedByKey.Add(keyTemp, summonerSpellTemp);
        }
    }

    // Get SUMMONER_SPELLS_DATA Func
    ///<summary>  Get SUMMONER_SPELLS_DATA Information(included Image texture) from URI and Apply to SummonerSpellsData </summary>
    public IEnumerator GetSummonerSpellsData()
    {
        GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.GetSummonerSpellsData;

        string uri = DATA_DRAGON_BASE_URI + "cdn/" + CURR_VERSION + "/data/ko_KR/summoner.json";

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequest_Base(uri));
        yield return getRequestWithData.coroutine;

        SummonerSpellsData = JsonConvert.DeserializeObject<SUMMONER_SPELLS_DATA>((string)getRequestWithData.result);
        GetSummonerTeleportCooldown();
        //SummonerSpellsData.Print();

        MakeSummonerSpellListSortedByKey();

        // with image texture
        yield return GiveAllOfSummonerSpellImage();

        yield return GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.None;
    }
    // Temporary 'alpha' Func(because of incomplete data)
    private void GetSummonerTeleportCooldown()
    {
        List<double> teleCooldown = new List<double>();
        string teleCooldownBurn = "";
        double cooldown_Base = 420;

        for(int i = 0; i < 18; i++)
        {
            double currCooldown = Math.Round(cooldown_Base - i * (180.0 / 17.0), 1);
            teleCooldown.Add(currCooldown);
            teleCooldownBurn += currCooldown.ToString();
            if (i < 17) teleCooldownBurn += "/";
        }

        SummonerSpellsData.data["SummonerTeleport"].cooldown = teleCooldown;
        SummonerSpellsData.data["SummonerTeleport"].cooldownBurn = teleCooldownBurn;
    }

    ///<summary>  give texture image into summoner spells data  </summary>
    public IEnumerator GiveAllOfSummonerSpellImage()
    {
        foreach(SUMMONER_SPELLS_DATA.Data a in SummonerSpellsData.data.Values)
        {
            yield return GiveSummonerSpellImage(a.id);
        }
    }
    ///<summary>  Get image of spell and then Apply a Texture to that summoner spells data  </summary>
    ///<param name="spellId"> spell id to which you wanna get image </param>
    public IEnumerator GiveSummonerSpellImage(string spellId)
    {
        string summonerSpellFullname = SummonerSpellsData.data[spellId].image.full;
        string uri = DATA_DRAGON_IMG_SPELL_URI(summonerSpellFullname);

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequestTexture_Base(uri));
        yield return getRequestWithData.coroutine;

        SummonerSpellsData.data[spellId].image.texture = (Texture)getRequestWithData.result;
    }

    ///<summary>  Get data that is type Data about only one summoner spell By spell key  </summary>
    /// <param name="spellKey"> key for the summoner spell to which you want to get data </param>
    /// <returns></returns>
    public SUMMONER_SPELLS_DATA.Data GetDataAbout_A_SummonerSpell(long spellKey)
    {
        return SummonerSpellsData.data[summonerSpellListSortedByKey[spellKey]];
    }
    ///<summary>  Get data that is type Data about only one summoner spell By spell id </summary>
    /// <param name="spellId"> key for the summoner spell to which you want to get data </param>
    /// <returns></returns>
    public SUMMONER_SPELLS_DATA.Data GetDataAbout_A_SummonerSpell(string spellId)
    {
        return SummonerSpellsData.data[spellId];
    }


    /* ==== PERKS DATA ==== */
    // initial definition
    [Serializable]
    public class PERKS_DATA
    {
        // TODO : fill
    }


    /* ==== Utility for Private Use ==== */
    ///<summary>  To summarize sentence if sentence length is too long(over the maximum length)  </summary>
    ///<param name="sentence"> target sentence </param>
    ///<param name="maxLength"> maximum length to contribute to the summary </param>
    private static string SummarizeTooLongSentence(string sentence, int maxLength) =>
        sentence == "" ?
            "" :
            Math.Min(sentence.Length, maxLength) == sentence.Length ?
                sentence.Substring(0, sentence.Length) + " ..." :
                sentence.Substring(0, maxLength);


    /* ==== AND SO ON ==== */
    // to initailize
    public void Init()
    {
        Summoner = null;
        Spectator = null;
    }

    ///<summary>  Get all of versions to confirm current version  </summary>
    public IEnumerator GetVersions()
    {
        GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.GetVersion;

        string uri = DATA_DRAGON_BASE_URI + "api/versions.json";

        CoroutineWithData getRequestWithData = new CoroutineWithData(this, GetRequest_Base(uri));
        yield return getRequestWithData.coroutine;

        versions = JsonConvert.DeserializeObject<List<string>>((string)getRequestWithData.result);

        yield return GameManager.Instance.NETWORK_PROCESS = GameManager.NetworkProcess.None;
    }

    ///<summary>  Set CURR_PLATFORM variable to Current Platform(param)  </summary>
    ///<param name="platform"> current platform for changing </param>
    public void SetCurrPlatform(string platform)
    {
        CURR_PLATFORM = platform;
    }
}

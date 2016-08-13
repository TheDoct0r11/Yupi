/**
     Because i love chocolat...                                      
                                    88 88  
                                    "" 88  
                                       88  
8b       d8 88       88 8b,dPPYba,  88 88  
`8b     d8' 88       88 88P'    "8a 88 88  
 `8b   d8'  88       88 88       d8 88 ""  
  `8b,d8'   "8a,   ,a88 88b,   ,a8" 88 aa  
    Y88'     `"YbbdP'Y8 88`YbbdP"'  88 88  
    d8'                 88                 
   d8'                  88     
   
   Private Habbo Hotel Emulating System
   @author Claudio A. Santoro W.
   @author Kessiler R.
   @version dev-beta
   @license MIT
   @copyright Sulake Corporation Oy
   @observation All Rights of Habbo, Habbo Hotel, and all Habbo contents and it's names, is copyright from Sulake
   Corporation Oy. Yupi! has nothing linked with Sulake. 
   This Emulator is Only for DEVELOPMENT uses. If you're selling this you're violating Sulakes Copyright.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Yupi.Emulator.Data.Base.Adapters.Interfaces;
using Yupi.Emulator.Game.Browser.Enums;
using Yupi.Emulator.Game.Browser.Models;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms.Data.Models;


namespace Yupi.Emulator.Game.Browser
{
    /// <summary>
    ///     Class NavigatorManager.
    /// </summary>
     public class HotelBrowserManager
    {
        /// <summary>
        ///     The _public items
        /// </summary>
        public readonly Dictionary<uint, PublicItem> PublicRooms;

        /// <summary>
        ///     The _navigator headers
        /// </summary>
        public readonly List<NavigatorHeader> NavigatorHeaders;

        /// <summary>
        ///     The in categories
        /// </summary>
     public Dictionary<string, NavigatorCategory> NavigatorCategories;

        /// <summary>
        ///     The private categories
        /// </summary>
     public HybridDictionary PrivateCategories;

        /// <summary>
        ///     The promo categories
        /// </summary>
     public Dictionary<int, PromoCategory> PromoCategories;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HotelBrowserManager" /> class.
        /// </summary>
     public HotelBrowserManager()
        {
            PrivateCategories = new HybridDictionary();

            NavigatorCategories = new Dictionary<string, NavigatorCategory>();

            PublicRooms = new Dictionary<uint, PublicItem>();

            NavigatorHeaders = new List<NavigatorHeader>();

            PromoCategories = new Dictionary<int, PromoCategory>();
        }

        /// <summary>
        ///     Get the Number of Flat Caegories
        /// </summary>
        /// <value>The flat cats count.</value>
     public int FlatCatsCount => PrivateCategories.Count;

        /// <summary>
        ///     Initializes the specified database client.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        /// <param name="navLoaded">The nav loaded.</param>
        public void Initialize(IQueryAdapter dbClient, out uint navLoaded)
        {
            Initialize(dbClient);

            navLoaded = (uint) NavigatorHeaders.Count;
        }

        /// <summary>
        ///     Initializes the specified database client.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        public void Initialize(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM navigator_flatcats WHERE `enabled` = '2'");
            DataTable navigatorFlatCats = dbClient.GetTable();

            dbClient.SetQuery("SELECT * FROM navigator_publics");
            DataTable navigatorPublicRooms = dbClient.GetTable();

            dbClient.SetQuery("SELECT * FROM navigator_promocats");
            DataTable navigatorPromoCats = dbClient.GetTable();

            if (navigatorPromoCats != null)
            {
                PromoCategories.Clear();

                foreach (DataRow dataRow in navigatorPromoCats.Rows)
                    PromoCategories.Add((int) dataRow["id"],
                        new PromoCategory((int) dataRow["id"], (string) dataRow["caption"], (int) dataRow["min_rank"],
                            Yupi.EnumToBool((string) dataRow["visible"])));
            }

            if (navigatorFlatCats != null)
            {
                PrivateCategories.Clear();

                foreach (DataRow dataRow in navigatorFlatCats.Rows)
                    PrivateCategories.Add((int) dataRow["id"],
                        new PublicCategory((int) dataRow["id"], (string) dataRow["caption"], (int) dataRow["min_rank"]));
            }

            if (navigatorPublicRooms != null)
            {
                PublicRooms.Clear();

                foreach (DataRow row in navigatorPublicRooms.Rows)
                    PublicRooms.Add(Convert.ToUInt32(row["id"]),
                        new PublicItem(Convert.ToUInt32(row["id"]), int.Parse(row["bannertype"].ToString()),
                            (string) row["caption"],
                            (string) row["description"], (string) row["image"],
                            row["image_type"].ToString().ToLower() == ""
                                ? PublicImageType.Internal
                                : PublicImageType.External, (uint) row["room_id"], 0, (int) row["category_parent_id"],
                            row["recommended"].ToString() == "1", (int) row["typeofdata"]));
            }

            InitializeCategories();
        }

        public void InitializeCategories()
        {
            using (IQueryAdapter dbClient = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM navigator_pubcats");
                DataTable navigatorPublicCats = dbClient.GetTable();

                dbClient.SetQuery("SELECT * FROM navigator_sub_pubcats");
                DataTable navigatorSubCats = dbClient.GetTable();

                List<NavigatorSubCategory> subCategories = new List<NavigatorSubCategory>();

                if (navigatorSubCats != null)
                    subCategories.AddRange(from DataRow dataRow in navigatorSubCats.Rows select new NavigatorSubCategory((int)dataRow["id"], (string)dataRow["caption"], (string)dataRow["main_cat"], (string)dataRow["default_state"] == "opened", (string)dataRow["default_size"] == "image"));

                if (navigatorPublicCats != null)
                {
                    NavigatorCategories.Clear();

                    foreach (DataRow dataRow in navigatorPublicCats.Rows)
                        NavigatorCategories.Add((string)dataRow["caption"], new NavigatorCategory((int)dataRow["id"], (string)dataRow["caption"], (string)dataRow["default_state"] == "opened", (string)dataRow["default_size"] == "image", subCategories.Where(c => c.MainCategory == (string)dataRow["caption"]).ToList()));
                }
            }
        }

        public void AddPublicRoom(PublicItem item)
        {
            if (item == null)
                return;

            PublicRooms.Add(Convert.ToUInt32(item.Id), item);
        }

        public void RemovePublicRoom(uint id)
        {
            if (!PublicRooms.ContainsKey(id))
                return;

            PublicRooms.Remove(id);
        }
			
        /// <summary>
        ///     Gets the flat cat.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>FlatCat.</returns>
     public PublicCategory GetFlatCat(int id)
            => PrivateCategories.Contains(id) ? (PublicCategory) PrivateCategories[id] : null;

       
        /// <summary>
        ///     Gets the name of the flat cat identifier by.
        /// </summary>
        /// <param name="flatName">Name of the flat category.</param>
     public int GetFlatCatIdByName(string flatName) => PrivateCategories.Values.Cast<PublicCategory>().First(flat => flat?.Caption == flatName).Id;

        /// <summary>
        ///     Gets a navigator category by caption
        /// </summary>
        /// <param name="navigatorCategoryCaption">Name of the category.</param>
     public NavigatorCategory GetNavigatorCategory(string navigatorCategoryCaption) => NavigatorCategories.FirstOrDefault(c => c.Key == navigatorCategoryCaption).Value;

        /// <summary>
        ///     Gets a Public Room Data
        /// </summary>
        /// <param name="roomId">Public Room Id.</param>
     public PublicItem GetPublicRoom(uint roomId)
        {
            IEnumerable<KeyValuePair<uint, PublicItem>> search = PublicRooms.Where(i => i.Value.RoomId == roomId);

            IEnumerable<KeyValuePair<uint, PublicItem>> keyValuePairs = search as KeyValuePair<uint, PublicItem>[] ?? search.ToArray();

            return !keyValuePairs.Any() || keyValuePairs.FirstOrDefault().Value == null ? null : keyValuePairs.FirstOrDefault().Value;
        }

        /// <summary>
        ///     Gets the new length of the navigator.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
    
    }
}
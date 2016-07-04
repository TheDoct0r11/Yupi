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
using System.Drawing;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Pets.Enums;
using Yupi.Emulator.Game.Pets.Structs;
using Yupi.Emulator.Game.Rooms;
using Yupi.Emulator.Messages;

namespace Yupi.Emulator.Game.Pets
{
    /// <summary>
    ///     Class Pet.
    /// </summary>
     public class Pet
    {
        /// <summary>
        ///     The anyone can ride
        /// </summary>
     public int AnyoneCanRide;

        /// <summary>
        ///     The breading tile
        /// </summary>
     public Point BreadingTile;

        /// <summary>
        ///     The color
        /// </summary>
     public string Color;

        /// <summary>
        ///     The creation stamp
        /// </summary>
     public double CreationStamp;

        /// <summary>
        ///     The database state
        /// </summary>
     public DatabaseUpdateState DbState;

        /// <summary>
        ///     The energy
        /// </summary>
     public uint Energy;

        /// <summary>
        ///     The experience
        /// </summary>
     public uint Experience;

        /// <summary>
        ///     The experience levels
        /// </summary>
     public int[] ExperienceLevels =
        {
            100,
            200,
            400,
            600,
            1000,
            1300,
            1800,
            2400,
            3200,
            4300,
            7200,
            8500,
            10100,
            13300,
            17500,
            23000,
            51900
        };

        /// <summary>
        ///     The hair dye
        /// </summary>
     public int HairDye;

        /// <summary>
        ///     The have saddle
        /// </summary>
     public bool HaveSaddle;

        /// <summary>
        ///     The last health
        /// </summary>
     public DateTime LastHealth;

        /// <summary>
        ///     The mopla breed
        /// </summary>
     public MoplaBreed MoplaBreed;

        /// <summary>
        ///     The name
        /// </summary>
     public string Name;

        /// <summary>
        ///     The nutrition
        /// </summary>
     public uint Nutrition;

        /// <summary>
        ///     The owner identifier
        /// </summary>
     public uint OwnerId;

        /// <summary>
        ///     The pet hair
        /// </summary>
     public int PetHair;

        /// <summary>
        ///     The pet identifier
        /// </summary>
     public uint PetId;

        /// <summary>
        ///     The placed in room
        /// </summary>
     public bool PlacedInRoom;

        /// <summary>
        ///     The race
        /// </summary>
     public uint Race;

     public uint RaceId;

        /// <summary>
        ///     The rarity
        /// </summary>
     public int Rarity;

        /// <summary>
        ///     The respect
        /// </summary>
     public uint Respect;

        /// <summary>
        ///     The room identifier
        /// </summary>
     public uint RoomId;

        /// <summary>
        ///     The type
        /// </summary>
     public string Type;

        /// <summary>
        ///     The until grown
        /// </summary>
     public DateTime UntilGrown;

        /// <summary>
        ///     The virtual identifier
        /// </summary>
     public int VirtualId;

        /// <summary>
        ///     The waiting for breading
        /// </summary>
     public uint WaitingForBreading;

        /// <summary>
        ///     The x
        /// </summary>
     public int X;

        /// <summary>
        ///     The y
        /// </summary>
     public int Y;

        /// <summary>
        ///     The z
        /// </summary>
     public double Z;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pet" /> class.
        /// </summary>
        /// <param name="petId">The pet identifier.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="race">The race.</param>
        /// <param name="experience">The experience.</param>
        /// <param name="energy">The energy.</param>
        /// <param name="nutrition">The nutrition.</param>
        /// <param name="respect">The respect.</param>
        /// <param name="creationStamp">The creation stamp.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="havesaddle">if set to <c>true</c> [havesaddle].</param>
        /// <param name="anyoneCanRide">The anyone can ride.</param>
        /// <param name="dye">The dye.</param>
        /// <param name="petHer">The pet her.</param>
        /// <param name="rarity">The rarity.</param>
        /// <param name="lastHealth">The last health.</param>
        /// <param name="untilGrown">The until grown.</param>
        /// <param name="moplaBreed">The mopla breed.</param>
        /// <param name="color"></param>
     public Pet(uint petId, uint ownerId, uint roomId, string name, string type, uint race,
            uint experience, uint energy, uint nutrition, uint respect, double creationStamp, int x, int y, double z,
            bool havesaddle, int anyoneCanRide, int dye, int petHer, int rarity, DateTime lastHealth,
            DateTime untilGrown, MoplaBreed moplaBreed, string color)
        {
            PetId = petId;
            OwnerId = ownerId;
            RoomId = roomId;
            Name = name;
            Type = type;
            Race = race;
            Experience = experience;
            Energy = energy;
            Nutrition = nutrition;
            Respect = respect;
            CreationStamp = creationStamp;
            X = x;
            Y = y;
            Z = z;
            PlacedInRoom = false;
            DbState = DatabaseUpdateState.Updated;
            HaveSaddle = havesaddle;
            AnyoneCanRide = anyoneCanRide;
            PetHair = petHer;
            HairDye = dye;
            Rarity = rarity;
            LastHealth = lastHealth;
            UntilGrown = untilGrown;
            MoplaBreed = moplaBreed;
            WaitingForBreading = 0;
            RaceId = PetTypeManager.GetPetRaceIdByType(Type);
            Color = color;
        }

        /// <summary>
        ///     The maximum level.
        /// </summary>
        /// <value>The maximum level.</value>
     public const uint MaxLevel = 20;

        /// <summary>
        ///     The maximum energy
        /// </summary>
        /// <value>The maximum energy.</value>
	public const uint MaxEnergy = 100;

        /// <summary>
        ///     Gets the maximum nutrition.
        /// </summary>
        /// <value>The maximum nutrition.</value>
     public const uint MaxNutrition = 150;

        /// <summary>
        ///     Gets the room.
        /// </summary>
        /// <value>The room.</value>
		public Room Room {
			get {
				return !IsInRoom ? null : Yupi.GetGame().GetRoomManager().GetRoom(RoomId);
			}
		}

        /// <summary>
        ///     Gets a value indicating whether this instance is in room.
        /// </summary>
        /// <value><c>true</c> if this instance is in room; otherwise, <c>false</c>.</value>
		public bool IsInRoom {
			get {
				return RoomId > 0u;
			}
		}

        /// <summary>
        ///     Gets the level.
        /// </summary>
        /// <value>The level.</value>
     public int Level
        {
            get
            {
                {
                    for (int i = 0; i < ExperienceLevels.Length; i++)
                        if (Experience < ExperienceLevels[i])
                            return i + 1;

                    return ExperienceLevels.Length + 1;
                }
            }
        }

        /// <summary>
        ///     Gets the experience goal.
        /// </summary>
        /// <value>The experience goal.</value>
     public int ExperienceGoal 
		{ 
			get {
				return ExperienceLevels [Level - 1];
			}
	 }

        /// <summary>
        ///     Gets the age.
        /// </summary>
        /// <value>The age.</value>
		public int Age {
			get { 
				return (int)(DateTime.Now - Yupi.UnixToDateTime (CreationStamp)).TotalDays;
			}
		}

        /// <summary>
        ///     Gets the look.
        /// </summary>
        /// <value>The look.</value>
		public string Look {
			get { 
				return string.Concat (RaceId, " ", Race, " ", Color);
			}
		}

        /// <summary>
        ///     Gets the name of the owner.
        /// </summary>
        /// <value>The name of the owner.</value>
		public string OwnerName {
			get { 
				return Yupi.GetGame ().GetClientManager ().GetUserNameByUserId (OwnerId);
			}
		}

        /// <summary>
        ///     Called when [respect].
        /// </summary>
     public void OnRespect()
        {
            Respect++;

            GameClient ownerSession = Yupi.GetGame().GetClientManager().GetClientByUserId(OwnerId);

            if (ownerSession != null)
                Yupi.GetGame()
                    .GetAchievementManager()
                    .ProgressUserAchievement(ownerSession, "ACH_PetRespectReceiver", 1);

			router.GetComposer<RespectPetMessageComposer> ().Compose (this.Room, this.VirtualId);
			router.GetComposer<PetRespectNotificationMessageComposer> ().Compose (this.Room, this);

            if (DbState != DatabaseUpdateState.NeedsInsert)
                DbState = DatabaseUpdateState.NeedsUpdate;

            if (Type != "pet_monster" && Experience <= 51900)
                AddExperience(10);

            if (Type == "pet_monster")
                Energy = 100;

            LastHealth = DateTime.Now.AddSeconds(129600.0);
        }

        /// <summary>
        ///     Adds the experience.
        /// </summary>
        /// <param name="amount">The amount.</param>
     public void AddExperience(uint amount)
        {
            int oldExperienceGoal = ExperienceGoal;

            Experience += amount;

            if (Experience >= 51900)
                return;

            if (DbState != DatabaseUpdateState.NeedsInsert)
                DbState = DatabaseUpdateState.NeedsUpdate;

            if (Room == null)
                return;

			this.Room.Router.GetComposer<AddPetExperienceMessageComposer> ().Compose (this.Room, this, amount);

            if (Experience < oldExperienceGoal)
                return;

            GameClient ownerSession = Yupi.GetGame().GetClientManager().GetClientByUserId(OwnerId);

            Dictionary<uint, PetCommand> totalPetCommands = PetCommandHandler.GetAllPetCommands();

            Dictionary<uint, PetCommand> petCommands = PetCommandHandler.GetPetCommandByPetType(Type);

            if (ownerSession == null)
                return;

			router.GetComposer<NotifyNewPetLevelMessageComposer> ().Compose (ownerSession, this);
			router.GetComposer<PetTrainerPanelMessageComposer> ().Compose (ownerSession, this.PetId);
        }

        /// <summary>
        ///     Pets the energy.
        /// </summary>
        /// <param name="add">if set to <c>true</c> [add].</param>
     public void PetEnergy(bool add)
        {
            uint num;

            if (add)
            {
                if (Energy > 100)
                {
                    Energy = 100;

                    return;
                }

                if (Energy > 85)
                    return;

                if (Energy > 85)
                    num = MaxEnergy - Energy;
                else
                    num = 10;
            }

            else
                num = 15;

            if (num <= 4)
                num = 15;

            uint randomNumber = (uint) Yupi.GetRandomNumber(4, (int) num);

            if (!add)
            {
                Energy -= randomNumber;

                if (Energy == 0)
                    Energy = 1;
            }
            else
                Energy += randomNumber;

            if (DbState != DatabaseUpdateState.NeedsInsert)
                DbState = DatabaseUpdateState.NeedsUpdate;
        }
    }
}
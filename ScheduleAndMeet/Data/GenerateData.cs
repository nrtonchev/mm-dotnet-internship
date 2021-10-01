using Newtonsoft.Json;
using ScheduleAndMeet.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ScheduleAndMeet.Data
{
	public class GenerateData
	{
		private ICollection<Room> _rooms;
		private string _jsonFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"..\..\..\..\Data\RoomSchedules.json";

		public GenerateData()
		{
			string jsonString = File.ReadAllText(_jsonFilePath);
			_rooms = JsonConvert.DeserializeObject<ICollection<Room>>(jsonString);
		}

		public ICollection<Room> GetAllRooms()
		{
			return _rooms;
		}

		/// <summary>
		/// Adding booked schedules to collection
		/// </summary>
		/// <param name="roomName">The room for which the booking is made</param>
		/// <param name="booking">The timeslot for the booking</param>
		public void AddSchedules(string roomName, Timeslot booking)
		{
			var roomToBook = _rooms.FirstOrDefault(x => x.RoomName == roomName);
			roomToBook.Schedule.Add(booking);
			SerializeJson();
		}

		/// <summary>
		/// This method converts the collection of rooms into a string and records it into the JSON file
		/// </summary>
		private void SerializeJson()
		{
			string jsonString = JsonConvert.SerializeObject(_rooms, Formatting.Indented); ;
			File.WriteAllText(_jsonFilePath, jsonString);
		}
	}
}

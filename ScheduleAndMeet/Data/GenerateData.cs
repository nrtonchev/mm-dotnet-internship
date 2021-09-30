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

		public void AddSchedules(string roomName, Timeslot booking)
		{
			var roomToBook = _rooms.FirstOrDefault(x => x.RoomName == roomName);
			roomToBook.Schedule.Add(booking);
			SerializeJson();
		}

		private void SerializeJson()
		{
			string jsonString = JsonConvert.SerializeObject(_rooms, Formatting.Indented);
			File.WriteAllText(_jsonFilePath, jsonString);
		}
	}
}

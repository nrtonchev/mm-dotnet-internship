using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleAndMeet.Models
{
	public class Room
	{
		public string RoomName { get; set; }
		public int Capacity { get; set; }
		public TimeSpan AvailableFrom { get; set; }
		public TimeSpan AvailableTo { get; set; }
		public ICollection<Timeslot> Schedule { get; set; }
	}
}

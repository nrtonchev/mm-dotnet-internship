using Microsoft.AspNetCore.Mvc;
using ScheduleAndMeet.Data;
using ScheduleAndMeet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleAndMeet.Controllers
{
	public class ScheduleController : Controller
	{
		private readonly GenerateData _data;
		public ScheduleController()
		{
			_data = new GenerateData();
		}

		public IActionResult SchedulingTable(DateTime date, int capacity, int minutes)
		{
			var availableRooms = _data.GetAllRooms();
			Dictionary<Room, List<Timeslot>> availableSlots = new Dictionary<Room, List<Timeslot>>();

			if (availableRooms.Any(x => x.Capacity >= capacity))
			{
				TimeSpan requestedTime = TimeSpan.FromMinutes(minutes);

				foreach (var room in availableRooms.Where(x=>x.Capacity >= capacity))
				{
					availableSlots.Add(room, new List<Timeslot>());
					availableSlots[room].AddRange(AvailableSlots(room, date, requestedTime));
				}
			}

			return View(availableSlots);
		}

		public List<Timeslot> AvailableSlots(Room currRoom, DateTime date, TimeSpan requrestedTime)
		{
			TimeSpan from = currRoom.AvailableFrom;
			TimeSpan to = currRoom.AvailableTo;
			List<Timeslot> availableSlots = new List<Timeslot>();

			for (TimeSpan i = from; i <= to - requrestedTime; i += new TimeSpan(0, 15, 0))
			{
				DateTime fromDate = date.Date + i;
				DateTime toDate = date.Date + (i + requrestedTime);
				List<Timeslot> schedulesForSameDay = currRoom.Schedule.Where(x => x.From.Date == fromDate.Date).ToList();

				if (isAvailable(fromDate, toDate, schedulesForSameDay, requrestedTime) || currRoom.Schedule.Count == 0)
				{
					Timeslot toAdd = new Timeslot
					{
						From = fromDate,
						To = toDate
					};

					availableSlots.Add(toAdd);
				}
			}

			return availableSlots;
		}

		private bool isAvailable(DateTime from, DateTime to, List<Timeslot> schedulesForSameDay, TimeSpan requestedTime)
		{
			bool isAvailable = false;

			for (int i = 0; i < schedulesForSameDay.Count; i++)
			{
				if ((from + requestedTime) <= schedulesForSameDay[0].From)
				{
					isAvailable = true;
					break;
				}
				else if(schedulesForSameDay[i].To <= from && schedulesForSameDay[i + 1].From >= to)
				{
					isAvailable = true;
					break;
				}
			}

			return isAvailable;
		}
	}
}

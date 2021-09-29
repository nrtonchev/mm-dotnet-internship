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

		public IActionResult SchedulingTable()
		{
			GetRoomsTimespans(DateTime.Today, 3, 30);
			return View();
		}

		[HttpGet]
		public IActionResult GetRoomsTimespans(DateTime date, int capacity, int minutes)
		{
			var availableRooms = _data.GetAllRooms();

			if (availableRooms.Any(x => x.Capacity >= capacity))
			{
				TimeSpan requestedTime = TimeSpan.FromMinutes(minutes);
				Dictionary<Room, List<Timeslot>> availableSlots = new Dictionary<Room, List<Timeslot>>();

				foreach (var room in availableRooms)
				{
					availableSlots.Add(room, new List<Timeslot>());
					availableSlots[room].AddRange(AvailableSlots(room, date, requestedTime));
				}

				return View(availableSlots);
			}
			else
			{
				return NotFound();
			}
		}

		public List<Timeslot> AvailableSlots(Room currRoom, DateTime date, TimeSpan requrestedTime)
		{
			TimeSpan from = new TimeSpan(0, 0, 0);
			TimeSpan to = currRoom.AvailableTo;
			List<Timeslot> availableSlots = new List<Timeslot>();

			for (TimeSpan i = from; i <= to - requrestedTime; i += new TimeSpan(0, 15, 0))
			{
				DateTime fromDate = DateTime.Now.Date + i;
				DateTime toDate = DateTime.Now.Date + (i + requrestedTime);

				if (!isOccupied(currRoom, fromDate, toDate) || currRoom.Schedule.Count == 0)
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

		public bool isOccupied(Room currRoom, DateTime from, DateTime to)
		{
			//To refactor logic!
			//return currRoom.Schedule.Any(x => x.From == from || x.To == to || (x.From < from && x.To > to) || (x.From > from && x.From < to) || (x.To > from || x.To < to));
		}
	}
}

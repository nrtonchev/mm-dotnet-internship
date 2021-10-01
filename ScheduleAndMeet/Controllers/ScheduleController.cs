using Microsoft.AspNetCore.Mvc;
using ScheduleAndMeet.Data;
using ScheduleAndMeet.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleAndMeet.Controllers
{
	public class ScheduleController : Controller
	{
		private readonly GenerateData _data;
		public ScheduleController()
		{
			_data = new GenerateData();
		}

		/// <summary>
		/// This method accepts the parameters from the search form and returns results based on the search criteria
		/// </summary>
		/// <param name="date">The date the schedule is requested</param>
		/// <param name="capacity">The number of participants</param>
		/// <param name="minutes">Time in minutes of occupancy</param>
		/// <returns></returns>
		public IActionResult SchedulingTable(DateTime date, int capacity, int minutes)
		{
			var availableRooms = _data.GetAllRooms();
			Dictionary<Room, List<Timeslot>> availableSlots = new Dictionary<Room, List<Timeslot>>();

			if (availableRooms.Any(x => x.Capacity >= capacity) && minutes <= 1440)
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

		/// <summary>
		/// This methord books the requested timeslot and triggers the recording of the JSON file
		/// </summary>
		/// <param name="currRoom">The room for which the booking is made</param>
		/// <param name="slotFrom">The start time of the selected booking</param>
		/// <param name="slotTo">The end time of the selected booking</param>
		/// <returns></returns>
		public IActionResult BookTimeslot(string currRoom, string slotFrom, string slotTo)
		{
			Timeslot slotToPass = new Timeslot
			{
				From = DateTime.Parse(slotFrom),
				To = DateTime.Parse(slotTo)
			};

			_data.AddSchedules(currRoom, slotToPass);

			return View();
		}

		/// <summary>
		/// This method checks a room for available timeslots based on passed parameters
		/// </summary>
		/// <param name="currRoom">The room for which the check is made</param>
		/// <param name="date">The date of the check request</param>
		/// <param name="requrestedTime">The requested booking timespan</param>
		/// <returns></returns>
		private List<Timeslot> AvailableSlots(Room currRoom, DateTime date, TimeSpan requrestedTime)
		{
			TimeSpan from = currRoom.AvailableFrom;
			TimeSpan to = currRoom.AvailableTo;
			List<Timeslot> availableSlots = new List<Timeslot>();
			List<Timeslot> schedulesForSameDay = currRoom.Schedule.Where(x => x.From.Date == date.Date).ToList();

			for (TimeSpan i = from; i <= to - requrestedTime; i += new TimeSpan(0, 15, 0))
			{
				DateTime fromDate = date.Date + i;
				DateTime toDate = date.Date + (i + requrestedTime);
				
				if (schedulesForSameDay.Count == 0 || isAvailable(fromDate, toDate, schedulesForSameDay, requrestedTime))
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

		/// <summary>
		/// This method checks wether the timeslot is available for booking
		/// </summary>
		/// <param name="from">Date and start time of booking</param>
		/// <param name="to">Date and end time of booking</param>
		/// <param name="schedulesForSameDay">List of existing schedules of the same day</param>
		/// <param name="requestedTime">The requested booking timespan</param>
		/// <returns></returns>
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
				else if (i < schedulesForSameDay.Count - 1 && schedulesForSameDay[i].To <= from && schedulesForSameDay[i + 1].From >= to)
				{
					isAvailable = true;
					break;
				}
				else if(i == schedulesForSameDay.Count - 1 && schedulesForSameDay[i].To <= from)
				{
					isAvailable = true;
					break;
				}
			}

			return isAvailable;
		}
	}
}

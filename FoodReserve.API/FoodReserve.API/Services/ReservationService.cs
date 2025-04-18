﻿using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class ReservationService(DatabaseContext context)
    {
        public PagedList<ReservationResponse> GetAll(int pageNumber, int pageSize, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<ReservationResponse>.ToPagedList(
                    context.Set<Reservation>()
                        .Include(r => r.User)
                        .Include(r => r.Outlet)
                        .OrderByDescending(m => m.UpdatedAt.Date)
                        .ThenByDescending(m => m.UpdatedAt.TimeOfDay)
                        .OrderByDescending(m => m.CreatedAt.Date)
                        .ThenByDescending(m => m.CreatedAt.TimeOfDay)
                        .Select(r => (ReservationResponse)r),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<ReservationResponse>.ToPagedList(
                    context.Set<Reservation>()
                        .Include(r => r.User)
                        .Include(r => r.Outlet)
                        .Where(r => r.Name.Contains(keyword) || r.PhoneNumber.Contains(keyword))
                        .OrderByDescending(m => m.UpdatedAt.Date)
                        .ThenByDescending(m => m.UpdatedAt.TimeOfDay)
                        .OrderByDescending(m => m.CreatedAt.Date)
                        .ThenByDescending(m => m.CreatedAt.TimeOfDay)
                        .Select(r => (ReservationResponse)r),
                    pageNumber, pageSize);
            }
        }

        public PagedList<ReservationResponse> GetAllByOutletId(int pageNumber, int pageSize, string outletId, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<ReservationResponse>.ToPagedList(
                    context.Set<Reservation>()
                        .Include(r => r.User)
                        .Include(r => r.Outlet)
                        .Where(m => m.Outlet.Id == outletId)
                        .OrderByDescending(m => m.UpdatedAt.Date)
                        .ThenByDescending(m => m.UpdatedAt.TimeOfDay)
                        .OrderByDescending(m => m.CreatedAt.Date)
                        .ThenByDescending(m => m.CreatedAt.TimeOfDay)
                        .Select(r => (ReservationResponse)r),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<ReservationResponse>.ToPagedList(
                    context.Set<Reservation>()
                        .Include(r => r.User)
                        .Include(r => r.Outlet)
                        .Where(m => m.Outlet.Id == outletId)
                        .Where(r => r.Name.Contains(keyword) || r.PhoneNumber.Contains(keyword))
                        .OrderByDescending(m => m.UpdatedAt.Date)
                        .ThenByDescending(m => m.UpdatedAt.TimeOfDay)
                        .OrderByDescending(m => m.CreatedAt.Date)
                        .ThenByDescending(m => m.CreatedAt.TimeOfDay)
                        .Select(r => (ReservationResponse)r),
                    pageNumber, pageSize);
            }
        }

        public PagedList<ReservationResponse> GetAllByUserId(int pageNumber, int pageSize, string userId)
        {
            return PagedList<ReservationResponse>.ToPagedList(
                context.Set<Reservation>()
                    .Include(r => r.User)
                    .Include(r => r.Outlet)
                    .Where(r => r.User != null && r.User.Id == userId)
                    .OrderByDescending(m => m.UpdatedAt.Date)
                    .ThenByDescending(m => m.UpdatedAt.TimeOfDay)
                    .OrderByDescending(m => m.CreatedAt.Date)
                    .ThenByDescending(m => m.CreatedAt.TimeOfDay)
                    .Select(r => (ReservationResponse)r),
                pageNumber, pageSize);
        }

        public async Task<Reservation> GetByIdAsync(string id)
        {
            return await context.Reservations
                .Include(r => r.User)
                .Include(r => r.Outlet)
                .FirstOrDefaultAsync(r => r.Id == id) ?? throw new InvalidOperationException("Reservation not found");
        }

        public async Task CreateAsync(Reservation reservation)
        {
            try
            {
                await context.Reservations.AddAsync(reservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create reservation failed");
            }
        }

        public async Task CreateAsync(ReservationRequest reservationRequest)
        {
            var outlet = await context.Outlets.FindAsync(reservationRequest.OutletId)
                ?? throw new InvalidOperationException("Outlet not found");

            User? user = null;
            if (!string.IsNullOrEmpty(reservationRequest.UserId))
            {
                user = await context.Users.FindAsync(reservationRequest.UserId);
            }

            Reservation reservation = new()
            {
                User = user,
                Outlet = outlet,
                Name = reservationRequest.Name!,
                PhoneNumber = reservationRequest.PhoneNumber!,
                NumberOfGuest = reservationRequest.NumberOfGuest!,
                Date = reservationRequest.Date,
                Status = ReservationStatus.Pending
            };

            try
            {
                await context.Reservations.AddAsync(reservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create reservation failed");
            }
        }

        public async Task UpdateByIdAsync(string id, ReservationRequest reservationRequest)
        {
            var existingReservation = await GetByIdAsync(id);

            if (!string.IsNullOrEmpty(reservationRequest.OutletId))
            {
                var outlet = await context.Outlets.FindAsync(reservationRequest.OutletId)
                    ?? throw new InvalidOperationException("Outlet not found");
                existingReservation.Outlet = outlet;
            }

            if (!string.IsNullOrEmpty(reservationRequest.UserId))
            {
                var user = await context.Users.FindAsync(reservationRequest.UserId);
                existingReservation.User = user;
            }

            existingReservation.Name = reservationRequest.Name ?? existingReservation.Name;
            existingReservation.PhoneNumber = reservationRequest.PhoneNumber ?? existingReservation.PhoneNumber;
            existingReservation.NumberOfGuest = reservationRequest.NumberOfGuest;
            existingReservation.Date = reservationRequest.Date;
            existingReservation.Status = (ReservationStatus)reservationRequest.Status;

            try
            {
                context.Reservations.Update(existingReservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Update reservation failed");
            }
        }

        public async Task UpdateStatusAsync(string id, ReservationStatus status)
        {
            var existingReservation = await GetByIdAsync(id);
            existingReservation.Status = status;

            try
            {
                context.Reservations.Update(existingReservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Update reservation status failed");
            }
        }

        public async Task DeleteByIdAsync(string id)
        {
            var existingReservation = await GetByIdAsync(id);

            try
            {
                context.Reservations.Remove(existingReservation);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Delete reservation failed");
            }
        }
    }
}

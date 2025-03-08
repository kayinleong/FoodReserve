using FoodReserve.API.Context;
using FoodReserve.API.Models;
using FoodReserve.SharedLibrary.Constants;
using FoodReserve.SharedLibrary.Requests;
using FoodReserve.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Services
{
    public class CustomerService(DatabaseContext context)
    {
        public PagedList<CustomerResponse> GetAll(int pageNumber, int pageSize, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return PagedList<CustomerResponse>.ToPagedList(
                    context.Set<Customer>()
                        .Include(c => c.User)
                        .Include(c => c.Reservations)
                        .Select(c => (CustomerResponse)c),
                    pageNumber, pageSize);
            }
            else
            {
                return PagedList<CustomerResponse>.ToPagedList(
                    context.Set<Customer>()
                        .Include(c => c.User)
                        .Include(c => c.Reservations)
                        .Where(c => c.User.Username.Contains(keyword) || c.User.Email.Contains(keyword))
                        .Select(c => (CustomerResponse)c),
                    pageNumber, pageSize);
            }
        }

        public async Task<Customer> GetByIdAsync(string id)
        {
            return await context.Customers
                .Include(c => c.User)
                .Include(c => c.Reservations)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException("Customer not found");
        }

        public async Task<Customer> GetByUserIdAsync(string userId)
        {
            return await context.Customers
                .Include(c => c.User)
                .Include(c => c.Reservations)
                .FirstOrDefaultAsync(c => c.User.Id == userId) ?? throw new InvalidOperationException("Customer not found");
        }

        public async Task CreateAsync(Customer customer)
        {
            try
            {
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create customer failed");
            }
        }

        public async Task CreateAsync(CustomerRequest customerRequest)
        {
            var user = await context.Users.FindAsync(customerRequest.UserId) 
                ?? throw new InvalidOperationException("User not found");
            
            Customer customer = new()
            {
                User = user,
                Reservations = [],
                Status = (CustomerStatus)customerRequest.Status
            };

            try
            {
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Create customer failed");
            }
        }

        public async Task UpdateByIdAsync(string id, Customer customer)
        {
            var existingCustomer = await GetByIdAsync(id);

            if (customer.User != null && customer.User.Id != existingCustomer.User.Id)
            {
                var user = await context.Users.FindAsync(customer.User.Id)
                    ?? throw new InvalidOperationException("User not found");
                existingCustomer.User = user;
            }

            if (customer.Reservations != null && customer.Reservations.Count != 0)
            {
                existingCustomer.Reservations.Clear();

                foreach (var reservation in customer.Reservations)
                {
                    var existingReservation = await context.Reservations.FindAsync(reservation.Id);
                    if (existingReservation != null)
                    {
                        existingCustomer.Reservations.Add(existingReservation);
                    }
                }
            }

            existingCustomer.Status = customer.Status;

            try
            {
                context.Customers.Update(existingCustomer);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Update customer failed");
            }
        }

        public async Task ToggleStatusByIdAsync(string id)
        {
            var existingCustomer = await GetByIdAsync(id);
            existingCustomer.Status = existingCustomer.Status switch
            {
                CustomerStatus.Active => CustomerStatus.Banned,
                CustomerStatus.Banned => CustomerStatus.Active,
                _ => throw new InvalidOperationException("Invalid customer status")
            };
            try
            {
                context.Customers.Update(existingCustomer);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Toggle customer status failed");
            }
        }

        public async Task DeleteByIdAsync(string id)
        {
            var existingCustomer = await GetByIdAsync(id);

            try
            {
                context.Customers.Remove(existingCustomer);
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Delete customer failed");
            }
        }
    }
}

using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Payment.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

public class PaymentRepository(AppDbContext context) : RepositoryBase<Payment>(context), IPaymentRepository;
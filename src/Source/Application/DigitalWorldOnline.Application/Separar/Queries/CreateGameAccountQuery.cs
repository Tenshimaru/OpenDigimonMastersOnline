using DigitalWorldOnline.Commons.DTOs.Account;
using MediatR;

namespace DigitalWorldOnline.Application.Separar.Queries;

public record CreateGameAccountQuery(string Username, string Password) : IRequest<AccountDTO>;
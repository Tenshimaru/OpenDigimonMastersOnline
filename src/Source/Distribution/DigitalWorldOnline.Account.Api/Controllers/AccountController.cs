using DigitalWorldOnline.Api.Dtos.Converters;
using DigitalWorldOnline.Api.Dtos.In;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DigitalWorldOnline.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AccountController : BaseController
    {
        private readonly ISender _sender;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;
        public AccountController(
            ISender sender,
            Serilog.ILogger logger,
            IConfiguration configuration)
        {
            _sender = sender;
            _logger = logger;
            _configuration = configuration;
        }

        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountIn account)
        {
            if (GetToken() != _configuration["Authentication:TokenKey"])
            {
                _logger.Information($"Authentication failed for account create request.");
                return Unauthorized();
            }

            try
            {
                var command = CreateAccountCommandConverter.Convert(account);

                var validator = new CreateUserAccountCommandValidator();
                var validationResult = validator.Validate(command);

                if (validationResult.IsValid)
                {
                    var result = await _sender.Send(command);

                    if (result == AccountCreateResult.Created)
                    {
                        _logger.Information($"Account created for username {account.Username}.");
                        return Created("", new { Result = HttpStatusCode.Created });
                    }
                    else
                    {
                        _logger.Information($"Unable to create account for username {account.Username}: {result}.");
                        return Problem(detail: result.GetDescription(), statusCode: result.GetHashCode());
                    }
                }
                else
                {
                    var validationErrors = string.Join(',', validationResult.Errors.Select(x => x.ErrorMessage));

                    _logger.Information($"Invalid data for account create request. {validationErrors}");

                    return Problem(
                        detail: validationErrors,
                        statusCode: AccountCreateResult.InvalidData.GetHashCode()
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error on account create request. Ex.: {ex.Message}. Stack: {ex.StackTrace}");
                return Problem(detail: ex.Message, statusCode: ex.HResult);
            }
        }

    }
}
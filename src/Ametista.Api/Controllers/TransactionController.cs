﻿using Ametista.Api.Model;
using Ametista.Command;
using Ametista.Command.Commands;
using Ametista.Query;
using Ametista.Query.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Ametista.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;

        public TransactionController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
            this.queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        }

        // GET: api/Transaction
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetTransactionsRequest request)
        {
            var query = new GetTransactionListQuery()
            {
                Amount = request.Amount,
                CardHolder = request.CardHolder,
                CardNumber = request.CardNumber,
                ChargeDate = request.ChargeDate,
                Limit = request.Limit,
                Offset = request.Offset
            };

            var result = await queryDispatcher.ExecuteAsync(query);

            if (result.Count() == 0)
            {
                return NotFound(query);
            }

            return Ok(result);
        }

        // GET: api/Transaction/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Transaction
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateTransactionRequest request)
        {
            var command = new CreateTransactionCommand(request.Amount, request.CurrencyCode, request.CardId, request.UniqueId, request.ChargeDate);
            var result = await commandDispatcher.Dispatch(command);

            if (result.Success)
            {
                var response = new CreateTransactionResponse()
                {
                    Amount = result.Amount,
                    CardId = result.CardId,
                    ChargeDate = result.ChargeDate,
                    CurrencyCode = result.CurrencyCode,
                    Id = result.Id,
                    UniqueId = result.UniqueId
                };

                return Ok(response);
            }

            return BadRequest(command);
        }

        // PUT: api/Transaction/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
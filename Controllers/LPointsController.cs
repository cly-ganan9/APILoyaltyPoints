
using LoyaltyPointsAPI.Models;
using LoyaltyPointsAppServices;
using LoyaltyPointsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsAPI.Controllers
{
    [Route("api/LoyaltyPointsSystem")]
    [ApiController]
    public class LPointsController : ControllerBase
    {
        private readonly LPAppServices _appservice;

        public LPointsController(LPAppServices appservice)
        {
            _appservice = appservice;
        }

        [HttpGet("accounts")]
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            var accounts = _appservice.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpGet("accounts/{id}")]
        public ActionResult<Account> GetAccountById(string id)
        {
            var account = _appservice.GetAccountById(id);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet("rewards")]
        public ActionResult<IEnumerable<Reward>> GetRewards()
        {
            var rewards = _appservice.GetRewards();
            return Ok(rewards);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Register request)
        {
            if (request == null)
            {
                return BadRequest("Account data is required.");
            }

            var newAccount = new Account
            {
                AccountId = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Birthdate = request.Birthdate,
                Username = request.Username,
                Password = request.Password,
                LoyaltyPoints = 0
            };

            _appservice.RegisterAccount(newAccount);

            return CreatedAtAction(
                nameof(GetAccountById),
                new { id = newAccount.AccountId },
                newAccount);
        }


        [HttpPost("earn")]
        public IActionResult EarnPoints([FromBody] EarnPoints request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var account = _appservice.GetAccountById(request.AccountId);

            if (account == null)
            {
                return NotFound("Account not found.");
            }

            var flight = _appservice.GetFlightById(request.FlightId);

            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            _appservice.EarnPoints(account,flight,request.ClassOption);

            return Ok(account);
        }

        [HttpPost("redeem")]
        public IActionResult Redeem([FromBody] RedeemReward request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var account = _appservice.GetAccountById(request.AccountId);

            if (account == null)
            {
                return NotFound();
            }

            var reward = _appservice.GetRewards().FirstOrDefault(r => r.RewardId == request.RewardId);

            if (reward == null)
            {
                return NotFound();
            }

            bool success = _appservice.RedeemPoints(account, reward);

            if (!success)
            {
                return BadRequest("Not enough points.");
            }

            return Ok(account);
        }


        [HttpPatch("password/{accountId}")]
        public IActionResult UpdatePassword(string accountId,[FromBody] UpdatePass request)
        {
            if (request == null)
            {
                return BadRequest("Password is required.");
            }

            var existingAccount = _appservice.GetAccountById(accountId);

            if (existingAccount == null)
            {
                return NotFound();
            }

            existingAccount.Password = request.Password;

            _appservice.UpdateAccount(existingAccount);

            return NoContent();
        }

        [HttpDelete("transaction/{accountId}/{transactionId}")]
        public IActionResult DeleteTransaction(string accountId,int transactionId)
        {
            var existingAccount = _appservice.GetAccountById(accountId);

            if (existingAccount == null)
            {
                return NotFound();
            }

            bool success = _appservice.DeleteTransaction(existingAccount,transactionId);

            if (!success)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete("account/{accountId}")]
        public IActionResult DeleteAccount(string accountId,[FromBody] DeleteAccount request)
        {
            var existingAccount = _appservice.GetAccountById(accountId);

            if (existingAccount == null)
            {
                return NotFound();
            }

            bool success = _appservice.DeleteAccount(existingAccount, request.Username,request.Password);

            if (!success)
            {
                return BadRequest("Invalid credentials.");
            }

            return NoContent();
        }

    }
}


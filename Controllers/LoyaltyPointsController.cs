
using LoyaltyPointsAppServices;
using LoyaltyPointsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyPointsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyPointsController : ControllerBase
    {
        private readonly LPAppServices _appservice;

        public LoyaltyPointsController(LPAppServices appservice)
        {
            _appservice = appservice;
        }

        [HttpGet("accounts")]
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            return Ok(_appservice.GetAllAccounts());
        }

        [HttpGet("flights")]
        public ActionResult<IEnumerable<Flight>> GetFlights()
        {
            return Ok(_appservice.GetFlights());
        }

        [HttpGet("rewards")]
        public ActionResult<IEnumerable<Reward>> GetRewards()
        {
            return Ok(_appservice.GetRewards());
        }

      


    }
}


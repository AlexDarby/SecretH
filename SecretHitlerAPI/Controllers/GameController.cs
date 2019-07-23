using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretHitler.Api.Infrastructure.Models;
using SecretHitler.Api.Infrastructure.Services;


namespace SecretHitler.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        public GameController(GameService gameService, UserService userService)
        {
            _gameService = gameService;
            _userService = userService;
        }

        private readonly GameService _gameService;
        private readonly UserService _userService;

        [HttpGet("{name}")]
        public ActionResult<User> Get(string name)
        {
            var user = _userService.Get(name);
            if(user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost]
        public ActionResult PostGame(Game newGame)
        {

            _gameService.AddGame(newGame);
            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;

namespace SecretHitler.Api.Infrastructure.Models
{
    public class Game
    {
        public List<string> Liberals { get; set; }

        public List<string> Fascists { get; set; }

        public string WinningTeam { get; set; }

        public string VictoryType { get; set; }
    }
}

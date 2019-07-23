using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using SecretHitler.Api.Infrastructure.Models;
using System.Linq;

namespace SecretHitler.Api.Infrastructure.Services
{
    public class GameService
    {
        private float LiberalTotalScore = 0;
        private float FascistTotalScore = 0;
        private int LiberalAvgScore = 0;
        private int FascistAvgScore = 0;

        public GameService()
        {
        }

        public void ImportGames()
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("SH Scores.csv");
            while ((line = file.ReadLine()) != null)
            {
                Game game = new Game
                {
                    Liberals = new List<string>(),
                    Fascists = new List<string>()
                };
                while(line != ",,,")
                {
                    List<string> lineList = line.Split(',').ToList<string>();
                    if (lineList[1] == "Win")
                    {
                        game.WinningTeam = "Liberal";
                    }
                    else if (lineList[1] == "Loss")
                    {
                        game.WinningTeam = "Fascist";
                    }
                    game.VictoryType = "LiberalCards";
                    if(lineList[1] == "Liberal")
                    {
                        game.Liberals.Add(lineList[0]);
                    }
                    else if (lineList[1] == "Fascist" || lineList[1] == "Hitler")
                    {
                        game.Fascists.Add(lineList[0]);
                    }
                    line = file.ReadLine();
                }
                AddGame(game);

            }
        }
        public void AddGame(Game game)
        {
            float modifier;
            switch (game.VictoryType)
            {
                case "LiberalCards":
                    modifier = GetWinModifier(WinType.liberalCards);
                    break;
                case "FascistCards":
                    modifier = GetWinModifier(WinType.FascistCards);
                    break;
                case "HitlerElected":
                    modifier = GetWinModifier(WinType.HitlerElected);
                    break;
                default:
                    modifier = GetWinModifier(WinType.HitlerAssassinated);
                    break;
            }

            Users users = JsonConvert.DeserializeObject<Users>(File.ReadAllText("Users.json"));

            foreach (string liberal in game.Liberals)
            {
                LiberalTotalScore += users.UserDict[liberal];
            }
            foreach (string fascist in game.Fascists)
            {
                FascistTotalScore += users.UserDict[fascist];
            }
            LiberalAvgScore = (int)(LiberalTotalScore / game.Liberals.Count);
            FascistAvgScore = (int)(FascistTotalScore / game.Fascists.Count);
            float delta;
            if(game.WinningTeam == "Liberal")
            {
                delta = CalculateELO(LiberalAvgScore, FascistAvgScore, GameOutcome.Win);
                foreach (string liberal in game.Liberals)
                {
                    users.UserDict[liberal] += delta * modifier;
                }
                foreach (string fascist in game.Fascists)
                {
                    users.UserDict[fascist] -= delta * modifier;
                }
            }
            else
            {
                delta = CalculateELO(LiberalAvgScore, FascistAvgScore, GameOutcome.Loss);
                foreach (string liberal in game.Liberals)
                {
                    users.UserDict[liberal] -= delta * modifier;
                }
                foreach (string fascist in game.Fascists)
                {
                    users.UserDict[fascist] += delta * modifier;
                }
            }
            File.WriteAllText("Users.json", JsonConvert.SerializeObject(users));
            
        }

        static double ExpectationToWin(int playerOneRating, int playerTwoRating)
        {
            return 1 / (1 + Math.Pow(10, (playerTwoRating - playerOneRating) / 400.0));
        }

        enum GameOutcome
        {
            Win = 1,
            Loss = 0
        }

        enum WinType
        {
            liberalCards,
            FascistCards,
            HitlerElected,
            HitlerAssassinated

        }

        float GetWinModifier(WinType winType)
        {
            switch (winType)
            {
                case WinType.liberalCards :
                    return 1;
                case WinType.FascistCards :
                    return 1;
                case WinType.HitlerAssassinated :
                    return 1.2F;
                case WinType.HitlerElected :
                    return 1.2F;
                default:
                    return 1;
            }   

        }

        static float CalculateELO(int playerOneRating, int playerTwoRating, GameOutcome outcome)
        {
            int eloK = 32;
            int delta = (int)(eloK * ((int)outcome - ExpectationToWin(playerOneRating, playerTwoRating)));
            return delta;
        }

    }
}

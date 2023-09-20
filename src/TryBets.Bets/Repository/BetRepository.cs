using TryBets.Bets.DTO;
using TryBets.Bets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;
    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        var userFound = _context.Users.FirstOrDefault(user => user.Email == email)!;
        if (userFound == null) throw new Exception("User not founded");

        var matchFound = _context.Matches.FirstOrDefault(match => match.MatchId == betRequest.MatchId)!;
        if (matchFound == null) throw new Exception("Match not founded");

        var teamFound = _context.Teams.FirstOrDefault(team => team.TeamId == betRequest.TeamId)!;
        if (teamFound == null) throw new Exception("Team not founded");

        if (matchFound.MatchFinished) throw new Exception("Match finished");

        if (matchFound.MatchTeamAId != betRequest.TeamId && matchFound.MatchTeamBId != betRequest.TeamId) throw new Exception("Team is not in this match");

        var newBet = new Bet {
            UserId = userFound.UserId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue
        };

        _context.Bets.Add(newBet);
        _context.SaveChanges();

        Bet betCreated = _context.Bets.Include(bet => bet.Team).Include(bet => bet.Match).Where(bet => bet.BetId == newBet.BetId).FirstOrDefault()!;

        return new BetDTOResponse {
            BetId = betCreated.BetId,
            MatchId = betCreated.MatchId,
            TeamId = betCreated.TeamId,
            BetValue = betCreated.BetValue,
            MatchDate = betCreated.Match!.MatchDate,
            TeamName = betCreated.Team!.TeamName,
            Email = betCreated.User!.Email
        };
    }
    public BetDTOResponse Get(int BetId, string email)
    {
        var userFound = _context.Users.FirstOrDefault(user => user.Email == email)!;
        if (userFound == null) throw new Exception("User not founded");

        var betFound = _context.Bets.Include(bet => bet.Team).Include(bet => bet.Match).Where(bet => bet.BetId == BetId).FirstOrDefault()!;
        if (betFound == null) throw new Exception("Bet not founded");

        if (betFound.User!.Email != email) throw new Exception("Bet view not allowed");

        return new BetDTOResponse {
            BetId = betFound.BetId,
            MatchId = betFound.MatchId,
            TeamId = betFound.TeamId,
            BetValue = betFound.BetValue,
            MatchDate = betFound.Match!.MatchDate,
            TeamName = betFound.Team!.TeamName,
            Email = betFound.User!.Email
        };
    }
}
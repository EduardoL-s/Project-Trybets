using TryBets.Odds.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Globalization;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;
    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        var matchFound = _context.Matches.FirstOrDefault(match => match.MatchId == MatchId)!;
        if (matchFound == null) throw new Exception("Match not founded");

        var teamFound = _context.Teams.FirstOrDefault(team => team.TeamId == TeamId)!;
        if (teamFound == null) throw new Exception("Team not founded");

        if (matchFound.MatchTeamAId != TeamId && matchFound.MatchTeamBId != TeamId) throw new Exception("Team is not in this match");

        string repBetValue = BetValue.Replace(",", ".");
        decimal decBetValue = decimal.Parse(repBetValue, CultureInfo.InvariantCulture);

        if (matchFound.MatchTeamAId == TeamId) {
            matchFound.MatchTeamAValue += decBetValue;
        } else {
            matchFound.MatchTeamBValue += decBetValue;
        }

        _context.Matches.Update(matchFound);
        _context.SaveChanges();

        return new Match {
            MatchId = matchFound.MatchId,
            MatchDate = matchFound.MatchDate,
            MatchTeamAId = matchFound.MatchTeamAId,
            MatchTeamBId = matchFound.MatchTeamBId,
            MatchTeamAValue = matchFound.MatchTeamAValue,
            MatchTeamBValue = matchFound.MatchTeamBValue,
            MatchFinished = matchFound.MatchFinished,
            MatchWinnerId = null,
            MatchTeamA = null,
            MatchTeamB = null,
            Bets = null
        };
    }
}
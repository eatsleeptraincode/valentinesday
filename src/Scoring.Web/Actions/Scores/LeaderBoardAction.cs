using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using Raven.Client;
using Raven.Client.Linq;
using Scoring.Web.Indexes;
using Scoring.Web.Models;

namespace Scoring.Web.Actions.Scores
{
    public class LeaderBoardAction
    {
        private readonly IDocumentSession session;

        public LeaderBoardAction(IDocumentSession session)
        {
            this.session = session;
        }

        public LeaderBoardViewModel Get(LeaderBoardRequest request)
        {
            var scores = session
                .Query<Score, ScoreBoardIndex>()
                .Take(1024)
                .As<ScoreDisplay>()
                .ToList()
                .GroupBy(s => s.Athlete.Id);

            var events = session.Query<Event>().OrderBy(e => e.Number).ToList();

            var scoreRows = CompileScoresIntoRows(events, scores).ToList();

            return new LeaderBoardViewModel
                   {
                       Events = events,
                       Scores = scoreRows
                   };
        }

        private static IEnumerable<ScoreRow> CompileScoresIntoRows(IEnumerable<Event> events, IEnumerable<IGrouping<string, ScoreDisplay>> scores)
        {
            return scores.Select(score => new ScoreRow
                                              {
                                                  Athlete = score.First().Athlete,
                                                  Total = score.Sum(s => s.Place),
                                                  Place = events.Select(e => new Tuple<int, int?>(e.Number, score.SingleOrDefault(s => s.Event.Number == e.Number).IfNotNull(s => s.Place)))
                                              });
        }
    }

    public class LeaderBoardRequest
    {
    }

    public class LeaderBoardViewModel
    {
        public IEnumerable<ScoreRow> Scores { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }

    public class ScoreRow
    {
        public Athlete Athlete { get; set; }
        public IEnumerable<Tuple<int, int?>> Place { get; set; }
        public int? Total { get; set; }
    }
}
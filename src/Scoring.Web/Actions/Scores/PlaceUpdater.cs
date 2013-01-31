using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;
using Scoring.Web.Models;

namespace Scoring.Web.Actions.Scores
{
    public class PlaceUpdater
    {
        private readonly IDocumentSession session;

        public PlaceUpdater(IDocumentSession session)
        {
            this.session = session;
        }

        public void Update(Event theEvent, Gender gender)
        {
            var orderedScores = GetOrderedScores(theEvent);

            if (theEvent.Reverse)
                orderedScores.Reverse();

            var i = 1;
            Score previousScore = null;
            foreach (var orderedScore in orderedScores)
            {
                if (previousScore != null &&
                    orderedScore.Time.Minutes == previousScore.Time.Minutes
                    && orderedScore.Time.Seconds == previousScore.Time.Seconds
                    && orderedScore.Reps == previousScore.Reps)
                    orderedScore.Place = previousScore.Place;
                else
                    orderedScore.Place = i;

                session.Store((Score) orderedScore);
                previousScore = orderedScore;
                i++;
            }
        }

        private List<Score> GetOrderedScores(Event theEvent)
        {
            var scores = session
                .Query<Score>()
                .Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
                .Where(s => s.EventId == theEvent.Id)
                .ToList();

            var orderedScores = scores
                .OrderBy(s => s.Time.Minutes)
                .ThenBy(s => s.Time.Seconds)
                .ThenByDescending(s => s.Reps)
                .ToList();
            return orderedScores;
        }
    }
}
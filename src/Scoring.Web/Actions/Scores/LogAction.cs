using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using Raven.Client;
using Raven.Client.Linq;
using Scoring.Web.Actions.Events;
using Scoring.Web.Models;

namespace Scoring.Web.Actions.Scores
{
    public class LogAction
    {
        private readonly IDocumentSession session;

        public LogAction(IDocumentSession session)
        {
            this.session = session;
        }

        public LogScoreViewModel Get(LogScoreRequest request)
        {
            var athlete = session.Load<Athlete>(request.AthleteId);
            var theEvent = session.Load<Event>(request.EventId);
            return new LogScoreViewModel {Athlete = athlete, Event = theEvent, Score = new Score()};
        }

        public FubuContinuation Post(LogScoreViewModel request)
        {
            var score = session
                            .Query<Score>()
                            .SingleOrDefault(s => s.AthleteId == request.Athlete.Id
                                                  && s.EventId == request.Event.Id);

            var theEvent = session.Load<Event>(request.Event.Id);

            if (score == null)
                score = CreateNewScore(request, theEvent);

            score.Reps = request.Score.Reps;
            score.Time = request.Score.Time;
            score.Place = request.Score.Place;

            session.Store(score);
            session.SaveChanges();

            UpdatePlaces(theEvent);

            return FubuContinuation.RedirectTo(new EventDetailsRequest{EventId = request.Event.Id});
        }

        private Score CreateNewScore(LogScoreViewModel request, Event theEvent)
        {
            var athlete = session.Load<Athlete>(request.Athlete.Id);
            return new Score
                        {
                            AthleteId = athlete.Id,
                            EventId = theEvent.Id,
                            ScoreType = theEvent.ScoreType,
                            AthleteName = athlete.Name
                        };
        }

        private void UpdatePlaces(Event theEvent)
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
                                .ThenBy(s => s.ScoredPlace)
                                .ToList();

            if (theEvent.Reverse)
            {
                orderedScores.Reverse();
            }

            var i = 1;
            Score previousScore = null;
            foreach (var orderedScore in orderedScores)
            {
                if (previousScore != null &&
                    orderedScore.Time.Minutes == previousScore.Time.Minutes
                    && orderedScore.Time.Seconds == previousScore.Time.Seconds
                    && orderedScore.Reps == previousScore.Reps
                    && orderedScore.ScoredPlace == previousScore.ScoredPlace)
                    orderedScore.Place = previousScore.Place;
                else
                    orderedScore.Place = i;

                session.Store((Score) orderedScore);
                previousScore = orderedScore;
                i++;
            }
        }
    }

    public class LogScoreRequest
    {
        [RouteInput]
        public string AthleteId { get; set; }

        [RouteInput]
        public string EventId { get; set; }
    }

    public class LogScoreViewModel
    {
        public Score Score { get; set; }
        public Athlete Athlete { get; set; }
        public Event Event { get; set; }
    }
}
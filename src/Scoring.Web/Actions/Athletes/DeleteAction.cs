using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using Raven.Client;
using Scoring.Web.Models;

namespace Scoring.Web.Actions.Athletes
{
    public class DeleteAction
    {
        private readonly IDocumentSession session;

        public DeleteAction(IDocumentSession session)
        {
            this.session = session;
        }

        public FubuContinuation Get(DeleteAthleteRequest request)
        {
            var athlete = session.Load<Athlete>(request.AthleteId);
            session.Delete(athlete);
            return FubuContinuation.RedirectTo(new AthleteListRequest());
        }

    }

    public class DeleteAthleteRequest
    {
        [RouteInput]
        public string AthleteId { get; set; }
    }
}
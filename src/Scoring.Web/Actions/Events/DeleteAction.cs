using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using Raven.Client;
using Scoring.Web.Models;

namespace Scoring.Web.Actions.Events
{
    public class DeleteAction
    {
        private readonly IDocumentSession session;

        public DeleteAction(IDocumentSession session)
        {
            this.session = session;
        }

        public FubuContinuation Get(DeleteEventRequest request)
        {
            var @event = session.Load<Event>(request.EventId);
            session.Delete(@event);
            return FubuContinuation.RedirectTo(new EventListRequest());
        }
    }

    public class DeleteEventRequest
    {
        [RouteInput]
        public string EventId { get; set; }
    }
}
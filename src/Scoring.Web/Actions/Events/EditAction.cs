using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using Raven.Client;
using Scoring.Web.Models;

namespace Scoring.Web.Actions.Events
{
    public class EditAction
    {
        private readonly IDocumentSession session;

        public EditAction(IDocumentSession session)
        {
            this.session = session;
        }

        public EditEventViewModel Get(EditEventRequest request)
        {
            var @event = session.Load<Event>(request.EventId);
            return new EditEventViewModel{Event = @event};
        }

        public FubuContinuation Post(EditEventViewModel request)
        {
            session.Store(request.Event);
            return FubuContinuation.RedirectTo(new EventListRequest());
        }
    }

    public class EditEventRequest
    {
        [RouteInput]
        public string EventId { get; set; }
    }

    public class EditEventViewModel
    {
        public Event Event { get; set; }
    }
}
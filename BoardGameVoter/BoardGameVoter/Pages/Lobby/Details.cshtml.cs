using BoardGameVoter.Logic.VoteSessions;
using BoardGameVoter.Models.EntityModels;
using BoardGameVoter.Models.EntityModels.VoteSessions;
using BoardGameVoter.Pages.Shared;
using BoardGameVoter.Repositorys.Locations;
using BoardGameVoter.Repositorys.VoteSessions;
using BoardGameVoter.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameVoter.Pages.Lobby
{
    public class DetailsModel : BoardGameVoterPageBase
    {
        private const string CLOSE_VOTE_ACTION = "CLOSEVOTE";
        private const string DELETE_ATTENDEE_ACTION = "DELETEATTENDEE";
        private const string DELETE_SESSION_ACTION = "DELETESESSION";
        private const string REDIRECT_ACTION = "REDIRECT";

        private readonly LocationRepository __LocationRepository;
        private readonly VoteSessionAttendeeRepository __VoteSessionAttendeeRepository;
        private readonly VoteSessionManager __VoteSessionManager;
        private readonly VoteSessionRepository __VoteSessionRepository;

        public DetailsModel(IBGVServiceProvider bGVServiceProvider)
            : base(bGVServiceProvider)
        {
            __VoteSessionRepository = new VoteSessionRepository(bGVServiceProvider);
            __VoteSessionAttendeeRepository = new VoteSessionAttendeeRepository(bGVServiceProvider);
            __LocationRepository = new LocationRepository(bGVServiceProvider);
            __VoteSessionManager = new VoteSessionManager(bGVServiceProvider);
        }

        private void HandleAction()
        {
            switch (Action)
            {
                case CLOSE_VOTE_ACTION:
                    __VoteSessionManager.CloseVote(VoteSession);
                    break;
                case DELETE_ATTENDEE_ACTION:
                    __VoteSessionManager.RemoveAttendee(UserManager.User.ID, VoteSession);
                    Action = REDIRECT_ACTION;
                    break;
                case DELETE_SESSION_ACTION:
                    __VoteSessionManager.DeleteSession(VoteSession);
                    Action = REDIRECT_ACTION;
                    break;
            }
        }

        public void OnGet()
        {
            if (VoteSessionUID == Guid.Empty)
            {
                throw new ArgumentException("Must Pass Valid UID", nameof(VoteSessionUID));
            }
            PopulateDetails();
        }

        public void OnPost()
        {
            PopulateDetails();
            HandleAction();
        }

        private void PopulateDetails()
        {
            VoteSession = __VoteSessionRepository.GetByUID(VoteSessionUID);
            if (VoteSession != null)
            {
                Attendees = __VoteSessionAttendeeRepository.GetByVoteSessionID(VoteSession.ID, true);
                if (!Attendees.Any(attendee => attendee.UserID == UserManager.User.ID))
                {
                    MyDetails = __VoteSessionManager.AddNewAttendee(UserManager.User.ID, VoteSession.ID);
                    Attendees.Add(MyDetails);
                    MyDetails.User = UserManager.User;
                }
                if (MyDetails == null)
                {
                    MyDetails = Attendees.FirstOrDefault(attendee => attendee.UserID == UserManager.User.ID);
                }
                Location = __LocationRepository.GetByID(VoteSession.LocationID);
            }
            else
            {
                Response.Redirect("/Lobby");
            }

        }

        [BindProperty]
        public string Action { get; set; }

        public List<VoteSessionAttendee> Attendees { get; set; }
        public Location Location { get; set; }
        public VoteSessionAttendee MyDetails { get; set; }
        public VoteSession VoteSession { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid VoteSessionUID { get; set; }
    }
}

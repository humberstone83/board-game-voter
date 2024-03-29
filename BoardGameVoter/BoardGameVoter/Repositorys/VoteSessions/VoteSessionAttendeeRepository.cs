﻿using BoardGameVoter.Models.EntityModels;
using BoardGameVoter.Models.EntityModels.Users;
using BoardGameVoter.Models.EntityModels.VoteSessions;
using BoardGameVoter.Repositorys.Library;
using BoardGameVoter.Repositorys.Shared;
using BoardGameVoter.Repositorys.Users;
using BoardGameVoter.Services;

namespace BoardGameVoter.Repositorys.VoteSessions
{
    public class VoteSessionAttendeeRepository : RepositoryBase<VoteSessionAttendee, VoteSession, VoteSessionAttendeeLoadOptions>, IVoteSessionAttendeeRepository
    {
        private readonly LibraryGameRepository __LibraryGameRepository;
        private readonly UserRepository __UserRepository;

        public VoteSessionAttendeeRepository(IBGVServiceProvider bGVServiceProvider)
            : this(bGVServiceProvider, new())
        {
        }

        public VoteSessionAttendeeRepository(IBGVServiceProvider bGVServiceProvider, VoteSessionAttendeeLoadOptions loadOptions)
            : base(bGVServiceProvider, loadOptions)
        {
            __UserRepository = new UserRepository(bGVServiceProvider);
            __LibraryGameRepository = new LibraryGameRepository(bGVServiceProvider);
        }

        public List<VoteSessionAttendee> GetByUserID(int userID)
        {
            return Data.Where(attendee => attendee.UserID == userID).ToList();
        }

        public List<VoteSessionAttendee> GetByVoteSessionID(int voteSessionID, bool IncludeUser = false, bool IncludeGames = false)
        {
            List<VoteSessionAttendee> _Attendees = Data.Where(attendee => attendee.VoteSessionID == voteSessionID).ToList();

            if (IncludeUser && __UserRepository != null)
            {
                _Attendees = LoadWithUsers(_Attendees);
            }

            if (IncludeGames && __LibraryGameRepository != null)
            {
                _Attendees = LoadWithLibraryGames(_Attendees, true);
            }

            return _Attendees;
        }

        public bool IsExistingAttendee(int voteSessionID, int userID)
        {
            return GetByVoteSessionID(voteSessionID).Any(attendee => attendee.UserID == userID);
        }

        private List<VoteSessionAttendee> LoadWithLibraryGames(List<VoteSessionAttendee> attendees, bool IncludeBoardGames = false)
        {
            List<LibraryGame> _LibraryGames = __LibraryGameRepository.GetByUserID(attendees.Select(attendee => attendee.UserID), IncludeBoardGames);
            foreach (VoteSessionAttendee _Attendee in attendees)
            {
                _Attendee.LibraryGames = _LibraryGames.Where(game => game.UserID == _Attendee.UserID).ToList();
            }
            return attendees;
        }

        private List<VoteSessionAttendee> LoadWithUsers(List<VoteSessionAttendee> attendees)
        {
            List<User> _Users = __UserRepository.GetByID(attendees.Select(attendee => attendee.UserID)).ToList();
            foreach (VoteSessionAttendee _Attendee in attendees)
            {
                _Attendee.User = _Users.FirstOrDefault(user => user.ID == _Attendee.UserID);
            }
            return attendees;
        }
    }
}

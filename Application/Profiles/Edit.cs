using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DisplayName)
                    .NotEmpty()
                    .MinimumLength(3);
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAcessor;
            public Handler(DataContext context, IUserAccessor userAcessor)
            {
                _userAcessor = userAcessor;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUser = _userAcessor.GetCurrentUsername();
                var user = await _context.Users.SingleOrDefaultAsync(
                    u => u.UserName == currentUser
                );

                user.DisplayName = request.DisplayName ?? user.DisplayName;
                user.Bio = request.Bio ?? user.Bio;

                /// SaveChangesAsync returns Task<int>, and if the number is 
                /// greater than 0, it means we successfully added an Activity
                var success = await _context.SaveChangesAsync() > 0;

                // If it was a success, return the Value
                if (success) return Unit.Value;

                // If there was an error, throw an error
                throw new Exception("Problem updating profile");
            }
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id);

                if (activity == null)
                    throw new Exception("Could not find activity");

                _context.Remove(activity);

                /// SaveChangesAsync returns Task<int>, and if the number is 
                /// greater than 0, it means we successfully added an Activity
                var success = await _context.SaveChangesAsync() > 0;

                // If it was a success, return the Value
                if (success) return Unit.Value;

                // If there was an error, throw an error
                throw new Exception("Problem saving changes");
            }
        }
    }
}

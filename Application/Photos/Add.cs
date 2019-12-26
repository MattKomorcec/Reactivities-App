using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Photo>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Photo>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context,
                IUserAccessor userAccessor,
                IPhotoAccessor photoAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
                _photoAccessor = photoAccessor;
            }

            public async Task<Photo> Handle(Command request, CancellationToken cancellationToken)
            {
                // Try to upload the photo
                var photoUploadResult = _photoAccessor.AddPhoto(request.File);

                // Get the current user
                var user = await _context.Users.SingleOrDefaultAsync(x =>
                    x.UserName == _userAccessor.GetCurrentUsername()
                );

                // If the upload succeeded, create a new instance of Photo
                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId
                };

                // If the user doesn't have a main photo yet, set this one as the  main one
                if (!user.Photos.Any(x => x.IsMain))
                {
                    photo.IsMain = true;
                }
                // Add the photo to the collection of user's photos
                user.Photos.Add(photo);

                var success = await _context.SaveChangesAsync() > 0;

                // If it was a success, return the Value
                if (success) return photo;

                // If there was an error, throw an error
                throw new Exception("Problem saving changes");
            }
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.CP.UnitInSectionImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Core.Entities;

namespace YemenBooking.Application.Handlers.Commands.UnitInSectionImages
{
    public class ReorderUnitInSectionImagesCommandHandler : IRequestHandler<ReorderUnitInSectionImagesCommand, ResultDto<bool>>
    {
        private readonly IUnitInSectionImageRepository _repo;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;
        public ReorderUnitInSectionImagesCommandHandler(IUnitInSectionImageRepository repo, IAuditService auditService, ICurrentUserService currentUserService)
        {
            _repo = repo;
            _auditService = auditService;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<bool>> Handle(ReorderUnitInSectionImagesCommand request, CancellationToken cancellationToken)
        {
            var tuples = request.Assignments.ConvertAll(a => (a.ImageId, a.DisplayOrder));
            var ok = await _repo.UpdateDisplayOrdersAsync(tuples, cancellationToken);
            if (ok)
            {
                await _auditService.LogAuditAsync(
                    entityType: nameof(UnitInSectionImage),
                    entityId: request.UnitInSectionId ?? System.Guid.Empty,
                    action: YemenBooking.Core.Entities.AuditAction.UPDATE,
                    oldValues: null,
                    newValues: System.Text.Json.JsonSerializer.Serialize(new { Reordered = true, Count = tuples.Count }),
                    performedBy: _currentUserService.UserId,
                    notes: $"تم إعادة ترتيب صور عنصر وحدة في القسم بواسطة {_currentUserService.Username} (ID={_currentUserService.UserId})",
                    cancellationToken: cancellationToken);
            }
            return ok ? ResultDto<bool>.Ok(true) : ResultDto<bool>.Failed("فشل إعادة الترتيب");
        }
    }
}


using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;

namespace Application.Students.Command;

public sealed record UpdateStudentGradeCommand : IRequest<Result>
{
    public Guid StudentId { get; init; }
    public double NewGrade { get; init; }
}

public class UpdateStudentGradeCommandHandler : IRequestHandler<UpdateStudentGradeCommand, Result>
{
    private readonly ApplicationContext _context;

    public UpdateStudentGradeCommandHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateStudentGradeCommand request, CancellationToken token)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, token);

        if (student == null)
            return Result.Failure("Student not found");

        var updateResult = student.UpdateGrade(request.NewGrade);
        if (updateResult.IsFailure)
            return updateResult;

        await _context.SaveChangesAsync(token);
        return Result.Success();
    }
}

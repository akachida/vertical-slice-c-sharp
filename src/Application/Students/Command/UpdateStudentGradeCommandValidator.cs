using FluentValidation;

namespace Application.Students.Command;

public class UpdateStudentGradeCommandValidator : AbstractValidator<UpdateStudentGradeCommand>
{
    public UpdateStudentGradeCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required");

        RuleFor(x => x.NewGrade)
            .InclusiveBetween(0, 100)
            .WithMessage("Grade must be between 0 and 100");
    }
}

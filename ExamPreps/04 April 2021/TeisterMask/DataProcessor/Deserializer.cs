// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Globalization;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using Microsoft.EntityFrameworkCore;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var root = new XmlRootAttribute("Projects");

            var serializer = new XmlSerializer(typeof(List<ImportProjectDto>), root);

            using var reader = new StringReader(xmlString);

            var deserializedData = (List<ImportProjectDto>)serializer.Deserialize(reader)!;

            var validEntities = new List<Project>();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);
                var isOpenDateValid = DateTime.TryParseExact(dto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var openDate);
                var dueDateData = GetDueDate(dto.DueDate);
                var isDueDateValid = dueDateData.isValid;

                if (!isValid || !isOpenDateValid || !isDueDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Project()
                {
                    Name = dto.Name,
                    OpenDate = openDate,
                    DueDate = dueDateData.dueDate,
                    Tasks = new List<Task>()
                };

                foreach (var childDto in dto.Tasks)
                {
                    var isChilDtoValid = IsValid(childDto);
                    var isChilDtoOpenDateValid = DateTime.TryParseExact(childDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var childDtoOpenDate);
                    var isChilDtoDueDateValid = DateTime.TryParseExact(childDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var childDtoDueDate);

                    isChilDtoOpenDateValid = childDtoOpenDate < entity.OpenDate ? false : isChilDtoOpenDateValid;
                    isChilDtoDueDateValid = childDtoDueDate > entity.DueDate ? false : isChilDtoDueDateValid;

                    if (!isChilDtoValid || !isChilDtoOpenDateValid || !isChilDtoDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var childEntity = new Task()
                    {
                        Name = childDto.Name,
                        OpenDate = childDtoOpenDate,
                        DueDate = childDtoDueDate,
                        ExecutionType = (ExecutionType)childDto.ExecutionType,
                        LabelType = (LabelType)childDto.LabelType
                    };

                    entity.Tasks.Add(childEntity);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedProject, entity.Name, entity.Tasks.Count));
            }

            context.Projects.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static (bool isValid, DateTime? dueDate) GetDueDate(string? dueDate)
        {
            if (string.IsNullOrEmpty(dueDate)) return (true, null);
            var isValid = DateTime.TryParseExact(dueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
            return (isValid, date);
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedData = JsonConvert.DeserializeObject<List<ImportEmployeeDto>>(jsonString)!;

            var validEntities = new List<Employee>();

            var taskIds = context.Tasks
                .AsNoTracking()
                .Select(t => t.Id)
                .ToList();

            foreach (var dto in deserializedData)
            {
                var isValid = IsValid(dto);

                if (!isValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var entity = new Employee()
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    EmployeesTasks = new List<EmployeeTask>()
                };

                dto.Tasks = dto.Tasks.Distinct().ToList();

                foreach (var id in dto.Tasks)
                {
                    var isChildValid = taskIds.Contains(id);

                    if (!isChildValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var child = new EmployeeTask()
                    {
                        Employee = entity,
                        TaskId = id
                    };

                    entity.EmployeesTasks.Add(child);
                }

                validEntities.Add(entity);

                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, entity.Username, entity.EmployeesTasks.Count));
            }
            context.Employees.AddRange(validEntities);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
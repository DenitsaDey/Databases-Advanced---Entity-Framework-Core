namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var projectsToImport = new List<Project>();
            var projects = XmlConverter.Deserializer<ProjectXmlInputModel>(xmlString, "Projects");

            foreach (var currentProject in projects)
            {
                var isValidCurrentProjectOpenDate = DateTime.TryParseExact(
                       currentProject.OpenDate,
                       "dd/MM/yyyy",
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None,
                       out DateTime currentProjectOpenDate);

                if (!IsValid(currentProject)
                    || !isValidCurrentProjectOpenDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidDueDate = DateTime.TryParseExact(
                    currentProject.DueDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime currentDueDate);

                var projectToAdd = new Project
                {
                    Name = currentProject.Name,
                    OpenDate = currentProjectOpenDate,
                    DueDate = isValidDueDate ? (DateTime?)currentDueDate : null,
                };

                foreach (var currentTask in currentProject.Tasks)
                {
                    var isValidTaskOpenDate = DateTime.TryParseExact(
                        currentTask.OpenDate,
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime currentTaskOpenDate);

                    var isValidTaskDueDate = DateTime.TryParseExact(
                       currentTask.DueDate,
                       "dd/MM/yyyy",
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None,
                       out DateTime currentTaskDueDate);

                    if (!IsValid(currentTask) ||
                        !isValidTaskOpenDate ||
                        !isValidTaskDueDate ||
                        currentTaskOpenDate < currentProjectOpenDate ||
                        currentTaskDueDate > projectToAdd.DueDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var taskToAdd = new Task
                    {
                        Name = currentTask.Name,
                        OpenDate = currentTaskOpenDate,
                        DueDate = currentTaskDueDate,
                        ExecutionType = Enum.Parse<ExecutionType>(currentTask.ExecutionType),
                        LabelType = Enum.Parse<LabelType>(currentTask.LabelType)
                    };

                    projectToAdd.Tasks.Add(taskToAdd);
                }

                projectsToImport.Add(projectToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedProject, projectToAdd.Name, projectToAdd.Tasks.Count));
            }

            context.Projects.AddRange(projectsToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var employees = JsonConvert.DeserializeObject<EmployeeJsonInputModel[]>(jsonString);

            foreach (var currentEmployee in employees)
            {
                if (!IsValid(currentEmployee))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employeeToImport = new Employee
                {
                    Username = currentEmployee.Username,
                    Email = currentEmployee.Email,
                    Phone = currentEmployee.Phone
                };

                foreach (var task in currentEmployee.Tasks.Distinct())
                {
                    var taskToAdd = context.Tasks.FirstOrDefault(t => t.Id == task);

                    if (taskToAdd == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employeeToImport.EmployeesTasks.Add(new EmployeeTask
                    {
                        Employee = employeeToImport,
                        Task = taskToAdd
                    });
                }

                context.Employees.Add(employeeToImport);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employeeToImport.Username, employeeToImport.EmployeesTasks.Count));
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
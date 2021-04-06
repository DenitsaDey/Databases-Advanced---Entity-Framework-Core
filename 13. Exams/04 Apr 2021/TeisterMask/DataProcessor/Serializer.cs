namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects
                .ToArray()
                .Where(p => p.Tasks.Count > 0)
                .OrderByDescending(p => p.Tasks.Count)
                .ThenBy(p => p.Name)
                .Select(p => new ProjectXmlExportModel
                {
                    TasksCount = p.Tasks.Count.ToString(),
                    Name = p.Name,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks.Select(t => new TaskXmlExportModel
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(t => t.Name)
                    .ToArray()
                })
                .ToArray();

            var xml = XmlConverter.Serialize(projects, "Projects");
            return xml;
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(e=>e.EmployeesTasks.Any(et=>et.Task.OpenDate >= date))
                .ToArray()
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                            .ToArray()
                            .Where(et => et.Task.OpenDate >= date)
                            .OrderByDescending(et => et.Task.DueDate)
                            .ThenBy(et => et.Task.Name)
                            .Select(et => new
                            {
                                TaskName = et.Task.Name,
                                OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                                DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                                LabelType = et.Task.LabelType.ToString(),
                                ExecutionType = et.Task.ExecutionType.ToString()
                            })
                            .ToArray()
                })
                .OrderByDescending(e => e.Tasks.Length)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToArray();

            string json = JsonConvert.SerializeObject(employees, Formatting.Indented);
            return json;
        }
    }
}
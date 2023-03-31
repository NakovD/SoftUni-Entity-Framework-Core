using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ExportProjecDto
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        [XmlElement("ProjectName")]
        public string ProjectName { get; set; } = null!;

        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; } = null!;

        [XmlArray("Tasks")]
        public List<ExportTaskDto> Tasks { get; set; } = new List<ExportTaskDto>();
    }
}

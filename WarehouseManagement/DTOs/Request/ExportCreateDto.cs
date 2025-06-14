﻿using System.Text.Json.Serialization;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Request
{
    public class ExportCreateDto
    {
        [JsonIgnore]
        public int? EmployId { get; set; }
        public double Quantity { get; set; } = 0;
        public double TotalPrice { get; set; } = 0;
        public string? ConsumerName { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
        public ExportEnum? Status { get; set; }
    }

}

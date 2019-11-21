using System.Collections.Generic;
using Guid = System.Guid;
using DateTime = System.DateTime;
using Models = Icon.Models;
using System.Linq;
using Events = Icon.Events;

namespace Icon.Aggregates
{
    public sealed class ComponentInformationAggregateData
      : Validatable
    {
        public static ComponentInformationAggregateData From(
            Events.ComponentInformationEventData information
            )
        {
            return new ComponentInformationAggregateData(
                name: information.Name.NotNull(),
                abbreviation: information.Abbreviation,
                description: information.Description.NotNull(),
                availableFrom: information.AvailableFrom,
                availableUntil: information.AvailableUntil,
                categories:
                information.Categories.NotNull()
                .Select(Events.ComponentCategoryEventDataExtensions.ToModel)
                .ToList()
                );
        }

        public string Name { get; set; }
        public string? Abbreviation { get; set; }
        public string Description { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableUntil { get; set; }
        public ICollection<Models.ComponentCategory> Categories { get; set; }

        #nullable disable
        public ComponentInformationAggregateData() { }
        #nullable enable

        public ComponentInformationAggregateData(
            string name,
            string? abbreviation,
            string description,
            DateTime? availableFrom,
            DateTime? availableUntil,
            ICollection<Models.ComponentCategory> categories
            )
        {
            Name = name;
            Abbreviation = abbreviation;
            Description = description;
            AvailableFrom = availableFrom;
            AvailableUntil = availableUntil;
            Categories = categories;
        }

        public override bool IsValid()
        {
            return
              !(Name is null) &&
              !(Description is null) &&
              !(Categories is null);
        }

        public Models.ComponentInformation ToModel()
        {
            return new Models.ComponentInformation(
                  name: Name,
                  abbreviation: Abbreviation,
                  description: Description,
                  availableFrom: AvailableFrom,
                  availableUntil: AvailableUntil,
                  categories: Categories
                );
        }
    }
}
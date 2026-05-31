using Friday.BuildingBlocks.Application;
using Friday.Modules.Salon.Application;
using LinKit.Core.Cqrs;

namespace Friday.MCHair.Web.Cqrs;

[CqrsContext(typeof(SalonApplicationAssemblyMarker), typeof(BuildingBlockApplicationMarker))]
public sealed class MChairCqrsContext;

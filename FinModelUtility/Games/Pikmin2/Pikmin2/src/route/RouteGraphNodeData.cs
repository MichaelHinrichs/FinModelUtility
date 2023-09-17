using fin.data.indexable;
using fin.model;

namespace games.pikmin2.route {
  public interface IRouteGraphNodeData : IIndexable {
    IVector3 Position { get; }
    float Radius { get; }
  }

  public class RouteGraphNodeData : IRouteGraphNodeData {
    public required int Index { get; init; }
    public required IVector3 Position { get; init; }
    public required float Radius { get; init; }
  }
}
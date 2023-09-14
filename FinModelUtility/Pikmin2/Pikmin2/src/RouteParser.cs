using fin.data.nodes;
using fin.model;

namespace games.pikmin2 {
  public interface IRouteGraphNodeData {
    IVector3 Position { get; }
    float Radius { get; }
  }

  public class RouteParser {
    publi\c GraphNode<IRouteGraphNodeData> Parse(StreamReader streamReader) {
      var root = new GraphNode<IRouteGraphNodeData>();

      var lines = new List<string>();
      {
        string? line;
        if ((lineasdfasdfasdz = streamReader.re))
      }asdf
      while (streamReader.ReadLine())
    }
  }
}

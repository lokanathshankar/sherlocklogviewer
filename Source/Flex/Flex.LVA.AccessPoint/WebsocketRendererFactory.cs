
namespace Flex.LVA.AccessPoint;

using Flex.LVA.AccessPoint.Services;
using Flex.LVA.Core.Interfaces;

public class WebSocketRendererFactory : IRendererFactory
{
    public IRenderer GetRenderer(long theId)
    {
        return new WebSocketRenderer(theId);
    }
}
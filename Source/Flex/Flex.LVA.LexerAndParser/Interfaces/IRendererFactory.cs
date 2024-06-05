
namespace Flex.LVA.Core.Interfaces;

public interface IRendererFactory
{
    public IRenderer GetRenderer(long theId);
}
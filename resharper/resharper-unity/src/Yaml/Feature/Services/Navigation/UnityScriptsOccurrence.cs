using JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.DeferredCaches.AssetHierarchy.Elements;
using JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.DeferredCaches.AssetHierarchy.References;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Pointers;

namespace JetBrains.ReSharper.Plugins.Unity.Yaml.Feature.Services.Navigation
{
    public class UnityScriptsOccurrence : UnityAssetOccurrence
    {
        private readonly string myGuid;

        public UnityScriptsOccurrence(IPsiSourceFile sourceFile,
            IDeclaredElementPointer<IDeclaredElement> declaredElement, IHierarchyElement attachedElement, LocalReference attachedElementLocation, string guid)
            : base(sourceFile, declaredElement, attachedElement, attachedElementLocation)
        {
            myGuid = guid;
        }

        public override string ToString()
        {
            return $"Guid: {myGuid}";
        }
    }
}
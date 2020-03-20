using JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.DeferredCaches.AssetHierarchy.Elements;
using JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.DeferredCaches.AssetHierarchy.References;
using JetBrains.ReSharper.Plugins.Unity.Yaml.Psi.DeferredCaches.AssetInspectorValues;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Pointers;
using JetBrains.ReSharper.Resources.Shell;

namespace JetBrains.ReSharper.Plugins.Unity.Yaml.Feature.Services.Navigation
{
    public class UnityInspectorValuesOccurrence: UnityAssetOccurrence
    {
        public InspectorVariableUsage InspectorVariableUsage { get; }

        public UnityInspectorValuesOccurrence(IPsiSourceFile sourceFile, InspectorVariableUsage inspectorVariableUsage,
            IDeclaredElementPointer<IDeclaredElement> declaredElement, IHierarchyElement attachedElement, LocalReference attachedElementLocation)
            : base(sourceFile, declaredElement, attachedElement, attachedElementLocation)
        {
            InspectorVariableUsage = inspectorVariableUsage;
        }


        public override string ToString()
        {
            using (ReadLockCookie.Create())
            {
                using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
                {
                    var de = DeclaredElementPointer.FindDeclaredElement();
                    if (de == null)
                        return "INVALID";
                    var value = InspectorVariableUsage.Value.GetPresentation(GetSolution(), de, true);
                    return $"{de.ShortName} = {value}";
                }
            }
        }
    }
}
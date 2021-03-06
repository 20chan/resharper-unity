using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Impl;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Plugins.Unity.JsonNew.Feature.CodeCompletion.Settings;
using JetBrains.ReSharper.Plugins.Unity.JsonNew.Psi.Tree;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Unity.JsonNew.Feature.CodeCompletion
{
    [IntellisensePart]
    public class ShaderLabCodeCompletionContextProvider : CodeCompletionContextProviderBase
    {
        private readonly JsonNewIntellisenseManager myIntellisenseManager;

        public ShaderLabCodeCompletionContextProvider(JsonNewIntellisenseManager manager)
        {
            myIntellisenseManager = manager;
        }

        public override bool IsApplicable(CodeCompletionContext context) => context.File is IJsonNewFile;

        public override ISpecificCodeCompletionContext GetCompletionContext(CodeCompletionContext context)
        {
            if (!(context.File is IJsonNewFile file)) return null;

            if (context.CodeCompletionType == CodeCompletionType.BasicCompletion
                && !IsIntellisenseEnabled(context))
            {
                return null;
            }

            var unterminatedContext = new JsonNewReparsedCompletionContext(file, context.SelectedTreeRange, "__");
            unterminatedContext.Init();

            var referenceToComplete = unterminatedContext.Reference;
            var elementToComplete = unterminatedContext.TreeNode;

            if (elementToComplete == null)
                return null;

            var referenceRange = referenceToComplete?.GetTreeTextRange() ?? GetElementRange(elementToComplete);
            var referenceDocumentRange = unterminatedContext.ToDocumentRange(referenceRange);

            if (!referenceDocumentRange.IsValid())
                return null;

            if (!referenceDocumentRange.Contains(context.EffectiveCaretDocumentOffset))
                return null;

            var ranges = GetTextLookupRanges(context, referenceDocumentRange);
            return new JsonNewCodeCompletionContext(context, ranges, unterminatedContext);
            
        }

        private bool IsIntellisenseEnabled(CodeCompletionContext context)
        {
            return myIntellisenseManager.GetIntellisenseEnabled(context.ContextBoundSettingsStore);
        }

        private TreeTextRange GetElementRange(ITreeNode element)
        {
            if (element is ITokenNode tokenNode)
            {
                var tokenNodeType = tokenNode.GetTokenType();
                if (tokenNodeType.IsIdentifier || tokenNodeType.IsKeyword)
                    return tokenNode.GetTreeTextRange();
            }
            return new TreeTextRange(element.GetTreeTextRange().EndOffset);
        }
    }
}
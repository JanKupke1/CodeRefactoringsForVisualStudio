using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MediatRAddRequestHandlerAndRequest
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MediatRAddRequestHandlerAndRequestCodeRefactoringProvider)), Shared]
    internal partial class MediatRAddRequestHandlerAndRequestCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var methodDeclarations = root.ExtractSelectedNodesOfType<MethodDeclarationSyntax>(context.Span, true).ToArray();
            var classDeclarations = root.ExtractSelectedNodesOfType<ClassDeclarationSyntax>(context.Span, true).ToArray();

            IEnumerable<MemberDeclarationSyntax> targets = methodDeclarations;
            if (!targets.Any())
            {
                targets = classDeclarations.Where(x => x.BaseList != null)
                                           .Where(x => x.BaseList.Types.Any(t => t.ToString().StartsWith("IRequest")))
                                           .ToArray();
            }
            if (!targets.Any())
            {
                return;
            }

            var addRequest = CodeAction.Create("Add IRequest<> (empty)", c => Add(context.Document, targets, c, Mode.AddRequest));
            var addRequestHandler = CodeAction.Create("Add IRequestHandler<,> (empty)", c => Add(context.Document, targets, c, Mode.AddRequestHandler));
            var addBoth = CodeAction.Create("Add IRequest<> and IRequestHandler<,> (empty)", c => Add(context.Document, targets, c, Mode.Both));
            var group = CodeAction.Create("MediatR", ImmutableArray.Create(addRequest, addRequestHandler, addBoth), false);

            context.RegisterRefactoring(group);
        }      

        private async Task<Solution> Add(Document document, IEnumerable<MemberDeclarationSyntax> methods, CancellationToken cancellationToken, Mode whatToAdd)
        {
            foreach (var method in methods)
            {
                var data = await BasicData.GetFromMethodDeclaration(document.Project.Solution, method, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                if (whatToAdd.HasFlag(Mode.AddRequest))
                {
                    var requestClassDocument = RequestClassGenerator.GenerateDocument(data);
                    document = document.Project.AddDocument(requestClassDocument);
                }

                if (whatToAdd.HasFlag(Mode.AddRequestHandler))
                {
                    var requestHandlerDocument = RequestHandlerClassGenerator.GenerateDocument(data);
                    document = document.Project.AddDocument(requestHandlerDocument);
                }  

                cancellationToken.ThrowIfCancellationRequested();
            }   

            return document.Project.Solution;
        }

        private enum Mode { AddRequestHandler = 1, AddRequest = 2, Both = 3 }
    }
}
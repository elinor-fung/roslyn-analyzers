﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using Analyzer.Utilities;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.CodeQuality.Analyzers.ApiDesignGuidelines
{
    /// <summary>
    /// This rule is not implemented for C# as the compiler warning CS0628 already covers this part.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    public sealed class DoNotDeclareProtectedMembersInSealedTypes : DiagnosticAnalyzer
    {
        internal const string RuleId = "CA1047";

        private static readonly LocalizableString s_localizableTitle = new LocalizableResourceString(nameof(MicrosoftCodeQualityAnalyzersResources.DoNotDeclareProtectedMembersInSealedTypesTitle), MicrosoftCodeQualityAnalyzersResources.ResourceManager, typeof(MicrosoftCodeQualityAnalyzersResources));
        private static readonly LocalizableString s_localizableMessage = new LocalizableResourceString(nameof(MicrosoftCodeQualityAnalyzersResources.DoNotDeclareProtectedMembersInSealedTypesMessage), MicrosoftCodeQualityAnalyzersResources.ResourceManager, typeof(MicrosoftCodeQualityAnalyzersResources));
        private static readonly LocalizableString s_localizableDescription = new LocalizableResourceString(nameof(MicrosoftCodeQualityAnalyzersResources.DoNotDeclareProtectedMembersInSealedTypesDescription), MicrosoftCodeQualityAnalyzersResources.ResourceManager, typeof(MicrosoftCodeQualityAnalyzersResources));

        internal static DiagnosticDescriptor Rule = DiagnosticDescriptorHelper.Create(
            RuleId,
            s_localizableTitle,
            s_localizableMessage,
            DiagnosticCategory.Design,
            RuleLevel.IdeSuggestion,
            description: s_localizableDescription,
            isPortedFxCopRule: true,
            isDataflowRule: false);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext analysisContext)
        {
            analysisContext.EnableConcurrentExecution();
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            analysisContext.RegisterSymbolAction(context =>
            {
                var symbol = context.Symbol;

                // FxCop compat: only analyze externally visible symbols by default.
                if (!symbol.MatchesConfiguredVisibility(context.Options, Rule, context.CancellationToken))
                {
                    return;
                }

                if (!symbol.IsProtected() ||
                    symbol.IsOverride ||
                    !symbol.ContainingType.IsSealed)
                {
                    return;
                }

                if (symbol is IMethodSymbol method && method.IsFinalizer())
                {
                    return;
                }

                context.ReportDiagnostic(symbol.CreateDiagnostic(Rule, symbol.Name, symbol.ContainingType.Name));
            }, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);
        }
    }
}

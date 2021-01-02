using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Markdig.Extensions.Abbreviations;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.DefinitionLists;
using Markdig.Extensions.Emoji;
using Markdig.Extensions.Figures;
using Markdig.Extensions.Footers;
using Markdig.Extensions.Footnotes;
using Markdig.Extensions.JiraLinks;
using Markdig.Extensions.Mathematics;
using Markdig.Extensions.SmartyPants;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Spectre.Console.Rendering;

namespace Spectre.Console
{
    public class Markdown : Renderable
    {
        private readonly string _markdownText;
        private HttpClient _httpClient;

        public Markdown(string markdownText)
        {
            _markdownText = markdownText;
            _httpClient = new HttpClient();
        }

        protected override IEnumerable<Segment> Render(RenderContext context, int maxWidth)
        {
            var result = new List<Segment>();
            var doc = Markdig.Markdown.Parse(_markdownText);

            foreach (var element in doc)
            {
                result.AddRange(this.RenderBlock(element, context, maxWidth));
            }

            result.Add(Segment.LineBreak);

            return result;
        }

        private IEnumerable<Segment> RenderBlock(Block block, RenderContext context, int maxWidth)
        {
            switch (block)
            {
                case BlankLineBlock blankLineBlock:
                    break;
                case HeadingLinkReferenceDefinition headingLinkReferenceDefinition:
                    break;
                case CustomContainer customContainer:
                    break;
                case DefinitionItem definitionItem:
                    break;
                case DefinitionList definitionList:
                    break;
                case DefinitionTerm definitionTerm:
                    break;
                case Figure figure:
                    break;
                case FigureCaption figureCaption:
                    break;
                case FooterBlock footerBlock:
                    break;
                case Footnote footnote:
                    break;
                case FootnoteGroup footnoteGroup:
                    break;
                case FootnoteLinkReferenceDefinition footnoteLinkReferenceDefinition:
                    break;
                case MathBlock mathBlock:
                    break;
                case Markdig.Extensions.Tables.Table table:
                    break;
                case TableCell tableCell:
                    break;
                case Markdig.Extensions.Tables.TableRow tableRow:
                    break;
                case YamlFrontMatterBlock yamlFrontMatterBlock:
                    break;
                case Abbreviation abbreviation:
                    break;
                case FencedCodeBlock fencedCodeBlock:
                    break;
                case CodeBlock codeBlock:
                    break;
                case LinkReferenceDefinitionGroup linkReferenceDefinitionGroup:
                    break;
                case ListBlock listBlock:
                    return this.RenderListBlock(listBlock, context, maxWidth);
                case ListItemBlock listItemBlock:
                    return this.RenderListItemBlock(listItemBlock, context, maxWidth);
                case MarkdownDocument markdownDocument:
                    break;
                case QuoteBlock quoteBlock:
                    break;
                case ContainerBlock containerBlock:
                    break;
                case HeadingBlock headingBlock:
                    return this.RenderHeadingBlock(headingBlock, context, maxWidth);
                case HtmlBlock htmlBlock:
                    break;
                case LinkReferenceDefinition linkReferenceDefinition:
                    break;
                case ParagraphBlock paragraphBlock:
                    return this.RenderParagraphBlock(paragraphBlock, context, maxWidth);
                case ThematicBreakBlock thematicBreakBlock:
                    return ((IRenderable)new Rule { Style = new Style(decoration: Decoration.Bold) }).Render(context, maxWidth);
                case LeafBlock leafBlock:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(block));
            }

            return Enumerable.Empty<Segment>();
        }

        private IEnumerable<Segment> RenderListItemBlock(ListItemBlock listItemBlock, RenderContext context,
            int maxWidth)
        {
            return listItemBlock.SelectMany(block => this.RenderBlock(block, context, maxWidth));
        }

        private IEnumerable<Segment> RenderListBlock(ListBlock listBlock, RenderContext context,
            int maxWidth)
        {
            var bulletChar = listBlock.BulletType;
            foreach (var item in listBlock)
            {
                yield return new Segment($" {bulletChar} ");

                foreach (var segment in this.RenderBlock(item, context, maxWidth))
                {
                    yield return segment;
                }
            }
        }

        private IEnumerable<Segment> RenderParagraphBlock(ParagraphBlock paragraphBlock, RenderContext context,
            int maxWidth)
        {
            var text = this.RenderContainerInline(paragraphBlock.Inline, context, maxWidth);

            foreach (var segment in text.Render(context, maxWidth))
            {
                yield return segment;
            }

            yield return Segment.LineBreak;
        }

        private IEnumerable<Segment> RenderHeadingBlock(HeadingBlock headingBlock, RenderContext context, int maxWidth)
        {
            var inline = this.RenderContainerInline(headingBlock.Inline, context, maxWidth);
            foreach (var segment in ((IRenderable)new Rule(inline)).Render(context, maxWidth))
            {
                yield return segment;
            }
        }

        private IRenderable RenderContainerInline(ContainerInline inline, RenderContext context, int maxWidth, Style style = null)
        {
            return new CompositeRenderable(inline.Select(x => this.RenderInline(x, context, maxWidth, style)));
        }

        private IRenderable RenderInline(Inline inline, RenderContext context, int maxWidth, Style style)
        {
            switch (inline)
            {
                case FootnoteLink footnoteLink:
                    break;
                case CustomContainerInline customContainerInline:
                    break;
                case EmojiInline emojiInline:
                    break;
                case AbbreviationInline abbreviationInline:
                    break;
                case JiraLink jiraLink:
                    break;
                case MathInline mathInline:
                    break;
                case SmartyPant smartyPant:
                    break;
                case PipeTableDelimiterInline pipeTableDelimiterInline:
                    break;
                case TaskList taskList:
                    break;
                case AutolinkInline autolinkInline:
                    break;
                case CodeInline codeInline:
                    break;
                case EmphasisDelimiterInline emphasisDelimiterInline:
                    break;
                case LinkDelimiterInline linkDelimiterInline:
                    break;
                case DelimiterInline delimiterInline:
                    break;
                case EmphasisInline emphasisInline:
                    var styleDecoration =
                        emphasisInline.DelimiterCount switch
                        {
                            1 => Decoration.Italic,
                            2 => Decoration.Bold,
                            _ => Decoration.None,
                        };
                    var emphasisChildStyle = new Style(decoration: styleDecoration);
                    return this.RenderContainerInline(emphasisInline, context, maxWidth, emphasisChildStyle);
                case LinkInline linkInline:
                    if (linkInline.IsImage)
                    {
                        if (this.TryGetCanvasImageForUrl(linkInline.Url, maxWidth, out var canvasImage))
                        {
                            return canvasImage;
                        }

                        return Text.Empty;
                    }

                    var linkInlineChildStyle = new Style(link: linkInline.Url);
                    return this.RenderContainerInline(linkInline, context, maxWidth, linkInlineChildStyle);
                case ContainerInline containerInline:
                    break;
                case HtmlEntityInline htmlEntityInline:
                    break;
                case HtmlInline htmlInline:
                    break;
                case LineBreakInline lineBreakInline:
                    break;
                case LiteralInline literalInline:
                    return new Text(literalInline.Content.ToString(), style);
                case LeafInline leafInline:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inline));
            }

            return Text.Empty;
        }

        private bool TryGetCanvasImageForUrl(string url, int maxWidth, out CanvasImage canvasImage)
        {
            // TODO: Refactor this for easier unit testing - could do an initial "get image" pass to parallelise the IO too?
            try
            {
                var imageStream = _httpClient.GetStreamAsync(url).Result;
                using var memoryStream = new MemoryStream();
                imageStream.CopyTo(memoryStream);

                // TODO: Make the canvas size a little more dynamic
                canvasImage = new CanvasImage(memoryStream.ToArray()) { MaxWidth = maxWidth / 4 };
                return true;
            }
            catch (Exception)
            {
                canvasImage = null!;
                return false;
            }
        }

        private class CompositeRenderable : Renderable
        {
            private readonly IEnumerable<IRenderable> _renderables;

            public CompositeRenderable(IEnumerable<IRenderable> renderables)
            {
                this._renderables = renderables;
            }

            protected override IEnumerable<Segment> Render(RenderContext context, int maxWidth)
            {
                return this._renderables.SelectMany(x => x.Render(context, maxWidth));
            }
        }
    }
}
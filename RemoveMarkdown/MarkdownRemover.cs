using System.Text.RegularExpressions;

namespace RemoveMarkdown;

/// <summary>
///     Options for controlling how Markdown is removed from text.
/// </summary>
public class RemoveMarkdownOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to strip list leaders.
    /// </summary>
    /// <value>Default is true.</value>
    public bool StripListLeaders { get; set; } = true;

    /// <summary>
    ///     Gets or sets the character to insert instead of stripped list leaders.
    /// </summary>
    /// <value>Default is an empty string.</value>
    public string ListUnicodeChar { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether to support GitHub-Flavored Markdown.
    /// </summary>
    /// <value>Default is true.</value>
    public bool SupportGitHubFavored { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to replace images with alt-text, if present.
    /// </summary>
    /// <value>Default is true.</value>
    public bool UseImgAltText { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to remove abbreviations.
    /// </summary>
    /// <value>Default is false.</value>
    public bool RemoveAbbr { get; set; } = false;

    /// <summary>
    ///     Gets or sets a value indicating whether to replace links with URL.
    /// </summary>
    /// <value>Default is false.</value>
    public bool ReplaceLinksWithURL { get; set; } = false;

    /// <summary>
    ///     Gets or sets a collection of HTML tags to skip during removal.
    /// </summary>
    /// <value>Default is an empty array.</value>
    public string[] HtmlTagsToSkip { get; set; } = [];

    /// <summary>
    ///     Gets or sets a value indicating whether to throw an exception when an error occurs during processing.
    /// </summary>
    /// <value>Default is false.</value>
    public bool ThrowError { get; set; } = false;
}

/// <summary>
///     Provides methods for removing Markdown formatting from text.
/// </summary>
public static class MarkdownRemover
{
    /// <summary>
    ///     Removes Markdown formatting from the specified text.
    /// </summary>
    /// <param name="markdownText">The Markdown text to process.</param>
    /// <returns>The text with Markdown formatting removed.</returns>
    public static string Remove(string markdownText) => Remove(markdownText, new RemoveMarkdownOptions());

    /// <summary>
    ///     Removes Markdown formatting from the specified text using the specified options.
    /// </summary>
    /// <param name="markdownText">The Markdown text to process.</param>
    /// <param name="options">The options for controlling the removal process.</param>
    /// <returns>The text with Markdown formatting removed.</returns>
    public static string Remove(string markdownText, RemoveMarkdownOptions options)
    {
        if (string.IsNullOrEmpty(markdownText)) return string.Empty;

        var output = markdownText;

        try
        {
            // Remove horizontal rules (stripListHeaders conflict with this rule, which is why it has been moved to the top)
            output = Regex.Replace(output, @"^ {0,3}((?:-[\t ]*){3,}|(?:_[ \t]*){3,}|(?:\*[ \t]*){3,})(?:\n+|$)", "", RegexOptions.Multiline);

            if (options.StripListLeaders)
            {
                output = !string.IsNullOrEmpty(options.ListUnicodeChar)
                             ? Regex.Replace(output, @"^([\s\t]*)([\*\-\+]|\d+\.)\s+", options.ListUnicodeChar + " $1", RegexOptions.Multiline)
                             : Regex.Replace(output, @"^([\s\t]*)([\*\-\+]|\d+\.)\s+", "$1",                            RegexOptions.Multiline);
            }

            if (options.SupportGitHubFavored)
            {
                // Header
                output = Regex.Replace(output, @"\n={2,}", "\n");
                // Fenced codeblocks
                output = Regex.Replace(output, @"~{3}.*\n", "");
                // Strikethrough
                output = Regex.Replace(output, @"~~", "");
                // Fenced codeblocks
                output = Regex.Replace(output, @"`{3}.*\n", "");
            }

            if (options.RemoveAbbr)
            {
                // Remove abbreviations
                output = Regex.Replace(output, @"\*\[.*\]:.*\n", "");
            }

            // HTML tag handling
            if (options.HtmlTagsToSkip != null && options.HtmlTagsToSkip.Length > 0)
            {
                // Create a regex that matches tags not in htmlTagsToSkip
                var tagPattern          = string.Join("|", options.HtmlTagsToSkip);
                var htmlTagsToSkipRegex = new Regex($@"<(/)?({tagPattern})(\s+[^>]*|)>", RegexOptions.Compiled);

                // First, temporarily replace the tags we want to keep
                output = htmlTagsToSkipRegex.Replace(output, m => $"###KEEP_TAG_{m.Groups[2].Value}_{m.Groups[1].Value}_{m.Index}###");

                // Remove all HTML tags
                output = Regex.Replace(output, @"<[^>]*>", "");

                // Restore the tags we want to keep
                output = Regex.Replace(output, @"###KEEP_TAG_(\w+)_(\/?)?_(\d+)###", m =>
                {
                    var tagName      = m.Groups[1].Value;
                    var closingSlash = m.Groups[2].Success && m.Groups[2].Value == "/" ? "/" : "";
                    return $"<{closingSlash}{tagName}>";
                });
            }
            else
            {
                // Remove all HTML tags
                output = Regex.Replace(output, @"<[^>]*>", "");
            }

            // Remove setext-style headers
            output = Regex.Replace(output, @"^[=\-]{2,}\s*$", "", RegexOptions.Multiline);
            // Remove footnotes?
            output = Regex.Replace(output, @"\[\^.+?\](\: .*?$)?",  "");
            output = Regex.Replace(output, @"\s{0,2}\[.*?\]: .*?$", "");
            // Remove images
            output = Regex.Replace(output, @"\!\[(.*?)\][\[\(].*?[\]\)]", options.UseImgAltText ? "$1" : "");
            // Remove inline links
            output = Regex.Replace(output, @"\[(.*?)\][\[\(].*?[\]\)]", options.ReplaceLinksWithURL ? "$2" : "$1");
            // Remove blockquotes
            output = Regex.Replace(output, @"^(\n)?\s{0,3}>\s?", "$1", RegexOptions.Multiline);
            // Remove reference-style links?
            output = Regex.Replace(output, @"^\s{1,2}\[(.*?)\]: (\S+)( "".*?"")?\s*$", "", RegexOptions.Multiline);
            // Remove atx-style headers
            output = Regex.Replace(output, @"^(\n)?\s{0,}#{1,6}\s*( (.+))? +#+$|^(\n)?\s{0,}#{1,6}\s*( (.+))?$", "$1$3$4$6", RegexOptions.Multiline);
            // Remove * emphasis
            output = Regex.Replace(output, @"([\*]+)(\S)(.*?\S)??\1", "$2$3");
            // Remove _ emphasis
            output = Regex.Replace(output, @"(^|\W)([_]+)(\S)(.*?\S)??\2($|\W)", "$1$3$4$5");
            // Remove code blocks
            output = Regex.Replace(output, @"(`{3,})(.*?)\1", "$2", RegexOptions.Singleline);
            // Remove inline code
            output = Regex.Replace(output, @"`(.+?)`", "$1");
            // Replace strike through
            output = Regex.Replace(output, @"~(.*?)~", "$1");

            return output;
        }
        catch (Exception ex)
        {
            if (options.ThrowError) throw;

            Console.Error.WriteLine($"RemoveMarkdown encountered error: {ex.Message}");
            return markdownText;
        }
    }
}

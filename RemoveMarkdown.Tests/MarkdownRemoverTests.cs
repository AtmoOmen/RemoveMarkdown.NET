namespace RemoveMarkdown.Tests;

public class MarkdownRemoverTests
{
    [Fact]
    public void Should_Leave_String_Alone_Without_Markdown()
    {
        const string text   = "Javascript Developers are the best.";
        var          result = MarkdownRemover.Remove(text);
        Assert.Equal(text, result);
    }

    [Fact]
    public void Should_Strip_Out_Remaining_Markdown()
    {
        const string text     = "*Javascript* developers are the _best_.";
        const string expected = "Javascript developers are the best.";
        var          result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Leave_Non_Matching_Markdown()
    {
        const string text     = "*Javascript* developers* are the _best_.";
        const string expected = "Javascript developers* are the best.";
        var          result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Leave_Non_Matching_Markdown_But_Strip_Empty_Anchors()
    {
        const string text     = "*Javascript* [developers]()* are the _best_.";
        const string expected = "Javascript developers* are the best.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Strip_HTML()
    {
        const string text     = "<p>Hello World</p>";
        const string expected = "Hello World";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Strip_Anchors()
    {
        const string text     = "*Javascript* [developers](https://engineering.condenast.io/)* are the _best_.";
        const string expected = "Javascript developers* are the best.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Strip_Img_Tags()
    {
        const string text     = "![](https://placebear.com/640/480)*Javascript* developers are the _best_.";
        const string expected = "Javascript developers are the best.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Use_The_Alt_Text_Of_An_Image_If_It_Is_Provided()
    {
        const string text     = "![This is the alt-text](https://www.example.com/images/logo.png)";
        const string expected = "This is the alt-text";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Strip_Code_Tags()
    {
        const string text     = "In `Getting Started` we set up `something` foo.";
        const string expected = "In Getting Started we set up something foo.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Leave_Hashtags_In_Headings()
    {
        const string text     = "## This #heading contains #hashtags";
        const string expected = "This #heading contains #hashtags";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Emphasis()
    {
        const string text     = "I italicized an *I* and it _made_ me *sad*.";
        const string expected = "I italicized an I and it made me sad.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Emphasis_Only_If_There_Is_No_Space_Between_Word_And_Emphasis_Characters()
    {
        const string text     = "There should be no _space_, *before* *closing * _ephasis character _.";
        const string expected = "There should be no space, before *closing * _ephasis character _.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Underscore_Emphasis_Only_If_There_Is_Space_Before_Opening_And_After_Closing_Emphasis_Characters()
    {
        const string text     = "._Spaces_ _ before_ and _after _ emphasised character results in no emphasis.";
        const string expected = ".Spaces _ before_ and _after _ emphasised character results in no emphasis.";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Double_Emphasis()
    {
        const string text     = "**this sentence has __double styling__**";
        const string expected = "this sentence has double styling";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Not_Mistake_A_Horizontal_Rule_When_Symbols_Are_Mixed()
    {
        const string text     = "Some text on a line\n\n--*\n\nA line below";
        const string expected = "Some text on a line\n\n--*\n\nA line below";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Horizontal_Rules()
    {
        const string text     = "Some text on a line\n\n---\n\nA line below";
        const string expected = "Some text on a line\n\nA line below";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Horizontal_Rules_With_Space_Separated_Asterisks()
    {
        const string text     = "Some text on a line\n\n* * *\n\nA line below";
        const string expected = "Some text on a line\n\nA line below";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Blockquotes()
    {
        const string text     = ">I am a blockquote";
        const string expected = "I am a blockquote";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Blockquotes_With_Spaces()
    {
        const string text     = "> I am a blockquote";
        const string expected = "I am a blockquote";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Indented_Blockquotes()
    {
        var tests = new[]
        {
            new { Text = " > I am a blockquote", Expected   = "I am a blockquote" },
            new { Text = "  > I am a blockquote", Expected  = "I am a blockquote" },
            new { Text = "   > I am a blockquote", Expected = "I am a blockquote" }
        };

        foreach (var test in tests)
        {
            var result = MarkdownRemover.Remove(test.Text);
            Assert.Equal(test.Expected, result);
        }
    }

    [Fact]
    public void Should_Remove_Blockquotes_Over_Multiple_Lines()
    {
        const string text     = "> I am a blockquote firstline  \n>I am a blockquote secondline";
        const string expected = "I am a blockquote firstline  \nI am a blockquote secondline";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Remove_Blockquotes_Following_Other_Content()
    {
        const string text     = "## A headline\n\nA paragraph of text\n\n> I am a blockquote";
        const string expected = "A headline\n\nA paragraph of text\n\nI am a blockquote";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Not_Remove_Greater_Than_Signs()
    {
        var tests = new[]
        {
            new { Text = "100 > 0", Expected   = "100 > 0" },
            new { Text = "100 >= 0", Expected  = "100 >= 0" },
            new { Text = "100>0", Expected     = "100>0" },
            new { Text = "> 100 > 0", Expected = "100 > 0" },
            new { Text = "1 < 100", Expected   = "1 < 100" },
            new { Text = "1 <= 100", Expected  = "1 <= 100" }
        };

        foreach (var test in tests)
        {
            var result = MarkdownRemover.Remove(test.Text);
            Assert.Equal(test.Expected, result);
        }
    }

    [Fact]
    public void Should_Strip_Unordered_List_Leaders()
    {
        const string text     = "Some text on a line\n\n* A list Item\n* Another list item";
        const string expected = "Some text on a line\n\nA list Item\nAnother list item";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Strip_Ordered_List_Leaders()
    {
        const string text     = "Some text on a line\n\n9. A list Item\n10. Another list item";
        const string expected = "Some text on a line\n\nA list Item\nAnother list item";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Strip_List_Items_With_Bold_Word_In_The_Beginning()
    {
        const string text     = "Some text on a line\n\n- **A** list Item\n- **Another** list item";
        const string expected = "Some text on a line\n\nA list Item\nAnother list item";
        var result   = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Handle_Paragraphs_With_Markdown()
    {
        const string text =
            "\n## This is a heading ##\n\nThis is a paragraph with [a link](http://www.disney.com/).\n\n### This is another heading\n\nIn `Getting Started` we set up `something` foo.\n\n  * Some list\n  * With items\n    * Even indented";
        const string expected =
            "\nThis is a heading\n\nThis is a paragraph with a link.\n\nThis is another heading\n\nIn Getting Started we set up something foo.\n\n  Some list\n  With items\n    Even indented";
        var result = MarkdownRemover.Remove(text);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_Skip_Specified_HTML_Tags_When_HtmlTagsToSkip_Option_Is_Provided()
    {
        const string text   = "<div>HTML content <sub>Superscript</sub> <span>span text</span></div>";
        var          result = MarkdownRemover.Remove(text, new RemoveMarkdownOptions { HtmlTagsToSkip = ["sub"] });
        Assert.Equal("HTML content <sub>Superscript</sub> span text", result);

        var result2 = MarkdownRemover.Remove(text, new RemoveMarkdownOptions { HtmlTagsToSkip = ["sub", "span"] });
        Assert.Equal("HTML content <sub>Superscript</sub> <span>span text</span>", result2);
    }
}

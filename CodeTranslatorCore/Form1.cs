using CodeTranslator;
using DevExpress.Data.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeTranslatorCore
{
  public partial class Form1 : Form
  {
    private AIAgent _translator;

    private List<string> _validExtensions = new List<string>
    {
      "cs",
      "js",
      "ts",
      "vue",
      "html",
      "razor",
      "jsx",
      "tsx"
    };

    public Form1()
    {
      InitializeComponent();
      _translator = new AIAgent("danish", "english");
    }

    private async void buttonTranslate_Click(object sender, EventArgs e)
    {
      textResult.Text = await _translator.TranslateCodeAsync(textSource.Text);
    }

    private async void TranslateAllFilesInPath(List<string> pathlist, List<string> extensionlist)
    {
      string[] files = new string[] { };

      foreach (var path in pathlist)
        foreach (var ext in extensionlist)
        {
          var filespath = System.IO.Directory.GetFiles(path, "*." + ext, System.IO.SearchOption.AllDirectories);
          files = files.Concat(filespath).Distinct().ToArray();
        }

      // Orders files by last modified date
      files = files.OrderBy(f => System.IO.File.GetLastWriteTime(f)).ToArray();

      foreach (var file in files)
      {
        var timer = DateTime.Now;
        textTranslatedFiles.Text += file;
        var code = System.IO.File.ReadAllText(file);
        var translatedCode = await _translator.TranslateCodeAsync(code);
        System.IO.File.WriteAllText(file, translatedCode);

        var elapsed = DateTime.Now - timer;
        textTranslatedFiles.Text += $" - done {(int)Math.Round(elapsed.TotalSeconds)} seconds" + Environment.NewLine;
        // Scroll to end
        textTranslatedFiles.SelectionStart = textTranslatedFiles.Text.Length;
        textTranslatedFiles.ScrollToCaret();
      }
    }

    // Find extensions in path
    private void buttonReadExtensions_Click(object sender, EventArgs e)
    {
      var path = textSourcePath.Text;
      // Find all used fileextensions in path
      var files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
      var extensions = files.Select(f => System.IO.Path.GetExtension(f).TrimStart('.').ToLower()).Distinct();
      extensions = extensions.Where(ext => _validExtensions.Contains(ext));

      checkedListBoxControlExtensions
        .Items.Clear();
      foreach (var ext in extensions)
      {
        checkedListBoxControlExtensions.Items.Add(ext, false);
      }

      // Find all subdirectories in path
      var directories = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
      checkedListBoxControlDirectories.Items.Clear();
      foreach (var dir in directories)
      {
        checkedListBoxControlDirectories.Items.Add(dir, true);
      }
    }

    private void buttonTranslateAll_Click(object sender, EventArgs e)
    {
      //var path = textSourcePath.Text;
      var extensions = checkedListBoxControlExtensions.CheckedItems;
      var paths = checkedListBoxControlDirectories.CheckedItems;

      // get list of extensions
      var extensionList = new List<string>();
      foreach (var ext in extensions)
        extensionList.Add(ext.ToString());

      // get list of paths
      var pathList = new List<string>();
      foreach (var path in paths)
        pathList.Add(path.ToString());

      TranslateAllFilesInPath(pathList, extensionList);

      //foreach (var path in pathList)
      //  foreach (var extension in extensionList)
      //    TranslateAllFilesInPath(path.ToString(), extension.ToString());
    }

    private void textSourcePath_EditValueChanged(object sender, EventArgs e)
    {

    }
  }
}

using CodeTranslator;
using DevExpress.Data.Helpers;
using DevExpress.XtraSpreadsheet.Model.History;
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
  public partial class FormTranslator : Form
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

    private List<string> _predefinedPaths = new List<string>
    {
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetApplication",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetCommon",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetContracts",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetReport",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetReportData",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetWebApi",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet\GrantetWorker",
      @"C:\_WS\Chapter5\Chapter 5 - Grantet Web\Chapter 5 - Foundas Web\src",

    };

    public FormTranslator()
    {
      InitializeComponent();
      _translator = new AIAgent("danish", "english");

      dateEditFilesBefore.DateTime = DateTime.Now.AddHours(-2);
      // Add predefined paths to checkedListBoxControlDirectories
      foreach (var path in _predefinedPaths)
      {
        comboBoxEditPredefinedPaths.Properties.Items.Add(path);
      }
    }

    private async void buttonTranslate_Click(object sender, EventArgs e)
    {
      textResult.Text = await _translator.TranslateCodeAsync(textSource.Text);
    }

    private async void TranslateAllFilesInPath(List<string> pathlist, List<string> extensionlist)
    {
      string[] files = new string[] { };

      pathlist = pathlist
        .Where(l => !l.ToLower().Contains("\\bin"))
        .Where(l => !l.ToLower().Contains("\\obj"))
        .Where(l => !l.ToLower().Contains("\\."))
        .Where(l => !l.ToLower().Contains("\\packages\\"))
        .Where(l => !l.ToLower().Contains("\\resources"))
        .ToList();

        foreach (var path in pathlist)
        foreach (var ext in extensionlist)
        {
          var filespath = System.IO.Directory.GetFiles(path, "*." + ext, System.IO.SearchOption.AllDirectories);
          files = files.Concat(filespath).Distinct().ToArray();
        }

      // Orders files by last modified date
      files = files
        .Where(l => !l.ToLower().Contains("\\bin"))
        .Where(l => !l.ToLower().Contains("\\obj"))
        .Where(l => !l.ToLower().Contains("\\."))
        .Where(l => !l.ToLower().Contains("\\packages\\"))
        .Where(l => !l.ToLower().Contains("\\resources"))

        .OrderBy(f => System.IO.File.GetLastWriteTime(f)).ToArray();

      var translated = 0;
      foreach (var file in files)
      {
        var changedDate = System.IO.File.GetLastWriteTime(file);
        var timer = DateTime.Now;
        var code = System.IO.File.ReadAllText(file);
        textTranslatedFiles.Text += $"{changedDate:d} {changedDate:t} {file} {code.Length} chars";
        if (changedDate > dateEditFilesBefore.DateTime)
        {
          textTranslatedFiles.Text += " - skipped (not changed)" + Environment.NewLine;
        }
        else
        {
          _translator.ResetThread();
          try
          {
            var translatedCode = await _translator.TranslateCodeAsync(code);
            translatedCode = translatedCode.Replace("\r\n", "\n").Replace("\n", "\r\n"); // Normalize line endings
                                                                                         // Ensure ending line break
            translatedCode = translatedCode.TrimEnd() + Environment.NewLine;

            System.IO.File.WriteAllText(file, translatedCode);

            var elapsed = DateTime.Now - timer;
            textTranslatedFiles.Text += $" - done {(int)Math.Round(elapsed.TotalSeconds)} sec - {translatedCode.Length:n0} chars" + Environment.NewLine;

            translated++;
          }
          catch (System.AggregateException ex)
          {
            var elapsed = DateTime.Now - timer;

            textTranslatedFiles.Text += $" - FAILED (timeout after {(int)Math.Round(elapsed.TotalSeconds)} seconds)" + Environment.NewLine;
            textTranslatedFiles.Text += ex.Message + Environment.NewLine;
          }
        }

        // Scroll to end
        textTranslatedFiles.SelectionStart = textTranslatedFiles.Text.Length;
        textTranslatedFiles.ScrollToCaret();
      }
      textTranslatedFiles.Text += $"Translation completed. {translated} files translated." + Environment.NewLine;
      //MessageBox.Show($"Translation completed. {translated} files translated.");
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
        checkedListBoxControlExtensions.Items.Add(ext, true);
      }

      // Find all subdirectories in path
      var directories = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);
      checkedListBoxControlDirectories.Items.Clear();
      foreach (var dir in directories)
      {
        var chcked = true;
        if (dir.ToLower().Contains("node_modules") ||
            dir.ToLower().Contains("\\.") ||
            dir.ToLower().Contains("\\packages\\") ||
            dir.ToLower().Contains("\\bin") ||
            dir.ToLower().Contains("\\obj"))
          chcked = false; // DO Not add them at all
        else
          checkedListBoxControlDirectories.Items.Add(dir, chcked);
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
    }

    private void comboBoxEditPredefinedPaths_SelectedIndexChanged(object sender, EventArgs e)
    {
      var selectedPath = comboBoxEditPredefinedPaths.SelectedItem.ToString();
      textSourcePath.Text = selectedPath;
    }

    private void simpleButtonUncheckDirs_Click(object sender, EventArgs e)
    {
      // uncheck all in checkedListBoxControlDirectories
      for (int i = 0; i < checkedListBoxControlDirectories.Items.Count; i++)
      {
        checkedListBoxControlDirectories.SetItemChecked(i, false);
      }
    }
  }
}

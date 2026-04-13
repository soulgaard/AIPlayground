using CodeTranslator;
using DevExpress.Data.Helpers;
using DevExpress.XtraEditors;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CodeTranslatorCore
{
  public partial class FormTranslator : Form
  {
    private AIAgent _translator;

    private List<string> _validExtensions = new List<string>
    {
      "json",

      "resx",

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
      @"C:\_WS\Chapter5\Chapter 5 - Grantet Web\Chapter 5 - Foundas Web\src\views\Applicant\Apply",

    };

    /// <summary>
    /// Here you can set language and translation type.
    /// </summary>
    private const string _sourceLanguage = "english";
    private const string _destLanguage = "german";
    private AIAgent.TranslationTypeType _translationType = AIAgent.TranslationTypeType.Vuei18nFiles;

    public FormTranslator()
    {
      InitializeComponent();
      _translator = null;

      lookUpEditTranslationType.Properties.DataSource = Enum.GetValues(typeof(AIAgent.TranslationTypeType));
      lookUpEditTranslationType.EditValue = _translationType;

      dateEditFilesBefore.DateTime = DateTime.Now.AddHours(-2);
      // Add predefined paths to checkedListBoxControlDirectories
      foreach (var path in _predefinedPaths)
      {
        comboBoxEditPredefinedPaths.Properties.Items.Add(path);
      }

      ShowFilesFromDir(textSourcePath.Text);
    }

    private async void buttonTranslate_Click(object sender, EventArgs e)
    {
      textLog.Text += "Start " + DateTime.Now.ToShortTimeString();
      textResult.Text = await _translator.TranslateCodeAsync(textSource.Text);
      textLog.Text += " - Finished " + DateTime.Now.ToShortTimeString();
    }

    private async Task<List<string>> Getfiles(List<string> pathlist, List<string> extensionlist)
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

      return files.ToList();
    }


    private async void TranslateAllFilesInPath(List<string> pathlist, List<string> extensionlist)
    {
      var files = await Getfiles(pathlist, extensionlist);
      TranslateFiles(files);
    }
    private async void TranslateFiles(List<string> files)
    {
      var translated = 0;
      foreach (var file in files)
      {
        var changedDate = System.IO.File.GetLastWriteTime(file);
        var timer = DateTime.Now;
        var code = System.IO.File.ReadAllText(file);

        if (changedDate > dateEditFilesBefore.DateTime)
        {
          textLog.Text += ".";
          //textTranslatedFiles.Text += " - skipped (not changed)" + Environment.NewLine;
        }
        else
        {
          Log($"{changedDate:d} {changedDate:t} {file} {code.Length} chars");
          _translator.ResetThread();
          try
          {
            var translatedCode = await _translator.TranslateCodeAsync(code);
            translatedCode = translatedCode.Replace("\r\n", "\n").Replace("\n", "\r\n"); // Normalize line endings
                                                                                         // Ensure ending line break
            translatedCode = translatedCode.TrimEnd() + Environment.NewLine;

            System.IO.File.WriteAllText(file, translatedCode);

            var elapsed = DateTime.Now - timer;
            LogLine($" - done {(int)Math.Round(elapsed.TotalSeconds)} sec - {translatedCode.Length:n0} chars");

            translated++;
          }
          catch (System.AggregateException ex)
          {
            var elapsed = DateTime.Now - timer;

            LogLine($" - FAILED (timeout after {(int)Math.Round(elapsed.TotalSeconds)} seconds)");
            LogLine(ex.Message);
          }
        }

        // Scroll to end
        //textLog.SelectionStart = textLog.Text.Length;
        //textLog.ScrollToCaret();
      }
      LogLine($"Translation completed. {translated} files translated.");
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
      var files = GetSelectedStringList(checkedListBoxControlFiles);
      TranslateFiles(files);
    }

    private List<string> GetSelectedStringList(CheckedListBoxControl listBox)
    {
      var items = listBox.CheckedItems;

      // get list of extensions
      var itemList = new List<string>();
      foreach (var ext in items)
        itemList.Add(ext.ToString());

      return itemList;
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

    private void buttonBrowse_Click(object sender, EventArgs e)
    {
      var dialog = new FolderBrowserDialog();
      dialog.Description = "Select a folder";
      dialog.InitialDirectory = textSourcePath.Text;
      dialog.UseDescriptionForTitle = true; // Shows description as window title (.NET 5+)

      if (dialog.ShowDialog() == DialogResult.OK)
      {
        textSourcePath.Text = dialog.SelectedPath;

        ShowFilesFromDir(dialog.SelectedPath);
      }
    }

    private void ShowFilesFromDir(string selectedPath)
    {
      var files = System.IO.Directory.GetFiles(selectedPath, "*.*", System.IO.SearchOption.TopDirectoryOnly);
      var fileList = files.ToList().OrderBy(l => l).ToList();
      checkedListBoxControlFiles.Items.Clear();

      foreach (var item in fileList)
      {
        checkedListBoxControlFiles.Items.Add(item, false);
      }
    }

    private void buttonVueLocalizable_Click(object sender, EventArgs e)
    {
      var files = GetSelectedStringList(checkedListBoxControlFiles);

      if (files.Count == 0)
      {
        MessageBox.Show("Please select a file to translate.");
        return;
      }
      LogLine($"Translating {files.Count} files...");

      foreach (var file in files)
        LocalizeVueFile(file);

      // Translation is running asynchronously, so we do not wait for it to finish here. The results will be logged as they come in.
    }

    private async void LocalizeVueFile(string file)
    {
      var timer = DateTime.Now;
      var code = System.IO.File.ReadAllText(file);
      var fileNoPath = System.IO.Path.GetFileName(file);

      LogLine($"{file} {code.Length} chars -  running");

      _translator.ResetThread();
      try
      {
        var translatedCode = await _translator.TranslateCodeAsync(code);
        translatedCode = translatedCode.Replace("\r\n", "\n").Replace("\n", "\r\n"); // Normalize line endings
                                                                                     // Ensure ending line break
        translatedCode = translatedCode.TrimEnd() + Environment.NewLine;
        var codesplit = translatedCode.Split("--TranslatedJson--", StringSplitOptions.RemoveEmptyEntries);

        var vueCode = codesplit[0];
        if (codesplit.Length > 1)
        {
          var jsonCode = codesplit[1];
          var jsonFile = @"C:\_WS\Chapter5\Chapter 5 - Grantet Web\Chapter 5 - Foundas Web\src\assets\localization\en.json";
          //var jsonFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + ".json");
          System.IO.File.AppendAllText(jsonFile, jsonCode);
          //LogLine(jsonCode);

          textResult.Text += jsonCode + Environment.NewLine;
        }

        System.IO.File.WriteAllText(file, vueCode);

        var elapsed = DateTime.Now - timer;
        LogLine($" - {fileNoPath} done {(int)Math.Round(elapsed.TotalSeconds)} sec - {translatedCode.Length:n0} chars");
      }
      catch (System.AggregateException ex)
      {
        var elapsed = DateTime.Now - timer;

        LogLine($" - FAILED (timeout after {(int)Math.Round(elapsed.TotalSeconds)} seconds)");
        LogLine(ex.Message);
      }
    }

    private void LogLine(string txt)
    {
      textLog.Text += txt + Environment.NewLine;
    }
    private void Log(string txt)
    {
      textLog.Text += txt;
    }

    private async void buttonShowFiles_Click(object sender, EventArgs e)
    {
      var pathList = GetSelectedStringList(checkedListBoxControlDirectories);
      var extensionList = GetSelectedStringList(checkedListBoxControlExtensions);
      var files = await Getfiles(pathList, extensionList);
      checkedListBoxControlFiles.Items.Clear();

      foreach (var item in files)
      {
        checkedListBoxControlFiles.Items.Add(item, false);
      }
    }

    private void lookUpEditTranslationType_EditValueChanged(object sender, EventArgs e)
    {
      if (lookUpEditTranslationType.EditValue == null)
      {
        this.Text = "Awaiting translation type selection...";
        _translator = null;
      }
      else
      {
        _translationType = (AIAgent.TranslationTypeType)lookUpEditTranslationType.EditValue;
        _translator = new AIAgent(_sourceLanguage, _destLanguage, _translationType);
        this.Text = $"Tranlator - {_sourceLanguage} to {_destLanguage} - for {_translationType}";
      }
    }

    private void buttonCheckAllFiles_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < checkedListBoxControlFiles.Items.Count; i++)
      {
        var val = checkedListBoxControlFiles.GetItemChecked(i);
        checkedListBoxControlFiles.SetItemChecked(i, !val);
      }
    }
  }
}

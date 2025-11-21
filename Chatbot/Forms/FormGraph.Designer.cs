namespace Chatbot.Forms
{
  partial class FormGraph
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      var xyDiagram1 = new DevExpress.XtraCharts.XYDiagram();
      var series1 = new DevExpress.XtraCharts.Series();
      var stackedBarSeriesView1 = new DevExpress.XtraCharts.StackedBarSeriesView();
      chartControl = new DevExpress.XtraCharts.ChartControl();
      memoEditText = new DevExpress.XtraEditors.MemoEdit();
      ((System.ComponentModel.ISupportInitialize)chartControl).BeginInit();
      ((System.ComponentModel.ISupportInitialize)xyDiagram1).BeginInit();
      ((System.ComponentModel.ISupportInitialize)series1).BeginInit();
      ((System.ComponentModel.ISupportInitialize)stackedBarSeriesView1).BeginInit();
      ((System.ComponentModel.ISupportInitialize)memoEditText.Properties).BeginInit();
      SuspendLayout();
      // 
      // chartControl
      // 
      chartControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      xyDiagram1.AxisX.VisibleInPanesSerializable = "-1";
      xyDiagram1.AxisY.VisibleInPanesSerializable = "-1";
      chartControl.Diagram = xyDiagram1;
      chartControl.Location = new Point(12, 12);
      chartControl.Name = "chartControl";
      series1.Name = "Series 1";
      series1.SeriesID = 0;
      series1.View = stackedBarSeriesView1;
      chartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[]
  {
    series1
  };
      chartControl.Size = new Size(801, 496);
      chartControl.TabIndex = 0;
      // 
      // memoEditText
      // 
      memoEditText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      memoEditText.Location = new Point(12, 514);
      memoEditText.Name = "memoEditText";
      memoEditText.Size = new Size(801, 292);
      memoEditText.TabIndex = 1;
      // 
      // FormGraph
      // 
      AutoScaleDimensions = new SizeF(10F, 25F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(822, 818);
      Controls.Add(memoEditText);
      Controls.Add(chartControl);
      Name = "FormGraph";
      Text = "FormGraph";
      Shown += FormGraph_Shown;
      ((System.ComponentModel.ISupportInitialize)xyDiagram1).EndInit();
      ((System.ComponentModel.ISupportInitialize)stackedBarSeriesView1).EndInit();
      ((System.ComponentModel.ISupportInitialize)series1).EndInit();
      ((System.ComponentModel.ISupportInitialize)chartControl).EndInit();
      ((System.ComponentModel.ISupportInitialize)memoEditText.Properties).EndInit();
      ResumeLayout(false);
    }

    #endregion

    private DevExpress.XtraCharts.ChartControl chartControl;
    private DevExpress.XtraEditors.MemoEdit memoEditText;
  }
}
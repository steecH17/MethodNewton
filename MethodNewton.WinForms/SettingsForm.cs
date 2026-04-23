using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class SettingsForm : Form
{
    private AppSettings _settings;
    private NumericUpDown _toleranceInput;
    private GroupBox _functionsGroupBox;
    private GroupBox _powersGroupBox;
    private CheckBox _sinCheckBox;
    private CheckBox _cosCheckBox;
    private CheckBox _tgCheckBox;
    private CheckBox _ctgCheckBox;
    private CheckBox _expCheckBox;
    private CheckBox _lnCheckBox;
    private CheckBox _xCheckBox;
    private CheckBox _x2CheckBox;
    private CheckBox _x3CheckBox;
    private CheckBox _x4CheckBox;
    private CheckBox _x5CheckBox;
    private Button _okButton;
    private Button _cancelButton;
    private Button _resetButton;
    private GroupBox _selectionGroupBox;
    private RadioButton _bestR2Radio;
    private RadioButton _thresholdRadio;
    private NumericUpDown _thresholdInput;

    public SettingsForm(AppSettings settings)
    {
        _settings = settings;
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Настройки приложения";
        this.Size = new Size(450, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        var toleranceGroupBox = new GroupBox
        {
            Text = "Параметры вычислений",
            Location = new Point(10, 10),
            Size = new Size(415, 70)
        };

        var toleranceLabel = new Label
        {
            Text = "Точность (tolerance):",
            Location = new Point(10, 25),
            Size = new Size(150, 23)
        };

        _toleranceInput = new NumericUpDown
        {
            Location = new Point(170, 23),
            Size = new Size(120, 23),
            Minimum = 1e-10m,
            Maximum = 1e-1m,
            DecimalPlaces = 8,
            Increment = 0.000001m
        };

        toleranceGroupBox.Controls.AddRange(new Control[] { toleranceLabel, _toleranceInput });

        _functionsGroupBox = new GroupBox
        {
            Text = "Тригонометрические функции",
            Location = new Point(10, 90),
            Size = new Size(200, 150)
        };

        _sinCheckBox = new CheckBox { Text = "sin(x)", Location = new Point(10, 20), Size = new Size(80, 24) };
        _cosCheckBox = new CheckBox { Text = "cos(x)", Location = new Point(10, 45), Size = new Size(80, 24) };
        _tgCheckBox = new CheckBox { Text = "tg(x)", Location = new Point(10, 70), Size = new Size(80, 24) };
        _ctgCheckBox = new CheckBox { Text = "ctg(x)", Location = new Point(10, 95), Size = new Size(80, 24) };
        _expCheckBox = new CheckBox { Text = "exp(x)", Location = new Point(100, 20), Size = new Size(80, 24) };
        _lnCheckBox = new CheckBox { Text = "ln(x)", Location = new Point(100, 45), Size = new Size(80, 24) };

        _functionsGroupBox.Controls.AddRange(new Control[]
        {
            _sinCheckBox, _cosCheckBox, _tgCheckBox,
            _ctgCheckBox, _expCheckBox, _lnCheckBox
        });

        _powersGroupBox = new GroupBox
        {
            Text = "Степенные функции",
            Location = new Point(220, 90),
            Size = new Size(205, 150)
        };

        _xCheckBox = new CheckBox { Text = "x", Location = new Point(10, 20), Size = new Size(70, 24) };
        _x2CheckBox = new CheckBox { Text = "x²", Location = new Point(80, 20), Size = new Size(70, 24) };
        _x3CheckBox = new CheckBox { Text = "x³", Location = new Point(150, 20), Size = new Size(70, 24) };
        _x4CheckBox = new CheckBox { Text = "x⁴", Location = new Point(10, 45), Size = new Size(70, 24) };
        _x5CheckBox = new CheckBox { Text = "x⁵", Location = new Point(80, 45), Size = new Size(70, 24) };

        var infoLabel = new Label
        {
            Text = "Выберите компоненты для составления функции",
            Location = new Point(10, 75),
            Size = new Size(180, 30),
            ForeColor = Color.Gray,
            Font = new Font("Arial", 8, FontStyle.Italic)
        };

        _powersGroupBox.Controls.AddRange(new Control[]
        {
            _xCheckBox, _x2CheckBox, _x3CheckBox,
            _x4CheckBox, _x5CheckBox, infoLabel
        });


        this.Controls.AddRange(new Control[]
        {
            toleranceGroupBox,
            _functionsGroupBox,
            _powersGroupBox
        });

        _selectionGroupBox = new GroupBox
        {
            Text = "Критерий выбора функции",
            Location = new Point(10, 250),
            Size = new Size(415, 120)
        };

        _bestR2Radio = new RadioButton
        {
            Text = "Лучшая по коэффициенту детерминации (R²)",
            Location = new Point(10, 20),
            Size = new Size(300, 25),
            Checked = true
        };

        _thresholdRadio = new RadioButton
        {
            Text = "С порогом точности:",
            Location = new Point(10, 50),
            Size = new Size(150, 25)
        };

        _thresholdInput = new NumericUpDown
        {
            Location = new Point(165, 48),
            Size = new Size(80, 25),
            Minimum = 0.5m,
            Maximum = 0.9999m,
            Value = 0.95m,
            DecimalPlaces = 4,
            Increment = 0.01m,
            Enabled = false
        };

        _bestR2Radio.CheckedChanged += (s, e) =>
            _thresholdInput.Enabled = !_bestR2Radio.Checked;

        _thresholdRadio.CheckedChanged += (s, e) =>
            _thresholdInput.Enabled = _thresholdRadio.Checked;

        _selectionGroupBox.Controls.AddRange(new Control[]
        {
        _bestR2Radio,
        _thresholdRadio,
        _thresholdInput
        });

        this.Controls.Add(_selectionGroupBox);

        var buttonPanel = new Panel
        {
            Location = new Point(10, 390),
            Size = new Size(415, 40)
        };

        _okButton = new Button
        {
            Text = "OK",
            Location = new Point(130, 5),
            Size = new Size(75, 30),
            DialogResult = DialogResult.OK
        };
        _okButton.Click += OkButton_Click;

        _cancelButton = new Button
        {
            Text = "Отмена",
            Location = new Point(210, 5),
            Size = new Size(75, 30),
            DialogResult = DialogResult.Cancel
        };

        _resetButton = new Button
        {
            Text = "Сброс",
            Location = new Point(290, 5),
            Size = new Size(75, 30)
        };
        _resetButton.Click += ResetButton_Click;

        buttonPanel.Controls.AddRange(new Control[] { _okButton, _cancelButton, _resetButton });

        this.AcceptButton = _okButton;
        this.CancelButton = _cancelButton;

        this.Controls.Add(buttonPanel);

    }

    private void LoadSettings()
    {
        _toleranceInput.Value = (decimal)_settings.Tolerance;

        _sinCheckBox.Checked = _settings.UseSin;
        _cosCheckBox.Checked = _settings.UseCos;
        _tgCheckBox.Checked = _settings.UseTg;
        _ctgCheckBox.Checked = _settings.UseCtg;
        _expCheckBox.Checked = _settings.UseExp;
        _lnCheckBox.Checked = _settings.UseLn;

        _xCheckBox.Checked = _settings.UseX;
        _x2CheckBox.Checked = _settings.UseX2;
        _x3CheckBox.Checked = _settings.UseX3;
        _x4CheckBox.Checked = _settings.UseX4;
        _x5CheckBox.Checked = _settings.UseX5;

        _bestR2Radio.Checked = _settings.BestModel;
        _thresholdRadio.Checked = _settings.MostCompactThreshold;
        _thresholdInput.Value = (decimal)_settings.Threshold;
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
        _settings.Tolerance = (double)_toleranceInput.Value;

        _settings.UseSin = _sinCheckBox.Checked;
        _settings.UseCos = _cosCheckBox.Checked;
        _settings.UseTg = _tgCheckBox.Checked;
        _settings.UseCtg = _ctgCheckBox.Checked;
        _settings.UseExp = _expCheckBox.Checked;
        _settings.UseLn = _lnCheckBox.Checked;

        _settings.UseX = _xCheckBox.Checked;
        _settings.UseX2 = _x2CheckBox.Checked;
        _settings.UseX3 = _x3CheckBox.Checked;
        _settings.UseX4 = _x4CheckBox.Checked;
        _settings.UseX5 = _x5CheckBox.Checked;

        _settings.BestModel = _bestR2Radio.Checked;
        _settings.MostCompactThreshold = _thresholdRadio.Checked;
        _settings.Threshold = (double)_thresholdInput.Value;
    }

    private void ResetButton_Click(object sender, EventArgs e)
    {
        _settings.Tolerance = 1e-6;
        _settings.UseSin = true;
        _settings.UseCos = true;
        _settings.UseTg = false;
        _settings.UseCtg = false;
        _settings.UseExp = true;
        _settings.UseLn = false;
        _settings.UseX = true;
        _settings.UseX2 = false;
        _settings.UseX3 = false;
        _settings.UseX4 = false;
        _settings.UseX5 = false;
        _settings.BestModel = true;
        _settings.MostCompactThreshold = false;
        _settings.Threshold = 0.9d;

        LoadSettings();
    }
}
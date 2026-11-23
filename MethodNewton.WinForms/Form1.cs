using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace MethodNewton.WinForms;

public partial class Form1 : Form
{
    private string _excelFilePath;
    private List<List<double>>? xData = null;
    private List<double>? yData = null;
    private int countArgs;
    private int countDataInTable;
    private bool canEditTable;
    private DataGridView _dataGridView;
    private DataTable _dataTable;
    private AppSettings _appSettings;
    private FunctionModel? _functionModel = null;
    private StatisticModels? _statisticModels = null;
    private Label functionValueLabel;
    private Label determValueLabel;
    private Label iterationsValueLabel;
    private Label componentsValueLabel;
    // НОВЫЕ метки для вкладки "Результаты"
    private Label functionResValueLabel;
    private Label determResValueLabel;
    private Label iterationsResValueLabel;
    private Label componentsResValueLabel;
    private Label timeResValueLabel;
    private TabControl _tabControl;
    private TabPage _inputTab, _resultsTab, _chartTab;
    private DataGridView _resultsDataGridView; // Таблица для вкладки результатов
    private PlotView _plotView; // Контрол для графика
    private Label timeValueLabel; // Для времени поиска
    private Button saveChartBtn;
    public Form1()
    {
        InitializeComponent();
        InitializeToolStrip();
        _appSettings = new AppSettings();
        countDataInTable = 0;
        canEditTable = false;
    }

    private void InitializeToolStrip()
    {
        ConfigureForm();
        this.MainMenuStrip = CreateMenuStrip();

        _tabControl = new TabControl { Dock = DockStyle.Fill, Padding = new Point(12, 8) };
        _inputTab = new TabPage(" Ввод данных ");
        _resultsTab = new TabPage(" Результаты ");
        _chartTab = new TabPage(" График ");
        _tabControl.TabPages.AddRange(new[] { _inputTab, _resultsTab, _chartTab });

        SetupInputTab();
        SetupResultTab();
        SetupChartTab();

        // Сборка формы
        this.Controls.Clear();
        this.Controls.Add(_tabControl);
        this.Controls.Add(this.MainMenuStrip);
        this.PerformLayout();
    }

    private void SetupChartTab()
    {
        _plotView = new PlotView { Dock = DockStyle.Fill, BackColor = Color.White };
        var chartTools = new Panel { Dock = DockStyle.Right, Width = 160, BackColor = Color.FromArgb(240, 240, 240) };
        saveChartBtn = CreateStyledButton("💾 Сохранить PNG", Color.White, Color.Black, new Size(140, 40));
        saveChartBtn.Location = new Point(10, 10);
        saveChartBtn.Enabled = false;
        saveChartBtn.Click += SaveChartBtn_Click;

        chartTools.Controls.Add(saveChartBtn);
        _chartTab.Controls.Add(_plotView);
        _chartTab.Controls.Add(chartTools);
    }

    private void SaveChartBtn_Click(object? sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "PNG Image|*.png",
            FileName = "ApproximationChart.png"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // Создаем экспортер
                var pngExporter = new OxyPlot.WindowsForms.PngExporter
                {
                    Width = 1000,
                    Height = 700
                };

                // Создаем поток для записи файла и передаем его в метод Export
                using (var stream = System.IO.File.Create(sfd.FileName))
                {
                    pngExporter.Export(_plotView.Model, stream);
                }

                MessageBox.Show("График сохранен успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void SetupResultTab()
    {
        var resultsMainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

        var metricsGroup = new GroupBox { Text = " Метрики аппроксимации ", Dock = DockStyle.Top, Height = 140, Font = new Font("Segoe UI Semibold", 9F) };

        functionResValueLabel = new Label { Text = "Функция: —", Location = new Point(20, 25), Size = new Size(1000, 35), Font = new Font("Consolas", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 102, 204) };
        determResValueLabel = new Label { Text = "R²: —", Location = new Point(20, 65), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.Green };
        iterationsResValueLabel = new Label { Text = "Итераций: —", Location = new Point(20, 95), AutoSize = true };
        timeResValueLabel = new Label { Text = "Время поиска: —", Location = new Point(250, 95), AutoSize = true };
        componentsResValueLabel = new Label { Text = "Компонентов: —", Location = new Point(500, 95), AutoSize = true };

        metricsGroup.Controls.AddRange(new Control[] { functionResValueLabel, determResValueLabel, iterationsResValueLabel, timeResValueLabel, componentsResValueLabel });

        // Контекстное меню для копирования
        var copyMenu = new ContextMenuStrip();
        copyMenu.Items.Add("Копировать как Текст", null, (s, e) => { if (_functionModel != null) Clipboard.SetText(_functionModel.ToString()); });
        copyMenu.Items.Add("Копировать как LaTeX", null, (s, e) => { if (_functionModel != null) Clipboard.SetText(_functionModel.ToLatex()); });
        functionResValueLabel.ContextMenuStrip = copyMenu;

        _resultsDataGridView = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        // Панель инструментов справа (Экспорт)
        var resultsTools = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 180,
            BackColor = Color.FromArgb(240, 240, 240),
            Padding = new Padding(10)
        };

        var exportResultsBtn = CreateStyledButton("💾 Экспорт в Excel", Color.White, Color.Black, new Size(150, 65));
        exportResultsBtn.Click += (s, e) => ExportResultsToExcel();

        resultsTools.Controls.Add(exportResultsBtn);

        resultsMainPanel.Controls.Add(resultsTools);
        resultsMainPanel.Controls.Add(_resultsDataGridView);
        resultsMainPanel.Controls.Add(new Label { Text = "Таблица сравнения (исходные vs расчетные):", Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.BottomLeft });
        resultsMainPanel.Controls.Add(metricsGroup);
        _resultsTab.Controls.Add(resultsMainPanel);
    }

    private void ExportResultsToExcel()
    {
        if (_functionModel == null || xData == null || yData == null)
        {
            MessageBox.Show("Сначала выполните расчет!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "Excel Files|*.xlsx",
            FileName = "Результаты_анализа.xlsx"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // Подготавливаем список рассчитанных значений f(x)
                List<double> yCalc = new List<double>();
                foreach (var xRow in xData)
                {
                    double val = 0;
                    double[] xArray = xRow.ToArray();
                    for (int i = 0; i < _functionModel.Function.Count; i++)
                    {
                        val += _functionModel.Function[i].Calculate(xArray, _functionModel.CoeffFunction[i]);
                    }
                    yCalc.Add(val);
                }

                // Вызываем обновленный метод принтера
                ExcelPrinter printer = new ExcelPrinter(sfd.FileName);
                printer.SaveResultsExcel(xData, yData, yCalc, sfd.FileName);

                MessageBox.Show("Файл успешно сохранен!", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void SetupInputTab()
    {
        // Тулбар для кнопок быстрого действия
        var toolStrip = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, BackColor = Color.White, Padding = new Padding(5) };
        toolStrip.Items.Add(new ToolStripButton("➕", null, (s, e) => AddEmptyRow()) { ToolTipText = "Добавить строку" });
        toolStrip.Items.Add(new ToolStripButton("❌", null, (s, e) => DeleteSelectedRows()) { ToolTipText = "Удалить строку" });

        _dataGridView = CreateMainDataGrid();

        var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 160, BackColor = Color.FromArgb(245, 246, 247), Padding = new Padding(10) };
        var buttonsPanel = new FlowLayoutPanel { Width = 200, Dock = DockStyle.Left, FlowDirection = FlowDirection.TopDown };

        var calculateButton = CreateStyledButton("🔍 НАЙТИ ФУНКЦИЮ", Color.FromArgb(0, 120, 215), Color.White);
        calculateButton.FlatAppearance.BorderSize = 0;
        calculateButton.Click += CalculateButton_Click;

        var quickResGroupBox = CreateQuickResultGroupBox();

        buttonsPanel.Controls.Add(calculateButton);
        

        bottomPanel.Controls.Add(quickResGroupBox);
        bottomPanel.Controls.Add(buttonsPanel);

        _inputTab.Controls.Add(_dataGridView);
        _inputTab.Controls.Add(toolStrip);
        _inputTab.Controls.Add(bottomPanel);
    }

    private GroupBox CreateQuickResultGroupBox()
    {
        var quickResGroup = new GroupBox { Text = " Быстрый результат ", Dock = DockStyle.Fill, Font = new Font("Segoe UI Semibold", 9F) };
        functionValueLabel = new Label { Text = "—", Location = new Point(15, 35), Size = new Size(850, 35), Font = new Font("Consolas", 12F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 102, 204), TextAlign = ContentAlignment.MiddleLeft };

        var quickMetricsFlow = new FlowLayoutPanel { Location = new Point(15, 80), Size = new Size(900, 40), FlowDirection = FlowDirection.LeftToRight };
        determValueLabel = new Label { Text = "R²: —", AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.Green, Margin = new Padding(0, 5, 25, 0) };
        iterationsValueLabel = new Label { Text = "Итераций: —", AutoSize = true, Margin = new Padding(0, 5, 25, 0) };
        componentsValueLabel = new Label { Text = "Слагаемых: —", AutoSize = true, Margin = new Padding(0, 5, 25, 0) };
        timeValueLabel = new Label { Text = "Время: —", AutoSize = true, Margin = new Padding(0, 5, 0, 0) };

        quickMetricsFlow.Controls.AddRange(new Control[] { determValueLabel, iterationsValueLabel, componentsValueLabel, timeValueLabel });
        quickResGroup.Controls.AddRange(new Control[] { functionValueLabel, quickMetricsFlow });

        // Контекстное меню для копирования
        var copyMenu = new ContextMenuStrip();
        copyMenu.Items.Add("Копировать как Текст", null, (s, e) => { if (_functionModel != null) Clipboard.SetText(_functionModel.ToString()); });
        copyMenu.Items.Add("Копировать как LaTeX", null, (s, e) => { if (_functionModel != null) Clipboard.SetText(_functionModel.ToLatex()); });
        functionValueLabel.ContextMenuStrip = copyMenu;

        return quickResGroup;
    }

    private MenuStrip CreateMenuStrip()
    {
        var menuStrip = new MenuStrip { BackColor = Color.WhiteSmoke, Padding = new Padding(6, 2, 0, 2) };
        var fileMenu = new ToolStripMenuItem("Файл");
        fileMenu.DropDownItems.Add("Новый", null, (s, e) => ClearData());
        fileMenu.DropDownItems.Add("Открыть...", null, (s, e) => LoadFromFile());
        fileMenu.DropDownItems.Add("Сохранить", null, (s, e) => SaveToFile());
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Настройки...", null, (s, e) => ShowSettingsForm());
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add("Выход", null, (s, e) => this.Close());

        var editMenu = new ToolStripMenuItem("Правка");
        editMenu.DropDownItems.Add("Добавить строку", null, (s, e) => AddEmptyRow());
        editMenu.DropDownItems.Add("Удалить строку", null, (s, e) => DeleteSelectedRows());
        editMenu.DropDownItems.Add(new ToolStripSeparator());
        editMenu.DropDownItems.Add("Копировать", null, (s, e) => CopySelectedCells());
        editMenu.DropDownItems.Add("Вставить", null, (s, e) => PasteToCells());

        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu });
        return menuStrip;
    }

    private void ConfigureForm()
    {
        this.Size = new Size(1300, 800);
        this.Text = "Автоматизация выбора трансцендентной функции";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 9F);
    }

    private DataGridView CreateMainDataGrid()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            RowHeadersVisible = true,
            MultiSelect = true,
            SelectionMode = DataGridViewSelectionMode.RowHeaderSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgv.EnableHeadersVisualStyles = false;
        dgv.DataError += _dataGridView_DataError;

        return dgv;
    }

    private Button CreateStyledButton(string text, Color backColor, Color foreColor, Size? size = null)
    {
        var btn = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Size = size ?? new Size(180, 50),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = (backColor == Color.White) ? 1 : 0;
        return btn;
    }

    private void _dataGridView_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        // Проверяем только ячейки данных, игнорируем заголовки и новую строку
        if (e.RowIndex < 0 || _dataGridView.Rows[e.RowIndex].IsNewRow) return;

        string input = e.FormattedValue.ToString().Replace(',', '.').Trim();

        // Пытаемся распарсить с учетом точки или запятой
        if (!double.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _))
        {
            _dataGridView.Rows[e.RowIndex].ErrorText = "Введите числовое значение (например, 12.5)";
            e.Cancel = true; // Блокируем выход из ячейки

            MessageBox.Show("Ошибка: Введено некорректное значение. Допускаются только числа.",
                            "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            _dataGridView.Rows[e.RowIndex].ErrorText = ""; // Очищаем ошибку, если всё ок
        }

    }

    private void _dataGridView_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        MessageBox.Show(
        $"Ошибка в ячейке: {e.Exception.Message}\n" +
        $"Пожалуйста, введите корректное значение.",
        "Ошибка ввода данных",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    );
    }

    // Метод для проверки всех данных перед расчетом
    private bool ValidateAllData()
    {
        bool isValid = true;

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            if (row.IsNewRow) continue;

            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    cell.ErrorText = "Обязательное поле";
                    isValid = false;
                }
                else if (!double.TryParse(cell.Value.ToString(), out _))
                {
                    cell.ErrorText = "Должно быть число";
                    isValid = false;
                }
            }
        }

        if (!isValid)
        {
            MessageBox.Show("Таблица содержит ошибки. Исправьте их перед расчетом.",
                "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        return isValid;
    }

    private void CalculateButton_Click(object? sender, EventArgs e)
    {
        SyncListsFromTable();

        if (xData == null || yData == null || xData.Count == 0) return;

        OptimalFunctionFinder finder = new OptimalFunctionFinder(_appSettings.GetFunctionComponents());
        _statisticModels = new StatisticModels(finder.FindOptimalFunction(xData, yData, _appSettings.Tolerance));

        LoadResultLabel();
        UpdateResultsTable(); // Заполняем таблицу на 2-й вкладке
        UpdateChartDisplay(); // Рисуем график на 3-й вкладке

        saveChartBtn.Enabled = true;
        // Автоматический переход на вкладку результатов
        _tabControl.SelectedTab = _resultsTab;
    }

    private void UpdateResultsTable()
    {
        if (_functionModel == null) return;

        DataTable dt = new DataTable();
        // Создаем колонки X
        for (int i = 0; i < countArgs; i++) dt.Columns.Add($"X{i + 1}", typeof(double));
        dt.Columns.Add("Y (Исходное)", typeof(double));
        dt.Columns.Add("f(x) (Расчетное)", typeof(double));
        dt.Columns.Add("Невязка", typeof(double));

        for (int i = 0; i < xData.Count; i++)
        {
            double[] xRow = xData[i].ToArray();
            // Рассчитываем значение f(x)
            double f_x = 0;
            for (int j = 0; j < _functionModel.Function.Count; j++)
                f_x += _functionModel.Function[j].Calculate(xRow, _functionModel.CoeffFunction[j]);

            DataRow row = dt.NewRow();
            for (int j = 0; j < countArgs; j++) row[j] = Math.Round(xData[i][j], 4);
            row[countArgs] = Math.Round(yData[i], 4);
            row[countArgs + 1] = Math.Round(f_x, 4);
            row[countArgs + 2] = Math.Round(Math.Abs(yData[i] - f_x), 5);
            dt.Rows.Add(row);
        }

        _resultsDataGridView.DataSource = dt;

        // Обновляем метрики времени
        timeValueLabel.Text = $"Время поиска: {_functionModel.SolveTime.ElapsedMilliseconds} мс";
    }

    private void UpdateChartDisplay()
    {
        if (countArgs == 1 && _functionModel != null)
        {
            // Создаем модель графика через ваш ChartsGenerator
            // Примечание: Для корректной работы измените ChartsGenerator 
            // или создайте PlotModel здесь вручную.

            var title = _functionModel.ToString();
            var model = new PlotModel { Title = "График аппроксимации", Subtitle = title, Background = OxyColors.White };

            model.PlotAreaBackground = OxyColors.White;

            var scatterSeries = new LineSeries
            {
                Title = "Исходные данные",
                StrokeThickness = 0,
                MarkerSize = 4,
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColors.Blue
            };
            for (int i = 0; i < xData.Count; i++) scatterSeries.Points.Add(new DataPoint(xData[i][0], yData[i]));

            var lineSeries = new LineSeries { Title = "Аппроксимация", Color = OxyColors.Red };
            double minX = xData.Min(x => x[0]);
            double maxX = xData.Max(x => x[0]);
            for (double x = minX; x <= maxX; x += (maxX - minX) / 100)
            {
                double f_x = 0;
                for (int j = 0; j < _functionModel.Function.Count; j++)
                    f_x += _functionModel.Function[j].Calculate(new[] { x }, _functionModel.CoeffFunction[j]);
                lineSeries.Points.Add(new DataPoint(x, f_x));
            }

            model.Series.Add(scatterSeries);
            model.Series.Add(lineSeries);
            _plotView.Model = model;
        }
    }

    private void LoadResultLabel()
    {
        if (_statisticModels != null)
        {
            _functionModel = _appSettings.BestModel ?
                _statisticModels.GetMaxDetermModel() :
                _statisticModels.GetMostCompactEquation(_appSettings.Threshold);

            if (_functionModel != null)
            {
                string funcText = _functionModel.ToString();
                string r2Text = Math.Round(_functionModel.DeterminationCoeff, 5).ToString();
                string iterText = _functionModel.IterConvergence.ToString();
                string compText = _functionModel.CountComponents.ToString();
                string timeText = $"{_functionModel.SolveTime.ElapsedMilliseconds} мс";

                // ОБНОВЛЯЕМ ВКЛАДКУ 1
                functionValueLabel.Text = funcText;
                determValueLabel.Text = $"R²: {r2Text}";
                iterationsValueLabel.Text = $"Итераций: {iterText}";
                componentsValueLabel.Text = $"Слагаемых: {compText}";
                timeValueLabel.Text = $"Время: {timeText}";

                // ОБНОВЛЯЕМ ВКЛАДКУ 2
                functionResValueLabel.Text = $"Функция: {funcText}";
                determResValueLabel.Text = $"R² (детерминация): {r2Text}";
                iterationsResValueLabel.Text = $"Количество итераций: {iterText}";
                componentsResValueLabel.Text = $"Компонентов: {compText}";
                timeResValueLabel.Text = $"Время поиска: {timeText}";
            }
        }
    }

    private void ShowSettingsForm()
    {
        var newSettings = _appSettings;

        using (var settingsForm = new SettingsForm(newSettings))
        {
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _appSettings = newSettings;
            }
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void ShowArgsCountDialog()
    {
        using (var dialog = new Form
        {
            Text = "Количество аргументов",
            Size = new Size(300, 200),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        })
        {
            var label = new Label
            {
                Text = "Введите количество аргументов (X):",
                Location = new Point(20, 20),
                Size = new Size(250, 30)
            };

            var numericUpDown = new NumericUpDown
            {
                Location = new Point(20, 60),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 20,
                Value = 3
            };

            var okButton = new Button
            {
                Text = "OK",
                Location = new Point(50, 110),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                Location = new Point(150, 110),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            dialog.Controls.AddRange(new Control[] { label, numericUpDown, okButton, cancelButton });
            dialog.AcceptButton = okButton;
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //SetupColumns((int)numericUpDown.Value);
                //AddInitialRow();
                countArgs = (int)numericUpDown.Value;
                InitEmptyData();
            }
        }
    }

    private void LoadFromFile()
    {
        using (var openFileDialog = new OpenFileDialog())
        {
            // Настройки диалога
            openFileDialog.Title = "Выберите файл Excel";
            openFileDialog.Filter = "Excel файлы (*.xlsx; *.xls)|*.xlsx;*.xls|Все файлы (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false; // Только один файл

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _excelFilePath = openFileDialog.FileName;

                LoadDataInTable();

            }
        }
    }

    private void LoadDataInTable()
    {
        if (string.IsNullOrEmpty(_excelFilePath) || !File.Exists(_excelFilePath))
        {
            MessageBox.Show("Сначала выберите файл Excel", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            ExcelReader reader = new ExcelReader(_excelFilePath);
            (xData, yData) = reader.ExcelFileRead();
            countArgs = xData[0].Count;
            if (xData != null && yData != null)
            {
                //UpdateDataInTable();
                UpdateData();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveToFile()
    {
        using (var saveFileDialog = new SaveFileDialog())
        {
            // Настройки диалога
            saveFileDialog.Title = "Сохранение данных в формате Excel";
            saveFileDialog.Filter = "Excel файлы (*.xlsx; *.xls)|*.xlsx;*.xls|Все файлы (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            // Начальная директория
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelPrinter printer = new ExcelPrinter(saveFileDialog.FileName);
                printer.SaveExcelFile(xData, yData);
            }
        }
    }

    private void ClearData()
    {
        if (_dataGridView.ColumnCount > 0)
        {
            var result = MessageBox.Show("Очистить все данные?", "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ShowArgsCountDialog();
            }
        }
        else
        {
            ShowArgsCountDialog();
        }

    }

    private void InitEmptyData()
    {
        if (xData != null && yData != null)
        {
            xData.Clear();
            yData.Clear();
        }
        else
        {
            xData = new List<List<double>>();
            yData = new List<double>();
        }

        for (int i = 0; i < 5; i++)
        {
            AddEmptyRow();

        }
    }

    private void AddEmptyRow()
    {
        List<double> zeroArgsData = new List<double>();

        for (int i = 0; i < countArgs; i++)
        {
            zeroArgsData.Add(0.0d);
        }

        xData.Add(zeroArgsData);
        yData.Add(0.0d);

        UpdateData();
    }
    private void _dataGridView_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

        // Если мы редактируем ячейку, мы должны сразу обновить значение в наших списках xData/yData
        if (xData != null && yData != null && e.RowIndex < xData.Count)
        {
            var cellValue = _dataGridView[e.ColumnIndex, e.RowIndex].Value;
            double val = 0;

            // Безопасное приведение к double
            if (cellValue != null && double.TryParse(cellValue.ToString(), out double res))
                val = res;

            if (e.ColumnIndex == countArgs) // Это колонка f(x)
                yData[e.RowIndex] = val;
            else
                xData[e.RowIndex][e.ColumnIndex] = val;
        }
    }

    private void DeleteSelectedRows()
    {
        if (_dataGridView.SelectedRows.Count > 0)
        {
            List<int> selectedIndices = _dataGridView.SelectedRows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .Select(row => row.Index)
                .ToList();

            foreach (int index in selectedIndices)
            {
                xData.RemoveAt(index);
                yData.RemoveAt(index);
            }

            UpdateData();
        }
        else
        {
            MessageBox.Show("Выберите строки для удаления!", "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void CopySelectedCells()
    {
        if (_dataGridView.GetCellCount(DataGridViewElementStates.Selected) > 0)
        {
            Clipboard.SetDataObject(_dataGridView.GetClipboardContent());
        }
    }

    private void PasteToCells()
    {
        throw new NotImplementedException();
    }

    private DataTable CreateDataTable(List<List<double>> xData, List<double> yData)
    {
        DataTable table = new DataTable();

        for (int i = 0; i < countArgs; i++)
            table.Columns.Add($"x{i + 1}", typeof(double));
        table.Columns.Add("f(x)", typeof(double));

        for (int i = 0; i < xData.Count; i++)
        {
            DataRow row = table.NewRow();
            for (int j = 0; j < xData[i].Count; j++)
                row[j] = xData[i][j];
            row[xData[0].Count] = yData[i];
            table.Rows.Add(row);
        }

        return table;
    }

    public void UpdateData()
    {
        _dataGridView.DataSource = null;
        _dataGridView.Columns.Clear();

        // Создаем новую DataTable на основе текущих xData и yData
        _dataTable = CreateDataTable(xData, yData);

        // Отключаем события на время смены источника, чтобы не было зацикливания
        _dataGridView.CellValueChanged -= _dataGridView_CellValueChanged;
        _dataGridView.CellValidating -= _dataGridView_CellValidating;

        _dataGridView.DataSource = _dataTable;

        _dataGridView.CellValueChanged += _dataGridView_CellValueChanged;
        _dataGridView.CellValidating += _dataGridView_CellValidating;

        // 6. Настраиваем внешний вид (запрет сортировки, чтобы не путать данные)
        foreach (DataGridViewColumn column in _dataGridView.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        // Автоматическая нумерация строк
        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            row.HeaderCell.Value = (row.Index + 1).ToString();
        }

        ConsolePrinter.PrintInputData(xData, yData);
    }

    private void SyncListsFromTable()
    {
        if (_dataGridView.DataSource is DataTable dt)
        {
            xData = new List<List<double>>();
            yData = new List<double>();

            foreach (DataRow row in dt.Rows)
            {
                List<double> xRow = new List<double>();
                for (int i = 0; i < countArgs; i++)
                {
                    xRow.Add(Convert.ToDouble(row[i]));
                }
                xData.Add(xRow);
                yData.Add(Convert.ToDouble(row[countArgs]));
            }
        }
    }
}
